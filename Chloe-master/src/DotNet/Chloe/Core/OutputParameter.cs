using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Core
{
    class OutputParameter
    {
        DbParam _param;
        IDbDataParameter _parameter;

        public OutputParameter(DbParam param, IDbDataParameter parameter)
        {
            this._param = param;
            this._parameter = parameter;
        }

        public void MapValue()
        {
            object val = this._parameter.Value;
            if (val == DBNull.Value)
                this._param.Value = null;
            else
                this._param.Value = val;
        }

        /* 只有在 DataReader.Close() 后（有些驱动还需要DataReader.Dispose()后）才能取得 output 的值 */
        public static void CallMapValue(List<OutputParameter> outputParameters)
        {
            if (outputParameters != null)
            {
                for (int i = 0; i < outputParameters.Count; i++)
                {
                    outputParameters[i].MapValue();
                }
            }
        }
    }
}
