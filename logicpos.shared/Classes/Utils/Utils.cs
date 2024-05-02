using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;

namespace logicpos.shared.Classes.Utils
{
    public class Utils
    {
        public static void SendMail(
            SmtpDeliveryMethod deliveryMethod,
            string to, 
            string cc, 
            string bcc, 
            string subject, 
            string body,
            List<string> attachmentFileNames,
            int timeOut = 5
            )
        {
            // Get Defaults fromLogicPOS.Settings.AppSettings.PreferenceParameters
            string smtpServer = LogicPOS.Settings.AppSettings.PreferenceParameters["SEND_MAIL_SMTP_SERVER"];
            string username = LogicPOS.Settings.AppSettings.PreferenceParameters["SEND_MAIL_SMTP_USERNAME"];
            string password = LogicPOS.Settings.AppSettings.PreferenceParameters["SEND_MAIL_SMTP_PASSWORD"];
            int port = Convert.ToInt16(LogicPOS.Settings.AppSettings.PreferenceParameters["SEND_MAIL_SMTP_PORT"]);
            bool enableSsl = Convert.ToBoolean(LogicPOS.Settings.AppSettings.PreferenceParameters["SEND_MAIL_SMTP_ENABLE_SSL"]);
            bool useHtmlBody = Convert.ToBoolean(LogicPOS.Settings.AppSettings.PreferenceParameters["SEND_MAIL_FINANCE_DOCUMENTS_HTML_BODY"]);

            SendMail(
                smtpServer, 
                username, 
                password, 
                port, 
                enableSsl, 
                deliveryMethod, 
                to, 
                cc, 
                bcc, 
                subject, 
                body, 
                useHtmlBody, 
                attachmentFileNames, 
                timeOut);
        }

        public static void SendMail(
            string smtpServer, 
            string username, 
            string password, 
            int port, 
            bool enableSsl,
            SmtpDeliveryMethod deliveryMethod,
            string to, 
            string cc, 
            string bcc, 
            string subject, 
            string body, 
            bool htmlBody,
            List<string> attachmentFileNames,
            int timeOut = 5
            )
        {
            SmtpClient smtpClient = new SmtpClient();
            NetworkCredential basicCredential = new NetworkCredential(username, password);
            MailMessage message = new MailMessage();
            MailAddress fromAddress = new MailAddress(username);

            // This tricks is what makes ports 465 work with .net framework, here we override 465 with 25, to force throws connection to protected 465
            if (port == 465) port = 25;

            smtpClient.Host = smtpServer;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Timeout = (timeOut * 1000);
            // Required for GMail 587,true,SmtpDeliveryMethod.Network
            smtpClient.Port = port;
            smtpClient.EnableSsl = enableSsl;
            smtpClient.DeliveryMethod = deliveryMethod;
            // Required to be the last to prevent 5.5.1 Authentication Required, this way we dont need to "enable less secure apps" :)
            smtpClient.Credentials = basicCredential;

            message.From = fromAddress;
            message.Subject = subject;
            message.IsBodyHtml = false;
            message.Body = body;
            if (!string.IsNullOrEmpty(to)) message.To.Add(to);
            if (!string.IsNullOrEmpty(cc)) message.CC.Add(cc);
            if (!string.IsNullOrEmpty(bcc)) message.Bcc.Add(bcc);

            // Enable Html Body
            message.IsBodyHtml = htmlBody;

            // Add Attachment
            foreach (var item in attachmentFileNames)
            {
                message.Attachments.Add(new Attachment(item));
            }

            smtpClient.Send(message);
        }


    }
}
