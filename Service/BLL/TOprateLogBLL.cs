using System.Collections.Generic;
using DAL;
using Model;
using System;
using System.Linq;
using System.Linq.Expressions;
namespace BLL
{
    public class TOprateLogBLL : IUploadData
    {
        public TOprateLogBLL()
        {
        }

        #region CommonMethods

        public List<TOprateLogModel> GetModelList()
        {
            return new TOprateLogDAL().GetModelList();
        }

        public List<TOprateLogModel> GetModelList(Expression<Func<TOprateLogModel, bool>> predicate)
        {
            return new TOprateLogDAL().GetModelList(predicate);
        }

        public TOprateLogModel GetModel(int id)
        {
            return new TOprateLogDAL().GetModel(id);
        }

        public TOprateLogModel Insert(TOprateLogModel model)
        {
            return new TOprateLogDAL().Insert(model);
        }

        public int Update(TOprateLogModel model)
        {
            return new TOprateLogDAL().Update(model);
        }

        public int Delete(TOprateLogModel model)
        {
            return new TOprateLogDAL().Delete(model);
        }

        #endregion

        public Dictionary<string, List<string>> GetQueryParams()
        {
            return new TOprateLogDAL().GetQueryParams();
        }

        public object Query(string tType, string tName, string oType, DateTime dtStart, DateTime dtEnd, string log)
        {
            return new TOprateLogDAL().Query(tType, tName, oType, dtStart, dtEnd, log);
        }

        
        public bool IsBasic
        {
            get { return false; }
        }

        public bool ProcessInsertData(int areaCode,  string targetDbName)
        {
            try
            {
                var sList = new TOprateLogDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TOprateLogDAL(targetDbName);
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
