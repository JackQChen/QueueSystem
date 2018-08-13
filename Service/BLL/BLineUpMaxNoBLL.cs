using DAL;
using Model;

namespace BLL
{
    public class BLineUpMaxNoBLL : BLLBase<BLineUpMaxNoDAL, BLineUpMaxNoModel>
    {
        public BLineUpMaxNoBLL()
            : base()
        {
        }

        public BLineUpMaxNoBLL(string connName)
            : base(connName)
        {
        }

        public BLineUpMaxNoBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }
    }
}
