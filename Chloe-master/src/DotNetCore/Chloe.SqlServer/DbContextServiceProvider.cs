using Chloe.Core.Visitors;
using Chloe.Infrastructure;
using Chloe.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Chloe.SqlServer
{
    class DbContextServiceProvider : IDbContextServiceProvider
    {
        IDbConnectionFactory _dbConnectionFactory;
        MsSqlContext _msSqlContext;

        public DbContextServiceProvider(IDbConnectionFactory dbConnectionFactory, MsSqlContext msSqlContext)
        {
            this._dbConnectionFactory = dbConnectionFactory;
            this._msSqlContext = msSqlContext;
        }
        public IDbConnection CreateConnection()
        {
            return this._dbConnectionFactory.CreateConnection();
        }
        public IDbExpressionTranslator CreateDbExpressionTranslator()
        {
            if (this._msSqlContext.PagingMode == PagingMode.ROW_NUMBER)
            {
                return DbExpressionTranslator.Instance;
            }
            else if (this._msSqlContext.PagingMode == PagingMode.OFFSET_FETCH)
            {
                return DbExpressionTranslator_OffsetFetch.Instance;
            }

            throw new NotSupportedException();
        }
    }
}
