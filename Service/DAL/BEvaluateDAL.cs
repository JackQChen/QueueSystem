using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class BEvaluateDAL : DALBase<BEvaluateModel>
    {
        public BEvaluateDAL()
            : base()
        {
        }

        public BEvaluateDAL(DbContext db)
            : base(db)
        {
        }

        public BEvaluateDAL(string connName)
            : base(connName)
        {
        }

        public BEvaluateDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }
    }
}
