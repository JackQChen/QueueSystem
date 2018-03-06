using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TGetCardBLL : IUploadData
    {

        private TGetCardDAL dal;

        public TGetCardBLL()
        {
            this.dal = new TGetCardDAL();
        }

        public TGetCardBLL(string dbKey)
        {
            this.dal = new TGetCardDAL(dbKey);
        }

        #region CommonMethods

        public List<TGetCardModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TGetCardModel> GetModelList(Expression<Func<TGetCardModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TGetCardModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TGetCardModel GetModel(Expression<Func<TGetCardModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TGetCardModel Insert(TGetCardModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TGetCardModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TGetCardModel model)
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
                var sList = new TGetCardDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TGetCardDAL(targetDbName);
                var odal = new TGetCardDAL(areaCode.ToString());
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
