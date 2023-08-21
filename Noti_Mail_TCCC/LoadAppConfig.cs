using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Noti_Mail_TCCC
{
    public class LoadAppConfig
    {
        public static string connectionString = ConfigurationSettings.AppSettings["ConnectionString"];
        public static string destDucumentCode = ConfigurationSettings.AppSettings["DestDocumentCode"];
        public static string mailToTestMode = ConfigurationSettings.AppSettings["MailToTestMode"];

        //SMTP Config
        public static string _SMTPServer = ConfigurationSettings.AppSettings["SMTPServer"];
        public static string _SMTPPort = ConfigurationSettings.AppSettings["SMTPPort"];
        public static string _SMTPEnableSSL = ConfigurationSettings.AppSettings["SMTPEnableSSL"];
        public static string _SMTPUser = ConfigurationSettings.AppSettings["SMTPUser"];
        public static string _SMTPPassword = ConfigurationSettings.AppSettings["SMTPPassword"];
        public static string _DisplayName = ConfigurationSettings.AppSettings["DisplayName"];
        //SMTP Config
    }
}
