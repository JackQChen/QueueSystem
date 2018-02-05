using Chloe.DbExpressions;
using System.Collections.Generic;

namespace Chloe.Infrastructure
{
    public interface IDbExpressionTranslator
    {
        string Translate(DbExpression expression, out List<DbParam> parameters);
    }
}
