using DAL;
using Model;

namespace BLL
{
    public class FCallStateBLL : BLLBase<FCallStateDAL, FCallStateModel>
    {
        public FCallStateBLL()
            : base()
        {
        }

        public FCallStateBLL(string connName)
            : base(connName)
        {
        }

        public FCallStateBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }
    }
}
