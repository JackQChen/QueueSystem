using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TLedControllerDAL : DALBase<TLedControllerModel>
    {
        public TLedControllerDAL()
            : base()
        {
        }

        public TLedControllerDAL(string connName)
            : base(connName)
        {
        }

        public TLedControllerDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.GetQuery()
                .Select(s => new
                {
                    s.ID,
                    s.IP,
                    s.Port,
                    s.DeviceAddress,
                    s.Name,
                    Model = s
                })
                .OrderBy(k => k.ID)
                .ToList();
        }
    }
}
