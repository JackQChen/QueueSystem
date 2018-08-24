using System;
using System.IO;
using BLL;
using Model;

namespace RateService
{
    public class RateProcess
    {

        const string serviceKey = "D840F2A3-C421-4B3A-B385-12B25727F70F";
        TWindowBLL winBll = new TWindowBLL();
        TUserBLL userBll = new TUserBLL();
        BCallBLL callBll = new BCallBLL();
        BEvaluateBLL evalBll = new BEvaluateBLL();

        public RateProcess()
        {
        }

        public object RS_GetWindowList()
        {
            return this.winBll.RS_GetWindowList();
        }

        public object RS_GetUserListByWindowNo(string winNum)
        {
            return this.winBll.RS_GetUserListByWindowNo(winNum);
        }

        public object GetUserPhoto(string userCode)
        {
            return this.winBll.RS_GetUserPhoto(userCode);
        }

        public string GetUserIdByCode(string userCode)
        {
            return this.userBll.GetModel(p => p.Code == userCode).ID.ToString();
        }

        public bool Login(string winNum, string userCode)
        {
            if (userCode == "QueueService" && winNum == serviceKey)
                return true;
            else
                return this.winBll.RS_GetModel(winNum, userCode) != null;
        }

        public bool RateSubmit(string WindowUser, string WindowNo, string RateId, string attitude, string quality, string efficiency, string honest)
        {
            try
            {
                //先不判断重复
                BCallModel cl = this.callBll.GetModelByHandleId(RateId);
                BEvaluateModel ev = new BEvaluateModel();
                //ev.ID = this.evalBll.GetMaxId();
                ev.type = 1;
                ev.handId = cl.ID;
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
                this.evalBll.Insert(ev);
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
