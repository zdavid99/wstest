using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using NPOI;
using NPOI.HSSF.UserModel;


namespace OwensCorning.Excel
{
    public class ExcelWriter
    {
        /// <summary>
        /// Writes the content of a data table to a spreadsheet.
        /// </summary>
        /// <param name="table">The DataTable containing the spreadsheet data.</param>
        /// <param name="outputStream">The stream to write the excel file to</param>
        public void WriteToStream(DataTable table, Stream outputStream)
        {
            WriteToStream(new List<DataTable>() { table }, outputStream);
        }

        /// <summary>
        /// Writes the contents of a list of data table to a spreadsheet. A new sheet is used for each table. 
        /// </summary>
        /// <param name="table">The list of DataTables containing data.</param>
        /// <param name="outputStream">The stream to write the excel file to</param>
        public void WriteToStream(IList<DataTable> tables, Stream outputStream)
        {
            HSSFWorkbook workbook = new HSSFWorkbook();

            if (outputStream == null) // Escape if we have nothing to write to. . .
                return;

            if (tables == null || tables.Count == 0)
                workbook.CreateSheet(); // Create an empty to prevent errors when opening the spreadsheet, even though there's no data to be included. . . 

            for (int i = 0; i < tables.Count; i++) // Iterate through the list, adding a new sheet for each table. . . 
            {
                var table = tables[i];

                var columns = new List<string>();
                foreach (DataColumn column in table.Columns) // Grab a list of the columns. . .
                    columns.Add(column.Caption);

                var sheet = workbook.CreateSheet(); // Create the sheet. . .

                var columnRow = sheet.CreateRow(0);
                for (int j = 0; j < columns.Count; j++) // Write the columns. . .
                {
                    var cell = columnRow.CreateCell(j);
                    cell.SetCellValue(columns[j]); // Set the values. . .
                }

                for (int j = 1; j < table.Rows.Count + 1; j++) // Write the rows. . .
                {
                    var row = sheet.CreateRow(j); // Create the row. . .

                    for (int k = 0; k < columns.Count; k++)
                    {
                        var cell = row.CreateCell(k);
                        cell.SetCellValue((string) table.Rows[j - 1][k]); // Set the values. . .
                    }                    
                }
            }

            workbook.SetActiveSheet(0); // Make sure that the first item is selected. . .
            
            workbook.Write(outputStream); // Write the spreadsheet. . .
        }
    }
}
