using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;

namespace ExpressionSerialization
{
    //GenerateXmlFromExpressionCore
    public partial class ExpressionSerializer
    {

        public XElement GenerateXmlFromExpressionCore(Expression e)
        {
            XElement replace;
            if (e == null)
                return null;
            else if (TryCustomSerializers(e, out replace))
                return replace;
            else if (e is BinaryExpression)
            {
                return BinaryExpressionToXElement((BinaryExpression)e);
            }

            else if (e is BlockExpression)
            {
                return BlockExpressionToXElement((BlockExpression)e);
            }

            else if (e is ConditionalExpression)
            {
                return ConditionalExpressionToXElement((ConditionalExpression)e);
            }

            else if (e is ConstantExpression)
            {
                return ConstantExpressionToXElement((ConstantExpression)e);
            }

            else if (e is DebugInfoExpression)
            {
                return DebugInfoExpressionToXElement((DebugInfoExpression)e);
            }

            else if (e is DefaultExpression)
            {
                return DefaultExpressionToXElement((DefaultExpression)e);
            }

            else if (e is DynamicExpression)
            {
                return DynamicExpressionToXElement((DynamicExpression)e);
            }

            else if (e is GotoExpression)
            {
                return GotoExpressionToXElement((GotoExpression)e);
            }

            else if (e is IndexExpression)
            {
                return IndexExpressionToXElement((IndexExpression)e);
            }

            else if (e is InvocationExpression)
            {
                return InvocationExpressionToXElement((InvocationExpression)e);
            }

            else if (e is LabelExpression)
            {
                return LabelExpressionToXElement((LabelExpression)e);
            }

            else if (e is LambdaExpression)
            {
                return LambdaExpressionToXElement((LambdaExpression)e);
            }

            else if (e is ListInitExpression)
            {
                return ListInitExpressionToXElement((ListInitExpression)e);
            }

            else if (e is LoopExpression)
            {
                return LoopExpressionToXElement((LoopExpression)e);
            }

            else if (e is MemberExpression)
            {
                return MemberExpressionToXElement((MemberExpression)e);
            }

            else if (e is MemberInitExpression)
            {
                return MemberInitExpressionToXElement((MemberInitExpression)e);
            }

            else if (e is MethodCallExpression)
            {
                return MethodCallExpressionToXElement((MethodCallExpression)e);
            }

            else if (e is NewArrayExpression)
            {
                return NewArrayExpressionToXElement((NewArrayExpression)e);
            }

            else if (e is NewExpression)
            {
                return NewExpressionToXElement((NewExpression)e);
            }

            else if (e is ParameterExpression)
            {
                return ParameterExpressionToXElement((ParameterExpression)e);
            }

            else if (e is RuntimeVariablesExpression)
            {
                return RuntimeVariablesExpressionToXElement((RuntimeVariablesExpression)e);
            }

            else if (e is SwitchExpression)
            {
                return SwitchExpressionToXElement((SwitchExpression)e);
            }

            else if (e is TryExpression)
            {
                return TryExpressionToXElement((TryExpression)e);
            }

            else if (e is TypeBinaryExpression)
            {
                return TypeBinaryExpressionToXElement((TypeBinaryExpression)e);
            }

            else if (e is UnaryExpression)
            {
                return UnaryExpressionToXElement((UnaryExpression)e);
            }
            else
                return null;
        }//end GenerateXmlFromExpressionCore


