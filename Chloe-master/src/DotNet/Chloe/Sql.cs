using System;

namespace Chloe
{
    public class Sql
    {
        /// <summary>
        /// 比较两个相同类型的对象值是否相等，仅支持在 lambda 表达式树中使用。
        /// 使用此方法与双等号（==）的区别是：a => Sql.Equals(a.Name, a.XName) 会被翻译成 a.Name == a.XName，而 a => a.Name == a.XName 则会被翻译成 a.Name == a.XName or (a.Name is null and a.XName is null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool Equals<T>(T value1, T value2)
        {
            throw new NotSupportedException();
        }
        /// <summary>
        /// 比较两个相同类型的对象值是否不相等，仅支持在 lambda 表达式树中使用。
        /// 使用此方法与不等号（!=）的区别是：a => Sql.NotEquals(a.Name, a.XName) 会被翻译成 a.Name &lt;&gt; a.XName，而 a => a.Name != a.XName 则会被翻译成 a.Name &lt;&gt; a.XName or (a.Name is null and a.XName is not null) or (a.Name is not null and a.XName is null)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value1"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        public static bool NotEquals<T>(T value1, T value2)
        {
            throw new NotSupportedException();
        }
    }
}
