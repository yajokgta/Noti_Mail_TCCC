using System;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using static Noti_Mail_TCCC.LoadAppConfig;

namespace Noti_Mail_TCCC
{
    internal class SendEmail
    {
        public static TCCCDataContext _dbcontext = new TCCCDataContext(connectionString);
        public SendEmail(TCCCDataContext dbcontext)
        {
            _dbcontext = dbcontext;
        }
        public static async Task sendEmail(string Body, string MailTo, string subject)
        {
            try
            {
                MailTo = await InfomationMailTo(MailTo);
                SmtpClient smtpClient = new SmtpClient();
                NetworkCredential basicCredential = new NetworkCredential(_SMTPUser, _SMTPPassword);
                MailMessage message = new MailMessage();
                MailAddress fromAddress = new MailAddress(_SMTPUser, _DisplayName);

                smtpClient.Host = _SMTPServer;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = basicCredential;
                smtpClient.EnableSsl = Convert.ToBoolean(_SMTPEnableSSL);
                smtpClient.Port = Convert.ToInt32(_SMTPPort);

                message.From = fromAddress;
                //message.CC = sCc;
                message.Subject = subject;

                //Set IsBodyHtml to true means you can send HTML email.
                message.IsBodyHtml = true;

                message.Priority = MailPriority.High;
                message.Body = Body;

                string[] mail = MailTo.Split(',');

                foreach (string s in mail)
                {
                    if (s != "")
                    {
                        message.Bcc.Add(new MailAddress(s));
                    }
                }

                message.Sender = fromAddress;
                await smtpClient.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static async Task<string> InfomationMailTo (string To)
        {
            var result = "";
            

            List<string> email = new List<string>();
            if (To.Contains(","))
            {
                string[] obj = To.Split(',').Distinct().ToArray();
                foreach (var item in obj)
                {
                    if (item.Contains("@"))
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            email.Add(item.Trim());
                        }
                    }

                    else
                    {
                        if (!string.IsNullOrEmpty(item.Trim()))
                        {
                            if (Regex.IsMatch(item, "^[a-zA-Z0-9]*$"))
                            {
                                var objemp = _dbcontext.MSTEmployees.FirstOrDefault(x => x.NameEn == item.Trim());
                                if (!string.IsNullOrEmpty(objemp.Email))
                                {
                                    email.Add(objemp.Email.Trim());
                                }
                            }
                            else
                            {
                                var objemp = _dbcontext.MSTEmployees.FirstOrDefault(x => x.NameTh == item.Trim());
                                if (!string.IsNullOrEmpty(objemp.Email))
                                {
                                    email.Add(objemp.Email.Trim());
                                }
                            }
                        }
                    }
                }
            }

            else
            {
                if (To.Contains("@"))
                {
                    if (!string.IsNullOrEmpty(To.Trim()))
                    {
                        email.Add(To.Trim());
                    }
                }

                else
                {
                    if (!string.IsNullOrEmpty(To.Trim()))
                    {
                        if (Regex.IsMatch(To, "^[a-zA-Z0-9]*$"))
                        {
                            var objemp = _dbcontext.MSTEmployees.FirstOrDefault(x => x.NameEn == To.Trim());
                            if (!string.IsNullOrEmpty(objemp.Email))
                            {
                                email.Add(objemp.Email.Trim());
                            }
                        }

                        else
                        {
                            var objemp = _dbcontext.MSTEmployees.FirstOrDefault(x => x.NameTh == To.Trim());
                            if (!string.IsNullOrEmpty(objemp.Email))
                            {
                                email.Add(objemp.Email.Trim());
                            }
                        }
                    }
                }
            }

            
            result = String.Join(",", email);
            return result;
        }
    }
}
