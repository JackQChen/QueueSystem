using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Infrastructure.Interception
{
    public class DbCommandInterceptionContext<TResult>
    {
        Dictionary<string, object> _dataBag = new Dictionary<string, object>();
        public TResult Result { get; set; }
        public Exception Exception { get; set; }
        public Dictionary<string, object> DataBag { get { return this._dataBag; } }
    }
}
