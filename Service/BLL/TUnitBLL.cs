using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TUnitBLL
    {
        public TUnitBLL()
        {
        }

        #region CommonMethods

        public List<TUnitModel> GetModelList()
        {
            return new TUnitDAL().GetModelList();
        }

        public TUnitModel GetModel(int ID)
        {
            return new TUnitDAL().GetModel(ID);
        }

        public TUnitModel Insert(TUnitModel model)
        {
            return new TUnitDAL().Insert(model);
        }

        public int Update(TUnitModel model)
        {
            return new TUnitDAL().Update(model);
        }

        public int Delete(TUnitModel model)
        {
            return new TUnitDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TUnitDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TUnitDAL().GetGridData();
        }
    }
}
