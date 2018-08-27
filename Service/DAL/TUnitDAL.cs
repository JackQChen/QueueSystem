using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TUnitDAL : DALBase<TUnitModel>
    {
        public TUnitDAL()
            : base()
        {
        }

        public TUnitDAL(string connName)
            : base(connName)
        {
        }

        public TUnitDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TUnitDAL(DbContext db)
            : base(db)
        {
        }

        public TUnitDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.GetQuery().Select(s => new
            {
                s.ID,
                s.unitSeq,
                s.unitName,
                s.orderNum,
                Model = s
            })
            .OrderBy(k => k.ID)
            .ToList();
        }

        public TUnitModel GetModel(int areaCode, int areaId)
        {
            return db.Query<TUnitModel>().Where(p => 1 == 1).FirstOrDefault();
        }
    }
}
