using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TWindowBLL
    {
        public TWindowBLL()
        {
        }

        #region CommonMethods

        public List<TWindowModel> GetModelList()
        {
            return new TWindowDAL().GetModelList();
        }

        public TWindowModel GetModel(int ID)
        {
            return new TWindowDAL().GetModel(ID);
        }

        public TWindowModel Insert(TWindowModel model)
        {
            return new TWindowDAL().Insert(model);
        }

        public int Update(TWindowModel model)
        {
            return new TWindowDAL().Update(model);
        }

        public int Delete(TWindowModel model)
        {
            return new TWindowDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TWindowDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TWindowDAL().GetGridData();
        }
    }
}
