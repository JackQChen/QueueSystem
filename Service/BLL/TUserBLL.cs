using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;
namespace BLL
{
    public class TUserBLL : IGridData, IUploadData
    {
        public TUserBLL()
        {
        }

        #region CommonMethods

        public List<TUserModel> GetModelList()
        {
            return new TUserDAL().GetModelList();
        }

        public List<TUserModel> GetModelList(Expression<Func<TUserModel, bool>> predicate)
        {
            return new TUserDAL().GetModelList(predicate);
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

       
        public bool IsBasic
        {
            get { return true; }
        }

        public bool ProcessInsertData(int areaCode,  string targetDbName)
        {
            try
            {
                var sList = new TUserDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TUserDAL(targetDbName);
                foreach (var s in sList)
                {
                    dal.Insert(s);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessUpdateData(int areaCode,  string targetDbName)
        {
            try
            {
                var sdal = new TUserDAL(areaCode.ToString());
                var tdal = new TUserDAL(targetDbName);
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

        public bool ProcessDeleteData(int areaCode,  string targetDbName)
        {
            return true;
        }
    }
}
