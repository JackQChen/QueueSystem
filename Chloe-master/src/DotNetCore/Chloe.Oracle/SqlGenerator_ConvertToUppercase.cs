using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.Oracle
{
    class SqlGenerator_ConvertToUppercase : SqlGenerator
    {
        protected override void QuoteName(string name)
        {
            base.QuoteName(name.ToUpper());
        }
    }
}
