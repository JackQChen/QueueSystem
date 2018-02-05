using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.SQLite
{
    static class DbExpressionHelper
    {
        /// <summary>
        /// 尝试将 exp 转换成 DbParameterExpression。
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        public static DbExpression OptimizeDbExpression(this DbExpression exp)
        {
            DbExpression stripedExp = DbExpressionExtension.StripInvalidConvert(exp);

            DbExpression tempExp = stripedExp;

            List<DbConvertExpression> cList = null;
            while (tempExp.NodeType == DbExpressionType.Convert)
            {
                if (cList == null)
                    cList = new List<DbConvertExpression>();

                DbConvertExpression c = (DbConvertExpression)tempExp;
                cList.Add(c);
                tempExp = c.Operand;
            }

            if (tempExp.NodeType == DbExpressionType.Constant || tempExp.NodeType == DbExpressionType.Parameter)
                return stripedExp;

            if (tempExp.NodeType == DbExpressionType.MemberAccess)
            {
                DbMemberExpression dbMemberExp = (DbMemberExpression)tempExp;

                if (ExistDateTime_NowOrDateTime_UtcNow(dbMemberExp))
                    return stripedExp;

                DbParameterExpression val;
                if (DbExpressionExtension.TryConvertToParameterExpression(dbMemberExp, out val))
                {
                    if (cList != null)
                    {
                        if (val.Value == DBNull.Value)//如果是 null，则不需要 Convert 了，在数据库里没意义
                            return val;

                        DbConvertExpression c = null;
                        for (int i = cList.Count - 1; i > -1; i--)
                        {
                            DbConvertExpression item = cList[i];
                            c = new DbConvertExpression(item.Type, val);
                        }

                        return c;
                    }

                    return val;
                }
            }

            return stripedExp;
        }

        public static bool ExistDateTime_NowOrDateTime_UtcNow(this DbMemberExpression exp)
        {
            while (exp != null)
            {
                if (exp.Member == UtilConstants.PropertyInfo_DateTime_Now || exp.Member == UtilConstants.PropertyInfo_DateTime_UtcNow)
                {
                    return true;
                }

                exp = exp.Expression as DbMemberExpression;
            }

            return false;
        }

    }
}
