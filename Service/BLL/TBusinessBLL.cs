using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TBusinessBLL : IGridData, IUploadData
    {

        private TBusinessDAL dal;

        public TBusinessBLL()
        {
            this.dal = new TBusinessDAL();
        }

        public TBusinessBLL(string dbKey)
        {
            this.dal = new TBusinessDAL(dbKey);
        }

        #region CommonMethods

        public List<TBusinessModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TBusinessModel> GetModelList(Expression<Func<TBusinessModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TBusinessModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TBusinessModel GetModel(Expression<Func<TBusinessModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TBusinessModel Insert(TBusinessModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TBusinessModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TBusinessModel model)
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
                var sList = new TBusinessDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TBusinessDAL(targetDbName);
                var odal = new TBusinessDAL(areaCode.ToString());
                foreach (var s in sList)
                {
                    dal.Insert(s);
                    s.id = s.areaId;
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
                var sdal = new TBusinessDAL(areaCode.ToString());
                var tdal = new TBusinessDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.id;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.id).FirstOrDefault();
                    var data = s;
                    data.id = nData.id;
                    data.areaCode = nData.areaCode;
                    data.areaId = nData.areaId;
                    tdal.Update(data);
                    s.sysFlag = 2;
                    s.id = id;
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
