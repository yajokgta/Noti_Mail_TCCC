using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Noti_Mail_TCCC
{
    public class Program
    {

        static async Task Main(string[] args)
        {
            await CoreProcess.coreProcess();
        }
    }
}
