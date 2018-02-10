using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TWindowBusinessBLL : IGridData, IUploadData
    {
        public TWindowBusinessBLL()
        {
        }

        #region CommonMethods


        public List<TWindowBusinessModel> GetModelList()
        {
            return new TWindowBusinessDAL().GetModelList();
        }

        public List<TWindowBusinessModel> GetModelList(Expression<Func<TWindowBusinessModel, bool>> predicate)
        {
            return new TWindowBusinessDAL().GetModelList(predicate);
        }

        public TWindowBusinessModel GetModel(int id)
        {
            return new TWindowBusinessDAL().GetModel(id);
        }

        public TWindowBusinessModel GetModel(Expression<Func<TWindowBusinessModel, bool>> predicate)
        {
            return new TWindowBusinessDAL().GetModel(predicate);
        }

        public TWindowBusinessModel Insert(TWindowBusinessModel model)
        {
            return new TWindowBusinessDAL().Insert(model);
        }

        public int Update(TWindowBusinessModel model)
        {
            return new TWindowBusinessDAL().Update(model);
        }

        public int Delete(TWindowBusinessModel model)
        {
            return new TWindowBusinessDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TWindowBusinessDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TWindowDAL().GetGridData();
        }

        public object GetGridDetailData(int winId)
        {
            return new TWindowBusinessDAL().GetGridDetailData(winId);
        }

      

        public bool IsBasic
        {
            get { return true; }
        }

        public int ProcessInsertData(int areaCode,  string targetDbName)
        {
            try
            {
                var sList = new TWindowBusinessDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TWindowBusinessDAL(targetDbName);
                var odal = new TWindowBusinessDAL(areaCode.ToString());
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

        public int ProcessUpdateData(int areaCode,  string targetDbName)
        {
            try
            {
                var sdal = new TWindowBusinessDAL(areaCode.ToString());
                var tdal = new TWindowBusinessDAL(targetDbName);
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

        public int ProcessDeleteData(int areaCode,  string targetDbName)
        {
            return 0;
        }
    }
}
