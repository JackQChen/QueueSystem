using Chloe.Core;
using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Entity;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Oracle
{
    public partial class OracleContext : DbContext
    {
        DbContextServiceProvider _dbContextServiceProvider;
        bool _convertToUppercase = true;
        public OracleContext(IDbConnectionFactory dbConnectionFactory)
        {
            Utils.CheckNull(dbConnectionFactory);

            this._dbContextServiceProvider = new DbContextServiceProvider(dbConnectionFactory, this);
        }


        /// <summary>
        /// 是否将 sql 中的表名/字段名转成大写。默认为 true。
        /// </summary>
        public bool ConvertToUppercase { get { return this._convertToUppercase; } set { this._convertToUppercase = value; } }
        public override IDbContextServiceProvider DbContextServiceProvider
        {
            get { return this._dbContextServiceProvider; }
        }

        public override TEntity Insert<TEntity>(TEntity entity, string table)
        {
            Utils.CheckNull(entity);

            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(entity.GetType());

            Dictionary<MappingMemberDescriptor, object> keyValueMap = CreateKeyValueMap(typeDescriptor);

            string sequenceName;
            object sequenceValue = null;
            MappingMemberDescriptor defineSequenceMemberDescriptor = GetDefineSequenceMemberDescriptor(typeDescriptor, out sequenceName);

            if (defineSequenceMemberDescriptor != null)
            {
                if (defineSequenceMemberDescriptor.IsPrimaryKey && typeDescriptor.PrimaryKeys.Count > 1)
                {
                    /* 自增列不能作为联合主键成员 */
                    throw new ChloeException("The member of marked sequence can not be union key.");
                }

                sequenceValue = ConvertIdentityType(this.GetSequenceNextValue(sequenceName), defineSequenceMemberDescriptor.MemberInfoType);
            }

            Dictionary<MappingMemberDescriptor, DbExpression> insertColumns = new Dictionary<MappingMemberDescriptor, DbExpression>();
            foreach (var kv in typeDescriptor.MappingMemberDescriptors)
            {
                MemberInfo member = kv.Key;
                MappingMemberDescriptor memberDescriptor = kv.Value;

                object val = null;
                if (defineSequenceMemberDescriptor != null && memberDescriptor == defineSequenceMemberDescriptor)
                {
                    val = sequenceValue;
                }
                else
                    val = memberDescriptor.GetValue(entity);

                if (keyValueMap.ContainsKey(memberDescriptor))
                {
                    keyValueMap[memberDescriptor] = val;
                }

                DbExpression valExp = DbExpression.Parameter(val, memberDescriptor.MemberInfoType);
                insertColumns.Add(memberDescriptor, valExp);
            }

            MappingMemberDescriptor nullValueKey = keyValueMap.Where(a => a.Value == null).Select(a => a.Key).FirstOrDefault();
            if (nullValueKey != null)
            {
                throw new ChloeException(string.Format("The primary key '{0}' could not be null.", nullValueKey.MemberInfo.Name));
            }

            DbTable dbTable = table == null ? typeDescriptor.Table : new DbTable(table, typeDescriptor.Table.Schema);
            DbInsertExpression e = new DbInsertExpression(dbTable);

            foreach (var kv in insertColumns)
            {
                e.InsertColumns.Add(kv.Key.Column, kv.Value);
            }

            this.ExecuteSqlCommand(e);

            if (defineSequenceMemberDescriptor != null)
                defineSequenceMemberDescriptor.SetValue(entity, sequenceValue);

            return entity;
        }
        public override object Insert<TEntity>(Expression<Func<TEntity>> content, string table)
        {
            Utils.CheckNull(content);

            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(typeof(TEntity));

            if (typeDescriptor.PrimaryKeys.Count > 1)
            {
                /* 对于多主键的实体，暂时不支持调用这个方法进行插入 */
                throw new NotSupportedException(string.Format("Can not call this method because entity '{0}' has multiple keys.", typeDescriptor.EntityType.FullName));
            }

            MappingMemberDescriptor keyMemberDescriptor = typeDescriptor.PrimaryKeys.FirstOrDefault();

            string sequenceName;
            object sequenceValue = null;
            MappingMemberDescriptor defineSequenceMemberDescriptor = GetDefineSequenceMemberDescriptor(typeDescriptor, out sequenceName);

            Dictionary<MemberInfo, Expression> insertColumns = InitMemberExtractor.Extract(content);

            DbTable explicitDbTable = null;
            if (table != null)
                explicitDbTable = new DbTable(table, typeDescriptor.Table.Schema);
            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(explicitDbTable);
            DbInsertExpression e = new DbInsertExpression(explicitDbTable ?? typeDescriptor.Table);

            object keyVal = null;

            foreach (var kv in insertColumns)
            {
                MemberInfo key = kv.Key;
                MappingMemberDescriptor memberDescriptor = typeDescriptor.TryGetMappingMemberDescriptor(key);

                if (memberDescriptor == null)
                    throw new ChloeException(string.Format("The member '{0}' does not map any column.", key.Name));

                if (memberDescriptor == defineSequenceMemberDescriptor)
                    throw new ChloeException(string.Format("Can not insert value into the column '{0}', because it's mapping member has define a sequence.", memberDescriptor.Column.Name));

                if (memberDescriptor.IsPrimaryKey)
                {
                    object val = ExpressionEvaluator.Evaluate(kv.Value);
                    if (val == null)
                        throw new ChloeException(string.Format("The primary key '{0}' could not be null.", memberDescriptor.MemberInfo.Name));
                    else
                    {
                        keyVal = val;
                        e.InsertColumns.Add(memberDescriptor.Column, DbExpression.Parameter(keyVal));
                        continue;
                    }
                }

                e.InsertColumns.Add(memberDescriptor.Column, expressionParser.Parse(kv.Value));
            }

            if (keyMemberDescriptor == defineSequenceMemberDescriptor)
            {
                sequenceValue = ConvertIdentityType(this.GetSequenceNextValue(sequenceName), defineSequenceMemberDescriptor.MemberInfoType);

                keyVal = sequenceValue;
                e.InsertColumns.Add(keyMemberDescriptor.Column, DbExpression.Parameter(keyVal));
            }

            if (keyMemberDescriptor != null && keyVal == null)
            {
                throw new ChloeException(string.Format("The primary key '{0}' could not be null.", keyMemberDescriptor.MemberInfo.Name));
            }

            this.ExecuteSqlCommand(e);
            return keyVal; /* It will return null if an entity does not define primary key. */
        }
        public override void InsertRange<TEntity>(List<TEntity> entities, bool keepIdentity = false)
        {
            /*
             * 将 entities 分批插入数据库
             * 每批生成 insert into TableName(...) select ... from dual union all select ... from dual...
             * 对于 oracle，貌似速度提升不了...- -
             * #期待各码友的优化建议#
             */

            Utils.CheckNull(entities);
            if (entities.Count == 0)
                return;

            int maxParameters = 1000;
            int batchSize = 40; /* 每批实体大小，此值通过测试得出相对插入速度比较快的一个值 */

            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(typeof(TEntity));

            var e = typeDescriptor.MappingMemberDescriptors.Select(a => a.Value);

            List<MappingMemberDescriptor> mappingMemberDescriptors = e.ToList();
            int maxDbParamsCount = maxParameters - mappingMemberDescriptors.Count; /* 控制一个 sql 的参数个数 */

            string sqlTemplate = AppendInsertRangeSqlTemplate(typeDescriptor, mappingMemberDescriptors, keepIdentity);

            Action insertAction = () =>
            {
                int batchCount = 0;
                List<DbParam> dbParams = new List<DbParam>();
                StringBuilder sqlBuilder = new StringBuilder();
                for (int i = 0; i < entities.Count; i++)
                {
                    var entity = entities[i];

                    if (batchCount > 0)
                        sqlBuilder.Append(" UNION ALL ");

                    sqlBuilder.Append(" SELECT ");
                    for (int j = 0; j < mappingMemberDescriptors.Count; j++)
                    {
                        if (j > 0)
                            sqlBuilder.Append(",");

                        MappingMemberDescriptor mappingMemberDescriptor = mappingMemberDescriptors[j];

                        string sequenceName;
                        if (keepIdentity == false && HasSequenceAttribute(mappingMemberDescriptor, out sequenceName))
                        {
                            sqlBuilder.AppendFormat("1");
                            continue;
                        }

                        object val = mappingMemberDescriptor.GetValue(entity);
                        if (val == null)
                        {
                            sqlBuilder.Append("NULL");
                            sqlBuilder.Append(" C").Append(j.ToString());
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
                        }
                        else if (val is bool)
                        {
                            if ((bool)val == true)
                                sqlBuilder.AppendFormat("1");
                            else
                                sqlBuilder.AppendFormat("0");
                        }
                        else
                        {
                            string paramName = UtilConstants.ParameterNamePrefix + dbParams.Count.ToString();
                            DbParam dbParam = new DbParam(paramName, val) { DbType = mappingMemberDescriptor.Column.DbType };
                            dbParams.Add(dbParam);
                            sqlBuilder.Append(paramName);
                        }

                        sqlBuilder.Append(" C").Append(j.ToString());
                    }

                    sqlBuilder.Append(" FROM DUAL");

                    batchCount++;

                    if ((batchCount >= 20 && dbParams.Count >= 400/*参数个数太多也会影响速度*/) || dbParams.Count >= maxDbParamsCount || batchCount >= batchSize || (i + 1) == entities.Count)
                    {
                        sqlBuilder.Insert(0, sqlTemplate);

                        if (keepIdentity == false)
                        {
                            sqlBuilder.Append(") T");
                        }

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


        public override int Update<TEntity>(TEntity entity, string table)
        {
            Utils.CheckNull(entity);

            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(entity.GetType());
            EnsureMappingTypeHasPrimaryKey(typeDescriptor);

            Dictionary<MappingMemberDescriptor, object> keyValueMap = CreateKeyValueMap(typeDescriptor);

            IEntityState entityState = this.TryGetTrackedEntityState(entity);
            Dictionary<MappingMemberDescriptor, DbExpression> updateColumns = new Dictionary<MappingMemberDescriptor, DbExpression>();
            foreach (var kv in typeDescriptor.MappingMemberDescriptors)
            {
                MappingMemberDescriptor memberDescriptor = kv.Value;

                if (keyValueMap.ContainsKey(memberDescriptor))
                {
                    keyValueMap[memberDescriptor] = memberDescriptor.GetValue(entity);
                    continue;
                }

                SequenceAttribute attr = (SequenceAttribute)memberDescriptor.GetCustomAttribute(typeof(SequenceAttribute));
                if (attr != null)
                    continue;

                object val = memberDescriptor.GetValue(entity);

                if (entityState != null && !entityState.HasChanged(memberDescriptor, val))
                    continue;

                DbExpression valExp = DbExpression.Parameter(val, memberDescriptor.MemberInfoType);
                updateColumns.Add(memberDescriptor, valExp);
            }

            if (updateColumns.Count == 0)
                return 0;

            DbTable dbTable = table == null ? typeDescriptor.Table : new DbTable(table, typeDescriptor.Table.Schema);
            DbExpression conditionExp = MakeCondition(keyValueMap, dbTable);
            DbUpdateExpression e = new DbUpdateExpression(dbTable, conditionExp);

            foreach (var item in updateColumns)
            {
                e.UpdateColumns.Add(item.Key.Column, item.Value);
            }

            int ret = this.ExecuteSqlCommand(e);
            if (entityState != null)
                entityState.Refresh();
            return ret;
        }
        public override int Update<TEntity>(Expression<Func<TEntity, bool>> condition, Expression<Func<TEntity, TEntity>> content, string table)
        {
            Utils.CheckNull(condition);
            Utils.CheckNull(content);

            TypeDescriptor typeDescriptor = TypeDescriptor.GetDescriptor(typeof(TEntity));

            Dictionary<MemberInfo, Expression> updateColumns = InitMemberExtractor.Extract(content);

            DbTable explicitDbTable = null;
            if (table != null)
                explicitDbTable = new DbTable(table, typeDescriptor.Table.Schema);
            DefaultExpressionParser expressionParser = typeDescriptor.GetExpressionParser(explicitDbTable);

            DbExpression conditionExp = expressionParser.ParseFilterPredicate(condition);

            DbUpdateExpression e = new DbUpdateExpression(explicitDbTable ?? typeDescriptor.Table, conditionExp);

            foreach (var kv in updateColumns)
            {
                MemberInfo key = kv.Key;
                MappingMemberDescriptor memberDescriptor = typeDescriptor.TryGetMappingMemberDescriptor(key);

                if (memberDescriptor == null)
                    throw new ChloeException(string.Format("The member '{0}' does not map any column.", key.Name));

                if (memberDescriptor.IsPrimaryKey)
                    throw new ChloeException(string.Format("Could not update the primary key '{0}'.", memberDescriptor.Column.Name));

                SequenceAttribute attr = (SequenceAttribute)memberDescriptor.GetCustomAttribute(typeof(SequenceAttribute));
                if (attr != null)
                    throw new ChloeException(string.Format("Could not update the column '{0}', because it's mapping member has define a sequence.", memberDescriptor.Column.Name));

                e.UpdateColumns.Add(memberDescriptor.Column, expressionParser.Parse(kv.Value));
            }

            if (e.UpdateColumns.Count == 0)
                return 0;

            return this.ExecuteSqlCommand(e);
        }

        int ExecuteSqlCommand(DbExpression e)
        {
            IDbExpressionTranslator translator = this.DbContextServiceProvider.CreateDbExpressionTranslator();
            List<DbParam> parameters;
            string cmdText = translator.Translate(e, out parameters);

            int r = this.Session.ExecuteNonQuery(cmdText, parameters.ToArray());
            return r;
        }
        object GetSequenceNextValue(string sequenceName)
        {
            if (this.ConvertToUppercase)
                sequenceName = sequenceName.ToUpper();

            object ret = this.Session.ExecuteScalar(string.Concat("SELECT \"", sequenceName, "\".\"NEXTVAL\" FROM \"DUAL\""));

            if (ret == null || ret == DBNull.Value)
            {
                throw new ChloeException(string.Format("Unable to get the sequence '{0}' next value.", sequenceName));
            }

            return ret;
        }

        string AppendInsertRangeSqlTemplate(TypeDescriptor typeDescriptor, List<MappingMemberDescriptor> mappingMemberDescriptors, bool keepIdentity)
        {
            StringBuilder sqlBuilder = new StringBuilder();

            sqlBuilder.Append("INSERT INTO ");
            sqlBuilder.Append(this.AppendTableName(typeDescriptor.Table));
            sqlBuilder.Append("(");

            for (int i = 0; i < mappingMemberDescriptors.Count; i++)
            {
                MappingMemberDescriptor mappingMemberDescriptor = mappingMemberDescriptors[i];
                if (i > 0)
                    sqlBuilder.Append(",");
                sqlBuilder.Append(this.QuoteName(mappingMemberDescriptor.Column.Name));
            }

            sqlBuilder.Append(")");

            if (keepIdentity == false)
            {
                sqlBuilder.Append(" SELECT ");
                for (int i = 0; i < mappingMemberDescriptors.Count; i++)
                {
                    MappingMemberDescriptor mappingMemberDescriptor = mappingMemberDescriptors[i];
                    if (i > 0)
                        sqlBuilder.Append(",");

                    string sequenceName;
                    if (HasSequenceAttribute(mappingMemberDescriptor, out sequenceName))
                        sqlBuilder.AppendFormat("{0}.{1}", this.QuoteName(sequenceName), this.QuoteName("NEXTVAL"));
                    else
                    {
                        sqlBuilder.Append("C").Append(i.ToString());
                    }
                }
                sqlBuilder.Append(" FROM (");
            }

            string sqlTemplate = sqlBuilder.ToString();
            return sqlTemplate;
        }
        string AppendTableName(DbTable table)
        {
            if (string.IsNullOrEmpty(table.Schema))
                return this.QuoteName(table.Name);

            return string.Format("{0}.{1}", this.QuoteName(table.Schema), this.QuoteName(table.Name));
        }
        string QuoteName(string name)
        {
            if (this._convertToUppercase)
                return string.Concat("\"", name.ToUpper(), "\"");

            return string.Concat("\"", name, "\"");
        }
    }
}
