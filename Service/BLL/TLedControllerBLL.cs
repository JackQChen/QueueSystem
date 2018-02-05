using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TLedControllerBLL
    {
        public TLedControllerBLL()
        {
        }

        #region CommonMethods

        public List<TLedControllerModel> GetModelList()
        {
            return new TLedControllerDAL().GetModelList();
        }

        public TLedControllerModel GetModel(int id)
        {
            return new TLedControllerDAL().GetModel(id);
        }

        public TLedControllerModel Insert(TLedControllerModel model)
        {
            return new TLedControllerDAL().Insert(model);
        }

        public int Update(TLedControllerModel model)
        {
            return new TLedControllerDAL().Update(model);
        }

        public int Delete(TLedControllerModel model)
        {
            return new TLedControllerDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TLedControllerDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TLedControllerDAL().GetGridData();
        }
    }
}
