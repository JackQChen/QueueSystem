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
        public TLineUpMaxNoBLL()
        {
        }

        #region CommonMethods

        public List<TLineUpMaxNoModel> GetModelList()
        {
            return new TLineUpMaxNoDAL().GetModelList();
        }

        public List<TLineUpMaxNoModel> GetModelList(Expression<Func<TLineUpMaxNoModel, bool>> predicate)
        {
            return new TLineUpMaxNoDAL().GetModelList(predicate);
        }

        public TLineUpMaxNoModel GetModel(int id)
        {
            return new TLineUpMaxNoDAL().GetModel(id);
        }

        public TLineUpMaxNoModel Insert(TLineUpMaxNoModel model)
        {
            return new TLineUpMaxNoDAL().Insert(model);
        }

        public int Update(TLineUpMaxNoModel model)
        {
            return new TLineUpMaxNoDAL().Update(model);
        }

        public int Delete(TLineUpMaxNoModel model)
        {
            return new TLineUpMaxNoDAL().Delete(model);
        }

        #endregion



        public bool IsBasic
        {
            get { return false; }
        }

        public bool ProcessInsertData(int areaCode,  string targetDbName)
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
            return true;
        }

        public bool ProcessDeleteData(int areaCode,  string targetDbName)
        {
            return true;
        }
    }
}
