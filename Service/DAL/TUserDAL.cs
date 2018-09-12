using Chloe;
using Model;

namespace DAL
{
    public class TUserDAL : DALBase<TUserModel>
    {
        public TUserDAL()
            : base()
        {
        }

        public TUserDAL(string connName)
            : base(connName)
        {
        }

        public TUserDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TUserDAL(DbContext db)
            : base(db)
        {
        }

        public TUserDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridData()
        {
            var dicState = new FDictionaryDAL(this.db, this.areaNo).GetModelQueryByName(FDictionaryString.WorkState);
            var dicSex = new FDictionaryDAL(this.db, this.areaNo).GetModelQueryByName(FDictionaryString.UserSex);
            var unitQuery = new TUnitDAL(this.db, this.areaNo).GetQuery();
            return this.GetQuery()
                     .LeftJoin(dicState, (u, d) => u.State == d.Value)
                     .LeftJoin(dicSex, (u, d, s) => u.Sex == s.Value)
                     .LeftJoin(unitQuery, (u, d, s, u2) => u.unitSeq == u2.unitSeq)
                     .Select((u, d, s, u2) => new
                     {
                         u.ID,
                         u.Code,
                         u.Name,
                         u2.unitName,
                         Sex = s.Name,
                         State = d.Name,
                         u.Remark
                     })
                     .OrderBy(k => k.ID).ToList();
        }
    }
}
