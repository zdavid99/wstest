using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OwensCorning.Excel;
using System.Data;
using OwensCorning.ContactService.Data;
using System.IO;
using System.Xml.Linq;
using System.Xml.Schema;

using OwensCorning.Utility.Extensions;
using OwensCorning.Utility.Logging;
using System.Text.RegularExpressions;


namespace com.ocwebservice.contactform.service
{
    public class ContactFormService
    {
		class ColumnMapEntry
		{
			public ColumnMapEntry(string fieldName, string displayName, int order)
			{
				FieldName = fieldName;
				DisplayName = displayName;
				ColumnOrder = order;
			}
			public string FieldName { get; private set; }
			public string DisplayName { get; private set; }
			public int ColumnOrder { get; private set; }
		}

        const string ALLFORMS_DESC = "[ALL FORMS]";

        ILogger logger;
        
        public ContactFormService() 
        { 
            logger = LoggerFactory.CreateLogger(typeof(ContactFormService));
        }

        public bool WriteContactListFileToResponse(string formType, string businessArea, string endingDateValue, string startingDateValue, string contactType, string format, HttpResponse Response)
        {
			return WriteContactListFileToResponse(formType, businessArea, endingDateValue, startingDateValue, contactType, format, string.Empty, string.Empty, string.Empty, Response);
        }

        public bool WriteContactListFileToResponseColumnMap(string formType, string businessArea, string endingDateValue, string startingDateValue, string contactType, string format,
			string columnMap, HttpResponse Response)
		{
			return WriteContactListFileToResponse(formType, businessArea, endingDateValue, startingDateValue, contactType, format, string.Empty, string.Empty, columnMap, Response);
		}

		public bool WriteContactListFileToResponse(string formType, string businessArea, string endingDateValue, string startingDateValue, string contactType, string format,
			string columnList, string columnOrderList, HttpResponse Response)
		{
			return WriteContactListFileToResponse(formType, businessArea, endingDateValue, startingDateValue, contactType, format, columnList, columnOrderList, string.Empty, Response);
		}

        public bool WriteContactListFileToResponse(string formType, string businessArea, string endingDateValue, string startingDateValue, string contactType, string format,
			string columnList, string columnOrderList, string columnMap, HttpResponse Response)
        {
            if (string.IsNullOrEmpty(formType)) // Make sure that we have something in formtype. . .
                formType = string.Empty;

			List<string> formTypes = formType.Split(',').ToList();
			if (!string.IsNullOrEmpty(formType) && formTypes.Count == 1)
			{
				var formTypeMapping = ContactFormDao.GetContactFormType(formType);
				if (formTypeMapping != null && string.IsNullOrEmpty(columnMap))
				{
					columnMap = formTypeMapping.ColumnMap ?? string.Empty;
				}
			}

            var forms = GetRequestedContactForms(businessArea, formTypes, startingDateValue, endingDateValue, contactType);

            if (forms == null) // Don't bother processing any further if we don't have anything. . .
                return false;

            IList<DataTable> tables = null;
            try
            {
				if (string.IsNullOrEmpty(columnMap))
				{
					tables = XmlToDataTables(forms.Select(x => x.FormData), columnList, columnOrderList);
				}
				else
				{
					Regex mapregExp = new Regex(@"(?:([a-zA-Z_0-9]+)\[((?<![\\])['""])((?:.(?!(?<![\\])\2))*.?\2)\:(\d+)\])*");

					//re-format columnMap into list and order list
					// format is:
					//	columnMap=fieldName["DisplayName with quotes as """:order]
					// e.g.
					//	columnMap=Zip["Zip/PostalCode":1]Email["Email Address":2]
					MatchCollection ms = mapregExp.Matches(columnMap);

					List<string> columnsToSelect = new List<string>();
					List<int> columnOrders = new List<int>();
					List<string> columnNames = new List<string>();

					foreach (Match m in ms)
					{
						if (m.Length > 0)
						{
							foreach (Capture c in m.Groups[1].Captures)
							{
								columnsToSelect.Add(c.Value);
							}
							foreach (Capture c in m.Groups[3].Captures)
							{
								columnNames.Add(c.Value.Substring(0, c.Value.Length - 1).Replace("\\\"", "\""));
							}
							foreach (Capture c in m.Groups[4].Captures)
							{
								columnOrders.Add(int.Parse(c.Value));
							}
						}
					}

					List<ColumnMapEntry> columnEntrys = new List<ColumnMapEntry>();
					for (int i = 0; i < columnsToSelect.Count; i++)
					{
						columnEntrys.Add(new ColumnMapEntry(columnsToSelect[i], columnNames[i], columnOrders[i]));
					}
					Dictionary<string, string> columnDisplayMap = columnEntrys.ToDictionary(c => c.FieldName, c => c.DisplayName);

					columnList = string.Join(",", columnsToSelect.ToArray());
					columnOrderList = string.Join(",", columnEntrys.OrderBy(c => c.ColumnOrder).Select(c => c.FieldName).ToArray());

					//then call existing method
					tables = XmlToDataTables(forms.Select(x => x.FormData), columnList, columnOrderList);

					//then rewrite all column names in output table list
					foreach (DataTable table in tables)
					{
						foreach (DataColumn column in table.Columns) 
						{
							column.Caption = columnDisplayMap[column.ColumnName];
						}
					}
				}
            }
            catch (Exception ex)
            {
                logger.Error("Failure generating report", ex); // Log the error. . .
            }

            Response.Clear();
            Response.Buffer = true;
            Response.ContentEncoding = System.Text.Encoding.GetEncoding("UTF-8");
            if (string.IsNullOrEmpty(format))
                format = "xls";

            switch (format)
            {
                case "xls":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=contacts.xls");
                    Response.ContentType = "application/ms-excel";
                    break;
                case "csv":
                    Response.AppendHeader("Content-Disposition", "attachment;filename=contacts.csv");
                    Response.ContentType = "text/csv";
                    break;
            }

            if (format == "csv")
                foreach (var schemaResult in tables) // Output each of the schema's data, separated by a few empty lines. . .
                {
                    schemaResult.Rows.Add(schemaResult.NewRow()); // Tack a couple empty rows to the bottom. . .
                    schemaResult.Rows.Add(schemaResult.NewRow());

                    schemaResult.WriteToCSV(Response.OutputStream); // Output the result. . . 
                }
            else // XLS. . .
            {
                ExcelWriter writer = new ExcelWriter(); // Write the tables to excel. . .
                writer.WriteToStream(tables, Response.OutputStream);
            }

            return true;
        }


