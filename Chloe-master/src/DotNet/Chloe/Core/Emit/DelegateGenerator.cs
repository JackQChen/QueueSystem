using Chloe.Extensions;
using Chloe.Infrastructure;
using Chloe.InternalExtensions;
using Chloe.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Chloe.Core.Emit
{
    public static class DelegateGenerator
    {
        public static Action<object, IDataReader, int> CreateSetValueFromReaderDelegate(MemberInfo member)
        {
            Action<object, IDataReader, int> del = null;

            DynamicMethod dm = new DynamicMethod("SetValueFromReader_" + Guid.NewGuid().ToString("N"), null, new Type[] { typeof(object), typeof(IDataReader), typeof(int) }, true);
            ILGenerator il = dm.GetILGenerator();

            il.Emit(OpCodes.Ldarg_S, 0);//将第一个参数 object 对象加载到栈顶
            il.Emit(OpCodes.Castclass, member.DeclaringType);//将 object 对象转换为强类型对象 此时栈顶为强类型的对象

            var readerMethod = DataReaderConstant.GetReaderMethod(member.GetMemberType());

            //ordinal
            il.Emit(OpCodes.Ldarg_S, 1);    //加载参数DataReader
            il.Emit(OpCodes.Ldarg_S, 2);    //加载 read ordinal
            il.EmitCall(OpCodes.Call, readerMethod, null);     //调用对应的 readerMethod 得到 value  reader.Getxx(ordinal);  此时栈顶为 value

            EmitHelper.SetValueIL(il, member); // object.XX = value; 此时栈顶为空

            il.Emit(OpCodes.Ret);   // 即可 return

            del = (Action<object, IDataReader, int>)dm.CreateDelegate(typeof(Action<object, IDataReader, int>));
            return del;
        }
        public static Action<object, IDataReader, int> CreateSetValueFromReaderDelegate1(MemberInfo member)
        {
            Action<object, IDataReader, int> del = null;

            var p = Expression.Parameter(typeof(object), "instance");
            var instance = Expression.Convert(p, member.DeclaringType);
            var reader = Expression.Parameter(typeof(IDataReader), "reader");
            var ordinal = Expression.Parameter(typeof(int), "ordinal");

            var readerMethod = DataReaderConstant.GetReaderMethod(member.GetMemberType());

            var getValue = Expression.Call(null, readerMethod, reader, ordinal);

            var assign = ExpressionExtension.Assign(member, instance, getValue);

            var lambda = Expression.Lambda<Action<object, IDataReader, int>>(assign, p, reader, ordinal);
            del = lambda.Compile();

            return del;
        }

        public static Func<IDataReader, ReaderOrdinalEnumerator, ObjectActivatorEnumerator, object> CreateObjectGenerator(ConstructorInfo constructor)
        {
            Utils.CheckNull(constructor);

            Func<IDataReader, ReaderOrdinalEnumerator, ObjectActivatorEnumerator, object> ret = null;

            var pExp_reader = Expression.Parameter(typeof(IDataReader), "reader");
            var pExp_readerOrdinalEnumerator = Expression.Parameter(typeof(ReaderOrdinalEnumerator), "readerOrdinalEnumerator");
            var pExp_objectActivatorEnumerator = Expression.Parameter(typeof(ObjectActivatorEnumerator), "objectActivatorEnumerator");

            ParameterInfo[] parameters = constructor.GetParameters();
            List<Expression> arguments = new List<Expression>(parameters.Length);

            foreach (ParameterInfo parameter in parameters)
            {
                if (MappingTypeSystem.IsMappingType(parameter.ParameterType))
                {
                    var readerMethod = DataReaderConstant.GetReaderMethod(parameter.ParameterType);
                    //int ordinal = readerOrdinalEnumerator.Next();
                    var readerOrdinal = Expression.Call(pExp_readerOrdinalEnumerator, ReaderOrdinalEnumerator.MethodOfNext);
                    //DataReaderExtensions.GetValue(reader,readerOrdinal)
                    var getValue = Expression.Call(readerMethod, pExp_reader, readerOrdinal);
                    arguments.Add(getValue);
                }
                else
                {
                    //IObjectActivator oa = objectActivatorEnumerator.Next();
                    var oa = Expression.Call(pExp_objectActivatorEnumerator, ObjectActivatorEnumerator.MethodOfNext);
                    //object obj = oa.CreateInstance(IDataReader reader);
                    var entity = Expression.Call(oa, typeof(IObjectActivator).GetMethod("CreateInstance"), pExp_reader);
                    //(T)entity;
                    var val = Expression.Convert(entity, parameter.ParameterType);
                    arguments.Add(val);
                }
            }

            var body = Expression.New(constructor, arguments);

            ret = Expression.Lambda<Func<IDataReader, ReaderOrdinalEnumerator, ObjectActivatorEnumerator, object>>(body, pExp_reader, pExp_readerOrdinalEnumerator, pExp_objectActivatorEnumerator).Compile();

            return ret;
        }
        public static Func<IDataReader, int, object> CreateMappingTypeGenerator(Type type)
        {
            var pExp_reader = Expression.Parameter(typeof(IDataReader), "reader");
            var pExp_readerOrdinal = Expression.Parameter(typeof(int), "readerOrdinal");

            var readerMethod = DataReaderConstant.GetReaderMethod(type);
            var getValue = Expression.Call(readerMethod, pExp_reader, pExp_readerOrdinal);
            var body = Expression.Convert(getValue, typeof(object));

            Func<IDataReader, int, object> ret = Expression.Lambda<Func<IDataReader, int, object>>(body, pExp_reader, pExp_readerOrdinal).Compile();

            return ret;
        }

        public static Action<object, object> CreateValueSetter(MemberInfo propertyOrField)
        {
            PropertyInfo propertyInfo = propertyOrField as PropertyInfo;
            if (propertyInfo != null)
                return CreateValueSetter(propertyInfo);

            FieldInfo fieldInfo = propertyOrField as FieldInfo;
            if (fieldInfo != null)
                return CreateValueSetter(fieldInfo);

            throw new ArgumentException();
        }
        public static Action<object, object> CreateValueSetter(PropertyInfo propertyInfo)
        {
            var p = Expression.Parameter(typeof(object), "instance");
            var pValue = Expression.Parameter(typeof(object), "value");
            var instance = Expression.Convert(p, propertyInfo.DeclaringType);
            var value = Expression.Convert(pValue, propertyInfo.PropertyType);

            var pro = Expression.Property(instance, propertyInfo);
            var setValue = Expression.Assign(pro, value);

            Expression body = setValue;

            var lambda = Expression.Lambda<Action<object, object>>(body, p, pValue);
            Action<object, object> ret = lambda.Compile();

            return ret;
        }
        public static Action<object, object> CreateValueSetter(FieldInfo fieldInfo)
        {
            var p = Expression.Parameter(typeof(object), "instance");
            var pValue = Expression.Parameter(typeof(object), "value");
            var instance = Expression.Convert(p, fieldInfo.DeclaringType);
            var value = Expression.Convert(pValue, fieldInfo.FieldType);

            var field = Expression.Field(instance, fieldInfo);
            var setValue = Expression.Assign(field, value);

            Expression body = setValue;

            var lambda = Expression.Lambda<Action<object, object>>(body, p, pValue);
            Action<object, object> ret = lambda.Compile();

            return ret;
        }
        public static Func<object, object> CreateValueGetter(MemberInfo propertyOrField)
        {
            var p = Expression.Parameter(typeof(object), "a");
            var instance = Expression.Convert(p, propertyOrField.DeclaringType);
            var memberAccess = Expression.MakeMemberAccess(instance, propertyOrField);

            Type type = ReflectionExtension.GetMemberType(propertyOrField);

            Expression body = memberAccess;
            if (type.IsValueType)
            {
                body = Expression.Convert(memberAccess, typeof(object));
            }

            var lambda = Expression.Lambda<Func<object, object>>(body, p);
            Func<object, object> ret = lambda.Compile();

            return ret;
        }
    }
}
