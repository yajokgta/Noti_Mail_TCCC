using Noti_Mail_TCCC;
using System.Threading.Tasks;
using static Noti_Mail_TCCC.LoadAppConfig;

namespace Noti_Mail_TCCC
{
    public class CoreProcess
    {
        private readonly TCCCDataContext _dbContext = new TCCCDataContext(connectionString);
        public CoreProcess(TCCCDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task mainCore()
        {

        }
    }
}
