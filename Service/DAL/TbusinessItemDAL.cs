using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TBusinessItemDAL : DALBase<TBusinessItemModel>
    {
        public TBusinessItemDAL()
            : base()
        {
        }

        public TBusinessItemDAL(string connName)
            : base(connName)
        {
        }

        public TBusinessItemDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TBusinessItemDAL(DbContext db)
            : base(db)
        {
        }

        public TBusinessItemDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            var busiAttrQuery = this.GetQuery();
            var unitQuery = new TUnitDAL(this.db, this.areaNo).GetQuery();
            var busiQuery = new TBusinessDAL(this.db, this.areaNo).GetQuery();
            return busiAttrQuery
                .LeftJoin(unitQuery, (m, u) => m.unitSeq == u.unitSeq)
                .LeftJoin(busiQuery, (m, u, b) => m.busiSeq == b.busiSeq && m.unitSeq == b.unitSeq)
                .Where((m, u, b) => m.unitSeq == unitSeq && m.busiSeq == busiSeq)
                .Select((m, u, b) => new
                {
                    m.ID,
                    m.itemName,
                    m.remark
                })
                .OrderBy(k => k.ID)
                .ToList();
        }
    }
}