        internal XElement BinaryExpressionToXElement(BinaryExpression e)
        {
            object value;
            string xName = "BinaryExpression";
            object[] XElementValues = new object[9];
            value = ((BinaryExpression)e).CanReduce;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            value = ((BinaryExpression)e).Right;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Right", value ?? string.Empty);
            value = ((BinaryExpression)e).Left;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Left", value ?? string.Empty);
            value = ((BinaryExpression)e).Method;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Reflection.MethodInfo),
                "Method", value ?? string.Empty);
            value = ((BinaryExpression)e).Conversion;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.LambdaExpression),
                "Conversion", value ?? string.Empty);
            value = ((BinaryExpression)e).IsLifted;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "IsLifted", value ?? string.Empty);
            value = ((BinaryExpression)e).IsLiftedToNull;
            XElementValues[6] = GenerateXmlFromProperty(typeof(System.Boolean),
                "IsLiftedToNull", value ?? string.Empty);
            value = ((BinaryExpression)e).NodeType;
            XElementValues[7] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((BinaryExpression)e).Type;
            XElementValues[8] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement BlockExpressionToXElement(BlockExpression e)
        {
            object value;
            string xName = "BlockExpression";
            object[] XElementValues = new object[6];
            value = ((BlockExpression)e).Expressions;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Expressions", value ?? string.Empty);
            value = ((BlockExpression)e).Variables;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression>),
                "Variables", value ?? string.Empty);
            value = ((BlockExpression)e).Result;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Result", value ?? string.Empty);
            value = ((BlockExpression)e).NodeType;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((BlockExpression)e).Type;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((BlockExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement ConditionalExpressionToXElement(ConditionalExpression e)
        {
            object value;
            string xName = "ConditionalExpression";
            object[] XElementValues = new object[6];
            value = ((ConditionalExpression)e).NodeType;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((ConditionalExpression)e).Type;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((ConditionalExpression)e).Test;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Test", value ?? string.Empty);
            value = ((ConditionalExpression)e).IfTrue;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "IfTrue", value ?? string.Empty);
            value = ((ConditionalExpression)e).IfFalse;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "IfFalse", value ?? string.Empty);
            value = ((ConditionalExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement ConstantExpressionToXElement(ConstantExpression e)
        {
            object value;
            string xName = "ConstantExpression";
            object[] XElementValues = new object[4];
            value = ((ConstantExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((ConstantExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((ConstantExpression)e).Value;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Object),
                "Value", value ?? string.Empty);
            value = ((ConstantExpression)e).CanReduce;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement DebugInfoExpressionToXElement(DebugInfoExpression e)
        {
            object value;
            string xName = "DebugInfoExpression";
            object[] XElementValues = new object[9];
            value = ((DebugInfoExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((DebugInfoExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((DebugInfoExpression)e).StartLine;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Int32),
                "StartLine", value ?? string.Empty);
            value = ((DebugInfoExpression)e).StartColumn;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Int32),
                "StartColumn", value ?? string.Empty);
            value = ((DebugInfoExpression)e).EndLine;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Int32),
                "EndLine", value ?? string.Empty);
            value = ((DebugInfoExpression)e).EndColumn;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Int32),
                "EndColumn", value ?? string.Empty);
            value = ((DebugInfoExpression)e).Document;
            XElementValues[6] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.SymbolDocumentInfo),
                "Document", value ?? string.Empty);
            value = ((DebugInfoExpression)e).IsClear;
            XElementValues[7] = GenerateXmlFromProperty(typeof(System.Boolean),
                "IsClear", value ?? string.Empty);
            value = ((DebugInfoExpression)e).CanReduce;
            XElementValues[8] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement DefaultExpressionToXElement(DefaultExpression e)
        {
            object value;
            string xName = "DefaultExpression";
            object[] XElementValues = new object[3];
            value = ((DefaultExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((DefaultExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((DefaultExpression)e).CanReduce;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement DynamicExpressionToXElement(DynamicExpression e)
        {
            object value;
            string xName = "DynamicExpression";
            object[] XElementValues = new object[6];
            value = ((DynamicExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((DynamicExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((DynamicExpression)e).Binder;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Runtime.CompilerServices.CallSiteBinder),
                "Binder", value ?? string.Empty);
            value = ((DynamicExpression)e).DelegateType;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Type),
                "DelegateType", value ?? string.Empty);
            value = ((DynamicExpression)e).Arguments;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Arguments", value ?? string.Empty);
            value = ((DynamicExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement GotoExpressionToXElement(GotoExpression e)
        {
            object value;
            string xName = "GotoExpression";
            object[] XElementValues = new object[6];
            value = ((GotoExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((GotoExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((GotoExpression)e).Value;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Value", value ?? string.Empty);
            value = ((GotoExpression)e).Target;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.LabelTarget),
                "Target", value ?? string.Empty);
            value = ((GotoExpression)e).Kind;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.GotoExpressionKind),
                "Kind", value ?? string.Empty);
            value = ((GotoExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement IndexExpressionToXElement(IndexExpression e)
        {
            object value;
            string xName = "IndexExpression";
            object[] XElementValues = new object[6];
            value = ((IndexExpression)e).NodeType;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((IndexExpression)e).Type;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((IndexExpression)e).Object;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Object", value ?? string.Empty);
            value = ((IndexExpression)e).Indexer;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Reflection.PropertyInfo),
                "Indexer", value ?? string.Empty);
            value = ((IndexExpression)e).Arguments;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Arguments", value ?? string.Empty);
            value = ((IndexExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement InvocationExpressionToXElement(InvocationExpression e)
        {
            object value;
            string xName = "InvocationExpression";
            object[] XElementValues = new object[5];
            value = ((InvocationExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((InvocationExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((InvocationExpression)e).Expression;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Expression", value ?? string.Empty);
            value = ((InvocationExpression)e).Arguments;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Arguments", value ?? string.Empty);
            value = ((InvocationExpression)e).CanReduce;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement LabelExpressionToXElement(LabelExpression e)
        {
            object value;
            string xName = "LabelExpression";
            object[] XElementValues = new object[5];
            value = ((LabelExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((LabelExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((LabelExpression)e).Target;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.LabelTarget),
                "Target", value ?? string.Empty);
            value = ((LabelExpression)e).DefaultValue;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "DefaultValue", value ?? string.Empty);
            value = ((LabelExpression)e).CanReduce;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement LambdaExpressionToXElement(LambdaExpression e)
        {
            object value;
            string xName = "LambdaExpression";
            object[] XElementValues = new object[8];
            value = ((LambdaExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((LambdaExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((LambdaExpression)e).Parameters;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression>),
                "Parameters", value ?? string.Empty);
            value = ((LambdaExpression)e).Name;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.String),
                "Name", value ?? string.Empty);
            value = ((LambdaExpression)e).Body;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Body", value ?? string.Empty);
            value = ((LambdaExpression)e).ReturnType;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Type),
                "ReturnType", value ?? string.Empty);
            value = ((LambdaExpression)e).TailCall;
            XElementValues[6] = GenerateXmlFromProperty(typeof(System.Boolean),
                "TailCall", value ?? string.Empty);
            value = ((LambdaExpression)e).CanReduce;
            XElementValues[7] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement ListInitExpressionToXElement(ListInitExpression e)
        {
            object value;
            string xName = "ListInitExpression";
            object[] XElementValues = new object[5];
            value = ((ListInitExpression)e).NodeType;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((ListInitExpression)e).Type;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((ListInitExpression)e).CanReduce;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            value = ((ListInitExpression)e).NewExpression;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.NewExpression),
                "NewExpression", value ?? string.Empty);
            value = ((ListInitExpression)e).Initializers;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ElementInit>),
                "Initializers", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement LoopExpressionToXElement(LoopExpression e)
        {
            object value;
            string xName = "LoopExpression";
            object[] XElementValues = new object[6];
            value = ((LoopExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((LoopExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((LoopExpression)e).Body;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Body", value ?? string.Empty);
            value = ((LoopExpression)e).BreakLabel;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.LabelTarget),
                "BreakLabel", value ?? string.Empty);
            value = ((LoopExpression)e).ContinueLabel;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.LabelTarget),
                "ContinueLabel", value ?? string.Empty);
            value = ((LoopExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement MemberExpressionToXElement(MemberExpression e)
        {
            object value;
            string xName = "MemberExpression";
            object[] XElementValues = new object[5];
            value = ((MemberExpression)e).Member;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Reflection.MemberInfo),
                "Member", value ?? string.Empty);
            value = ((MemberExpression)e).Expression;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Expression", value ?? string.Empty);
            value = ((MemberExpression)e).NodeType;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((MemberExpression)e).Type;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((MemberExpression)e).CanReduce;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement MemberInitExpressionToXElement(MemberInitExpression e)
        {
            object value;
            string xName = "MemberInitExpression";
            object[] XElementValues = new object[5];
            value = ((MemberInitExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((MemberInitExpression)e).CanReduce;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            value = ((MemberInitExpression)e).NodeType;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((MemberInitExpression)e).NewExpression;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.NewExpression),
                "NewExpression", value ?? string.Empty);
            value = ((MemberInitExpression)e).Bindings;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.MemberBinding>),
                "Bindings", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement MethodCallExpressionToXElement(MethodCallExpression e)
        {
            object value;
            string xName = "MethodCallExpression";
            object[] XElementValues = new object[6];
            value = ((MethodCallExpression)e).NodeType;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((MethodCallExpression)e).Type;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((MethodCallExpression)e).Method;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Reflection.MethodInfo),
                "Method", value ?? string.Empty);
            value = ((MethodCallExpression)e).Object;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Object", value ?? string.Empty);
            value = ((MethodCallExpression)e).Arguments;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Arguments", value ?? string.Empty);
            value = ((MethodCallExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement NewArrayExpressionToXElement(NewArrayExpression e)
        {
            object value;
            string xName = "NewArrayExpression";
            object[] XElementValues = new object[4];
            value = ((NewArrayExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((NewArrayExpression)e).Expressions;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Expressions", value ?? string.Empty);
            value = ((NewArrayExpression)e).NodeType;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((NewArrayExpression)e).CanReduce;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement NewExpressionToXElement(NewExpression e)
        {
            object value;
            string xName = "NewExpression";
            object[] XElementValues = new object[6];
            value = ((NewExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((NewExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((NewExpression)e).Constructor;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Reflection.ConstructorInfo),
                "Constructor", value ?? string.Empty);
            value = ((NewExpression)e).Arguments;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.Expression>),
                "Arguments", value ?? string.Empty);
            value = ((NewExpression)e).Members;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Reflection.MemberInfo>),
                "Members", value ?? string.Empty);
            value = ((NewExpression)e).CanReduce;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement ParameterExpressionToXElement(ParameterExpression e)
        {
            object value;
            string xName = "ParameterExpression";
            object[] XElementValues = new object[5];
            value = ((ParameterExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((ParameterExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((ParameterExpression)e).Name;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.String),
                "Name", value ?? string.Empty);
            value = ((ParameterExpression)e).IsByRef;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Boolean),
                "IsByRef", value ?? string.Empty);
            value = ((ParameterExpression)e).CanReduce;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement RuntimeVariablesExpressionToXElement(RuntimeVariablesExpression e)
        {
            object value;
            string xName = "RuntimeVariablesExpression";
            object[] XElementValues = new object[4];
            value = ((RuntimeVariablesExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((RuntimeVariablesExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((RuntimeVariablesExpression)e).Variables;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.ParameterExpression>),
                "Variables", value ?? string.Empty);
            value = ((RuntimeVariablesExpression)e).CanReduce;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement SwitchExpressionToXElement(SwitchExpression e)
        {
            object value;
            string xName = "SwitchExpression";
            object[] XElementValues = new object[7];
            value = ((SwitchExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((SwitchExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((SwitchExpression)e).SwitchValue;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "SwitchValue", value ?? string.Empty);
            value = ((SwitchExpression)e).Cases;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.SwitchCase>),
                "Cases", value ?? string.Empty);
            value = ((SwitchExpression)e).DefaultBody;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "DefaultBody", value ?? string.Empty);
            value = ((SwitchExpression)e).Comparison;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Reflection.MethodInfo),
                "Comparison", value ?? string.Empty);
            value = ((SwitchExpression)e).CanReduce;
            XElementValues[6] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement TryExpressionToXElement(TryExpression e)
        {
            object value;
            string xName = "TryExpression";
            object[] XElementValues = new object[7];
            value = ((TryExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((TryExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((TryExpression)e).Body;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Body", value ?? string.Empty);
            value = ((TryExpression)e).Handlers;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Collections.ObjectModel.ReadOnlyCollection<System.Linq.Expressions.CatchBlock>),
                "Handlers", value ?? string.Empty);
            value = ((TryExpression)e).Finally;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Finally", value ?? string.Empty);
            value = ((TryExpression)e).Fault;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Fault", value ?? string.Empty);
            value = ((TryExpression)e).CanReduce;
            XElementValues[6] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement TypeBinaryExpressionToXElement(TypeBinaryExpression e)
        {
            object value;
            string xName = "TypeBinaryExpression";
            object[] XElementValues = new object[5];
            value = ((TypeBinaryExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((TypeBinaryExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((TypeBinaryExpression)e).Expression;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Expression", value ?? string.Empty);
            value = ((TypeBinaryExpression)e).TypeOperand;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Type),
                "TypeOperand", value ?? string.Empty);
            value = ((TypeBinaryExpression)e).CanReduce;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method
        internal XElement UnaryExpressionToXElement(UnaryExpression e)
        {
            object value;
            string xName = "UnaryExpression";
            object[] XElementValues = new object[7];
            value = ((UnaryExpression)e).Type;
            XElementValues[0] = GenerateXmlFromProperty(typeof(System.Type),
                "Type", value ?? string.Empty);
            value = ((UnaryExpression)e).NodeType;
            XElementValues[1] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.ExpressionType),
                "NodeType", value ?? string.Empty);
            value = ((UnaryExpression)e).Operand;
            XElementValues[2] = GenerateXmlFromProperty(typeof(System.Linq.Expressions.Expression),
                "Operand", value ?? string.Empty);
            value = ((UnaryExpression)e).Method;
            XElementValues[3] = GenerateXmlFromProperty(typeof(System.Reflection.MethodInfo),
                "Method", value ?? string.Empty);
            value = ((UnaryExpression)e).IsLifted;
            XElementValues[4] = GenerateXmlFromProperty(typeof(System.Boolean),
                "IsLifted", value ?? string.Empty);
            value = ((UnaryExpression)e).IsLiftedToNull;
            XElementValues[5] = GenerateXmlFromProperty(typeof(System.Boolean),
                "IsLiftedToNull", value ?? string.Empty);
            value = ((UnaryExpression)e).CanReduce;
            XElementValues[6] = GenerateXmlFromProperty(typeof(System.Boolean),
                "CanReduce", value ?? string.Empty);
            return new XElement(xName, XElementValues);
        }//end static method


    }//end ExpressionSerializer class
}

