using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Model;

namespace BLL
{
    public class BLLBase<TDAL, TModel> : AbstractBLLBase<TModel>
        where TDAL : AbstractDALBase<TModel>
        where TModel : ModelBase
    {
        protected TDAL dal;

        public BLLBase()
        {
            dal = Activator.CreateInstance<TDAL>();
        }

        public BLLBase(string connName)
        {
            dal = Activator.CreateInstance(typeof(TDAL), connName) as TDAL;
        }

        public BLLBase(string connName, string areaNo)
        {
            dal = Activator.CreateInstance(typeof(TDAL), connName, areaNo) as TDAL;
        }

        public override List<TModel> GetModelList()
        {
            return dal.GetModelList();
        }

        public override List<TModel> GetModelList(Expression<Func<TModel, bool>> predicate)
        {
            return dal.GetModelList(predicate);
        }

        public override List<TModel> GetModelList(string expression)
        {
            return dal.GetModelList(ExpressionConverter.Deserialize<Func<TModel, bool>>(expression));
        }

        public override TModel GetModel(int id)
        {
            return dal.GetModel(id);
        }

        public override TModel GetModel(Expression<Func<TModel, bool>> predicate)
        {
            return dal.GetModel(predicate);
        }

        public override TModel GetModel(string expression)
        {
            return dal.GetModel(ExpressionConverter.Deserialize<Func<TModel, bool>>(expression));
        }

        public override TModel Insert(TModel model)
        {
            return dal.Insert(model);
        }

        public override int Update(TModel model)
        {
            return dal.Update(model);
        }

        public override int Delete(TModel model)
        {
            return dal.Delete(model);
        }

        public override int GetMaxId()
        {
            return dal.GetMaxId();
        }
    }
}
