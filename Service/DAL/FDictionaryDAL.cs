using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class FDictionaryDAL : DALBase<FDictionaryModel>
    {
        public FDictionaryDAL()
            : base()
        {
        }

        public FDictionaryDAL(string connName)
            : base(connName)
        {
        }

        public FDictionaryDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public FDictionaryDAL(DbContext db)
            : base(db)
        {
        }

        public FDictionaryDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public IQuery<FDictionaryModel> GetModelQueryByName(string name)
        {
            return this.GetQuery().Where(p => p.Name == name && p.Group == 0)
                .LeftJoin<FDictionaryModel>((g, i) => g.ID == i.Group && i.AreaNo == this.areaNo)
                .Select((c, i) => i);
        }

        public List<FDictionaryModel> GetModelListByName(string name)
        {
            return this.GetModelQueryByName(name).ToList();
        }
    }
}
