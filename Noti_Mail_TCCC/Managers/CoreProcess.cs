using Noti_Mail_TCCC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using static Noti_Mail_TCCC.AdvanceFormExt;
using static Noti_Mail_TCCC.LoadAppConfig;

namespace Noti_Mail_TCCC
{
    public class CoreProcess
    {
        private static TCCCDataContext _dbContext = new TCCCDataContext(connectionString);

        public CoreProcess(TCCCDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public static async Task coreProcess()
        {
            var templateModel = _dbContext.MSTTemplates.FirstOrDefault(x => x.DocumentCode == destDucumentCode);

            if (templateModel != null)
            {
                var listTrnMemo = _dbContext.TRNMemos.Where(x => x.TemplateId == templateModel.TemplateId && 
                (x.StatusName == Status._WaitForRequestorReview || x.StatusName != Status._Completed) 
                && x.StatusName != Status._Draft
                && x.StatusName != Status._Cancelled
                && x.StatusName != Status._Rejected).ToList();

                if (listTrnMemo.Any())
                {
                    var listCarModel = validateMemo(listTrnMemo).OrderByDescending(x => x.Status_Name).ToList();

                    if (listCarModel.Any())
                    {
                        foreach (var itemCar in listCarModel)
                        {
                            var listLineApprove = _dbContext.TRNLineApproves.Where(x => x.MemoId == itemCar.MemoId).Select(y => y.NameEn).ToList();

                            var mailTo = string.Join(",",listLineApprove);

                            if (itemCar.Status_Name == Status._WaitForRequestorReview && itemCar.RequestDate_CountDay >= 21 && itemCar.RequestDate_CountDay <= 23)
                            {
                                var mstEmailTemplate = _dbContext.MSTEmailTemplates.FirstOrDefault(x => x.FormState == CARType.Remind);
                            }

                            if (itemCar.Status_Name != Status._WaitForRequestorReview && itemCar.Audition_CountDay >= -5 && itemCar.Audition_CountDay <= -2)
                            {
                                var mstEmailTemplate = _dbContext.MSTEmailTemplates.FirstOrDefault(x => x.FormState == CARType.Audition);
                            }

                            if (itemCar.Status_Name != Status._WaitForRequestorReview && itemCar.Preventive_Action_CountDay >= -5 && itemCar.Preventive_Action_CountDay <= -2)
                            {
                                var mstEmailTemplate = _dbContext.MSTEmailTemplates.FirstOrDefault(x => x.FormState == CARType.Preventive);
                            }
                        }
                    }
                }

                else
                {
                    Console.WriteLine("Not Found Memo By Template");
                }
            }

            else
            {
                Console.WriteLine("Please Check DocumentCode in Config");
            }
        }

        public static List<CARModel> validateMemo(List<TRNMemo> listTrnMemo)
        {
            var listCarModel = new List<CARModel>();

            foreach(var trnMemo in listTrnMemo)
            {
                var advanceform = ToList(trnMemo.MAdvancveForm);
                var preventrive = advanceform.FirstOrDefault(x => x.label.Contains("กำหนดแล้วเสร็จ"));
                var audition = advanceform.FirstOrDefault(x => x.label.Contains("กำหนดแล้วเสร็จ (Due Date)"));

                var carModel = new CARModel();
                carModel.MemoId = trnMemo.MemoId;
                carModel.Status_Name = trnMemo.StatusName;
                carModel.RequestDate = trnMemo.RequestDate != null ? trnMemo.RequestDate.Value : new DateTime();
                carModel.DocumentNo= trnMemo.DocumentNo;
                carModel.Requester = trnMemo.RNameTh;
                carModel.Preventive_Action = convertDateTime(preventrive != null ? preventrive.value : "");
                carModel.Audition = convertDateTime(audition != null ? audition.value : "");

                carModel.RequestDate_CountDay = CountDateTime(carModel.RequestDate);
                carModel.Preventive_Action_CountDay = CountDateTime(carModel.Preventive_Action);
                carModel.Audition_CountDay = CountDateTime(carModel.Audition);

                listCarModel.Add(carModel);
            }

            return listCarModel;
        }

        public static DateTime convertDateTime(string targetDateTime)
        {
            DateTime dt = new DateTime();

            List<string> strsplit = new List<string>();

            if (!string.IsNullOrEmpty(targetDateTime))
            {
                if (targetDateTime.Contains(' '))
                {
                    strsplit = targetDateTime.Split(' ').ToList();
                }
                else if (targetDateTime.Contains('/'))
                {
                    strsplit = targetDateTime.Split('/').ToList();
                }

                if (strsplit[1].Length == 3)
                {
                    dt = Convert.ToDateTime(targetDateTime);
                }
                else if (strsplit[1].Length == 2)
                {
                    dt = DateTime.ParseExact(targetDateTime, "dd/MM/yyyy", null);
                }
            }
            
            return dt;
        }

        public static class Status
        {
            public static string _NewRequest = "New Request";
            public static string _WaitForRequestorReview = "Wait for Requestor Review";
            public static string _WaitForApprove = "Wait for Approve";
            public static string _WaitForComment = "Wait for Comment";
            public static string _Rework = "Rework";
            public static string _Draft = "Draft";
            public static string _Completed = "Completed";
            public static string _Rejected = "Rejected";
            public static string _Cancelled = "Cancelled";
            public static string _Pending = "Pending";
            public static string _RequestCancel = "Request Cancel";

            public static List<string> _EditableStatus = new List<string>() { Status._Draft, Status._Rework, Status._NewRequest, Status._WaitForRequestorReview };
        }

        public static int CountDateTime (DateTime dt)
        {
            TimeSpan difference = DateTime.Now.Subtract(dt);
            var count = (int)difference.TotalDays;
            return count;
        }

        public static string ReplaceEmilBody(string body, CARModel item, string type)
        {
            body = body.Replace("[DOCUMENTNO]",item.DocumentNo).Replace("[REQUESTER]",item.Requester);

            switch (type)
            {
                case CARType.Remind:
                    body.Replace("[DUEDATE]", item.RequestDate.AddDays(20).ToString("dd/MMM/yyyy"));
                    break;

                case CARType.Audition:
                    body.Replace("[DUEDATE]", item.Audition.ToString("dd/MMM/yyyy"));
                    break;

                case CARType.Preventive:
                    body.Replace("[DUEDATE]", item.Preventive_Action.ToString("dd/MMM/yyyy"));
                    break;
            }

            return body;
        }

        public static string ReplaceEmilSubject(string subject, CARModel item)
        {
            subject = subject.Replace("[DOCUMENTNO]",item.DocumentNo);

            return subject;
        }

        public class CARType
        {
            public const string Remind = "Remind";
            public const string Audition = "Audition";
            public const string Preventive = "Preventive";
        }
    }
}
