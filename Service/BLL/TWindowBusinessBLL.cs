using DAL;
using Model;

namespace BLL
{
    public class TWindowBusinessBLL : BLLBase<TWindowBusinessDAL, TWindowBusinessModel>
    {
        public TWindowBusinessBLL()
            : base()
        {
        }

        public TWindowBusinessBLL(string connName)
            : base(connName)
        {
        }

        public TWindowBusinessBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return new TWindowDAL().GetGridData();
        }

        public object GetGridBusiData(int winId)
        {
            return this.CreateDAL().GetGridBusiData(winId);
        }

        public object GetGridUserData(int winId)
        {
            return this.CreateDAL().GetGridUserData(winId);
        }
    }
}
