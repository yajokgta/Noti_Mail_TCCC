using System;

namespace Noti_Mail_TCCC
{
    public class CARModel
    {
        public int MemoId { get; set; }
        public DateTime Preventive_Action { get; set; }
        public DateTime Audition { get; set; }
        public DateTime RequestDate { get; set; }
        public string Status_Name { get; set; }
        public int Preventive_Action_CountDay { get; set; }
        public int Audition_CountDay { get; set; }
        public int RequestDate_CountDay { get; set; }
        public string DocumentNo { get; set; }
        public string Requester { get; set; }

    }
}
