using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TBusinessItemBLL : BLLBase<TBusinessItemDAL, TBusinessItemModel>
    {

        public TBusinessItemBLL()
            : base()
        {
        }

        public TBusinessItemBLL(string connName)
            : base(connName)
        {
        }

        public TBusinessItemBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            return this.CreateDAL().GetGridDetailData(unitSeq, busiSeq);
        }
    }
}
