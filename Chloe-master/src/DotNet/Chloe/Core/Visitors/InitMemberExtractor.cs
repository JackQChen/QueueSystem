using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.Core.Visitors
{
    public class InitMemberExtractor : ExpressionVisitor<Dictionary<MemberInfo, Expression>>
    {
        static readonly InitMemberExtractor _extractor = new InitMemberExtractor();
        InitMemberExtractor()
        {
        }
        public static Dictionary<MemberInfo, Expression> Extract(Expression exp)
        {
            return _extractor.Visit(exp);
        }
        public override Dictionary<MemberInfo, Expression> Visit(Expression exp)
        {
            if (exp == null)
                return null;

            switch (exp.NodeType)
            {
                case ExpressionType.Lambda:
                    return this.VisitLambda((LambdaExpression)exp);
                case ExpressionType.MemberInit:
                    return this.VisitMemberInit((MemberInitExpression)exp);
                default:
                    throw new Exception(string.Format("Unhandled expression type: '{0}'", exp.NodeType));
            }
        }
        protected override Dictionary<MemberInfo, Expression> VisitLambda(LambdaExpression exp)
        {
            return this.Visit(exp.Body);
        }
        protected override Dictionary<MemberInfo, Expression> VisitMemberInit(MemberInitExpression exp)
        {
            Dictionary<MemberInfo, Expression> ret = new Dictionary<MemberInfo, Expression>(exp.Bindings.Count);

            foreach (MemberBinding binding in exp.Bindings)
            {
                if (binding.BindingType != MemberBindingType.Assignment)
                {
                    throw new NotSupportedException();
                }

                MemberAssignment memberAssignment = (MemberAssignment)binding;
                MemberInfo member = memberAssignment.Member;

                ret.Add(member, memberAssignment.Expression);
            }

            return ret;
        }
    }
}
