using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TEvaluateBLL : IUploadData
    {
        public TEvaluateBLL()
        {
        }

        #region CommonMethods


        public List<TEvaluateModel> GetModelList()
        {
            return new TEvaluateDAL().GetModelList();
        }

        public List<TEvaluateModel> GetModelList(Expression<Func<TEvaluateModel, bool>> predicate)
        {
            return new TEvaluateDAL().GetModelList(predicate);
        }

        public TEvaluateModel GetModel(int id)
        {
            return new TEvaluateDAL().GetModel(id);
        }

        public TEvaluateModel GetModel(Expression<Func<TEvaluateModel, bool>> predicate)
        {
            return new TEvaluateDAL().GetModel(predicate);
        }

        public TEvaluateModel Insert(TEvaluateModel model)
        {
            return new TEvaluateDAL().Insert(model);
        }

        public int Update(TEvaluateModel model)
        {
            return new TEvaluateDAL().Update(model);
        }

        public int Delete(TEvaluateModel model)
        {
            return new TEvaluateDAL().Delete(model);
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
                var sList = new TEvaluateDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TEvaluateDAL(targetDbName);
                var odal = new TEvaluateDAL(areaCode.ToString());
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
