using Chloe.DbExpressions;
using Chloe.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Query
{
    /// <summary>
    /// 查询的结果集
    /// </summary>
    class ResultElement
    {
        public ResultElement(ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables)
        {
            this.Orderings = new List<DbOrdering>();
            this.GroupSegments = new List<DbExpression>();

            if (scopeTables == null)
                this.ScopeTables = new KeyDictionary<string>();
            else
                this.ScopeTables = scopeTables.Clone();

            if (scopeParameters == null)
                this.ScopeParameters = new ScopeParameterDictionary();
            else
                this.ScopeParameters = scopeParameters.Clone();
        }

        public IMappingObjectExpression MappingObjectExpression { get; set; }

        /// <summary>
        /// Orderings 是否是传承下来的
        /// </summary>
        public bool InheritOrderings { get; set; }

        public List<DbOrdering> Orderings { get; private set; }
        public List<DbExpression> GroupSegments { get; private set; }

        /// <summary>
        /// 如 takequery 了以后，则 table 的 Expression 类似 (select T.Id.. from User as T),Alias 则为新生成的
        /// </summary>
        public DbFromTableExpression FromTable { get; set; }
        public DbExpression Condition { get; set; }
        public DbExpression HavingCondition { get; set; }

        public KeyDictionary<string> ScopeTables { get; private set; }
        public ScopeParameterDictionary ScopeParameters { get; private set; }
        public void AppendCondition(DbExpression condition)
        {
            if (this.Condition == null)
                this.Condition = condition;
            else
                this.Condition = new DbAndExpression(this.Condition, condition);
        }
        public void AppendHavingCondition(DbExpression condition)
        {
            if (this.HavingCondition == null)
                this.HavingCondition = condition;
            else
                this.HavingCondition = new DbAndExpression(this.HavingCondition, condition);
        }

        public string GenerateUniqueTableAlias(string prefix = UtilConstants.DefaultTableAlias)
        {
            string alias = prefix;
            int i = 0;
            DbFromTableExpression fromTable = this.FromTable;
            while (this.ScopeTables.ContainsKey(alias) || ExistTableAlias(fromTable, alias))
            {
                alias = prefix + i.ToString();
                i++;
            }

            this.ScopeTables[alias] = alias;

            return alias;
        }

        static bool ExistTableAlias(DbMainTableExpression mainTable, string alias)
        {
            if (mainTable == null)
                return false;

            if (string.Equals(mainTable.Table.Alias, alias, StringComparison.OrdinalIgnoreCase))
                return true;

            foreach (DbJoinTableExpression joinTable in mainTable.JoinTables)
            {
                if (ExistTableAlias(joinTable, alias))
                    return true;
            }

            return false;
        }
    }
}
