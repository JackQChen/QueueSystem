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
        public TGetCardBLL()
        {
        }

        #region CommonMethods

        public List<TGetCardModel> GetModelList()
        {
            return new TGetCardDAL().GetModelList();
        }

        public List<TGetCardModel> GetModelList(Expression<Func<TGetCardModel, bool>> predicate)
        {
            return new TGetCardDAL().GetModelList(predicate);
        }

        public TGetCardModel GetModel(int id)
        {
            return new TGetCardDAL().GetModel(id);
        }

        public TGetCardModel Insert(TGetCardModel model)
        {
            return new TGetCardDAL().Insert(model);
        }

        public int Update(TGetCardModel model)
        {
            return new TGetCardDAL().Update(model);
        }

        public int Delete(TGetCardModel model)
        {
            return new TGetCardDAL().Delete(model);
        }

        #endregion

        

        public bool IsBasic
        {
            get { return false; }
        }

        public bool ProcessInsertData(int areaCode,   string targetDbName)
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
