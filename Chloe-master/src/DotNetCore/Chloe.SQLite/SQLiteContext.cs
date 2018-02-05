using Chloe.Core;
using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Entity;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.SQLite
{
    public class SQLiteContext : DbContext
    {
        DbContextServiceProvider _dbContextServiceProvider;
        public SQLiteContext(IDbConnectionFactory dbConnectionFactory)
            : this(dbConnectionFactory, true)
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnectionFactory"></param>
        /// <param name="concurrencyMode">是否支持读写并发安全</param>
        public SQLiteContext(IDbConnectionFactory dbConnectionFactory, bool concurrencyMode)
        {
            Utils.CheckNull(dbConnectionFactory);

            if (concurrencyMode == true)
                dbConnectionFactory = new ConcurrentDbConnectionFactory(dbConnectionFactory);

            this._dbContextServiceProvider = new DbContextServiceProvider(dbConnectionFactory);
        }


        public override IDbContextServiceProvider DbContextServiceProvider
        {
            get { return this._dbContextServiceProvider; }
        }
        protected override string GetSelectLastInsertIdClause()
        {
            return "SELECT LAST_INSERT_ROWID()";
        }

        public override void InsertRange<TEntity>(List<TEntity> entities, bool keepIdentity = false)
        {
            /*
             * 将 entities 分批插入数据库
             * 每批生成 insert into TableName(...) select ... union all select ...
             * 该方法相对循环一条一条插入，速度提升 1/2 这样
             */

            Utils.CheckNull(entities);
            if (entities.Count == 0)
                return;

            int maxParameters = 1000;
            int batchSize = 30; /* 每批实体大小，此值通过测试得出相对插入速度比较快的一个值 */

            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(typeof(TEntity));

            var e = typeDescriptor.MappingMemberDescriptors.Select(a => a.Value);
            if (keepIdentity == false)
                e = e.Where(a => a.IsAutoIncrement == false);
            List<MappingMemberDescriptor> mappingMemberDescriptors = e.ToList();
            int maxDbParamsCount = maxParameters - mappingMemberDescriptors.Count; /* 控制一个 sql 的参数个数 */

            string sqlTemplate = AppendInsertRangeSqlTemplate(typeDescriptor, mappingMemberDescriptors);

            Action insertAction = () =>
            {
                int batchCount = 0;
                List<DbParam> dbParams = new List<DbParam>();
                StringBuilder sqlBuilder = new StringBuilder();
                for (int i = 0; i < entities.Count; i++)
                {
                    var entity = entities[i];

                    if (batchCount > 0)
                        sqlBuilder.Append(" UNION ALL");

                    sqlBuilder.Append(" SELECT ");
                    for (int j = 0; j < mappingMemberDescriptors.Count; j++)
                    {
                        if (j > 0)
                            sqlBuilder.Append(",");

                        MappingMemberDescriptor mappingMemberDescriptor = mappingMemberDescriptors[j];

                        object val = mappingMemberDescriptor.GetValue(entity);
                        if (val == null)
                        {
                            sqlBuilder.Append("NULL");
                            continue;
                        }

                        Type valType = val.GetType();
                        if (valType.IsEnum)
                        {
                            val = Convert.ChangeType(val, Enum.GetUnderlyingType(valType));
                            valType = val.GetType();
                        }

                        if (Utils.IsToStringableNumericType(valType))
                        {
                            sqlBuilder.Append(val.ToString());
                            continue;
                        }

                        if (val is bool)
                        {
                            if ((bool)val == true)
                                sqlBuilder.AppendFormat("1");
                            else
                                sqlBuilder.AppendFormat("0");
                            continue;
                        }

                        string paramName = UtilConstants.ParameterNamePrefix + dbParams.Count.ToString();
                        DbParam dbParam = new DbParam(paramName, val) { DbType = mappingMemberDescriptor.Column.DbType };
                        dbParams.Add(dbParam);
                        sqlBuilder.Append(paramName);
                    }

                    batchCount++;

                    if ((batchCount >= 20 && dbParams.Count >= 200/*参数个数太多也会影响速度*/) || dbParams.Count >= maxDbParamsCount || batchCount >= batchSize || (i + 1) == entities.Count)
                    {
                        sqlBuilder.Insert(0, sqlTemplate);
                        string sql = sqlBuilder.ToString();
                        this.Session.ExecuteNonQuery(sql, dbParams.ToArray());

                        sqlBuilder.Clear();
                        dbParams.Clear();
                        batchCount = 0;
                    }
                }
            };

            Action fAction = insertAction;

            if (this.Session.IsInTransaction)
            {
                fAction();
            }
            else
            {
                /* 因为分批插入，所以需要开启事务保证数据一致性 */
                this.Session.BeginTransaction();
                try
                {
                    fAction();
                    this.Session.CommitTransaction();
                }
                catch
                {
                    if (this.Session.IsInTransaction)
                        this.Session.RollbackTransaction();
                    throw;
                }
            }
        }

        static string AppendInsertRangeSqlTemplate(TypeDescriptor typeDescriptor, List<MappingMemberDescriptor> mappingMemberDescriptors)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append(AppendTableName(typeDescriptor.Table));
            sqlBuilder.Append("(");

            for (int i = 0; i < mappingMemberDescriptors.Count; i++)
            {
                MappingMemberDescriptor mappingMemberDescriptor = mappingMemberDescriptors[i];
                if (i > 0)
                    sqlBuilder.Append(",");
                sqlBuilder.Append(Utils.QuoteName(mappingMemberDescriptor.Column.Name));
            }

            sqlBuilder.Append(")");

            string sqlTemplate = sqlBuilder.ToString();
            return sqlTemplate;
        }

        static string AppendTableName(DbTable table)
        {
            return Utils.QuoteName(table.Name);
        }
    }

    class ConcurrentDbConnectionFactory : IDbConnectionFactory
    {
        IDbConnectionFactory _dbConnectionFactory;
        public ConcurrentDbConnectionFactory(IDbConnectionFactory dbConnectionFactory)
        {
            this._dbConnectionFactory = dbConnectionFactory;
        }
        public IDbConnection CreateConnection()
        {
            IDbConnection conn = new ChloeSQLiteConcurrentConnection(this._dbConnectionFactory.CreateConnection());
            return conn;
        }
    }

}
