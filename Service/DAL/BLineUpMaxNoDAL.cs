using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class BLineUpMaxNoDAL : DALBase<BLineUpMaxNoModel>
    {
        public BLineUpMaxNoDAL()
            : base()
        {
        }

        public BLineUpMaxNoDAL(string connName)
            : base(connName)
        {
        }

        public BLineUpMaxNoDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public BLineUpMaxNoDAL(DbContext db)
            : base(db)
        {
        }

        public BLineUpMaxNoDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }
    }
}
