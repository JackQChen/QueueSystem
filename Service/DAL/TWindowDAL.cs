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

        /// <summary>
        /// 根据窗口区域获取窗口号列表
        /// </summary>
        /// <param name="aList"></param>
        /// <returns></returns>
        public List<string> GetWindowByArea(string aList)
        {
            string[] areaList = aList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var window = db.Query<TWindowModel>().Where(a => a.AreaNo == this.areaNo).Where(q => areaList.Contains(q.AreaName.ToString())).Select(s => s.Number).ToList();
            return window;
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
            var unitQuery = new TUnitDAL(this.db, this.areaNo).GetQuery();
            return winBusiQuery
                .InnerJoin(winQuery, (m, w) => m.WindowID == w.ID)
                .Where((m, w) => w.Number == winNum)
                .Select((m, w) => m)
                .GroupBy(k => k.unitSeq)
                .Select(s => s.unitSeq)
                .InnerJoin(userQuery, (s, user) => s == user.unitSeq)
                .InnerJoin(unitQuery, (s, user, unit) => s == unit.unitSeq)
                .Select((s, user, unit) => new
                {
                    UserCode = user.Code,
                    UserName = user.Name,
                    UnitSeq = unit.unitSeq,
                    UnitName = unit.unitName,
                    State = user.State
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

        public object RS_GetItemListByWindowNo(string winNo)
        {
            var win = this.GetQuery().Where(q => q.Number == winNo).FirstOrDefault();
            if (win == null)
                return null;
            var winBusi = new TWindowBusinessDAL(this.db, this.areaNo).GetQuery().Where(q => q.WindowID == win.ID).OrderByDesc(o => o.ID).FirstOrDefault();
            if (winBusi == null)
                return null;
            var busiItem = new TBusinessItemDAL(this.db, this.areaNo).GetQuery().Where(q => q.unitSeq == winBusi.unitSeq && q.busiSeq == winBusi.busiSeq)
                .Select(s => new
                {
                    winNum = win.Number,
                    unitSeq = s.unitSeq,
                    busiSeq = s.busiSeq,
                    item = s.itemName,
                    remark = s.remark,
                }).ToList();
            return busiItem;

        }
    }
}
