using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chloe.Exceptions
{
    //[Serializable]
    public class ChloeException : Exception
    {
        public ChloeException()
            : this("An exception occurred in the persistence layer.")
        {
        }

        public ChloeException(string message)
            : base(message)
        {
        }

        public ChloeException(Exception innerException)
            : base(innerException.Message, innerException)
        {
        }

        public ChloeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
