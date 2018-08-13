using System.Collections;
using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TUnitBLL : BLLBase<TUnitDAL, TUnitModel>
    {
        public TUnitBLL()
            : base()
        {
        }

        public TUnitBLL(string connName)
            : base(connName)
        {
        }

        public TUnitBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.dal.GetGridData();
        }

        public ArrayList UploadUnitAndBusy(List<TUnitModel> uList, List<TBusinessModel> bList)
        {
            return this.dal.UploadUnitAndBusy(uList, bList);
        }
    }
}
