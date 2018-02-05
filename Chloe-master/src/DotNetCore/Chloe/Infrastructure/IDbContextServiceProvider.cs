using System.Data;

namespace Chloe.Infrastructure
{
    public interface IDbContextServiceProvider
    {
        IDbConnection CreateConnection();
        IDbExpressionTranslator CreateDbExpressionTranslator();
    }
}
