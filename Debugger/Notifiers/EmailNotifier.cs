using System;
using System.Net;
using System.Net.Mail;
using UnityEngine;

namespace Rietmon.Debugging
{
    public class EmailNotifier : DebugNotifier
    {
        private readonly SmtpClient smtpServer;

        private readonly string mailTitle;

        private readonly string emailFrom;

        private readonly string emailTo;

        public EmailNotifier(string emailFrom, string passwordEmailFrom, string emailTo, string mailTitle)
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (s, certificate, chain, sslPolicyErrors) => true;

            smtpServer = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(emailFrom, passwordEmailFrom),
                EnableSsl = true
            };

            this.mailTitle = mailTitle;

            this.emailFrom = emailFrom;

            this.emailTo = emailTo;
        }

        public void GlobalNotify()
        {
            var mail = new MailMessage();

            mail.From = new MailAddress(emailFrom);
            mail.To.Add(emailTo);
            mail.Subject = $"{mailTitle} {DateTime.Now.ToString("g")}";
            mail.Body = Debugger.Log;

            smtpServer.Send(mail);
        }
    }
}
