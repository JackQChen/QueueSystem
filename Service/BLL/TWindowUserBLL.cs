using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TWindowUserBLL : IGridData, IUploadData
    {
        public TWindowUserBLL()
        {
        }

        #region CommonMethods


        public List<TWindowUserModel> GetModelList()
        {
            return new TWindowUserDAL().GetModelList();
        }

        public List<TWindowUserModel> GetModelList(Expression<Func<TWindowUserModel, bool>> predicate)
        {
            return new TWindowUserDAL().GetModelList(predicate);
        }

        public TWindowUserModel GetModel(int id)
        {
            return new TWindowUserDAL().GetModel(id);
        }

        public TWindowUserModel GetModel(Expression<Func<TWindowUserModel, bool>> predicate)
        {
            return new TWindowUserDAL().GetModel(predicate);
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


        public bool IsBasic
        {
            get { return true; }
        }

        public bool ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TWindowUserDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TWindowUserDAL(targetDbName);
                var odal = new TWindowUserDAL(areaCode.ToString());
                foreach (var s in sList)
                {
                    dal.Insert(s);
                    s.ID = s.areaId;
                    s.sysFlag = 2;
                    odal.Update(s);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessUpdateData(int areaCode, string targetDbName)
        {
            try
            {
                var sdal = new TWindowUserDAL(areaCode.ToString());
                var tdal = new TWindowUserDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.ID;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.ID).FirstOrDefault();
                    var data = s;
                    data.ID = nData.ID;
                    data.areaCode = nData.areaCode;
                    data.areaId = nData.areaId;
                    tdal.Update(data);
                    s.sysFlag = 2;
                    s.ID = id;
                    sdal.Update(s);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessDeleteData(int areaCode, string targetDbName)
        {
            return true;
        }
    }
}
