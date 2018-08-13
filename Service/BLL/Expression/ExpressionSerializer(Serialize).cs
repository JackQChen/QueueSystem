using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using System.Runtime.Serialization;
using System.IO;
using System.Text;

namespace ExpressionSerialization
{
    public partial class ExpressionSerializer
    {
        /// <summary>
        /// generate XML attributes for these primitive Types.
        /// </summary>
        static readonly Type[] primitiveTypes = new[] { typeof(string), typeof(int), typeof(bool), typeof(ExpressionType) };
        private Dictionary<string, ParameterExpression> parameters = new Dictionary<string, ParameterExpression>();
        private TypeResolver resolver;
        public List<CustomExpressionXmlConverter> Converters { get; private set; }

        public ExpressionSerializer(TypeResolver resolver, IEnumerable<CustomExpressionXmlConverter> converters = null)
        {
            this.resolver = resolver;
            if (converters != null)
                this.Converters = new List<CustomExpressionXmlConverter>(converters);
            else
                Converters = new List<CustomExpressionXmlConverter>();
        }

        public ExpressionSerializer()
        {
            this.resolver = new TypeResolver(null, null);
            Converters = new List<CustomExpressionXmlConverter>();
        }



        /*
         * SERIALIZATION 
         */

        public XElement Serialize(Expression e)
        {
            if (e.NodeType != ExpressionType.Lambda)
                e = Evaluator.PartialEval(e);//TODO: decide should we call PartialEval or not at all?
            return GenerateXmlFromExpressionCore(e);
        }


        /// <summary>
        /// Uses first applicable custom serializer, then returns.
        /// Does not attempt to use all custom serializers.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        bool TryCustomSerializers(Expression e, out XElement result)
        {
            result = null;
            int i = 0;
            while (i < this.Converters.Count)
            {
                if (this.Converters[i].TrySerialize(e, out result))
                    return true;
                i++;
            }
            return false;
        }


        private object GenerateXmlFromProperty(Type propType, string propName, object value)
        {
            if (primitiveTypes.Contains(propType))
                return GenerateXmlFromPrimitive(propName, value);

            if (propType.Equals(typeof(object)))//expected: caller invokes with value == a ConstantExpression.Value
            {
                return GenerateXmlFromObject(propName, value);
            }
            if (typeof(Expression).IsAssignableFrom(propType))
                return GenerateXmlFromExpression(propName, value as Expression);
            if (value is MethodInfo || propType.Equals(typeof(MethodInfo)))
                return GenerateXmlFromMethodInfo(propName, value as MethodInfo);
            if (value is PropertyInfo || propType.Equals(typeof(PropertyInfo)))
                return GenerateXmlFromPropertyInfo(propName, value as PropertyInfo);
            if (value is FieldInfo || propType.Equals(typeof(FieldInfo)))
                return GenerateXmlFromFieldInfo(propName, value as FieldInfo);
            if (value is ConstructorInfo || propType.Equals(typeof(ConstructorInfo)))
                return GenerateXmlFromConstructorInfo(propName, value as ConstructorInfo);
            if (propType.Equals(typeof(Type)))
                return GenerateXmlFromType(propName, value as Type);
            if (IsIEnumerableOf<Expression>(propType))
                return GenerateXmlFromExpressionList(propName, AsIEnumerableOf<Expression>(value));
            if (IsIEnumerableOf<MemberInfo>(propType))
                return GenerateXmlFromMemberInfoList(propName, AsIEnumerableOf<MemberInfo>(value));
            if (IsIEnumerableOf<ElementInit>(propType))
                return GenerateXmlFromElementInitList(propName, AsIEnumerableOf<ElementInit>(value));
            if (IsIEnumerableOf<MemberBinding>(propType))
                return GenerateXmlFromBindingList(propName, AsIEnumerableOf<MemberBinding>(value));
            throw new NotSupportedException(propName);
        }

        /// <summary>
        /// Called from somewhere on call stack... from ConstantExpression.Value
        /// Modified since original code for this method was incorrectly getting the value as 
        /// .ToString() for non-primitive types, which ExpressionSerializer was 
        /// unable to later parse back into a value (ExpressionSerializer.ParseConstantFromElement).
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="value">ConstantExpression.Value</param>
        /// <returns></returns>
        private object GenerateXmlFromObject(string propName, object value)
        {
            Assembly mscorlib = typeof(string).Assembly;
            object result = null;
            if (value is Type)
                result = GenerateXmlFromTypeCore((Type)value);
            else if (mscorlib.GetTypes().Any(t => t == value.GetType()))
                result = value.ToString();
            //else
            //    throw new ArgumentException(string.Format("Unable to generate XML for value of Type '{0}'.\nType is not recognized.", value.GetType().FullName));
            else
                result = value.ToString();
            return new XElement(propName,
                result);
        }

