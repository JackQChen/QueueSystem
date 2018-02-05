using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TLedWindowBLL
    {
        public TLedWindowBLL()
        {
        }

        #region CommonMethods

        public List<TLedWindowModel> GetModelList()
        {
            return new TLedWindowDAL().GetModelList();
        }

        public TLedWindowModel GetModel(int id)
        {
            return new TLedWindowDAL().GetModel(id);
        }

        public TLedWindowModel Insert(TLedWindowModel model)
        {
            return new TLedWindowDAL().Insert(model);
        }

        public int Update(TLedWindowModel model)
        {
            return new TLedWindowDAL().Update(model);
        }

        public int Delete(TLedWindowModel model)
        {
            return new TLedWindowDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TLedWindowDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TLedControllerDAL().GetGridData();
        }

        public object GetGridDataByControllerId(int controllerId)
        {
            return new TLedWindowDAL().GetGridDataByControllerId(controllerId);
        }
    }
}
