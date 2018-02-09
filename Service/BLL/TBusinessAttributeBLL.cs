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
        public TBusinessAttributeBLL()
        {
        }

        #region CommonMethods


        public List<TBusinessAttributeModel> GetModelList()
        {
            return new TBusinessAttributeDAL().GetModelList();
        }

        public List<TBusinessAttributeModel> GetModelList(Expression<Func<TBusinessAttributeModel, bool>> predicate)
        {
            return new TBusinessAttributeDAL().GetModelList(predicate);
        }

        public TBusinessAttributeModel GetModel(int id)
        {
            return new TBusinessAttributeDAL().GetModel(id);
        }

        public TBusinessAttributeModel GetModel(Expression<Func<TBusinessAttributeModel, bool>> predicate)
        {
            return new TBusinessAttributeDAL().GetModel(predicate);
        }

        public TBusinessAttributeModel Insert(TBusinessAttributeModel model)
        {
            return new TBusinessAttributeDAL().Insert(model);
        }

        public int Update(TBusinessAttributeModel model)
        {
            return new TBusinessAttributeDAL().Update(model);
        }

        public int Delete(TBusinessAttributeModel model)
        {
            return new TBusinessAttributeDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TBusinessAttributeDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TBusinessAttributeDAL().GetGridData();
        }

        public object GetGridDataByUnitSeq(string unitSeq)
        {
            return new TBusinessAttributeDAL().GetGridDataByUnitSeq(unitSeq);
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            return new TBusinessAttributeDAL().GetGridDetailData(unitSeq, busiSeq);
        }


        public bool IsBasic
        {
            get { return true; }
        }

        public bool ProcessInsertData(int areaCode, string targetDbName)
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
                var sdal = new TBusinessAttributeDAL(areaCode.ToString());
                var tdal = new TBusinessAttributeDAL(targetDbName);
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