        /// <summary>
        /// For use with ConstantExpression.Value
        /// </summary>
        /// <param name="xName"></param>
        /// <param name="instance"></param>
        /// <returns></returns>
        private object GenerateXmlFromKnownTypes(string xName, object instance, Type knownType)
        {
            string xml;
            XElement xelement;
            dynamic something = instance;

            if (typeof(IQueryable).IsAssignableFrom(instance.GetType()))
            {
                if (typeof(Query<>).MakeGenericType(knownType).IsAssignableFrom(instance.GetType()))
                {
                    return instance.ToString();
                }
                something = LinqHelper.CastToGenericEnumerable((IQueryable)instance, knownType);
                something = Enumerable.ToArray(something);
            }
            Type instanceType = something.GetType();
            DataContractSerializer serializer = new DataContractSerializer(instanceType, this.resolver.knownTypes);

            using (MemoryStream ms = new MemoryStream())
            {
                serializer.WriteObject(ms, something);
                ms.Position = 0;
                StreamReader reader = new StreamReader(ms, Encoding.UTF8);
                xml = reader.ReadToEnd();
                xelement = new XElement(xName, xml);
                return xelement;
            }
        }
        private bool IsIEnumerableOf<T>(Type propType)
        {
            if (!propType.IsGenericType)
                return false;
            Type[] typeArgs = propType.GetGenericArguments();
            if (typeArgs.Length != 1)
                return false;
            if (!typeof(T).IsAssignableFrom(typeArgs[0]))
                return false;
            if (!typeof(IEnumerable<>).MakeGenericType(typeArgs).IsAssignableFrom(propType))
                return false;
            return true;
        }
        private bool IsIEnumerableOf(Type enumerableType, Type elementType)
        {
            if (!enumerableType.IsGenericType)
                return false;
            Type[] typeArgs = enumerableType.GetGenericArguments();
            if (typeArgs.Length != 1)
                return false;
            if (!elementType.IsAssignableFrom(typeArgs[0]))
                return false;
            if (!typeof(IEnumerable<>).MakeGenericType(typeArgs).IsAssignableFrom(enumerableType))
                return false;
            return true;
        }


        private IEnumerable<T> AsIEnumerableOf<T>(object value)
        {
            if (value == null)
                return null;
            return (value as IEnumerable).Cast<T>();
        }

        private object GenerateXmlFromElementInitList(string propName, IEnumerable<ElementInit> initializers)
        {
            if (initializers == null)
                initializers = new ElementInit[] { };
            return new XElement(propName,
                from elementInit in initializers
                select GenerateXmlFromElementInitializer(elementInit));
        }

        private object GenerateXmlFromElementInitializer(ElementInit elementInit)
        {
            return new XElement("ElementInit",
                GenerateXmlFromMethodInfo("AddMethod", elementInit.AddMethod),
                GenerateXmlFromExpressionList("Arguments", elementInit.Arguments));
        }

        private object GenerateXmlFromExpressionList(string propName, IEnumerable<Expression> expressions)
        {
            XElement result = new XElement(propName,
                    from expression in expressions
                    select GenerateXmlFromExpressionCore(expression));
            return result;
        }

        private object GenerateXmlFromMemberInfoList(string propName, IEnumerable<MemberInfo> members)
        {
            if (members == null)
                members = new MemberInfo[] { };
            return new XElement(propName,
                   from member in members
                   select GenerateXmlFromProperty(member.GetType(), "Info", member));
        }

        private object GenerateXmlFromBindingList(string propName, IEnumerable<MemberBinding> bindings)
        {
            if (bindings == null)
                bindings = new MemberBinding[] { };
            return new XElement(propName,
                from binding in bindings
                select GenerateXmlFromBinding(binding));
        }

        private object GenerateXmlFromBinding(MemberBinding binding)
        {
            switch (binding.BindingType)
            {
                case MemberBindingType.Assignment:
                    return GenerateXmlFromAssignment(binding as MemberAssignment);
                case MemberBindingType.ListBinding:
                    return GenerateXmlFromListBinding(binding as MemberListBinding);
                case MemberBindingType.MemberBinding:
                    return GenerateXmlFromMemberBinding(binding as MemberMemberBinding);
                default:
                    throw new NotSupportedException(string.Format("Binding type {0} not supported.", binding.BindingType));
            }
        }

        private object GenerateXmlFromMemberBinding(MemberMemberBinding memberMemberBinding)
        {
            return new XElement("MemberMemberBinding",
                GenerateXmlFromProperty(memberMemberBinding.Member.GetType(), "Member", memberMemberBinding.Member),
                GenerateXmlFromBindingList("Bindings", memberMemberBinding.Bindings));
        }


