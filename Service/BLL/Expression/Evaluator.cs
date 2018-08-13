using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
namespace ExpressionSerialization
{
    public static class Evaluator
    {
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <param name="fnCanBeEvaluated">A function that decides whether a given expression node can be part of the local function.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression, Func<Expression, bool> fnCanBeEvaluated)
        {
            var nominator = new Nominator(fnCanBeEvaluated);
            var subtreeEvaluator = new SubtreeEvaluator(nominator.Nominate(expression));
            return subtreeEvaluator.Eval(expression);
        }
        /// <summary>
        /// Performs evaluation & replacement of independent sub-trees
        /// </summary>
        /// <param name="expression">The root of the expression tree.</param>
        /// <returns>A new tree with sub-trees evaluated and replaced.</returns>
        public static Expression PartialEval(Expression expression)
        {
            return PartialEval(expression, Evaluator.CanBeEvaluatedLocally);
        }

        /// <summary>
        /// Anything which involves has a sub-Expression as ParameterExpression, such as a MemberExpression,
        /// will not pass this check.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }
        /// <summary>
        /// Evaluates & replaces sub-trees when first candidate is reached (top-down)
        /// </summary>
        class SubtreeEvaluator : ExpressionVisitor
        {
            HashSet<Expression> candidates;
            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                this.candidates = candidates;
            }
            internal Expression Eval(Expression exp)
            {
                return this.Visit(exp);
            }

            /// <summary>
            /// Attempt to evaluate each node upon visiting. If the node is a "candidate" (Nominator),
            /// then we replace the Expression node with its evaluated form.
            /// </summary>
            /// <param name="exp"></param>
            /// <returns></returns>
            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }
                if (this.candidates.Contains(exp))
                {
                    return this.Evaluate(exp);//immediately returns from depth-first tree traversal.
                    //so, if it's a BinaryExpression and it's already a candidate, it can be evaluated 
                    //and immediately returned without visitng the Left, Right child nodes.
                }
                return base.Visit(exp);
                //if it's a BinaryExpression and isn't a candidate, then base.Visit will
                //call VisitBinary, which will attempt to evaluate both child Left, Right nodes.
            }

            private Expression Evaluate(Expression e)
            {
                //we have assumed no parameters required for this Expression
                //see (fnCanBeEvaluated)
                LambdaExpression lambda;
                Delegate fn;
                object result;
                switch (e.NodeType)
                {
                    case ExpressionType.Constant:
                        return e;
                    case ExpressionType.Lambda:
                        //case ExpressionType.Lambda:
                        //    lambda = (LambdaExpression)e;
                        //    fn = lambda.Compile();
                        //    result = fn.DynamicInvoke(null);
                        //    return Expression.Constant(result, lambda.ReturnType);
                        return e;
                    //Decided NOT to return a ConstantExpression of the LambdaExpression itself, nor 
                    //the result of invoking a zero-parameter LambdaExpression.
                    default:
                        lambda = Expression.Lambda(e);
                        fn = lambda.Compile();
                        result = fn.DynamicInvoke(null);
                        return Expression.Constant(result, e.Type);
                }
            }
        }

        /// <summary>
        /// Performs bottom-up analysis to determine which nodes can possibly
        /// be part of an evaluated sub-tree.
        /// </summary>
        class Nominator : ExpressionVisitor
        {
            Func<Expression, bool> fnCanBeEvaluated;
            HashSet<Expression> candidates;
            bool cannotBeEvaluated;

            internal Nominator(Func<Expression, bool> fnCanBeEvaluated)
            {
                this.fnCanBeEvaluated = fnCanBeEvaluated;
            }

            internal HashSet<Expression> Nominate(Expression expression)
            {
                this.candidates = new HashSet<Expression>();
                this.Visit(expression);
                return this.candidates;
            }

            /// <summary>
            /// If a child node cannot be evaluated, then its parent can't either. 
            /// A Expression node will fail to be a candidate if it (or a sub-Expression) has a ParameterExpression
            /// </summary>
            /// <param name="expression"></param>
            /// <returns></returns>
            public override Expression Visit(Expression expression)
            {
                if (expression != null)
                {
                    bool saveCannotBeEvaluated = this.cannotBeEvaluated;
                    this.cannotBeEvaluated = false;
                    base.Visit(expression);//visit all child (sub)-expressions...
                    //after finished visiting all child expressions:
                    if (!this.cannotBeEvaluated)
                    {
                        if (this.fnCanBeEvaluated(expression))
                        {
                            this.candidates.Add(expression);
                        }
                        else
                        {
                            this.cannotBeEvaluated = true;
                        }
                    }
                    this.cannotBeEvaluated |= saveCannotBeEvaluated;
                }
                return expression;
            }
        }
    }
}
