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
        public TBusinessBLL()
        {
        }

        #region CommonMethods

        public List<TBusinessModel> GetModelList()
        {
            return new TBusinessDAL().GetModelList();
        }

        public List<TBusinessModel> GetModelList(Expression<Func<TBusinessModel, bool>> predicate)
        {
            return new TBusinessDAL().GetModelList(predicate);
        }

        public TBusinessModel GetModel(int id)
        {
            return new TBusinessDAL().GetModel(id);
        }

        public TBusinessModel Insert(TBusinessModel model)
        {
            return new TBusinessDAL().Insert(model);
        }

        public int Update(TBusinessModel model)
        {
            return new TBusinessDAL().Update(model);
        }

        public int Delete(TBusinessModel model)
        {
            return new TBusinessDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TBusinessDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TBusinessDAL().GetGridData();
        }

        
        public bool IsBasic
        {
            get { return true; }
        }

        public bool ProcessInsertData(int areaCode,  string targetDbName)
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessDeleteData(int areaCode,  string targetDbName)
        {
            throw new System.NotImplementedException();
        }
    }
}
