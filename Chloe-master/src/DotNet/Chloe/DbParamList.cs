using System;
using System.Collections.Generic;
using System.Linq;

namespace Chloe
{
    public class DbParamList : List<DbParam>
    {
        public object this[string name]
        {
            set
            {
                this.Add(name, value);
            }
        }
        public DbParam Add(string name, object value)
        {
            var p = DbParam.Create(name, value);
            this.Add(p);
            return p;
        }
        public DbParam Add<T>(string name, T value)
        {
            var p = DbParam.Create(name, value);
            this.Add(p);
            return p;
        }
        public DbParam Add(string name, object value, Type type)
        {
            var p = DbParam.Create(name, value, type);
            this.Add(p);
            return p;
        }
    }
}
