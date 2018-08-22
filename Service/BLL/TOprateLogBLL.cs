using System;
using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TOprateLogBLL : BLLBase<TOprateLogDAL, TOprateLogModel>
    {
        public TOprateLogBLL()
            : base()
        {
        }

        public TOprateLogBLL(string connName)
            : base(connName)
        {
        }

        public TOprateLogBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public Dictionary<string, List<string>> GetQueryParams()
        {
            return this.CreateDAL().GetQueryParams();
        }

        public object Query(string tType, string tName, string oType, DateTime dtStart, DateTime dtEnd, string log)
        {
            return this.CreateDAL().Query(tType, tName, oType, dtStart, dtEnd, log);
        }
    }
}
