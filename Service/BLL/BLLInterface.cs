using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    public interface IGridData
    {
        void ResetIndex();
        object GetGridData();
    }

    public interface IUploadData
    {
        int ProcessInsertData(int areaCode, string targetDbName);
        int ProcessUpdateData(int areaCode, string targetDbName);
        int ProcessDeleteData(int areaCode, string targetDbName);
        bool IsBasic { get; }
    }
}
