using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TLineUpMaxNoBLL : IUploadData
    {

        private TLineUpMaxNoDAL dal;

        public TLineUpMaxNoBLL()
        {
            this.dal = new TLineUpMaxNoDAL();
        }

        public TLineUpMaxNoBLL(string dbKey)
        {
            this.dal = new TLineUpMaxNoDAL(dbKey: dbKey);
        }

        #region CommonMethods

        public List<TLineUpMaxNoModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TLineUpMaxNoModel> GetModelList(Expression<Func<TLineUpMaxNoModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TLineUpMaxNoModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TLineUpMaxNoModel GetModel(Expression<Func<TLineUpMaxNoModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TLineUpMaxNoModel Insert(TLineUpMaxNoModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TLineUpMaxNoModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TLineUpMaxNoModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion

        public bool IsBasic
        {
            get { return false; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TLineUpMaxNoDAL(dbKey: areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TLineUpMaxNoDAL(dbKey: targetDbName);
                var odal = new TLineUpMaxNoDAL(dbKey: areaCode.ToString());
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
            return 0;
        }

        public int ProcessDeleteData(int areaCode, string targetDbName)
        {
            return 0;
        }
    }
}
