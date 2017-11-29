using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using OwensCorning.Utility.Data;
using OwensCorning.Utility.Logging;
using OwensCorning.Utility.Status;
using OwensCorning.Utility;
using OwensCorning.SurveyService.Data;

namespace OwensCorning.SurveyService.Data
{
    public class AggregateResult
    {
        public string QuestionName { get; set; }
        public string Answer { get; set; }
        public int Total { get; set; }
    }

    public class SurveyDAO 
    {
        private static ILogger log = LoggerFactory.CreateLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static SurveyDAO self = new SurveyDAO();

        private SurveyDAO()
		{
		}

        public static SurveyDAO Instance
		{
			get { return self; }
        }

        public int Save(SurveyForm survey)
        {
            int id;

            // Check required fields
            if (String.IsNullOrEmpty(survey.FormName)
                || survey.FormData == null
                )
            {
                return -1;
            }

            // Save the form
            id = InsertItem(survey);
            return survey.Id;
        }

        private int InsertItem(SurveyForm form)
        {
            ValidateForm(form);
            using (SurveyInformationDataContext repository = SurveyDataSource.ContactDataContext())
            {
                Survey survey = new Survey
                {
                    FormName = form.FormName,
                    FormData = form.FormData,
                    DateCreated = DateTime.Now
                };
                repository.Surveys.InsertOnSubmit(survey);
                repository.SubmitChanges();
                return survey.SurveyID;
            }
        }

        private static void ValidateForm(SurveyForm form)
        {
            //TODO: better validation
            if (form == null)
                throw new ArgumentNullException("form");
        }

        public List<AggregateResult> GetSurveySummary(string surveyName, DateTime? startDate, DateTime? endDate)
        {
            XmlDocument xml = new XmlDocument();
            string surveyPath = GetSurveyPath(surveyName);
            object[] parameters = new object[2];
            List<AggregateResult> results;
            StringBuilder allQuestionQuery = new StringBuilder();
            string singleQuestionQuery = new StringBuilder()
                .AppendLine("SELECT")
                .AppendLine("   '{0}' AS QuestionName,")
                .AppendLine("   CONVERT(VARCHAR, FormData.query('data(/root/{0})')) AS Answer")
                .AppendLine("FROM")
                .AppendLine("   Surveys")
                .AppendLine("WHERE")
                .AppendLine("   DateCreated >= {1}")
                .AppendLine("AND")
                .AppendLine("   DateCreated <= {2}")
                .ToString();
            string outerQuery = new StringBuilder()
                .AppendLine("SELECT")
                .AppendLine("   S.QuestionName,")
                .AppendLine("   S.Answer,")
                .AppendLine("   COUNT(*) AS Total")
                .AppendLine("FROM")
                .AppendLine("   ({0}) AS S")
                .AppendLine("WHERE")
                .AppendLine("   RTRIM(LTRIM(S.Answer)) <> ''")
                .AppendLine("GROUP BY")
                .AppendLine("   S.QuestionName, S.Answer")
                .AppendLine("ORDER BY")
                .AppendLine("S.QuestionName")
                .ToString();
            StringBuilder query = new StringBuilder();

            parameters[0] = startDate;
            parameters[1] = endDate;

            xml.Load(surveyPath);
            XmlNodeList nodes = xml.SelectNodes("//Option");                                  

            foreach (XmlNode node in nodes)
            {
                if (node.Attributes["type"] != null && (node.Attributes["type"].Value == "radio" || node.Attributes["type"].Value == "check"))
                {
                    string questionName = node.Attributes["id"].Value;

                    if (allQuestionQuery.ToString() != "")
                    {
                        allQuestionQuery.AppendLine("UNION ALL");
                    }
                    allQuestionQuery.AppendFormat(singleQuestionQuery, questionName, "{0}", "{1}");
                }
            }

            query.AppendFormat(outerQuery, allQuestionQuery);

            using (SurveyInformationDataContext repository = SurveyDataSource.ContactDataContext())
            {
                results = repository.ExecuteQuery <AggregateResult>(query.ToString(), parameters).ToList<AggregateResult>();
            }

            return results;
        }

