using Chloe.Core;
using Chloe.DbExpressions;
using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.SqlServer
{
    class DbExpressionTranslator : IDbExpressionTranslator
    {
        public static readonly DbExpressionTranslator Instance = new DbExpressionTranslator();
        public string Translate(DbExpression expression, out List<DbParam> parameters)
        {
            SqlGenerator generator = SqlGenerator.CreateInstance();
            expression.Accept(generator);

            parameters = generator.Parameters;
            string sql = generator.SqlBuilder.ToSql();

            return sql;
        }
    }

    class DbExpressionTranslator_OffsetFetch : IDbExpressionTranslator
    {
        public static readonly DbExpressionTranslator_OffsetFetch Instance = new DbExpressionTranslator_OffsetFetch();
        public string Translate(DbExpression expression, out List<DbParam> parameters)
        {
            SqlGenerator_OffsetFetch generator = new SqlGenerator_OffsetFetch();
            expression.Accept(generator);

            parameters = generator.Parameters;
            string sql = generator.SqlBuilder.ToSql();

            return sql;
        }
    }
}
