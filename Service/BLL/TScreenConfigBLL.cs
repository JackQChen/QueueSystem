using DAL;
using Model;

namespace BLL
{
    public class TScreenConfigBLL : BLLBase<TScreenConfigDAL, TScreenConfigModel> 
    {
        public TScreenConfigBLL()
            : base()
        {
        }
        
        public TScreenConfigBLL(string connName)
            : base(connName)
        {
        }

        public TScreenConfigBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }
    }
}
