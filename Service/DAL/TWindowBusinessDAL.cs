using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowBusinessDAL : DALBase<TWindowBusinessModel>
    {
        public TWindowBusinessDAL()
            : base()
        {
        }

        public TWindowBusinessDAL(string connName)
            : base(connName)
        {
        }

        public TWindowBusinessDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TWindowBusinessDAL(DbContext db)
            : base(db)
        {
        }

        public TWindowBusinessDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridBusiData(int winId)
        {
            var unitQuery = new TUnitDAL(this.db, this.areaNo).GetQuery();
            var busiQuery = new TBusinessDAL(this.db, this.areaNo).GetQuery();
            return this.GetQuery()
                .Where(m => m.WindowID == winId)
                .InnerJoin(unitQuery, (m, u) => m.unitSeq == u.unitSeq)
                .InnerJoin(busiQuery, (m, u, b) => m.busiSeq == b.busiSeq && m.unitSeq == b.unitSeq)
                .Select((m, u, b) => new
                {
                    m.ID,
                    m.WindowID,
                    m.unitSeq,
                    u.unitName,
                    m.busiSeq,
                    b.busiName
                })
                .OrderBy(k => k.unitSeq)
                .ToList();
        }

        public object GetGridUserData(int winId)
        {
            var userQuery = new TUserDAL(this.db, this.areaNo).GetQuery();
            return this.GetQuery()
                .Where(m => m.WindowID == winId)
                .GroupBy(k => k.unitSeq)
                .Select(s => new { s.ID, s.unitSeq })
                .InnerJoin(userQuery, (m, u) => m.unitSeq == u.unitSeq)
                .Select((m, u) => new
                {
                    ID = m.ID,
                    UserID = u.ID,
                    UserName = u.Name
                })
                .OrderBy(k => k.UserID)
                .ToList();
        }
    }
}
