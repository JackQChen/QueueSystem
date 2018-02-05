using System;
using System.Collections.Generic;
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

        #region CommonMethods

        public List<TUserModel> GetModelList()
        {
            return db.Query<TUserModel>().ToList();
        }

        public TUserModel GetModel(int ID)
        {
            return db.Query<TUserModel>().Where(p => p.ID == ID).FirstOrDefault();
        }

        public TUserModel Insert(TUserModel model)
        {
            return this.db.Insert(model);
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
            var dicState = new TDictionaryDAL().GetModelQuery(this.db, DictionaryString.WorkState);
            var dicSex = new TDictionaryDAL().GetModelQuery(this.db, DictionaryString.UserSex);
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
