using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

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

        public TOprateLogModel GetModel(Expression<Func<TOprateLogModel, bool>> predicate)
        {
            return new TOprateLogDAL().GetModel(predicate);
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

        public int ProcessInsertData(int areaCode,  string targetDbName)
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
                var odal = new TOprateLogDAL(areaCode.ToString());
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
