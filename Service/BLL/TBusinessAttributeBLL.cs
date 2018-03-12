using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TBusinessAttributeBLL : IGridData, IUploadData
    {

        private TBusinessAttributeDAL dal;

        public TBusinessAttributeBLL()
        {
            this.dal = new TBusinessAttributeDAL();
        }

        public TBusinessAttributeBLL(string dbKey)
        {
            this.dal = new TBusinessAttributeDAL(dbKey);
        }

        #region CommonMethods

        public List<TBusinessAttributeModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TBusinessAttributeModel> GetModelList(Expression<Func<TBusinessAttributeModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TBusinessAttributeModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TBusinessAttributeModel GetModel(Expression<Func<TBusinessAttributeModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TBusinessAttributeModel Insert(TBusinessAttributeModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TBusinessAttributeModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TBusinessAttributeModel model)
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

        public object GetGridDataByUnitSeq(string unitSeq)
        {
            return this.dal.GetGridDataByUnitSeq(unitSeq);
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            return this.dal.GetGridDetailData(unitSeq, busiSeq);
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TBusinessAttributeDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TBusinessAttributeDAL(targetDbName);
                var odal = new TBusinessAttributeDAL(areaCode.ToString());
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
                var sdal = new TBusinessAttributeDAL(areaCode.ToString());
                var tdal = new TBusinessAttributeDAL(targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.id;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.id).FirstOrDefault();
                    if (nData == null)
                    {
                        s.areaCode = areaCode;
                        s.areaId = s.id;
                        tdal.Insert(s);
                        s.id = s.areaId;
                        s.sysFlag = 2;
                        sdal.Update(s);
                    }
                    else
                    {
                        var data = s;
                        data.id = nData.id;
                        data.areaCode = nData.areaCode;
                        data.areaId = nData.areaId;
                        tdal.Update(data);
                        s.sysFlag = 2;
                        s.id = id;
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
