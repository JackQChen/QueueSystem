using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TScreenConfigDAL : DALBase<TScreenConfigModel>
    {
        public TScreenConfigDAL()
            : base()
        {
        }

        public TScreenConfigDAL(string connName)
            : base(connName)
        {
        }

        public TScreenConfigDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TScreenConfigDAL(DbContext db)
            : base(db)
        {
        }

        public TScreenConfigDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }
    }
}