        /// <summary>
        /// Obtaions a dictionary of all contact forms that fall under the included criteria, grouping them by their source form name. 
        /// </summary>
        private IList<ContactForm> GetRequestedContactForms(string businessArea, List<string> formTypes, string startingDateValue, string endingDateValue,
            string contactType)
        {
            DateTime? startingDate;
            DateTime? endingDate;

            var contactForms = new List<ContactForm>();

            try
            {
                for (int i = 0; i < formTypes.Count; i++) // Check FormTypes
                    formTypes[i] = GetValidFormType(formTypes[i]);

                // Check Starting Date
                startingDate = GetValidStartDate(startingDateValue);

                endingDate = GetValidEndDate(endingDateValue);
                if (!endingDate.HasValue || endingDate.Value == DateTime.MinValue)
                    endingDate = DateTime.Now;

				endingDate = endingDate.Value.Date.AddDays(1).AddMilliseconds(-1); // Get the highest possible time for the date.

                if (!startingDate.HasValue)
                    startingDate = endingDate.Value.AddDays(-7);

                var forms = ContactFormDao.GetContactsByCriteria(businessArea, formTypes, startingDate.Value, endingDate.Value);
                foreach (var form in forms)
                {
                    if (string.IsNullOrEmpty(form.FormData)) // Skip items that dont' have any form data. . .
                        continue;
                    if (!string.IsNullOrEmpty(contactType) && // Filter by 'contactType,' if necessary. . .
                        !form.ContactTypes.Trim().Equals(contactType.Trim(), StringComparison.CurrentCultureIgnoreCase))  
                        continue;

                    contactForms.Add(form); // Add the form to the list. . .
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            return contactForms;
        }

        private static DateTime MinDateTime { get { return System.Data.SqlTypes.SqlDateTime.MinValue.Value; } }
        private static DateTime MaxDateTime { get { return System.Data.SqlTypes.SqlDateTime.MaxValue.Value; } }

        private DateTime? GetValidEndDate(string endDateValue)
        {
            DateTime endDate = DateTime.Now;
            if (String.IsNullOrEmpty(endDateValue))
            {
                return endDate;
            }
            else if (!DateTime.TryParse(endDateValue, out endDate))
            {
                throw new ArgumentException("End Date format is incorrect, please use MM/DD/YYYY format (ex 04/21/2009)");

            }

            if (endDate < MinDateTime ||
                endDate > MaxDateTime)
            {
                throw new ArgumentOutOfRangeException("Ending Date", String.Format("End Date format is incorrect, it must be between %0 and %1", MinDateTime.ToString(), MaxDateTime.ToString()));
            }

            return endDate;
        }

        private DateTime? GetValidStartDate(string startDateValue)
        {
            DateTime startDate = DateTime.MinValue;
            if (String.IsNullOrEmpty(startDateValue))
            {
                return null;
            }

            if (!DateTime.TryParse(startDateValue, out startDate))
            {
                throw new ArgumentException("Starting Date format is incorrect, please use MM/DD/YYYY format (ex 08/22/2008)");

            }

            if (startDate <= MinDateTime ||
                startDate >= MaxDateTime)
            {
                throw new ArgumentOutOfRangeException("StartingDate", String.Format("Starting Date format is incorrect, it must be between %0 and %1", MinDateTime.ToString(), MaxDateTime.ToString()));
            }

            return startDate;

        }

        /// <summary>
        /// Gets a valid form type from the text 
        /// </summary>
        /// <param name="formTypeCandidate"></param>
        /// <returns>Empty string if the formType is 1 charachter or 'All forms'</returns>
        private string GetValidFormType(string formTypeCandidate)
        {
            if (formTypeCandidate.Length < 1 ||
                formTypeCandidate == ALLFORMS_DESC)
            {
                return string.Empty;
            }
            return formTypeCandidate;
        }

        private IList<DataTable> XmlToDataTables(IEnumerable<string> xmlList, string displayColumns, string columnOrder)
        {
            var dataTableDictionary = new Dictionary<Dictionary<string, int>, // 
                                                List<Dictionary<string, string>>>();

            var columnList = new List<string>();
            var columnOrderList = new List<string>();
            if (!string.IsNullOrEmpty(displayColumns))
                columnList.AddRange(displayColumns.Split(',').ToList()); // Obtain the list of columns to display, if any. . .
            if (!string.IsNullOrEmpty(columnOrder))
                columnOrderList = columnOrder.Split(',').ToList(); // Obtain the order of columns to display. . .

            foreach (var xml in xmlList) // Iterate through the XML. . .
            {
                var dataTable = new DataTable("XMLTable");
                var columns = new Dictionary<string, string>(); // a dictionary representing the table's columns and values. . .
                var columnCount = new Dictionary<string, int>(); // a dictionary to keep track of the count of columns that possess the same name within a given table. . .

                using (var stringReader = new StringReader(xml))
                {
                    var root = XElement.Load(stringReader);

                    foreach (var element in root.Elements())
                    {
                        var pairs = GetKeyValuePairsForElement(element, string.Empty);
                        foreach (var pair in pairs)
                        {
                            var keyValue = pair.Key;

                            if (columns.ContainsKey(pair.Key))
                            {
                                var count = ++columnCount[pair.Key]; // Grab the value and update the count. . .
                                keyValue = FormatDuplicateColumn(pair.Key, count);// Update the column name. . .
                            }
                            else 
                                columnCount.Add(pair.Key, 1); // Initialize the count. . 

                            columns.Add(keyValue, pair.Value);
                        }
                    }
                }

                var added = false; // Keep track fo 

                foreach (var counts in dataTableDictionary.Keys) // See if there are any other items with the same schema. . .
                {
                    var matches = true;
                    foreach (var key in counts.Keys) // Iterate through the columns to see if this is the same schema. . .
                    {
                        if (!columnCount.ContainsKey(key))
                        {
                            matches = false;
                            break;
                        }
                    }
					foreach (var key in columnCount.Keys) // Iterate through the columns to see if this is the same schema. . .
					{
						if (!counts.ContainsKey(key))
						{
							matches = false;
							break;
						}
					}

                    if (matches) // Merge the items, as they have the same schema. . . 
                    {
                        dataTableDictionary[counts].Add(columns);

                        for (int i = 0; i < counts.Keys.Count; i++) // Update the column counts. . .
                        {
                            var key = counts.Keys.ElementAt(i);
                            if (counts[key] < columnCount[key])
                                counts[key] = columnCount[key]; // Local table is greater, update. . .
                        }

                        added = true; // Indicate that the xml data has been added. . .
                    }
                }

                if (!added) // otherwise, add it to the list. . .
                    dataTableDictionary.Add(columnCount, new List<Dictionary<string, string>>() { columns });
            }

			int formIndex = 0;
			var sourceFormNames = new Dictionary<string, List<Dictionary<string, string>>>();
			foreach (var dataTables in dataTableDictionary)
			{
				++formIndex;
				foreach (var form in dataTables.Value)
				{
					string formName;
					if (form.ContainsKey("SourceFormName"))
					{
						formName = form["SourceFormName"];
					}
					else
					{
						formName = string.Format("Sheet{0}", formIndex);
					}
					
					if (sourceFormNames.ContainsKey(formName))
					{
						// another of same form name
						// compare existing schema
						bool columnsManipulated = false;
						do
						{
							columnsManipulated = false;
							var existingSchema = sourceFormNames[formName].First().Keys.ToArray();
							var newSchema = form.Keys.ToArray();
							for (int columnIndex = 0; columnIndex < existingSchema.Length; columnIndex++)
							{
								string currentExistingColumnName = existingSchema[columnIndex];
								if (columnIndex < newSchema.Length)
								{
									// check if column exists in newSchema
									if (newSchema.Contains(currentExistingColumnName))
									{
										// ok, do nothing
									}
									else
									{
										// add it (as empty)
										form.Add(currentExistingColumnName, string.Empty);
										columnsManipulated = true;
										break;
									}
								}
								else
								{
									// this column must not exist in newSchema, add it (as empty)
									form.Add(currentExistingColumnName, string.Empty);
									columnsManipulated = true;
									break;
								}
							}
							for (int columnIndex = 0; columnIndex < newSchema.Length; columnIndex++)
							{
								string currentNewColumnName = newSchema[columnIndex];
								if (columnIndex < existingSchema.Length)
								{
									// check if column exists in newSchema
									if (existingSchema.Contains(currentNewColumnName))
									{
										// ok, do nothing
									}
									else
									{
										// add it (as empty) to all prior forms
										foreach (var existingForm in sourceFormNames[formName])
										{
											existingForm.Add(currentNewColumnName, string.Empty);
										}
										columnsManipulated = true;
										break;
									}
								}
								else
								{
									// this column must not exist in existingSchema, add it (as empty) to all prior forms
									foreach (var existingForm in sourceFormNames[formName])
									{
										existingForm.Add(currentNewColumnName, string.Empty);
									}
									columnsManipulated = true;
									break;
								}
							}
						} while (columnsManipulated);
						sourceFormNames[formName].Add(form);
					}
					else
					{
						// first of this form name
						sourceFormNames.Add(formName, new List<Dictionary<string, string>>() {form});
					}
				}
			}

            var tables = new List<DataTable>();

            // Convert to DataTables. . .
			foreach (var dataTables in sourceFormNames)
            {
                var table = new DataTable("");

                // Add the columns. . . 
                if (columnList.Count > 0) // If columns were passed in, only show the items specified. . .
                {
                    foreach (var displayColumn in columnList)
                    {
                        if (dataTables.Value.First().ContainsKey(displayColumn)) // If we have the item specified. . .
                        {
                            //for (int i = 0; i < dataTables.Key[displayColumn]; i++) // Add all of the columns. . .
                            //{
                                var columnName = displayColumn;
                                //if (i > 0) // If there's more than one, append the index. . .
                                //    columnName = FormatDuplicateColumn(columnName, i + 1);

                                table.Columns.Add(columnName);
                            //}
                        }
                    }
                }
                else
                {
                    foreach (var column in dataTables.Value.First())
                    {
                        //for (int i = 0; i < column.Value; i++) // Add all of the columns. . .
                        //{
                            var columnName = column.Key;
                            //if (i > 0) // If there's more than one, append the index. . .
                            //    columnName = FormatDuplicateColumn(columnName, i + 1);

                            table.Columns.Add(columnName);
                     
                        //}
                    }

                    if (columnOrderList.Count > 0) // If column ordering was passed in, apply the ordering. . .
                    {
                        var orderedColumnCount = 0;
                        foreach(var columnOrderName in columnOrderList)
                            foreach (DataColumn column in table.Columns)
                                if (column.Caption == columnOrderName) // If it matches,
                                {
                                    column.SetOrdinal(orderedColumnCount++); // Update the position and increment the column count. . .
                                    break;
                                }
                    }
                }

                foreach (var values in dataTables.Value) // Add the rows. . .
                {
                    var row = table.NewRow();

                    foreach (DataColumn column in table.Columns)
                    {
                        var value = string.Empty; // Default to empty. . .
                        if (values.ContainsKey(column.Caption))
                            value = values[column.Caption]; // Grab the actual value. . .

                        row[column] = value; // Set the row's value. . .
                    }

                    table.Rows.Add(row); // Add the row. . .
                }

                tables.Add(table); // Add to the list of tables. . .
            }

            return tables; // Return the list of tables. . .
        }

        private string FormatDuplicateColumn(string columnName, int index)
        {
            return string.Format("{0} - {1}", columnName, index);
        }


        private IList<KeyValuePair<string, string>> GetKeyValuePairsForElement(XElement element, string parentName)
        {
            var list = new List<KeyValuePair<string, string>>();
            var elementName = string.IsNullOrEmpty(parentName) ? element.Name.LocalName : string.Format("{0}.{1}", parentName, element.Name.LocalName);

            list.Add(new KeyValuePair<string, string>(elementName, element.Value == null ? string.Empty : element.Value));

            if (element.HasElements) // If there are child items, add. . .
                foreach (var childElement in element.Elements())
                    list.AddRange(GetKeyValuePairsForElement(childElement, elementName));

            return list;
        }

        private static string NormalizeCSVFieldValue(string value)
        {
            return (value ?? string.Empty).Replace("\"", "\"\"");
        }
    }
}