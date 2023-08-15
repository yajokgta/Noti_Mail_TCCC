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
    }
}
