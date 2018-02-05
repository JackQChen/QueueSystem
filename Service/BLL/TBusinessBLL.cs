using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TBusinessBLL
    {
        public TBusinessBLL()
        {
        }

        #region CommonMethods

        public List<TBusinessModel> GetModelList()
        {
            return new TBusinessDAL().GetModelList();
        }

        public TBusinessModel GetModel(int id)
        {
            return new TBusinessDAL().GetModel(id);
        }

        public TBusinessModel Insert(TBusinessModel model)
        {
            return new TBusinessDAL().Insert(model);
        }

        public int Update(TBusinessModel model)
        {
            return new TBusinessDAL().Update(model);
        }

        public int Delete(TBusinessModel model)
        {
            return new TBusinessDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TBusinessDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TBusinessDAL().GetGridData();
        }
    }
}
