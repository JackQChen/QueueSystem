using DAL;
using Model;

namespace BLL
{
    public class TWindowAreaBLL : BLLBase<TWindowAreaDAL, TWindowAreaModel> 
    {
        public TWindowAreaBLL()
            : base()
        {
        }
        
        public TWindowAreaBLL(string connName)
            : base(connName)
        {
        }

        public TWindowAreaBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }
    }
}
