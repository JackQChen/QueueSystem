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

        private TWindowUserDAL dal;

        public TWindowUserBLL()
        {
            this.dal = new TWindowUserDAL();
        }

        public TWindowUserBLL(string dbKey)
        {
            this.dal = new TWindowUserDAL(dbKey);
        }

        #region CommonMethods

        public List<TWindowUserModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TWindowUserModel> GetModelList(Expression<Func<TWindowUserModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TWindowUserModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TWindowUserModel GetModel(Expression<Func<TWindowUserModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TWindowUserModel Insert(TWindowUserModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TWindowUserModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TWindowUserModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion

        public TWindowUserModel GetModel(int wid, int uid)
        {
            return this.dal.GetModel(wid, uid);
        }

        public void ResetIndex()
        {
            this.dal.ResetIndex();
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }

        public object GetGridDetailData(int winId)
        {
            return this.dal.GetGridDetailData(winId);
        }

        //RateService相关
        public object RS_GetDataList()
        {
            return this.dal.RS_GetDataList();
        }
        public string RS_GetUserPhoto(string userCode)
        {
            return this.dal.RS_GetUserPhoto(userCode);
        }

        public TWindowUserModel RS_GetModel(string winNum, string userCode)
        {
            return this.dal.RS_GetModel(winNum, userCode);
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
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
                return sList.Count;
            }
            catch
            {
                return -1;
            }
        }

        public int ProcessUpdateData(int areaCode, string targetDbName)
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
                return sList.Count;
            }
            catch
            {
                return -1;
            }
        }

        public int ProcessDeleteData(int areaCode, string targetDbName)
        {
            return 0;
        }
    }
}
