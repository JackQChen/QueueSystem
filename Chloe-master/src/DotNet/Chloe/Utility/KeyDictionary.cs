using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Utility
{
    class KeyDictionary<TKey> : Dictionary<TKey, object>
    {
        public KeyDictionary()
        {
        }
        public KeyDictionary(int capacity) : base(capacity)
        {
        }
        public void Add(TKey key)
        {
            this.Add(key, null);
        }
        public void Set(TKey key)
        {
            this[key] = null;
        }
        public bool Exists(TKey key)
        {
            return this.ContainsKey(key);
        }

        public KeyDictionary<TKey> Clone()
        {
            KeyDictionary<TKey> ret = new KeyDictionary<TKey>(this.Count);
            foreach (var kv in this)
            {
                ret.Add(kv.Key, kv.Value);
            }

            return ret;
        }
    }
}
