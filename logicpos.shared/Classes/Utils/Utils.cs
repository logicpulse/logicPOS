using Ionic.Zip;
using logicpos.shared.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace logicpos.shared.Classes.Utils
{
    public class Utils
    {
        //Log4Net
        private static log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //DotNetZipLib Helper Classes

        public static bool ZipPack(string[] pFiles, string pFileDestination)
        {
            string password = null;
            return ZipPack(pFiles, pFileDestination, password, EncryptionAlgorithm.WinZipAes256, CompressionMethod.BZip2, Ionic.Zlib.CompressionLevel.BestCompression);
        }

        public static bool ZipPack(string[] pFiles, string pFileDestination, string pPassword)
        {
            return ZipPack(pFiles, pFileDestination, pPassword, EncryptionAlgorithm.WinZipAes256, CompressionMethod.BZip2, Ionic.Zlib.CompressionLevel.BestCompression);
        }

        public static bool ZipPack(string[] pFiles, string pDestinationFileName, string pPassword, EncryptionAlgorithm pEncryptionAlgorithm, CompressionMethod pCompressionMethod, Ionic.Zlib.CompressionLevel pCompressionLevel)
        {
            bool debug = false;
            bool result = false;

            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    //Set zip properties except compression ones: Must be here
                    if (pPassword != string.Empty)
                    {
                        zip.Password = pPassword;
                        zip.Encryption = pEncryptionAlgorithm;
                    }

                    //Add Files
                    for (int i = 0; i < pFiles.Length; i++)
                    {
                        if (File.Exists(pFiles[i]))
                        {
                            if (debug) _log.Debug(string.Format("Add File:[{0}] to Zip:[{1}]", pFiles[i], pDestinationFileName));
                            zip.AddFile(pFiles[i], System.IO.Path.GetDirectoryName(pFiles[i]));
                        }
                        else
                        {
                            if (debug) _log.Debug(string.Format("Error Adding File:[{0}] to Zip:[{1}]. Cant Find the File", pFiles[i], pDestinationFileName));
                        }
                    }

                    //Set compression just before Save()
                    zip.CompressionMethod = pCompressionMethod;
                    zip.CompressionLevel = pCompressionLevel;
                    zip.Save(pDestinationFileName);

                    result = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                result = false;
            }
            return result;
        }

        public static bool ZipUnPack(string pFileName, string pDestinationPath, bool pFlattenFoldersOnExtract = false)
        {
            string password = null;
            return ZipUnPack(pFileName, pDestinationPath, password, pFlattenFoldersOnExtract);
        }

        public static bool ZipUnPack(string pFileName, string pDestinationPath, string pPassword, bool pFlattenFoldersOnExtract = false)
        {
            return ZipUnPack(pFileName, pDestinationPath, pPassword, ExtractExistingFileAction.OverwriteSilently, pFlattenFoldersOnExtract);
        }

        public static bool ZipUnPack(string pFileName, string pDestinationPath, string pPassword, ExtractExistingFileAction pExtractExistingFileAction, bool pFlattenFoldersOnExtract = false)
        {
            bool debug = false;
            bool result = false;

            try
            {
                using (ZipFile zip = ZipFile.Read(pFileName))
                {
                    zip.FlattenFoldersOnExtract = pFlattenFoldersOnExtract;

                    foreach (ZipEntry e in zip)
                    {
                        if (debug) _log.Debug(string.Format("Extract file: [{0}] from [{1}] to destinationPath: [{2}]", e.FileName, pFileName, pDestinationPath));
                        e.ExtractWithPassword(pDestinationPath, pExtractExistingFileAction, pPassword);
                    }
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message, ex);
                result = false;
            }
            return result;
        }

        //public static string GenerateRandomString(int size)
        //{
        //    var b = new byte[size];
        //    new RNGCryptoServiceProvider().GetBytes(b);
        //    return Encoding.ASCII.GetString(b);
        //}

        public static string GenerateRandomString(int size)
        {
            Random rand = new Random();
            string stringchars = "abcdefghijklmnopqrstuvwyxzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = stringchars[rand.Next(stringchars.Length)];
            }

            return new string(chars);
        }

        public static string GenerateRandomStringAlphaUpper(int size)
        {
            Random rand = new Random();
            string stringchars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            char[] chars = new char[size];
            for (int i = 0; i < size; i++)
            {
                chars[i] = stringchars[rand.Next(stringchars.Length)];
            }

            return new string(chars);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Email

        // Gmail Fix for The server response was: 5.5.1 Authentication Required
        // https://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
        // go to security settings at the followig link https://www.google.com/settings/security/lesssecureapps and enable less secure apps . So that you will be able to login from all apps.
        // https://stackoverflow.com/questions/20906077/gmail-error-the-smtp-server-requires-a-secure-connection-or-the-client-was-not
        //
        //How can I send emails through SSL SMTP with the .NET Framework? - Implicit 465 ?????
        //https://stackoverflow.com/questions/1011245/how-can-i-send-emails-through-ssl-smtp-with-the-net-framework
        //with System.Net.Mail, use port 25 instead of 465:
        //You must set SSL=true and Port=25. Server responds to your request from unprotected 25 and then throws connection to protected 465.
        public static void SendMail(
            SmtpDeliveryMethod deliveryMethod, 
            string to, string cc, string bcc, string subject, string body, 
            List<string> attachmentFileNames, 
            int timeOut = 5
            )
        {
            // Get Defaults from GlobalFramework.PreferenceParameters
            string smtpServer = GlobalFramework.PreferenceParameters["SEND_MAIL_SMTP_SERVER"];
            string username  = GlobalFramework.PreferenceParameters["SEND_MAIL_SMTP_USERNAME"];
            string password  = GlobalFramework.PreferenceParameters["SEND_MAIL_SMTP_PASSWORD"];
            int port  = Convert.ToInt16(GlobalFramework.PreferenceParameters["SEND_MAIL_SMTP_PORT"]);
            bool enableSsl  = Convert.ToBoolean(GlobalFramework.PreferenceParameters["SEND_MAIL_SMTP_ENABLE_SSL"]);
            bool useHtmlBody = Convert.ToBoolean(GlobalFramework.PreferenceParameters["SEND_MAIL_FINANCE_DOCUMENTS_HTML_BODY"]);

            SendMail(smtpServer, username, password, port, enableSsl, deliveryMethod, to, cc, bcc, subject, body, useHtmlBody, attachmentFileNames, timeOut);
        }

        public static void SendMail(
            string smtpServer, string username, string password, int port, bool enableSsl, 
            SmtpDeliveryMethod deliveryMethod, 
            string to, string cc, string bcc, string subject, string body, bool htmlBody,
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
            smtpClient.DeliveryMethod = deliveryMethod ;
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

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Tokens

        // Override method to use more that on Dictionary, ex GlobalFramework.PreferenceParameters and Others 
        public static string replaceTextTokens(string input, List<Dictionary<string,string>> tokensDictionaryList)
        {
            Dictionary<string,string> tokensDictionary = new Dictionary<string, string>();

            // Convert List of Dictionary in one Dictionary
            foreach (Dictionary<string,string> itemList in tokensDictionaryList)
            {
                foreach (var item in itemList)
                {
                    tokensDictionary.Add(item.Key, item.Value);
                }
            }

            return replaceTextTokens(input, tokensDictionary);
        }

        public static string replaceTextTokens(string input, Dictionary<string,string> tokensDictionary)
        {
            string result = input;

            try
            {
                if (input != null)
                {
                    DateTime currentDate = DateTime.Now;
                    string replaceToken;

                    foreach (var item in tokensDictionary)
                    {
                        //replaceToken = token;//TODO check if it work $"${{{token}}}";
                        replaceToken = string.Format("${{{0}}}", item.Key);

                        if (result.Contains(replaceToken))
                        {
                            result = result.Replace(replaceToken, item.Value);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.Debug(ex.Message, ex);
            }

            return result;
        }
    }
}
