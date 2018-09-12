using DAL;
using Model;

namespace BLL
{
    public class TBusinessBLL : BLLBase<TBusinessDAL, TBusinessModel>
    {
        public TBusinessBLL()
            : base()
        {
        }

        public TBusinessBLL(string connName)
            : base(connName)
        {
        }

        public TBusinessBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.CreateDAL().GetGridData();
        }


        public object GetUnitList()
        {
            return this.CreateDAL().GetUnitList();
        }

        public object GetGridDataByUnitSeq(string unitSeq)
        {
            return this.CreateDAL().GetGridDataByUnitSeq(unitSeq);
        }
    }
}
