using Chloe.DbExpressions;
using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.SqlServer
{
    internal static class Utils
    {
        static readonly Dictionary<Type, Type> ToStringableNumericTypes;

        static Utils()
        {
            List<Type> toStringableNumericTypes = new List<Type>();
            toStringableNumericTypes.Add(typeof(byte));
            toStringableNumericTypes.Add(typeof(sbyte));
            toStringableNumericTypes.Add(typeof(short));
            toStringableNumericTypes.Add(typeof(ushort));
            toStringableNumericTypes.Add(typeof(int));
            toStringableNumericTypes.Add(typeof(uint));
            toStringableNumericTypes.Add(typeof(long));
            toStringableNumericTypes.Add(typeof(ulong));
            ToStringableNumericTypes = toStringableNumericTypes.ToDictionary(a => a, a => a);
        }

        public static string QuoteName(string name)
        {
            return string.Concat("[", name, "]");
        }
        public static void CheckNull(object obj, string paramName = null)
        {
            if (obj == null)
                throw new ArgumentNullException(paramName);
        }

        public static bool AreEqual(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
                return true;

            if (obj1 != null)
            {
                return obj1.Equals(obj2);
            }

            if (obj2 != null)
            {
                return obj2.Equals(obj1);
            }

            return object.Equals(obj1, obj2);
        }

        public static Dictionary<TKey, TValue> Clone<TKey, TValue>(Dictionary<TKey, TValue> source, IEqualityComparer<TKey> comparer = null)
        {
            Dictionary<TKey, TValue> ret;
            if (comparer == null)
                ret = new Dictionary<TKey, TValue>(source.Count);
            else
                ret = new Dictionary<TKey, TValue>(source.Count, comparer);

            foreach (var kv in source)
            {
                ret.Add(kv.Key, kv.Value);
            }

            return ret;
        }

        public static bool IsToStringableNumericType(Type type)
        {
            type = ReflectionExtension.GetUnderlyingType(type);
            return ToStringableNumericTypes.ContainsKey(type);
        }
    }
}
