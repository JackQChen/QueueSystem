using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TWindowUserDAL
    {
        DbContext db;
        public TWindowUserDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TWindowUserDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TWindowUserModel> GetModelList()
        {
            return db.Query<TWindowUserModel>().ToList();
        }

        public List<TWindowUserModel> GetModelList(Expression<Func<TWindowUserModel, bool>> predicate)
        {
            return db.Query<TWindowUserModel>().Where(predicate).ToList();
        }

        public TWindowUserModel GetModel(int id)
        {
            return db.Query<TWindowUserModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TWindowUserModel GetModel(Expression<Func<TWindowUserModel, bool>> predicate)
        {
            return db.Query<TWindowUserModel>().Where(predicate).FirstOrDefault();
        }

        public TWindowUserModel Insert(TWindowUserModel model)
        {
            return db.Insert(model);
        }

        public int Update(TWindowUserModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TWindowUserModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public TWindowUserModel GetModel(int wid, int uid)
        {
            return db.Query<TWindowUserModel>().Where(p => p.WindowID == wid && p.UserID == uid).FirstOrDefault();
        }

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_windowuser AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            return this.db.Query<TWindowUserModel>()
                .LeftJoin<TWindowModel>((m, w) => m.WindowID == w.ID)
                .LeftJoin<TUserModel>((m, w, u) => m.UserID == u.ID)
                .Select((m, w, u) => new
                {
                    m.ID,
                    WindowID = m.WindowID,
                    WindowName = w.Name,
                    UserID = m.UserID,
                    UserName = u.Name,
                    m.CreateDateTime,
                    Model = m
                })
                .OrderBy(k => k.ID)
                .ToList();
        }

        public object GetGridDetailData(int winId)
        {
            return this.db.Query<TWindowUserModel>()
                .Where(p => p.WindowID == winId)
                .LeftJoin<TWindowModel>((m, w) => m.WindowID == w.ID)
                .LeftJoin<TUserModel>((m, w, u) => m.UserID == u.ID)
                .Select((m, w, u) => new
                {
                    m.ID,
                    WindowID = m.WindowID,
                    UserID = m.UserID,
                    UserName = u.Name,
                    m.CreateDateTime,
                    Model = m
                })
                .OrderBy(k => k.ID)
                .ToList();
        }

        //RateService相关
        public object RS_GetDataList()
        {
            return this.db.Query<TWindowUserModel>()
                .LeftJoin<TWindowModel>((m, w) => m.WindowID == w.ID)
                .LeftJoin<TUserModel>((m, w, u) => m.UserID == u.ID)
                .Select((m, w, u) => new
                {
                    WindowNumber = w.Number,
                    WindowName = w.Name,
                    UserCode = u.Code,
                    UserName = u.Name
                })
                .OrderBy(k => k.WindowNumber)
                .ToList();
        }

        public string RS_GetUserPhoto(string userCode)
        {
            var user = this.db.Query<TUserModel>()
                .Where(p => p.Code == userCode)
                .FirstOrDefault();
            if (user != null)
                if (user.Photo != null)
                    return Convert.ToBase64String(user.Photo);
            return "";
        }

        public TWindowUserModel RS_GetModel(string winNum, string userCode)
        {
            return this.db.Query<TWindowUserModel>()
                 .LeftJoin<TWindowModel>((m, w) => m.WindowID == w.ID)
                 .LeftJoin<TUserModel>((m, w, u) => m.UserID == u.ID)
                 .Where((m, w, u) => w.Number == winNum && u.Code == userCode)
                 .Select((m, w, u) => m)
                 .FirstOrDefault();
        }
    }
}
