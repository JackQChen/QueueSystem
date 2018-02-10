using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TRegisterBLL : IUploadData
    {
        public TRegisterBLL()
        {
        }

        #region CommonMethods


        public List<TRegisterModel> GetModelList()
        {
            return new TRegisterDAL().GetModelList();
        }

        public List<TRegisterModel> GetModelList(Expression<Func<TRegisterModel, bool>> predicate)
        {
            return new TRegisterDAL().GetModelList(predicate);
        }

        public TRegisterModel GetModel(int id)
        {
            return new TRegisterDAL().GetModel(id);
        }

        public TRegisterModel GetModel(Expression<Func<TRegisterModel, bool>> predicate)
        {
            return new TRegisterDAL().GetModel(predicate);
        }

        public TRegisterModel Insert(TRegisterModel model)
        {
            return new TRegisterDAL().Insert(model);
        }

        public int Update(TRegisterModel model)
        {
            return new TRegisterDAL().Update(model);
        }

        public int Delete(TRegisterModel model)
        {
            return new TRegisterDAL().Delete(model);
        }

        #endregion

        public bool IsBasic
        {
            get { return false; }
        }

        public int ProcessInsertData(int areaCode,  string targetDbName)
        {
            try
            {
                var sList = new TRegisterDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TRegisterDAL(targetDbName);
                var odal = new TRegisterDAL(areaCode.ToString());
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

        public int ProcessUpdateData(int areaCode,  string targetDbName)
        {
            return 0;
        }

        public int ProcessDeleteData(int areaCode,  string targetDbName)
        {
            return 0;
        }
    }
}
