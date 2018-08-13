using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Chloe;
using Chloe.Entity;

namespace Model
{
    public abstract class AbstractDALBase<T> where T : ModelBase
    {
        protected string tableName;

        public AbstractDALBase()
        {
            var objArr = typeof(T).GetCustomAttributes(typeof(TableAttribute), false);
            if (objArr.Length > 0)
                tableName = ((TableAttribute)objArr[0]).Name;
        }

        public abstract IQuery<T> GetQuery();

        public abstract List<T> GetModelList();

        public abstract List<T> GetModelList(Expression<Func<T, bool>> predicate);

        public abstract T GetModel(int id);

        public abstract T GetModel(Expression<Func<T, bool>> predicate);

        public abstract T Insert(T model);

        public abstract int Update(T model);

        public abstract int Delete(T model);

        public abstract int GetMaxId();

    }
}
