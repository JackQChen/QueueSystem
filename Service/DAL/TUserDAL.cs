using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TUserDAL
    {
        DbContext db;
        public TUserDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TUserDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TUserModel> GetModelList()
        {
            return db.Query<TUserModel>().ToList();
        }

        public List<TUserModel> GetModelList(Expression<Func<TUserModel, bool>> predicate)
        {
            return db.Query<TUserModel>().Where(predicate).ToList();
        }

        public TUserModel GetModel(int id)
        {
            return db.Query<TUserModel>().Where(p => p.ID == id).FirstOrDefault();
        }

        public TUserModel GetModel(Expression<Func<TUserModel, bool>> predicate)
        {
            return db.Query<TUserModel>().Where(predicate).FirstOrDefault();
        }

        public TUserModel Insert(TUserModel model)
        {
            return db.Insert(model);
        }

        public int Update(TUserModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TUserModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_user AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            var dicState = new TDictionaryDAL(this.db).GetModelQuery(DictionaryString.WorkState);
            var dicSex = new TDictionaryDAL(this.db).GetModelQuery(DictionaryString.UserSex);
            return db.Query<TUserModel>()
                .LeftJoin(dicState, (u, d) => u.State == d.Value)
                .LeftJoin(dicSex, (u, d, s) => u.Sex == s.Value)
                .Select((u, d, s) => new
                {
                    u.ID,
                    u.Code,
                    u.Name,
                    Sex = s.Name,
                    State = d.Name,
                    u.Photo,
                    u.Remark,
                    Model = u
                })
                .OrderBy(k => k.ID)
                .ToList();
        }
    }
}
