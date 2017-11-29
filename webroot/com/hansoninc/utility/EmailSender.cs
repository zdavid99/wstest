using System;
using System.Net.Mail;

using log4net;
using System.Configuration;
using System.IO;
using System.Net.Mime;
using System.Web;

namespace com.hansoninc.utility
{
	/// <summary>
	/// 
	/// Properties (web.config settings):
	///		+ RelayServer ("mail.relayServer") 
	///          - this is the mail relay server
	///		+ MailDebug ("mail.debug") 
	///          - if set to "true" then the DebugAddress is used to override all email addresses (to and from) being sent.
	///     + DebugAddress ("mail.debug.address") 
	///          - the address that is used to override all email addresses in outgoing mail when MailDebug returns true
	/// 
	/// </summary>
	public class EmailSender
	{
        private static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private string relayServer;

        private static EmailSender self = new EmailSender();

        public static EmailSender Instance
        {
            get { return self; }
        }

		private EmailSender()
		{
			RelayServer = ConfigurationManager.AppSettings["mail.relayServer"];
		}


		public String DebugAddress
		{
			get { return ConfigurationManager.AppSettings["mail.debug.address"]; }
		}

		public Boolean MailDebug
		{
			get 
			{
				if (ConfigurationManager.AppSettings["mail.debug"].ToLower().Equals("true"))
				{
					return true;
				}
				return false;
			}
		}

        public String RelayServer
        {
            get { return this.relayServer; }
            private set { this.relayServer = value; }
        }

        public void SendEmail(string to, string from, string body, string subject)
        {
            SendEmail(to, from, body, subject, true);
        }

        public void SendEmail(string to, string from, string body, string subject, Boolean logEmail)
        {
            SendEmail(to, from, body, subject, null, logEmail);
        }

		public void SendEmail(string to, string from, string body, string subject, HttpPostedFile file)
		{
			Attachment attachment = null;
			if (file != null)
			{
				attachment = new Attachment(file.InputStream, file.ContentType);
				attachment.ContentDisposition.FileName = StripFilePath(file.FileName);
			}
			SendEmail(to, from, body, subject, attachment);
		}

        public void SendEmail(string to, string from, string body, string subject, Attachment attachment)
        {
            SendEmail(to, from, body, subject, attachment, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="to">comma separated list of recipient e-mail addresses</param>
        /// <param name="from">e-mail address of the sender</param>
        /// <param name="body">body of the e-mail message</param>
        /// <param name="subject">subject line of the e-mail message</param>
        /// <param name="attachment">email attachment</param>
        /// <param name="logEmail">override whether or not to log the email (useful for status page to set to false)</param>
        public void SendEmail(string to, string from, string body, string subject, Attachment attachment, Boolean logEmail)
		{
			MailMessage msgMail = null;
            SmtpClient smtp = null;

            try
            {
				if (MailDebug)
				{
					// the debug flag is set, override all addresses
					to = DebugAddress;
					from = DebugAddress;

					log.Warn("MailDebug flag is set to true, overriding all email addresses.");
				}

                if (RelayServer == null)
                {
                    log.Error("Relay Server not set!!!");
                    throw new ApplicationException("Email not sent, Relay Server not set!!!");
                }

                if (logEmail)
                {
                    log.Info("EMAIL:");
                    log.Info("  Relay Server: " + RelayServer);
                    log.Info("  to: " + to);
                    log.Info("  from: " + from);
                    log.Info("  subject: " + subject);
                    log.Info("  body: " + System.Environment.NewLine + body);
                }

                // create new mail message
                msgMail = new MailMessage(from, to, subject, body);
                msgMail.IsBodyHtml = false;

                if (attachment != null)
                {
                    msgMail.Attachments.Add(attachment);
                }
                smtp = new SmtpClient(RelayServer);

                // send mail
                smtp.Send(msgMail);

                if (logEmail)
                {
                    log.Info("Email Sent.");
                }
            }
            catch (Exception ex)
            {
                log.Error("Error sending email!");
                log.Error("  Relay Server: " + RelayServer);
                log.Error("  to: " + to);
                log.Error("  from: " + from);
                log.Error("  subject: " + subject);
                log.Error("  body: " + System.Environment.NewLine + body);
                
                throw new Exception("Error sending email.  Relay Server: " + RelayServer, ex);
            }
		}

		private String StripFilePath(String filePath)
		{
			int idx = filePath.LastIndexOf(@"\") + 1;

			if (idx >= 0)
			{
				filePath = filePath.Substring(idx, (filePath.Length - idx));
			}

			return filePath;
		}
	}
}
