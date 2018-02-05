using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TWindowUserBLL
    {
        public TWindowUserBLL()
        {
        }

        #region CommonMethods

        public List<TWindowUserModel> GetModelList()
        {
            return new TWindowUserDAL().GetModelList();
        }
        public TWindowUserModel GetModel(int id)
        {
            return new TWindowUserDAL().GetModel(id);
        }

        public TWindowUserModel Insert(TWindowUserModel model)
        {
            return new TWindowUserDAL().Insert(model);
        }

        public int Update(TWindowUserModel model)
        {
            return new TWindowUserDAL().Update(model);
        }

        public int Delete(TWindowUserModel model)
        {
            return new TWindowUserDAL().Delete(model);
        }

        #endregion

        public TWindowUserModel GetModel(int wid, int uid)
        {
            return new TWindowUserDAL().GetModel(wid, uid);
        }

        public void ResetIndex()
        {
            new TWindowUserDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TWindowUserDAL().GetGridData();
        }

        public object GetGridDetailData(int winId)
        {
            return new TWindowUserDAL().GetGridDetailData(winId);
        }

        //RateService相关
        public object RS_GetDataList()
        {
            return new TWindowUserDAL().RS_GetDataList();
        }
        public string RS_GetUserPhoto(string userCode)
        {
            return new TWindowUserDAL().RS_GetUserPhoto(userCode);
        }

        public TWindowUserModel RS_GetModel(string winNum, string userCode)
        {
            return new TWindowUserDAL().RS_GetModel(winNum, userCode);
        }
    }
}
