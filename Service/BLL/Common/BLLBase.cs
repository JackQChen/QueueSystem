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
        object[] args;

        public TDAL CreateDAL()
        {
            return Activator.CreateInstance(typeof(TDAL), args) as TDAL;
        }

        public BLLBase()
        {
        }

        public BLLBase(string connName)
        {
            args = new object[] { connName };
        }

        public BLLBase(string connName, string areaNo)
        {
            args = new object[] { connName, areaNo };
        }

        public override List<TModel> GetModelList()
        {
            return CreateDAL().GetModelList();
        }

        public override List<TModel> GetModelList(Expression<Func<TModel, bool>> predicate)
        {
            return CreateDAL().GetModelList(predicate);
        }

        public override List<TModel> GetModelList(string expression)
        {
            return CreateDAL().GetModelList(ExpressionConverter.Deserialize<Func<TModel, bool>>(expression));
        }

        public override TModel GetModel(int id)
        {
            return CreateDAL().GetModel(id);
        }

        public override TModel GetModel(Expression<Func<TModel, bool>> predicate)
        {
            return CreateDAL().GetModel(predicate);
        }

        public override TModel GetModel(string expression)
        {
            return CreateDAL().GetModel(ExpressionConverter.Deserialize<Func<TModel, bool>>(expression));
        }

        public override TModel Insert(TModel model)
        {
            return CreateDAL().Insert(model);
        }

        public override int Update(TModel model)
        {
            return CreateDAL().Update(model);
        }

        public override int Delete(TModel model)
        {
            return CreateDAL().Delete(model);
        }

        public override int GetMaxId()
        {
            return CreateDAL().GetMaxId();
        }
    }
}
