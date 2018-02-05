/*
 * Copy from Dapper: https://github.com/StackExchange/Dapper
 * License: http://www.apache.org/licenses/LICENSE-2.0
 * Home page: https://github.com/StackExchange/dapper-dot-net
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Query.Internals
{
    sealed partial class DapperTable
    {
        string[] fieldNames;
        readonly Dictionary<string, int> fieldNameLookup;

        internal string[] FieldNames { get { return fieldNames; } }

        public DapperTable(string[] fieldNames)
        {
            if (fieldNames == null) throw new ArgumentNullException("fieldNames");
            this.fieldNames = fieldNames;

            fieldNameLookup = new Dictionary<string, int>(fieldNames.Length, StringComparer.Ordinal);
            // if there are dups, we want the **first** key to be the "winner" - so iterate backwards
            for (int i = fieldNames.Length - 1; i >= 0; i--)
            {
                string key = fieldNames[i];
                if (key != null) fieldNameLookup[key] = i;
            }
        }

        internal int IndexOfName(string name)
        {
            int result;
            return (name != null && fieldNameLookup.TryGetValue(name, out result)) ? result : -1;
        }
        internal int AddField(string name)
        {
            if (name == null) throw new ArgumentNullException("name");
            if (fieldNameLookup.ContainsKey(name)) throw new InvalidOperationException("Field already exists: " + name);
            int oldLen = fieldNames.Length;
            Array.Resize(ref fieldNames, oldLen + 1); // yes, this is sub-optimal, but this is not the expected common case
            fieldNames[oldLen] = name;
            fieldNameLookup[name] = oldLen;
            return oldLen;
        }


        internal bool FieldExists(string key)
        {
            return key != null && fieldNameLookup.ContainsKey(key);
        }

        public int FieldCount { get { return fieldNames.Length; } }
    }
}
