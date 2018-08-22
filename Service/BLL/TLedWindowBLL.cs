using DAL;
using Model;

namespace BLL
{
    public class TLedWindowBLL : BLLBase<TLedWindowDAL, TLedWindowModel>
    {
        public TLedWindowBLL()
            : base()
        {
        }

        public TLedWindowBLL(string connName)
            : base(connName)
        {
        }

        public TLedWindowBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return new TLedControllerDAL().GetGridData();
        }

        public object GetGridDataByControllerId(int controllerId)
        {
            return this.CreateDAL().GetGridDataByControllerId(controllerId);
        }
    }
}
