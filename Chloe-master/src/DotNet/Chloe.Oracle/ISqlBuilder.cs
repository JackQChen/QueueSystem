using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.Oracle
{
    interface ISqlBuilder
    {
        string ToSql();
        ISqlBuilder Append(object obj);
        ISqlBuilder Append(object obj1, object obj2);
        ISqlBuilder Append(object obj1, object obj2, object obj3);
        ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4);
        ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4, object obj5);
        ISqlBuilder Append(params object[] objs);
    }

    class SqlBuilder : ISqlBuilder
    {
        StringBuilder _sb = new StringBuilder();
        public string ToSql()
        {
            return this._sb.ToString();
        }
        public ISqlBuilder Append(object obj)
        {
            this._sb.Append(obj);
            return this;
        }
        public ISqlBuilder Append(object obj1, object obj2)
        {
            return this.Append(obj1).Append(obj2);
        }
        public ISqlBuilder Append(object obj1, object obj2, object obj3)
        {
            return this.Append(obj1).Append(obj2).Append(obj3);
        }
        public ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4)
        {
            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4);
        }
        public ISqlBuilder Append(object obj1, object obj2, object obj3, object obj4, object obj5)
        {
            return this.Append(obj1).Append(obj2).Append(obj3).Append(obj4).Append(obj5);
        }
        public ISqlBuilder Append(params object[] objs)
        {
            if (objs == null)
                return this;

            for (int i = 0; i < objs.Length; i++)
            {
                var obj = objs[i];
                this.Append(obj);
            }

            return this;
        }
    }

}
