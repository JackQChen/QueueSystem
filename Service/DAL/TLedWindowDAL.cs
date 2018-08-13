using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TLedWindowDAL : DALBase<TLedWindowModel>
    {
        public TLedWindowDAL()
            : base()
        {
        }

        public TLedWindowDAL(string connName)
            : base(connName)
        {
        }

        public TLedWindowDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridDataByControllerId(int controllerId)
        {
            var winQuery = new TWindowDAL(this.db, this.areaNo).GetQuery();
            return this.GetQuery().Where(m => m.ControllerID == controllerId)
                .LeftJoin(winQuery, (m, w) => m.WindowNumber == w.Number)
                .Select((m, w) => new
                {
                    m.ID,
                    WindowNumber = w.Number,
                    WindowName = w.Name,
                    m.DisplayText,
                    m.Position,
                    Model = m
                })
                .OrderBy(k => k.WindowNumber)
                .ToList();
        }

    }
}
