using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TWindowAreaBLL
    {
        public TWindowAreaBLL()
        {
        }

        #region CommonMethods

        public List<TWindowAreaModel> GetModelList()
        {
            return new TWindowAreaDAL().GetModelList();
        }

        public TWindowAreaModel GetModel(int id)
        {
            return new TWindowAreaDAL().GetModel(id);
        }

        public TWindowAreaModel Insert(TWindowAreaModel model)
        {
            return new TWindowAreaDAL().Insert(model);
        }

        public int Update(TWindowAreaModel model)
        {
            return new TWindowAreaDAL().Update(model);
        }

        public int Delete(TWindowAreaModel model)
        {
            return new TWindowAreaDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TWindowAreaDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TWindowAreaDAL().GetGridData();
        }

    }
}
