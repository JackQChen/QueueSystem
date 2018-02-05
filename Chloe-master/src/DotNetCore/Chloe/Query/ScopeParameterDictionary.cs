using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Chloe.Query
{
    class ScopeParameterDictionary : Dictionary<ParameterExpression, IMappingObjectExpression>
    {
        public ScopeParameterDictionary()
        {
        }
        public ScopeParameterDictionary(int capacity) : base(capacity)
        {
        }
        public IMappingObjectExpression Get(ParameterExpression parameter)
        {
            IMappingObjectExpression moe;
            if (!this.TryGetValue(parameter, out moe))
            {
                throw new Exception("Can not find the ParameterExpression");
            }

            return moe;
        }

        public ScopeParameterDictionary Clone()
        {
            return this.Clone(this.Count);
        }
        public ScopeParameterDictionary Clone(int capacity)
        {
            ScopeParameterDictionary ret = new ScopeParameterDictionary(capacity);
            foreach (var kv in this)
            {
                ret.Add(kv.Key, kv.Value);
            }

            return ret;
        }
        public ScopeParameterDictionary Clone(ParameterExpression key, IMappingObjectExpression valueOfkey)
        {
            ScopeParameterDictionary ret = this.Clone(this.Count + 1);
            ret[key] = valueOfkey;
            return ret;
        }
    }
}
