using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using System.Xml.Linq;
using OwensCorning.Utility.Logging;
using OwensCorning.Utility.Location;
using OwensCorning.Utility.Tracking;
using OwensCorning.SurveyService.Data;
using OwensCorning.Survey.Service;
using OwensCorning.Utility.Notification;
using owenscorning.webservicesclient;

namespace OwensCorning.Survey.WebControls
{
    public class RadioButtonInfo
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }

    public enum ContactFields
    {
        Name,
        Company,
        Zip,
        Phone,
        Email
    }

    public partial class SurveyForm : System.Web.UI.UserControl
    {
        ILogger log = LoggerFactory.CreateLogger(typeof(SurveyForm));

        protected string _textBoxPrefix = "t";
        protected string _dropDownPrefix = "dd";
        protected string _requiredFieldPrefix = "vr";
        protected string _regexPrefix = "vre";
        protected string _lengthSuffix = "Length";
        protected string _emailFormatRegex = "";
        protected string _emailLengthRegex = "";
        protected string _nameRegex = "";
        private string _surveyNameFieldName = "surveyName";
        private static readonly char NAME_DELIMITER = '+';

        public string LabelCssClass { get; set; }
        public string TextBoxCssClass { get; set; }
        public string DropDownCssClass { get; set; }
        public string DivCssClass { get; set; }
        public string XmlFile { get; set; }
        public string ValidationGroup { get; set; }
        public string ThankYouText { get; set; }
        protected string ControlClientID { get { return this.ClientID.Replace("_","$"); } }

        // Contact Form Fields
        // protected ContactForm Contact { get; set; }
        Dictionary<string, ContactFields> FieldMappings { get; set; }

        /// <summary>
        /// Creates the field mappings.
        /// </summary>
        /// <param name="node">The node.</param>
        protected void CreateFieldMappings(XmlNode node)
        {
            FieldMappings = new Dictionary<string, ContactFields>();

            foreach (XmlNode childNode in node.SelectNodes("//Option[@type]"))
            {
                XmlAttribute idAttribute = childNode.Attributes["id"];
                string clientNamePrefix = ControlClientID + "$";
                string clientName = (idAttribute == null ? string.Empty : idAttribute.Value);

                switch (childNode.Attributes["type"].Value)
                {
                    case "name":
                        FieldMappings.Add(clientNamePrefix + "t" + clientName, ContactFields.Name);
                        break;
                    case "company":
                        FieldMappings.Add(clientNamePrefix + "t" + clientName, ContactFields.Company);
                        break;
                    case "zip":
                        FieldMappings.Add(clientNamePrefix + "t" + clientName, ContactFields.Zip);
                        break;
                    case "phone":
                        FieldMappings.Add(clientNamePrefix + "t" + clientName, ContactFields.Phone);
                        break;
                    case "email":
                        FieldMappings.Add(clientNamePrefix + "t" + clientName, ContactFields.Email);
                        break;
                }
            }
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            List<Control> controls;
            XmlDocument xml = new XmlDocument();

            if (XmlFile.ToLower().IndexOf("http://") == 0)
            {
                xml.Load(XmlFile);
            }
            else
            {
                xml.Load(Request.MapPath(XmlFile));
            }

            XmlNode surveyNode = xml.SelectSingleNode("//survey");
            CreateFieldMappings(surveyNode);
            string surveyName = surveyNode.Attributes["name"] == null ? "" : surveyNode.Attributes["name"].Value;
            HiddenField surveyNameField = new HiddenField();

            surveyNameField.ID = _surveyNameFieldName;
            surveyNameField.Value = surveyName;

            bSubmit.ValidationGroup = ValidationGroup;

            phForm.Controls.Add(surveyNameField);

            controls = GetControls(surveyNode.SelectNodes("Question"));

            foreach (WebControl control in controls)
            {
                phForm.Controls.Add(control);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void bSubmit_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                XmlDocument xml = new XmlDocument();
                XmlNode rootNode;
                XmlNode node;
                string surveyName = "";
                ContactForm contact = new ContactForm();

                rootNode = xml.CreateElement("root");
                xml.AppendChild(rootNode);

                foreach (string key in Request.Form)
                {
                    string value = Request[key];

                    if (key.StartsWith(ControlClientID) && key != ControlClientID + "$" + bSubmit.ID)
                    {
                        if (key == ControlClientID + "$" + _surveyNameFieldName)
                        {
                            surveyName = value;
                        }
                        else
                        {
                            node = xml.CreateElement(key.Replace(ControlClientID + "$", ""));
                            node.InnerText = value;

                            if (FieldMappings != null && FieldMappings.ContainsKey(key)) {
                                switch (FieldMappings[key])
                                {
                                    case ContactFields.Name:
                                        contact.Name = (contact.Name + NAME_DELIMITER + value).TrimStart(' ');
                                        break;
                                    case ContactFields.Company:
                                        contact.Company = value;
                                        break;
                                    case ContactFields.Zip:
                                        contact.Zip = value;
                                        break;
                                    case ContactFields.Phone:
                                        contact.Phone = value;
                                        break;
                                    case ContactFields.Email:
                                        contact.Email = value;
                                        break;
                                }
                            }

                            rootNode.AppendChild(node);
                        }
                    }
                }

                OwensCorning.SurveyService.Data.SurveyForm form = 
                    new OwensCorning.SurveyService.Data.SurveyForm()
                    {
                        FormName = surveyName,
                        FormData = XElement.Load(xml.CreateNavigator().ReadSubtree())
                    };

                using (SurveyClient client = new SurveyClient())
                {
                    client.SaveSurvey(form);
                }

                try
                {
                    //Horrible, needs to be fixed ASAP
                    string[] names = contact.Name.Split(NAME_DELIMITER);
                    ExactTargetService.Instance.Register(
                        new ExactTargetUser()
                        {
                            FirstName = names[0],
                            LastName = names[1],
                            Company = contact.Company,
                            Business = string.Empty, //EMBED THIS IN XML
                            Email = contact.Email,
                            Phone = contact.Phone
                            //INTERESTS?
                        }
                    );
                }
                catch (Exception ex)
                {
                    log.Error("Failed saving survey to ExactTarget: " + ex.Message, ex);
                }

                try
                {
                    contact.FormData = form.FormData.ToString();
                    ContactFormService.PublishContactForm(contact);
                }
                catch (Exception ex)
                {
                    log.Error("Failed publishing survey to contact form web service: " + ex.Message, ex);
                }

                pForm.Visible = false;
                pThankYou.Visible = true;
            }
        }

        protected List<Control> GetControls(XmlNodeList surveyNodeChildren)
        {
            List<Control> globalControls = new List<Control>();

            foreach (XmlNode questionNode in surveyNodeChildren)
            {
                List<Control> outercontrols = new List<Control>();
                List<WebControl> controls = new List<WebControl>();
                string text = questionNode.Attributes["text"] == null ? "" : questionNode.Attributes["text"].Value;
                string footnote = questionNode.Attributes["footnote"] == null ? "" : questionNode.Attributes["footnote"].Value;
                string qid = questionNode.Attributes["num"] == null ? "" : questionNode.Attributes["num"].Value;

                Literal textLabel = new Literal();
                //Literal footnoteLabel = new Literal();

                if ( string.IsNullOrEmpty(footnote) )
                {
                    textLabel.Text = string.Format("<label for=\"{0}\">{1}</label><br />", qid, text);
                }
                else
                {
                    textLabel.Text = string.Format("<label for=\"{0}\">{1}<br /><small>{2}</small></label><br />", qid, text, footnote);
                }
                //footnoteLabel.Text = footnote;

                outercontrols.Add(textLabel);
                //outercontrols.Add(footnoteLabel);

                foreach (XmlNode node in questionNode.ChildNodes)
                {
                    if (node.Name.ToLower() == "option")
                    {
                        bool required = false;
                        string type = node.Attributes["type"] == null ? "" : node.Attributes["type"].Value.ToLower();
                        string childText = node.Attributes["text"] == null ? "" : node.Attributes["text"].Value;
                        string id = node.Attributes["id"] == null ? "" : node.Attributes["id"].Value;
                        string fieldName = node.Attributes["fieldname"] == null ? "" : node.Attributes["fieldname"].Value;
                        string num = node.Attributes["num"] == null ? "" : node.Attributes["num"].Value;
                        string maxLength;
                        string maxLengthRegEx;

                        bool.TryParse(node.Attributes["required"] == null ? "false" : node.Attributes["required"].ToString(), out required);
                        //required = true;

                        switch (type)
                        {
                            case "name":
                                maxLength = node.Attributes["maxlength"] == null ? ValidationConstants.NameMaxLength.ToString() : node.Attributes["maxlength"].ToString();
                                maxLengthRegEx = @"[\S\s]{0," + maxLength + "}";
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required.", fieldName + " must be in correct format.",
                                    fieldName + " must no more than " + ValidationConstants.CityMaxLength.ToString() + " characters.", ValidationConstants.NameRegExp, @"[\S\s]{0," + maxLengthRegEx));
                                break;
                            case "email":
                                maxLength = node.Attributes["maxlength"] == null ? ValidationConstants.EmailMaxLength.ToString() : node.Attributes["maxlength"].ToString();
                                maxLengthRegEx = maxLength == null ? ValidationConstants.EmailLengthRegExp : @"[\S\s]{0," + maxLength + "}";
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required.", fieldName + " must be in correct format.",
                                    fieldName + " must no more than " + ValidationConstants.CityMaxLength.ToString() + " characters.", ValidationConstants.EmailRegExp, maxLengthRegEx));
                                break;
                            case "company":
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required."));
                                break;
                            case "phone":
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required", fieldName + " must be in correct format.",
                                    ValidationConstants.InternationalPhoneRegex));
                                break;
                            case "address":
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required", fieldName + " must be in correct format.",
                                    fieldName + " must no more than " + ValidationConstants.CityMaxLength.ToString() + " characters.", ValidationConstants.AddressRegExp, 
                                    ValidationConstants.AddressMaxLength.ToString()));
                                break;
                            case "city":
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required", fieldName + " must be in correct format.",
                                    fieldName + " must no more than " + ValidationConstants.CityMaxLength.ToString() + " characters.", ValidationConstants.CityRegExp, 
                                    ValidationConstants.CityMaxLength.ToString()));
                                break;
                            case "usstates":
                            case "states":
                                controls.AddRange(GetUsStates(id, childText, required, fieldName + " is required"));
                                break;
                            case "allstates":
                                controls.AddRange(GetAllStates(id, childText, required, fieldName + " is required"));
                                break;
                            case "usterritories":
                                controls.AddRange(GetUsStates(id, childText, required, fieldName + " is required"));
                                break;
                            case "canadianprovinces":
                                controls.AddRange(GetCanadianProvinces(id, childText, required, fieldName + " is required"));
                                break;
                            case "zip":
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required.", fieldName + " must be in correct format.",
                                    ValidationConstants.ZipPostalRegEx));
                                break;
                            case "website":
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required", fieldName + " must be in correct format.",
                                    fieldName + " must no more than " + ValidationConstants.WebsiteReferralOtherMaxLength.ToString() + " characters.", 
                                    ValidationConstants.WebsiteReferralOtherRegExp, ValidationConstants.WebsiteReferralOtherMaxLength.ToString()));
                                break;
                            case "check":
                                controls.AddRange(GetCheckboxControls(id, childText, node.ChildNodes, null));
                                break;
                            case "radio":
                                controls.AddRange(GetRadioControls(id, node.SelectNodes("Option")));
                                break;
                            case "optin":
                                controls.AddRange(GetOptinControl(null, id, childText, node.ChildNodes, null));
                                break;
                            default:
                                controls.AddRange(GetTextBoxControls(id, childText, ValidationGroup, required, fieldName + " is required", fieldName + " must be in correct format.",
                                    fieldName + " must no more than " + ValidationConstants.CityMaxLength.ToString() + " characters.", ValidationConstants.CityRegExp,
                                    ValidationConstants.CityMaxLength.ToString()));
                                break;
                        }
                    }
                }

                foreach (Control control in controls)
                {
                    outercontrols.Add(control);
                }

                globalControls.Add(GetQuestionPanel(outercontrols));
            }
            return globalControls;
        }

        protected T GetValidator<T>(T validator, string id, string controlToValidateId, string errorMessage, string validationGroup,
            ValidatorDisplay validatorDisplay) where T : BaseValidator
        {
            validator.ID = id;
            validator.ControlToValidate = controlToValidateId;
            validator.Text = errorMessage;
            validator.ErrorMessage = errorMessage;
            validator.ValidationGroup = validationGroup;
            validator.Display = validatorDisplay;

            return validator;
        }

        protected List<WebControl> GetUsStates(string id, string labelText, bool required, string errorMessage)
        {
            return GetStateDropDownControls(id, labelText, StateDAO.Instance.GetUSStates(), required, errorMessage);
        }

        protected List<WebControl> GetAllStates(string id, string labelText, bool required, string errorMessage)
        {
            return GetStateDropDownControls(id, labelText, StateDAO.Instance.GetAllStates(), required, errorMessage);
        }

        protected List<WebControl> GetUsTerritories(string id, string labelText, bool required, string errorMessage)
        {
            return GetStateDropDownControls(id, labelText, StateDAO.Instance.GetUSTerritories(), required, errorMessage);
        }

        protected List<WebControl> GetCanadianProvinces(string id, string labelText, bool required, string errorMessage)
        {
            return GetStateDropDownControls(id, labelText, StateDAO.Instance.GetCanadianProvinces(), required, errorMessage);
        }

        protected List<WebControl> GetStateDropDownControls(string id, string labelText, IList<State> states, bool required, string errorMessage)
        {
            return GetDropDownControls(id, labelText, states, "Abbreviation", "Name", required, errorMessage);
        }

        protected List<WebControl> GetDropDownControls<T>(string id, string labelText, IEnumerable<T> list, string valueField, string textField, bool required, string errorMessage)
        {
            DropDownList dropDown = new DropDownList();
            dropDown.ID = _dropDownPrefix + id;
            dropDown.DataSource = list;
            dropDown.DataValueField = valueField;
            dropDown.DataTextField = textField;
            dropDown.ValidationGroup = ValidationGroup;
            dropDown.DataBind();

            return GetDropDownControls(id, labelText, dropDown, required, errorMessage);
        }

        protected List<WebControl> GetDropDownControls(string id, string labelText, DropDownList dropDown, bool required, string errorMessage)
        {
            List<WebControl> controls = new List<WebControl>();

            Label label = new Label();
            label.CssClass = LabelCssClass;
            label.AssociatedControlID = dropDown.ID;
            label.Text = labelText;

            controls.Add(label);
            controls.Add(dropDown);

            if (required)
            {
                CompareValidator compareValidator = GetValidator<CompareValidator>(new CompareValidator(), id, dropDown.ID, errorMessage, ValidationGroup, ValidatorDisplay.Dynamic);
                compareValidator.ValueToCompare = dropDown.Items[0].Value;
                compareValidator.Operator = ValidationCompareOperator.NotEqual;
                controls.Add(compareValidator);
            }

            return controls;
        }
        protected List<WebControl> GetOptinControl(string nodePrefix, string id, string labelText, XmlNodeList childNodes, CheckBox parent)
        {
            List<WebControl> controls = new List<WebControl>();
            CheckBox checkBox = new CheckBox();
            checkBox.ID = (nodePrefix == null ? "" : nodePrefix + "_") + id;
            Label label = new Label();
            label.CssClass = LabelCssClass;
            label.AssociatedControlID = checkBox.ID;
            label.Text = labelText;
            Panel p = new Panel();
            p.CssClass = "checkbox";
            p.Controls.Add(checkBox);
            p.Controls.Add(label);
            controls.Add(p);
            return controls;
        }

        protected List<WebControl> GetCheckboxControls(string id, string labelText, XmlNodeList childNodes, CheckBox parent)
        {
            List<WebControl> controls = new List<WebControl>();
            CheckBox checkBox = new CheckBox();

            checkBox.ID = id;
            checkBox.Text = labelText;

            Panel panel = new Panel();
            panel.ID = "p" + id;
            panel.CssClass = "hidden";

            foreach (XmlNode node in childNodes)
            {
                if (node.Name.ToLower() == "option")
                {
                    string childId = node.Attributes["id"] == null ? "" : node.Attributes["id"].Value;
                    string childText = node.Attributes["text"] == null ? "" : node.Attributes["text"].Value;

                    List<WebControl> panelControls = GetCheckboxControls(childId, childText, node.ChildNodes, checkBox);

                    foreach (WebControl control in panelControls)
                    {
                        panel.Controls.Add(control);
                    }
                }
            }

            if (panel.Controls.Count > 0)
            {
                checkBox.Attributes.Add("onclick", "SetDisplay('" + ControlClientID + "_" + panel.ClientID + "', this.checked);");
            }

            controls.Add(checkBox);

            if (panel.Controls.Count > 0)
            {
                controls.Add(panel);
            }

            return controls;
        }

        protected List<WebControl> GetRadioControls(string group, XmlNodeList values)
        {
            List<WebControl> outercontrols = new List<WebControl>();
            List<WebControl> controls = new List<WebControl>();

            foreach (XmlNode option in values)
            {
                string text = option.Attributes["text"] == null ? "" : option.Attributes["text"].Value;
                string value = option.Attributes["value"] == null ? "" : option.Attributes["value"].Value;
                string type = option.Attributes["type"] == null ? "" : option.Attributes["type"].Value;
                bool selected;

                bool.TryParse(option.Attributes["checked"] == null ? "false" : option.Attributes["checked"].Value, out selected);

                RadioButton radio = new RadioButton();
                radio.Text = text;
                radio.GroupName = group;
                radio.ID = value;
                radio.Checked = selected;


                controls.Add(radio);
                if (type.Equals("other"))
                {
                    TextBox tbOther = new TextBox();
                    tbOther.ID = value + "_other";
                    tbOther.CssClass = "other";
                    controls.Add(tbOther);
                }
            }

            Panel optionPanel = new Panel();
            optionPanel.CssClass = "options";
            foreach (WebControl control in controls)
            {
                optionPanel.Controls.Add(control);
                Literal br = new Literal();
                br.Text = "<br />";
                optionPanel.Controls.Add(br);
            }
            outercontrols.Add(optionPanel);

            return outercontrols;
        }

        protected List<WebControl> GetTextBoxControls(string id, string labelText)
        {
            List<WebControl> controls = new List<WebControl>();

            TextBox textBox = new TextBox();
            textBox.ID = _textBoxPrefix + id;
            textBox.CssClass = TextBoxCssClass;
            textBox.ValidationGroup = ValidationGroup;

            //Label label = new Label();
            //label.CssClass = LabelCssClass;
            //label.AssociatedControlID = textBox.ID;
            //label.Text = labelText;

            //controls.Add(label);
            controls.Add(textBox);

            return controls;
        }

        protected List<WebControl> GetTextBoxControls(string id, string labelText, string validationGroup, bool required, string requiredErrorMessage)
        {
            return GetTextBoxControls(id, labelText, validationGroup, required, requiredErrorMessage, null, null);
        }

        protected List<WebControl> GetTextBoxControls(string id, string labelText, string validationGroup, bool required, string requiredErrorMessage,
            string regexErrorMessage, string regexPattern)
        {
            List<WebControl> controls = GetTextBoxControls(id, labelText);
            string textBoxID = controls[0].ID;

            if (required)
            {
                RequiredFieldValidator requiredFieldValidator = GetValidator<RequiredFieldValidator>(new RequiredFieldValidator(),
                    _requiredFieldPrefix + id, textBoxID, requiredErrorMessage, validationGroup, ValidatorDisplay.Dynamic);
                controls.Add(requiredFieldValidator);
            }

            if (!string.IsNullOrEmpty(regexPattern))
            {
                RegularExpressionValidator regularExpressionValidator = GetValidator<RegularExpressionValidator>(new RegularExpressionValidator(),
                    _regexPrefix + id, textBoxID, regexErrorMessage, validationGroup, ValidatorDisplay.Dynamic);
                regularExpressionValidator.ValidationExpression = regexPattern;
                controls.Add(regularExpressionValidator);
            }

            return controls;
        }

        protected List<WebControl> GetTextBoxControls(string id, string labelText, string validationGroup, bool required, string requiredErrorMessage,
            string regexFormatErrorMessage, string regexLengthErrorMessage, string regexPattern, string regexLengthPattern)
        {
            List<WebControl> controls = GetTextBoxControls(id, labelText, validationGroup, required, requiredErrorMessage, regexFormatErrorMessage, regexPattern);
            string textBoxID = controls[0].ID;

            RegularExpressionValidator regularExpressionValidator = GetValidator<RegularExpressionValidator>(new RegularExpressionValidator(), _regexPrefix + id + _lengthSuffix,
                textBoxID, regexLengthErrorMessage, validationGroup, ValidatorDisplay.Dynamic);
            regularExpressionValidator.ValidationExpression = regexLengthPattern;

            return controls;
        }

        protected Panel GetQuestionPanel(List<Control> controls)
        {
            Panel panel = new Panel();
            panel.CssClass = DivCssClass;

            foreach (Control control in controls)
            {
                panel.Controls.Add(control);
            }

            return panel;
        }
    }
}