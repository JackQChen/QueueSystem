using System.Collections.Generic;
using DAL;
using Model;
using System;

namespace BLL
{
    public class TOprateLogBLL
    {
        public TOprateLogBLL()
        {
        }

        #region CommonMethods

        public List<TOprateLogModel> GetModelList()
        {
            return new TOprateLogDAL().GetModelList();
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
    }
}