        private object GenerateXmlFromListBinding(MemberListBinding memberListBinding)
        {
            return new XElement("MemberListBinding",
                GenerateXmlFromProperty(memberListBinding.Member.GetType(), "Member", memberListBinding.Member),
                GenerateXmlFromProperty(memberListBinding.Initializers.GetType(), "Initializers", memberListBinding.Initializers));
        }

        private object GenerateXmlFromAssignment(MemberAssignment memberAssignment)
        {
            return new XElement("MemberAssignment",
                GenerateXmlFromProperty(memberAssignment.Member.GetType(), "Member", memberAssignment.Member),
                GenerateXmlFromProperty(memberAssignment.Expression.GetType(), "Expression", memberAssignment.Expression));
        }

        private XElement GenerateXmlFromExpression(string propName, Expression e)
        {
            return new XElement(propName, GenerateXmlFromExpressionCore(e));
        }

        private object GenerateXmlFromType(string propName, Type type)
        {
            return new XElement(propName, GenerateXmlFromTypeCore(type));
        }

        private XElement GenerateXmlFromTypeCore(Type type)
        {
            //vsadov: add detection of VB anon types
            if (type.Name.StartsWith("<>f__") || type.Name.StartsWith("VB$AnonymousType"))
                return new XElement("AnonymousType",
                    new XAttribute("Name", type.FullName),
                    from property in type.GetProperties()
                    select new XElement("Property",
                        new XAttribute("Name", property.Name),
                        GenerateXmlFromTypeCore(property.PropertyType)),
                    new XElement("Constructor",
                            from parameter in type.GetConstructors().First().GetParameters()
                            select new XElement("Parameter",
                                new XAttribute("Name", parameter.Name),
                                GenerateXmlFromTypeCore(parameter.ParameterType))
                    ));

            else
            {
                //vsadov: GetGenericArguments returns args for nongeneric types 
                //like arrays no need to save them.
                if (type.IsGenericType)
                {
                    return new XElement("Type",
                                            new XAttribute("Name", type.GetGenericTypeDefinition().FullName),
                                            from genArgType in type.GetGenericArguments()
                                            select GenerateXmlFromTypeCore(genArgType));
                }
                else
                {
                    return new XElement("Type", new XAttribute("Name", type.FullName));
                }

            }
        }

        private object GenerateXmlFromPrimitive(string propName, object value)
        {
            return new XAttribute(propName, value);
        }

        private object GenerateXmlFromMethodInfo(string propName, MethodInfo methodInfo)
        {
            if (methodInfo == null)
                return new XElement(propName);
            return new XElement(propName,
                        new XAttribute("MemberType", methodInfo.MemberType),
                        new XAttribute("MethodName", methodInfo.Name),
                        GenerateXmlFromType("DeclaringType", methodInfo.DeclaringType),
                        new XElement("Parameters",
                            from param in methodInfo.GetParameters()
                            select GenerateXmlFromType("Type", param.ParameterType)),
                        new XElement("GenericArgTypes",
                            from argType in methodInfo.GetGenericArguments()
                            select GenerateXmlFromType("Type", argType)));
        }

        private object GenerateXmlFromPropertyInfo(string propName, PropertyInfo propertyInfo)
        {
            if (propertyInfo == null)
                return new XElement(propName);
            return new XElement(propName,
                        new XAttribute("MemberType", propertyInfo.MemberType),
                        new XAttribute("PropertyName", propertyInfo.Name),
                        GenerateXmlFromType("DeclaringType", propertyInfo.DeclaringType),
                        new XElement("IndexParameters",
                            from param in propertyInfo.GetIndexParameters()
                            select GenerateXmlFromType("Type", param.ParameterType)));
        }

        private object GenerateXmlFromFieldInfo(string propName, FieldInfo fieldInfo)
        {
            if (fieldInfo == null)
                return new XElement(propName);
            return new XElement(propName,
                        new XAttribute("MemberType", fieldInfo.MemberType),
                        new XAttribute("FieldName", fieldInfo.Name),
                        GenerateXmlFromType("DeclaringType", fieldInfo.DeclaringType));
        }

        private object GenerateXmlFromConstructorInfo(string propName, ConstructorInfo constructorInfo)
        {
            if (constructorInfo == null)
                return new XElement(propName);
            return new XElement(propName,
                        new XAttribute("MemberType", constructorInfo.MemberType),
                        new XAttribute("MethodName", constructorInfo.Name),
                        GenerateXmlFromType("DeclaringType", constructorInfo.DeclaringType),
                        new XElement("Parameters",
                            from param in constructorInfo.GetParameters()
                            select new XElement("Parameter",
                                new XAttribute("Name", param.Name),
                                GenerateXmlFromType("Type", param.ParameterType))));
        }



    }
}