        public DataTable GetSurveyDetails(string surveyName, DateTime? startDate, DateTime? endDate)
        {
            DataTable returnValue = new DataTable(surveyName);
            string surveyPath = GetSurveyPath(surveyName);
            XDocument surveyDefinition = XDocument.Load(surveyPath);
            XmlDocument xml = new XmlDocument();
            //var results;
            Dictionary<string, string> columns = new Dictionary<string, string>();

            returnValue.Columns.Add("Date");

            using (SurveyInformationDataContext repository = SurveyDataSource.ContactDataContext())
            {
                var results = (from survey
                          in repository.Surveys
                               where survey.FormName == surveyName
                                 && startDate < survey.DateCreated
                                 && survey.DateCreated < endDate
                               select new { Date = survey.DateCreated, FormDate = XDocument.Parse(survey.FormData.ToString()) });



                xml.Load(surveyPath);
                XmlNodeList nodes = xml.SelectNodes("//Option");

                foreach (XmlNode node in nodes)
                {
                    XmlAttribute type = node.Attributes["type"];
                    XmlAttribute id = node.Attributes["id"];
                    if (type != null && id != null)
                    {
                        string prefix;

                        switch (type.Value)
                        {
                            case "name":
                            case "email":
                            case "company":
                            case "phone":
                            case "address":
                            case "city":
                            case "zip":
                            case "website":
                                prefix = "t";
                                break;
                            case "usstates":
                            case "states":
                            case "allstates":
                            case "usterritories":
                            case "canadianprovinces":
                                prefix = "dd";
                                break;
                            default:
                                prefix = "";
                                break;
                        }

                        string questionName = id.Value;
                        columns.Add(questionName, prefix + questionName);
                        returnValue.Columns.Add(questionName);

                        XmlNodeList otherNodes = node.SelectNodes("Option[@type='other']");

                        foreach(XmlNode otherNode in otherNodes)
                        {
                            string value = otherNode.Attributes["value"] == null ? "" : otherNode.Attributes["value"].Value;
                            questionName = value + "_other";
                            columns.Add(questionName, questionName);
                            returnValue.Columns.Add(questionName);
                        }
                    }
                }

                foreach (var survey in results)
                {
                    DataRow row = returnValue.NewRow();

                    row["Date"] = survey.Date.ToString();

                    foreach (KeyValuePair<string, string> column in columns)
                    {
                        string response = (from answer
                                       in survey.FormDate.Descendants(column.Value)
                                           select answer.Value.ToString()).FirstOrDefault<string>();

                        row[column.Key] = response;
                    }

                    returnValue.Rows.Add(row);
                }
            }

            return returnValue;
        }

        public string GetSurveyPath(string surveyName)
        {
            string surveyPath = "";

            using (SurveyInformationDataContext repository = SurveyDataSource.ContactDataContext())
            {
                List<Survey> surveys = (from survey in repository.Surveys select survey).ToList<Survey>();
                surveyPath = (from surveyTemplate 
                              in repository.SurveyTemplates
                              where (surveyTemplate.FormName == surveyName)
                              select surveyTemplate.SurveyPath).FirstOrDefault<string>();
            }

            return surveyPath;
        }

        public List<string> GetAllSurveyNames()
        {
            SurveyInformationDataContext repository = SurveyDataSource.ContactDataContext();
            return (from surveyTemplate
                    in repository.SurveyTemplates
                    join survey
                    in repository.Surveys
                    on surveyTemplate.FormName equals survey.FormName
                    select surveyTemplate.FormName).Distinct<string>().ToList<string>();
        }

        public List<string> GetSurveyNames(DateTime? startDate, DateTime? endDate)
        {
            SurveyInformationDataContext repository = SurveyDataSource.ContactDataContext();
            return (from surveyTemplate
                    in repository.SurveyTemplates
                    join survey
                    in repository.Surveys
                    on surveyTemplate.FormName equals survey.FormName
                    where survey.DateCreated >= startDate
                    && survey.DateCreated <= endDate
                    select surveyTemplate.FormName).Distinct<string>().ToList<string>();
        }
    }
}
