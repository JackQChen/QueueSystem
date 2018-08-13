using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Model
{
    public abstract class AbstractBLLBase<TModel> : MarshalByRefObject where TModel : ModelBase
    {

        public abstract List<TModel> GetModelList();

        public abstract List<TModel> GetModelList(Expression<Func<TModel, bool>> predicate);

        public abstract List<TModel> GetModelList(string expression);

        public abstract TModel GetModel(int id);

        public abstract TModel GetModel(Expression<Func<TModel, bool>> predicate);

        public abstract TModel GetModel(string expression);

        public abstract TModel Insert(TModel model);

        public abstract int Update(TModel model);

        public abstract int Delete(TModel model);

        public abstract int GetMaxId();

    }
}
