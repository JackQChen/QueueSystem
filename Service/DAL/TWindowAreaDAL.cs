using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowAreaDAL : DALBase<TWindowAreaModel>
    {
        public TWindowAreaDAL()
            : base()
        {
        }

        public TWindowAreaDAL(string connName)
            : base(connName)
        {
        }

        public TWindowAreaDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TWindowAreaDAL(DbContext db)
            : base(db)
        {
        }

        public TWindowAreaDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.GetQuery().Select(s => new
            {
                s.ID,
                s.areaName,
                s.remark,
                Model = s
            })
            .OrderBy(k => k.ID)
            .ToList();
        }

    }
}
