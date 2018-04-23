using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TCallStateBLL
    {

        private TCallStateDAL dal;

        public TCallStateBLL()
        {
            this.dal = new TCallStateDAL();
        }

        public TCallStateBLL(string dbKey)
        {
            this.dal = new TCallStateDAL(dbKey);
        }

        #region CommonMethods

        public List<TCallStateModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TCallStateModel> GetModelList(Expression<Func<TCallStateModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TCallStateModel GetModel(string id)
        {
            return this.dal.GetModel(id);
        }

        public TCallStateModel GetModel(Expression<Func<TCallStateModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TCallStateModel Insert(TCallStateModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TCallStateModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TCallStateModel model)
        {
            return this.dal.Delete(model);
        }

        #endregion
    }
}
