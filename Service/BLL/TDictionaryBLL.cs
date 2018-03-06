using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TDictionaryBLL : IUploadData
    {

        private TDictionaryDAL dal;

        public TDictionaryBLL()
        {
            this.dal = new TDictionaryDAL();
        }

        public TDictionaryBLL(string dbKey)
        {
            this.dal = new TDictionaryDAL(dbKey: dbKey);
        }

        #region CommonMethods

        public List<TDictionaryModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TDictionaryModel> GetModelList(Expression<Func<TDictionaryModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TDictionaryModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TDictionaryModel GetModel(Expression<Func<TDictionaryModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TDictionaryModel Insert(TDictionaryModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TDictionaryModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TDictionaryModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion

        public List<TDictionaryModel> GetModelList(string name)
        {
            return this.dal.GetModelList(name);
        }

        public bool IsBasic
        {
            get { return false; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TDictionaryDAL(dbKey: areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.ID;
                });
                var dal = new TDictionaryDAL(dbKey: targetDbName);
                var odal = new TDictionaryDAL(dbKey: areaCode.ToString());
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
            return 0;
        }

        public int ProcessDeleteData(int areaCode, string targetDbName)
        {
            return 0;
        }
    }
}
