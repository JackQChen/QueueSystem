using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowDAL : DALBase<TWindowModel>
    {
        public TWindowDAL()
            : base()
        {
        }

        public TWindowDAL(string connName)
            : base(connName)
        {
        }

        public TWindowDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TWindowDAL(DbContext db)
            : base(db)
        {
        }

        public TWindowDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridData()
        {
            var dic = new FDictionaryDAL(this.db, this.areaNo).GetModelQueryByName(FDictionaryString.WorkState);
            var winAreaQuery = new TWindowAreaDAL(this.db, this.areaNo).GetQuery();
            return this.GetQuery()
                .LeftJoin(dic, (w, d) => w.State == d.Value)
                .LeftJoin(winAreaQuery, (w, d, a) => w.AreaName == a.ID)
                .Select((w, d, a) => new
                {
                    w.ID,
                    w.Name,
                    w.Number,
                    w.Type,
                    State = d.Name,
                    CallNumber = w.CallNumber,
                    AreaName = a.areaName,
                    Model = w
                })
                .OrderBy(k => k.ID)
                .ToList();
        }

        //RateService相关

        public object RS_GetWindowList()
        {
            var winBusiQuery = new TWindowBusinessDAL(this.db, this.areaNo).GetQuery();
            var winQuery = this.GetQuery();
            return winBusiQuery
                .GroupBy(k => k.WindowID)
                .Select(s => s.WindowID)
                .InnerJoin(winQuery, (m, w) => m == w.ID)
                .Select((m, w) => new
                {
                    WindowNumber = w.Number,
                    WindowName = w.Name
                })
                .OrderBy(k => k.WindowNumber)
                .ToList();
        }

        public object RS_GetUserListByWindowNo(string winNum)
        {
            var winBusiQuery = new TWindowBusinessDAL(this.db, this.areaNo).GetQuery();
            var winQuery = this.GetQuery();
            var userQuery = new TUserDAL(this.db, this.areaNo).GetQuery();
            return winBusiQuery
                .InnerJoin(winQuery, (m, w) => m.WindowID == w.ID)
                .Where((m, w) => w.Number == winNum)
                .Select((m, w) => m)
                .GroupBy(k => k.unitSeq)
                .Select(s => s.unitSeq)
                .InnerJoin(userQuery, (s, u) => s == u.unitSeq)
                .Select((s, u) => new
                {
                    UserCode = u.Code,
                    UserName = u.Name
                })
                .OrderBy(k => k.UserCode)
                .ToList();
        }

        public string RS_GetUserPhoto(string userCode)
        {
            var userQuery = new TUserDAL(this.db, this.areaNo).GetQuery();
            var user = userQuery.Where(p => p.Code == userCode).FirstOrDefault();
            if (user != null)
                if (user.Photo != null)
                    return Convert.ToBase64String(user.Photo);
            return "";
        }

        public object RS_GetModel(string winNum, string userCode)
        {
            var winBusiQuery = new TWindowBusinessDAL(this.db, this.areaNo).GetQuery();
            var userQuery = new TUserDAL(this.db, this.areaNo).GetQuery();
            return winBusiQuery
                  .InnerJoin(this.GetQuery(), (m, w) => m.WindowID == w.ID)
                  .InnerJoin(userQuery, (m, w, u) => m.unitSeq == u.unitSeq)
                  .Where((m, w, u) => w.Number == winNum && u.Code == userCode)
                  .Select((m, w, u) => u).FirstOrDefault();
        }
    }
}
