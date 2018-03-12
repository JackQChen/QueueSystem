using System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TWindowBLL : IGridData, IUploadData
    {

        private TWindowDAL dal;

        public TWindowBLL()
        {
            this.dal = new TWindowDAL();
        }

        public TWindowBLL(string dbKey)
        {
            this.dal = new TWindowDAL(dbKey);
        }

        #region CommonMethods

        public List<TWindowModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TWindowModel> GetModelList(Expression<Func<TWindowModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TWindowModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TWindowModel GetModel(Expression<Func<TWindowModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TWindowModel Insert(TWindowModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TWindowModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TWindowModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.dal.ResetIndex();
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TWindowDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TWindowDAL(targetDbName);
                var odal = new TWindowDAL(areaCode.ToString());
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
                var sdal = new TWindowDAL(areaCode.ToString());
                var tdal = new TWindowDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.ID;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.ID).FirstOrDefault();
                    if (nData == null)
                    {
                        s.areaCode = areaCode;
                        s.areaId = s.ID;
                        tdal.Insert(s);
                        s.ID = s.areaId;
                        s.sysFlag = 2;
                        sdal.Update(s);
                    }
                    else
                    {
                        var data = s;
                        data.ID = nData.ID;
                        data.areaCode = nData.areaCode;
                        data.areaId = nData.areaId;
                        tdal.Update(data);
                        s.sysFlag = 2;
                        s.ID = id;
                        sdal.Update(s);
                    }
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
