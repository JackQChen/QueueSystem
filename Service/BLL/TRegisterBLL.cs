using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TRegisterBLL
    {
        public TRegisterBLL()
        {
        }

        #region CommonMethods

        public List<TRegisterModel> GetModelList()
        {
            return new TRegisterDAL().GetModelList();
        }

        public TRegisterModel GetModel(int ID)
        {
            return new TRegisterDAL().GetModel(ID);
        }

        public TRegisterModel Insert(TRegisterModel model)
        {
            return new TRegisterDAL().Insert(model);
        }

        public int Update(TRegisterModel model)
        {
            return new TRegisterDAL().Update(model);
        }

        public int Delete(TRegisterModel model)
        {
            return new TRegisterDAL().Delete(model);
        }

        #endregion
    }
}
