using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TBusinessAttributeDAL : DALBase<TBusinessAttributeModel>
    {
        public TBusinessAttributeDAL()
            : base()
        {
        }

        public TBusinessAttributeDAL(string connName)
            : base(connName)
        {
        }

        public TBusinessAttributeDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TBusinessAttributeDAL(DbContext db)
            : base(db)
        {
        }

        public TBusinessAttributeDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridData()
        {
            var unitQuery = new TUnitDAL(this.db, this.areaNo).GetQuery();
            return this.GetQuery()
                .GroupBy(k => k.unitSeq)
                .Select(s => new { s.unitSeq })
                .LeftJoin(unitQuery, (b, u) => b.unitSeq == u.unitSeq)
                .Select((b, u) => u)
            .OrderBy(k => k.unitSeq)
            .ToList();
        }

        public object GetGridDataByUnitSeq(string unitSeq)
        {
            var busiQuery = new TBusinessDAL(this.db, this.areaNo).GetQuery();
            var dicType = new FDictionaryDAL(this.db, this.areaNo).GetModelQueryByName(FDictionaryString.AppointmentType);
            return busiQuery.LeftJoin(dicType, (m, t) => m.busiType.ToString() == t.Value)
                .Where((m, t) => m.unitSeq == unitSeq)
                .Select((m, t) => new
                {
                    m.unitSeq,
                    m.busiSeq,
                    m.busiCode,
                    m.busiName,
                    busiType = t.Name,
                    acceptBusi = m.acceptBusi ? "是" : "否",
                    getBusi = m.getBusi ? "是" : "否",
                    askBusi = m.askBusi ? "是" : "否"
                })
                .OrderBy(k => k.unitSeq)
                .ToList();
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            var busiAttrQuery = new TBusinessAttributeDAL(this.db, this.areaNo).GetQuery();
            var unitQuery = new TUnitDAL(this.db, this.areaNo).GetQuery();
            var busiQuery = new TBusinessDAL(this.db, this.areaNo).GetQuery();
            return busiAttrQuery
                .LeftJoin(unitQuery, (m, u) => m.unitSeq == u.unitSeq)
                .LeftJoin(busiQuery, (m, u, b) => m.busiSeq == b.busiSeq && m.unitSeq == b.unitSeq)
                .Where((m, u, b) => m.unitSeq == unitSeq && m.busiSeq == busiSeq)
                .Select((m, u, b) => new
                {
                    m.ID,
                    u.unitSeq,
                    u.unitName,
                    b.busiSeq,
                    b.busiCode,
                    b.busiName,
                    m.timeInterval,
                    m.ticketRestriction,
                    m.lineUpMax,
                    m.lineUpWarningMax,
                    m.ticketPrefix,
                    isGreenChannel = m.isGreenChannel == 1 ? "是" : "否",
                    m.remark,
                    Model = m
                })
                .OrderBy(k => k.ID)
                .ToList();
        }
    }
}
