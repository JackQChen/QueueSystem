using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.Oracle
{
    static class UtilExceptions
    {
        public static NotSupportedException NotSupportedMethod(MethodInfo method)
        {
            return new NotSupportedException(string.Format("Does not support method '{0}'.", Utils.ToMethodString(method)));
        }
    }
}
