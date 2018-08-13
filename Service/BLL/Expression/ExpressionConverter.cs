using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using ExpressionSerialization;
using Model;
using System.Linq;
using System;

namespace BLL
{
    public class ExpressionConverter
    {
        static ExpressionSerializer _serializer;
        static TypeResolver _typeResolver;

        static ExpressionConverter()
        {
            _serializer = new ExpressionSerializer();
            var asm = typeof(TUserModel).Assembly;
            _typeResolver = new TypeResolver(
                new Assembly[] { asm },
                asm.GetTypes().Where(p => p.IsSubclassOf(typeof(ModelBase))).ToArray());
        }

        public static string Serialize(LambdaExpression exp)
        {
            return _serializer.Serialize(exp).ToString();
        }

        public static Expression<TDelegate> Deserialize<TDelegate>(string expString)
        {
            var convert = new ExpressionSerializer(_typeResolver, null);
            return convert.Deserialize<TDelegate>(XElement.Parse(expString));
        }

        public static Expression Deserialize(string expString)
        {
            var convert = new ExpressionSerializer(_typeResolver, null);
            return convert.Deserialize(XElement.Parse(expString));
        }
    }
}
