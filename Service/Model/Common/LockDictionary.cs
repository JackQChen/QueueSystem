using System;
using System.Collections.Concurrent;

namespace Model
{
    public class LockDictionary
    {
        ConcurrentDictionary<string, object> dict = new ConcurrentDictionary<string, object>();

        public object GetLockObject(string key)
        {
            object value = new object();
            return dict.AddOrUpdate(key, value, (tKey, existingVal) => { return existingVal; });
        }
    }

}
