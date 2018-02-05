using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TGetCardBLL
    {
        public TGetCardBLL()
        {
        }

        #region CommonMethods

        public List<TGetCardModel> GetModelList()
        {
            return new TGetCardDAL().GetModelList();
        }

        public TGetCardModel GetModel(int id)
        {
            return new TGetCardDAL().GetModel(id);
        }

        public TGetCardModel Insert(TGetCardModel model)
        {
            return new TGetCardDAL().Insert(model);
        }

        public int Update(TGetCardModel model)
        {
            return new TGetCardDAL().Update(model);
        }

        public int Delete(TGetCardModel model)
        {
            return new TGetCardDAL().Delete(model);
        }

        #endregion
    }
}
