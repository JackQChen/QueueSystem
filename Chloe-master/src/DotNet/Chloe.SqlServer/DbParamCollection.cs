using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.SqlServer
{
    class DbParamCollection
    {
        /* 以参数值为 key，DbParam 或 List<DbParam> 为 value */
        Dictionary<object, object> _dbParams = new Dictionary<object, object>();

        public int Count { get; private set; }
        public DbParam Find(object value, Type paramType, DbType? dbType)
        {
            object dicVal;
            if (!this._dbParams.TryGetValue(value, out dicVal))
                return null;

            DbParam dbParam = dicVal as DbParam;
            if (dbParam != null)
            {
                if (value == DBNull.Value)
                {
                    if (dbParam.Type == paramType)
                        return dbParam;
                }
                else if (dbParam.DbType == dbType)
                {
                    return dbParam;
                }

                return null;
            }


            List<DbParam> dbParamList = dicVal as List<DbParam>;
            if (value == DBNull.Value)
            {
                return dbParamList.Find(a => a.Type == paramType);
            }
            else
            {
                return dbParamList.Find(a => a.DbType == dbType);
            }
        }

        public void Add(DbParam param)
        {
            object value = param.Value;

            object dicVal;
            if (!this._dbParams.TryGetValue(value, out dicVal))
            {
                this._dbParams.Add(value, param);
                this.Count++;
                return;
            }

            DbParam dbParam = dicVal as DbParam;
            if (dbParam != null)
            {
                List<DbParam> dbParamList = new List<DbParam>(2) { dbParam, param };
                this._dbParams[value] = dbParamList;
                this.Count++;
            }
            else
            {
                List<DbParam> dbParamList = dicVal as List<DbParam>;
                dbParamList.Add(param);
                this.Count++;
            }
        }


        public List<DbParam> ToParameterList()
        {
            List<DbParam> ret = new List<DbParam>(this.Count);

            foreach (object dicVal in this._dbParams.Values)
            {
                DbParam dbParam = dicVal as DbParam;
                if (dbParam != null)
                {
                    ret.Add(dbParam);
                }
                else
                {
                    List<DbParam> dbParamList = dicVal as List<DbParam>;
                    for (int i = 0; i < dbParamList.Count; i++)
                    {
                        ret.Add(dbParamList[i]);
                    }
                }
            }

            return ret;
        }
    }
}
