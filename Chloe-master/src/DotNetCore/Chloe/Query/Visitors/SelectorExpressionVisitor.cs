using System;
using System.Linq.Expressions;
using System.Reflection;
using Chloe.InternalExtensions;
using Chloe.DbExpressions;
using Chloe.Query.Visitors;
using System.Collections.Generic;
using Chloe.Core.Visitors;
using Chloe.Extensions;
using Chloe.Infrastructure;
using Chloe.Utility;

namespace Chloe.Query
{
    class SelectorExpressionVisitor : ExpressionVisitor<IMappingObjectExpression>
    {
        ExpressionVisitorBase _visitor;
        LambdaExpression _lambda;
        ScopeParameterDictionary _scopeParameters;
        KeyDictionary<string> _scopeTables;
        SelectorExpressionVisitor(ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables)
        {
            this._scopeParameters = scopeParameters;
            this._scopeTables = scopeTables;
        }

        public static IMappingObjectExpression ResolveSelectorExpression(LambdaExpression selector, ScopeParameterDictionary scopeParameters, KeyDictionary<string> scopeTables)
        {
            SelectorExpressionVisitor visitor = new SelectorExpressionVisitor(scopeParameters, scopeTables);
            return visitor.Visit(selector);
        }

        IMappingObjectExpression FindMoe(ParameterExpression exp)
        {
            IMappingObjectExpression moe = this._scopeParameters.Get(exp);
            return moe;
        }
        DbExpression ResolveExpression(Expression exp)
        {
            return this._visitor.Visit(exp);
        }
        IMappingObjectExpression ResolveComplexMember(MemberExpression exp)
        {
            ParameterExpression p;
            if (ExpressionExtension.IsDerivedFromParameter(exp, out p))
            {
                IMappingObjectExpression moe = this.FindMoe(p);
                return moe.GetComplexMemberExpression(exp);
            }
            else
            {
                throw new Exception();
            }
        }

        public override IMappingObjectExpression Visit(Expression exp)
        {
            if (exp == null)
                return default(IMappingObjectExpression);
            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.New:
                    return this.VisitNew((NewExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                case ExpressionType.MemberAccess:
                    return this.VisitMemberAccess((MemberExpression)exp);
                case ExpressionType.Parameter:
                    return this.VisitParameter((ParameterExpression)exp);
                default:
                    return this.VisistMapTypeSelector(exp);
            }
        }

        protected override IMappingObjectExpression VisitLambda(LambdaExpression exp)
        {
            this._lambda = exp;
            this._visitor = new GeneralExpressionVisitor(exp, this._scopeParameters, this._scopeTables);
            return this.Visit(exp.Body);
        }
        protected override IMappingObjectExpression VisitNew(NewExpression exp)
        {
            IMappingObjectExpression result = new MappingObjectExpression(exp.Constructor);
            ParameterInfo[] parames = exp.Constructor.GetParameters();
            for (int i = 0; i < parames.Length; i++)
            {
                ParameterInfo pi = parames[i];
                Expression argExp = exp.Arguments[i];
                if (MappingTypeSystem.IsMappingType(pi.ParameterType))
                {
                    DbExpression dbExpression = this.ResolveExpression(argExp);
                    result.AddMappingConstructorParameter(pi, dbExpression);
                }
                else
                {
                    IMappingObjectExpression subResult = this.Visit(argExp);
                    result.AddComplexConstructorParameter(pi, subResult);
                }
            }

            return result;
        }
        protected override IMappingObjectExpression VisitMemberInit(MemberInitExpression exp)
        {
            IMappingObjectExpression result = this.Visit(exp.NewExpression);

            foreach (MemberBinding binding in exp.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }

                MemberAssignment memberAssignment = (MemberAssignment)binding;
                MemberInfo member = memberAssignment.Member;
                Type memberType = member.GetMemberType();

                //是数据库映射类型
                if (MappingTypeSystem.IsMappingType(memberType))
                {
                    DbExpression dbExpression = this.ResolveExpression(memberAssignment.Expression);
                    result.AddMappingMemberExpression(member, dbExpression);
                }
                else
                {
                    //对于非数据库映射类型，只支持 NewExpression 和 MemberInitExpression
                    IMappingObjectExpression subResult = this.Visit(memberAssignment.Expression);
                    result.AddComplexMemberExpression(member, subResult);
                }
            }

            return result;
        }
        /// <summary>
        /// a => a.Id   a => a.Name   a => a.User
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        protected override IMappingObjectExpression VisitMemberAccess(MemberExpression exp)
        {
            //create MappingFieldExpression object if exp is map type
            if (MappingTypeSystem.IsMappingType(exp.Type))
            {
                DbExpression dbExp = this.ResolveExpression(exp);
                MappingFieldExpression ret = new MappingFieldExpression(exp.Type, dbExp);
                return ret;
            }

            //如 a.Order a.User 等形式
            return this.ResolveComplexMember(exp);
        }
        protected override IMappingObjectExpression VisitParameter(ParameterExpression exp)
        {
            IMappingObjectExpression moe = this.FindMoe(exp);
            return moe;
        }

        IMappingObjectExpression VisistMapTypeSelector(Expression exp)
        {
            if (!MappingTypeSystem.IsMappingType(exp.Type))
            {
                throw new NotSupportedException(exp.ToString());
            }

            DbExpression dbExp = this.ResolveExpression(exp);
            MappingFieldExpression ret = new MappingFieldExpression(exp.Type, dbExp);
            return ret;
        }
    }
}
