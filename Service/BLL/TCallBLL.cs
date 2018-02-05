using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TCallBLL
    {
        public TCallBLL()
        {
        }

        #region CommonMethods

        public List<TCallModel> GetModelList()
        {
            return new TCallDAL().GetModelList();
        }

        public TCallModel GetModel(int id)
        {
            return new TCallDAL().GetModel(id);
        }

        public TCallModel Insert(TCallModel model)
        {
            return new TCallDAL().Insert(model);
        }

        public int Update(TCallModel model)
        {
            return new TCallDAL().Update(model);
        }

        public int Delete(TCallModel model)
        {
            return new TCallDAL().Delete(model);
        }

        #endregion

        public TCallModel CallNo(string unitSeq, string busiSeq, string windowNumber, string windowUser)
        {
            return new TCallDAL().CallNo(unitSeq, busiSeq, windowNumber, windowUser);
        }

        public TCallModel CallNo(List<TWindowBusinessModel> wlBusy, string windowNumber, string windowUser)
        {
            return new TCallDAL().CallNo(wlBusy, windowNumber, windowUser);
        }

        public List<TCallModel> ScreenShow()
        {
            return new TCallDAL().ScreenShow();
        }

        public List<TCallModel> ScreenShow(int AreaNo)
        {
            return new TCallDAL().ScreenShow(AreaNo);
        }
        public List<TCallModel> ScreenShowByArea(string AreaNo)
        {
            return new TCallDAL().ScreenShowByArea(AreaNo);
        }
        public TCallModel GetModelByHandleId(string handleId)
        {
            return new TCallDAL().GetModelByHandleId(handleId);
        }

        /// <summary>
        /// 根据窗口号返回叫号数据
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<TCallModel> GetCall(string windowNo, string ticket)
        {
            return new TCallDAL().GetCall(windowNo, ticket);
        }

        public List<TCallModel> GiveUpAll()
        {
            return new TCallDAL().GiveUpAll();
        }

        public bool Transfer(TCallModel call)
        {
            return new TCallDAL().Transfer(call);
        }
    }
}
