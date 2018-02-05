using Chloe.Core.Visitors;
using Chloe.Infrastructure;
using Chloe.Query;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Oracle
{
    class DbContextServiceProvider : IDbContextServiceProvider
    {
        IDbConnectionFactory _dbConnectionFactory;
        OracleContext _oracleContext;

        public DbContextServiceProvider(IDbConnectionFactory dbConnectionFactory, OracleContext oracleContext)
        {
            this._dbConnectionFactory = dbConnectionFactory;
            this._oracleContext = oracleContext;
        }
        public IDbConnection CreateConnection()
        {
            IDbConnection conn = this._dbConnectionFactory.CreateConnection();
            if ((conn is ChloeOracleConnection) == false)
                conn = new ChloeOracleConnection(conn);
            return conn;
        }
        public IDbExpressionTranslator CreateDbExpressionTranslator()
        {
            if (this._oracleContext.ConvertToUppercase == true)
            {
                return DbExpressionTranslator_ConvertToUppercase.Instance;
            }
            else
            {
                return DbExpressionTranslator.Instance;
            }

            throw new NotSupportedException();
        }
    }
}
