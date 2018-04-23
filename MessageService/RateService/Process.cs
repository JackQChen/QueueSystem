using BLL;
using Model;
using System;
using System.IO;

namespace RateService
{
    public class Process
    {
        TCallBLL cBll = new TCallBLL();
        TWindowBLL winUserBll = new TWindowBLL();
        TEvaluateBLL eBll = new TEvaluateBLL();
        const string serviceKey = "D840F2A3-C421-4B3A-B385-12B25727F70F";

        public Process()
        {
        }

        public object RS_GetUnitList()
        {
            return this.winUserBll.RS_GetUnitList();
        }

        public object RS_GetWindowListByUnitSeq(string unitSeq)
        {
            return this.winUserBll.RS_GetWindowListByUnitSeq(unitSeq);
        }

        public object RS_GetUserListByUnitSeq(string unitSeq)
        {
            return this.winUserBll.RS_GetUserListByUnitSeq(unitSeq);
        }

        public object GetUserPhoto(string userCode)
        {
            return this.winUserBll.RS_GetUserPhoto(userCode);
        }

        public bool Login(string winNum, string userCode)
        {
            if (userCode == "QueueService" && winNum == serviceKey)
                return true;
            else
                return this.winUserBll.RS_GetModel(winNum, userCode) != null;
        }

        public bool RateSubmit(string WindowUser, string WindowNo, string RateId, string attitude, string quality, string efficiency, string honest)
        {
            try
            {
                //先不判断重复
                TCallModel cl = cBll.GetModelByHandleId(RateId);
                TEvaluateModel ev = new TEvaluateModel();
                ev.type = 1;
                ev.handId = cl.id;
                ev.unitSeq = cl.unitSeq;
                ev.windowNumber = WindowNo;
                ev.handleTime = DateTime.Now;
                ev.custCardId = cl.idCard;
                ev.name = cl.qNmae;
                ev.windowUser = WindowUser;
                ev.approveSeq = cl.busiSeq;
                ev.evaluateAttitude = int.Parse(attitude);
                ev.evaluateEfficiency = int.Parse(efficiency);
                ev.evaluateHonest = int.Parse(honest);
                ev.evaluateQuality = int.Parse(quality);
                eBll.Insert(ev);
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log.txt", ex.Message + (ex.InnerException == null ? "" : ("\r\n" + ex.InnerException.Message)));
                return false;
            }
            return true;
        }

    }
}
