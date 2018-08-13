using DAL;
using Model;

namespace BLL
{
    public class BEvaluateBLL : BLLBase<BEvaluateDAL, BEvaluateModel>
    {
        public BEvaluateBLL()
            : base()
        {
        }

        public BEvaluateBLL(string connName)
            : base(connName)
        {
        }

        public BEvaluateBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }
    }
}
