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
        bool ProcessInsertData(int areaCode, string targetDbName);
        bool ProcessUpdateData(int areaCode, string targetDbName);
        bool ProcessDeleteData(int areaCode, string targetDbName);
        bool IsBasic { get; }
    }
}
