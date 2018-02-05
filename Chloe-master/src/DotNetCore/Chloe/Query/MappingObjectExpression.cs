using Chloe.Extensions;
using Chloe.InternalExtensions;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Query.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Query
{
    public class MappingObjectExpression : IMappingObjectExpression
    {
        public MappingObjectExpression(ConstructorInfo constructor)
            : this(EntityConstructorDescriptor.GetInstance(constructor))
        {
        }
        public MappingObjectExpression(EntityConstructorDescriptor constructorDescriptor)
        {
            this.ConstructorDescriptor = constructorDescriptor;
            this.MappingConstructorParameters = new Dictionary<ParameterInfo, DbExpression>();
            this.ComplexConstructorParameters = new Dictionary<ParameterInfo, IMappingObjectExpression>();
            this.MappingMembers = new Dictionary<MemberInfo, DbExpression>();
            this.ComplexMembers = new Dictionary<MemberInfo, IMappingObjectExpression>();
        }

        public DbExpression PrimaryKey { get; set; }
        public DbExpression NullChecking { get; set; }

        /// <summary>
        /// 返回类型
        /// </summary>
        public EntityConstructorDescriptor ConstructorDescriptor { get; private set; }
        public Dictionary<ParameterInfo, DbExpression> MappingConstructorParameters { get; private set; }
        public Dictionary<ParameterInfo, IMappingObjectExpression> ComplexConstructorParameters { get; private set; }
        public Dictionary<MemberInfo, DbExpression> MappingMembers { get; protected set; }
        public Dictionary<MemberInfo, IMappingObjectExpression> ComplexMembers { get; protected set; }

        public void AddMappingConstructorParameter(ParameterInfo p, DbExpression exp)
        {
            this.MappingConstructorParameters.Add(p, exp);
        }
        public void AddComplexConstructorParameter(ParameterInfo p, IMappingObjectExpression exp)
        {
            this.ComplexConstructorParameters.Add(p, exp);
        }
        public void AddMappingMemberExpression(MemberInfo memberInfo, DbExpression exp)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.ConstructorDescriptor.ConstructorInfo.DeclaringType);
            this.MappingMembers.Add(memberInfo, exp);
        }
        public void AddComplexMemberExpression(MemberInfo memberInfo, IMappingObjectExpression moe)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.ConstructorDescriptor.ConstructorInfo.DeclaringType);
            this.ComplexMembers.Add(memberInfo, moe);
        }
        /// <summary>
        /// 考虑匿名函数构造函数参数和其只读属性的对应
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        public DbExpression GetMappingMemberExpression(MemberInfo memberInfo)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.ConstructorDescriptor.ConstructorInfo.DeclaringType);
            DbExpression ret = null;
            if (!this.MappingMembers.TryGetValue(memberInfo, out ret))
            {
                ParameterInfo p = null;
                if (!this.ConstructorDescriptor.MemberParameterMap.TryGetValue(memberInfo, out p))
                {
                    return null;
                }

                if (!this.MappingConstructorParameters.TryGetValue(p, out ret))
                {
                    return null;
                }
            }

            return ret;
        }
        public IMappingObjectExpression GetComplexMemberExpression(MemberInfo memberInfo)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.ConstructorDescriptor.ConstructorInfo.DeclaringType);
            IMappingObjectExpression ret = null;
            if (!this.ComplexMembers.TryGetValue(memberInfo, out ret))
            {
                //从构造函数中查
                ParameterInfo p = null;
                if (!this.ConstructorDescriptor.MemberParameterMap.TryGetValue(memberInfo, out p))
                {
                    return null;
                }

                if (!this.ComplexConstructorParameters.TryGetValue(p, out ret))
                {
                    return null;
                }
            }

            return ret;
        }
        public DbExpression GetDbExpression(MemberExpression memberExpressionDeriveFromParameter)
        {
            Stack<MemberExpression> memberExpressions = ExpressionExtension.Reverse(memberExpressionDeriveFromParameter);

            DbExpression ret = null;
            IMappingObjectExpression moe = this;
            foreach (MemberExpression memberExpression in memberExpressions)
            {
                MemberInfo accessedMember = memberExpression.Member;

                if (moe == null && ret != null)
                {
                    /* a.F_DateTime.Value.Date */
                    ret = DbExpression.MemberAccess(accessedMember, ret);
                    continue;
                }

                /* **.accessedMember */
                DbExpression e = moe.GetMappingMemberExpression(accessedMember);
                if (e == null)
                {
                    /* Indicate current accessed member is not mapping member, then try get complex member like 'a.Order' */
                    moe = moe.GetComplexMemberExpression(accessedMember);

                    if (moe == null)
                    {
                        if (ret == null)
                        {
                            /*
                             * If run here,the member access expression must be like 'a.xx',
                             * and member 'xx' is neither mapping member nor complex member,in this case,we not supported.
                             */
                            throw new InvalidOperationException(memberExpressionDeriveFromParameter.ToString());
                        }
                        else
                        {
                            /* Non mapping member is not found also, then convert linq MemberExpression to DbMemberExpression */
                            ret = DbExpression.MemberAccess(accessedMember, ret);
                            continue;
                        }
                    }

                    if (ret != null)
                    {
                        /* This case and case #110 will not appear in normal, if you meet,please email me(so_while@163.com) or call 911 for help. */
                        throw new InvalidOperationException(memberExpressionDeriveFromParameter.ToString());
                    }
                }
                else
                {
                    if (ret != null)//Case: #110
                        throw new InvalidOperationException(memberExpressionDeriveFromParameter.ToString());

                    ret = e;
                }
            }

            if (ret == null)
            {
                /*
                 * If run here,the input argument 'memberExpressionDeriveFromParameter' expression must be like 'a.xx','a.**.xx','a.**.**.xx' ...and so on,
                 * and the last accessed member 'xx' is not mapping member, in this case, we not supported too.
                 */
                throw new InvalidOperationException(memberExpressionDeriveFromParameter.ToString());
            }

            return ret;
        }
        public IMappingObjectExpression GetComplexMemberExpression(MemberExpression memberExpressionDeriveParameter)
        {
            Stack<MemberExpression> memberExpressions = ExpressionExtension.Reverse(memberExpressionDeriveParameter);

            if (memberExpressions.Count == 0)
                throw new Exception();

            IMappingObjectExpression ret = this;
            foreach (MemberExpression memberExpression in memberExpressions)
            {
                MemberInfo member = memberExpression.Member;

                ret = ret.GetComplexMemberExpression(member);
                if (ret == null)
                {
                    throw new NotSupportedException(memberExpressionDeriveParameter.ToString());
                }
            }

            return ret;
        }
        public IObjectActivatorCreator GenarateObjectActivatorCreator(DbSqlQueryExpression sqlQuery)
        {
            MappingEntity mappingEntity = new MappingEntity(this.ConstructorDescriptor);

            foreach (var kv in this.MappingConstructorParameters)
            {
                ParameterInfo pi = kv.Key;
                DbExpression exp = kv.Value;

                int ordinal;
                ordinal = MappingObjectExpressionHelper.TryGetOrAddColumn(sqlQuery, exp, pi.Name).Value;

                if (exp == this.NullChecking)
                    mappingEntity.CheckNullOrdinal = ordinal;

                mappingEntity.ConstructorParameters.Add(pi, ordinal);
            }

            foreach (var kv in this.ComplexConstructorParameters)
            {
                ParameterInfo pi = kv.Key;
                IMappingObjectExpression val = kv.Value;

                IObjectActivatorCreator complexMappingMember = val.GenarateObjectActivatorCreator(sqlQuery);
                mappingEntity.ConstructorEntityParameters.Add(pi, complexMappingMember);
            }

            foreach (var kv in this.MappingMembers)
            {
                MemberInfo member = kv.Key;
                DbExpression exp = kv.Value;

                int ordinal;
                ordinal = MappingObjectExpressionHelper.TryGetOrAddColumn(sqlQuery, exp, member.Name).Value;

                if (exp == this.NullChecking)
                    mappingEntity.CheckNullOrdinal = ordinal;

                mappingEntity.MappingMembers.Add(member, ordinal);
            }

            foreach (var kv in this.ComplexMembers)
            {
                MemberInfo member = kv.Key;
                IMappingObjectExpression val = kv.Value;

                IObjectActivatorCreator complexMappingMember = val.GenarateObjectActivatorCreator(sqlQuery);
                mappingEntity.ComplexMembers.Add(kv.Key, complexMappingMember);
            }

            if (mappingEntity.CheckNullOrdinal == null)
                mappingEntity.CheckNullOrdinal = MappingObjectExpressionHelper.TryGetOrAddColumn(sqlQuery, this.NullChecking);

            return mappingEntity;
        }

        public IMappingObjectExpression ToNewObjectExpression(DbSqlQueryExpression sqlQuery, DbTable table)
        {
            MappingObjectExpression newMoe = new MappingObjectExpression(this.ConstructorDescriptor);

            foreach (var kv in this.MappingConstructorParameters)
            {
                ParameterInfo pi = kv.Key;
                DbExpression exp = kv.Value;

                DbColumnAccessExpression cae = null;
                cae = MappingObjectExpressionHelper.ParseColumnAccessExpression(sqlQuery, table, exp, pi.Name);

                newMoe.AddMappingConstructorParameter(pi, cae);
            }

            foreach (var kv in this.ComplexConstructorParameters)
            {
                ParameterInfo pi = kv.Key;
                IMappingObjectExpression val = kv.Value;

                IMappingObjectExpression complexMappingMember = val.ToNewObjectExpression(sqlQuery, table);
                newMoe.AddComplexConstructorParameter(pi, complexMappingMember);
            }

            foreach (var kv in this.MappingMembers)
            {
                MemberInfo member = kv.Key;
                DbExpression exp = kv.Value;

                DbColumnAccessExpression cae = null;
                cae = MappingObjectExpressionHelper.ParseColumnAccessExpression(sqlQuery, table, exp, member.Name);

                newMoe.AddMappingMemberExpression(member, cae);

                if (exp == this.PrimaryKey)
                {
                    newMoe.PrimaryKey = cae;
                    if (this.NullChecking == this.PrimaryKey)
                        newMoe.NullChecking = cae;
                }
            }

            foreach (var kv in this.ComplexMembers)
            {
                MemberInfo member = kv.Key;
                IMappingObjectExpression val = kv.Value;

                IMappingObjectExpression complexMappingMember = val.ToNewObjectExpression(sqlQuery, table);
                newMoe.AddComplexMemberExpression(member, complexMappingMember);
            }

            if (newMoe.NullChecking == null)
                newMoe.NullChecking = MappingObjectExpressionHelper.TryGetOrAddNullChecking(sqlQuery, table, this.NullChecking);

            return newMoe;
        }

        public void SetNullChecking(DbExpression exp)
        {
            if (this.NullChecking == null)
            {
                if (this.PrimaryKey != null)
                    this.NullChecking = this.PrimaryKey;
                else
                    this.NullChecking = exp;
            }

            foreach (var item in this.ComplexConstructorParameters.Values)
            {
                item.SetNullChecking(exp);
            }

            foreach (var item in this.ComplexMembers.Values)
            {
                item.SetNullChecking(exp);
            }
        }
    }
}
