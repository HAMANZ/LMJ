using Anz.LMJ.BLO;
using Anz.LMJ.BLO.LookUpObjects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

namespace Anz.LMJ.BLL
{
    static public class Tools
    {
        private static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static void sendEmail(string emailTo, string subject, string body)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");

                mail.From = new MailAddress("", "LMJ");
                mail.To.Add(emailTo);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;

                SmtpServer.Port = 587;
                SmtpServer.Credentials = new System.Net.NetworkCredential("", "LMJ_2021");
                SmtpServer.EnableSsl = true;

                //SmtpServer.Send(mail);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }



        static public void WriteToFile(string Message, string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string filepath = AppDomain.CurrentDomain.BaseDirectory + "\\Logs\\ResponseLog_" + DateTime.Now.Date.ToShortDateString().Replace('/', '_') + ".txt";
            if (!File.Exists(filepath))
            {
                // Create a file to write to.   
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
            else
            {

                using (StreamWriter sw = File.AppendText(filepath))
                {
                    sw.WriteLine(Message);
                }
            }
        }

       
        static public List<LookUpAttributes> GetAttributes(string ClassName, string XMLPath)
        {
            List<LookUpAttributes> attributes = new List<LookUpAttributes>();
            LookUpAttributes attr = new LookUpAttributes();
            XmlDocument doc = new XmlDocument();


            try
            {
                doc.Load(XMLPath);
                XmlNode Lookups = doc.ChildNodes[1];

                foreach (XmlNode child in Lookups.ChildNodes)
                {

                    if (child.Attributes["Name"].Value == ClassName)
                    {
                        foreach (XmlNode node in child)
                        {
                            attr = new LookUpAttributes();
                            attr.Name = node.Attributes["Name"].Value;
                            attr.Code = node.Attributes["Code"].Value;
                            if (node.Attributes["isLangNull"].Value == "False")
                                attr.isLangNull = false;
                            else
                                attr.isLangNull = true;

                            if (node.Attributes["isMedia"].Value == "False")
                                attr.isMedia = false;
                            else
                                attr.isMedia = true;

                            if (node.Attributes["isMain"].Value == "False")
                                attr.isMain = false;
                            else
                                attr.isMain = true;

                            if (node.Attributes["isList"].Value == "False")
                                attr.isList = false;
                            else
                                attr.isList = true;

                            if (node.Attributes["isVideo"].Value == "False")
                                attr.isVideo = false;
                            else
                                attr.isVideo = true;


                            attributes.Add(attr);
                        }

                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            return attributes;
        }

        static public String getUUID()
        {
            return System.Guid.NewGuid().ToString();
        }

        static public String getUTCDateTime()
        {
            DateTime time = DateTime.Now.ToUniversalTime();
            return time.ToString("yyyy-MM-dd'T'HH:mm:ss'Z'");
        }
    }
}
