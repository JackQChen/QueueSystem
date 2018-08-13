using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TOprateLogDAL : DALBase<TOprateLogModel>
    {
        public TOprateLogDAL()
            : base()
        {
        }

        public TOprateLogDAL(string connName)
            : base(connName)
        {
        }

        public TOprateLogDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public Dictionary<string, List<string>> GetQueryParams()
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            dic.Add("tType", this.db.Query<TOprateLogModel>().GroupBy(k => k.oprateType).Select(s => s.oprateType).ToList());
            dic.Add("tName", this.db.Query<TOprateLogModel>().GroupBy(k => k.oprateFlag).Select(s => s.oprateFlag).ToList());
            dic.Add("oType", this.db.Query<TOprateLogModel>().GroupBy(k => k.oprateClassifyType).Select(s => s.oprateClassifyType).ToList());
            return dic;
        }

        public object Query(string tType, string tName, string oType, DateTime dtStart, DateTime dtEnd, string log)
        {
            return this.db.Query<TOprateLogModel>()
                  .Where(p => p.oprateType == tType || tType == "")
                  .Where(p => p.oprateFlag == tName || tName == "")
                  .Where(p => p.oprateClassifyType == oType || oType == "")
                  .Where(p => p.oprateTime > dtStart && p.oprateTime < dtEnd)
                  .Where(p => p.oprateLog.Contains(log))
                  .Select(s => s)
                  .ToList()
                  .Select(s => new
                  {
                      id = s.ID,
                      tType = s.oprateType,
                      tName = s.oprateFlag,
                      oType = s.oprateClassifyType,
                      datetime = s.oprateTime.ToString("yyyy-MM-dd HH:mm:ss"),
                      log = s.oprateLog
                  })
                  .ToArray();
        }
    }
}
