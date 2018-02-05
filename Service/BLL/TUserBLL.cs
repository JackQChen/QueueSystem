using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TUserBLL
    {
        public TUserBLL()
        {
        }

        #region CommonMethods

        public List<TUserModel> GetModelList()
        {
            return new TUserDAL().GetModelList();
        }

        public TUserModel GetModel(int ID)
        {
            return new TUserDAL().GetModel(ID);
        }

        public TUserModel Insert(TUserModel model)
        {
            return new TUserDAL().Insert(model);
        }

        public int Update(TUserModel model)
        {
            return new TUserDAL().Update(model);
        }

        public int Delete(TUserModel model)
        {
            return new TUserDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TUserDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TUserDAL().GetGridData();
        }
    }
}
