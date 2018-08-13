using DAL;
using Model;

namespace BLL
{
    public class TLedControllerBLL : BLLBase<TLedControllerDAL, TLedControllerModel> 
    {
        public TLedControllerBLL()
            : base()
        {
        }

        public TLedControllerBLL(string connName)
            : base(connName)
        {
        }

        public TLedControllerBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }
    }
}
