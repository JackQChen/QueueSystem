using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TLineUpMaxNoBLL
    {
        public TLineUpMaxNoBLL()
        {
        }

        #region CommonMethods

        public List<TLineUpMaxNoModel> GetModelList()
        {
            return new TLineUpMaxNoDAL().GetModelList();
        }

        public TLineUpMaxNoModel GetModel(int id)
        {
            return new TLineUpMaxNoDAL().GetModel(id);
        }

        public TLineUpMaxNoModel Insert(TLineUpMaxNoModel model)
        {
            return new TLineUpMaxNoDAL().Insert(model);
        }

        public int Update(TLineUpMaxNoModel model)
        {
            return new TLineUpMaxNoDAL().Update(model);
        }

        public int Delete(TLineUpMaxNoModel model)
        {
            return new TLineUpMaxNoDAL().Delete(model);
        }

        #endregion
    }
}
