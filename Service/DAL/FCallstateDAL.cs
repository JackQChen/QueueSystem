using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class FCallStateDAL : DALBase<FCallStateModel>
    {
        public FCallStateDAL()
            : base()
        {
        }

        public FCallStateDAL(DbContext db)
            : base(db)
        {
        }

        public FCallStateDAL(string connName)
            : base(connName)
        {
        }

        public FCallStateDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }
    }
}
