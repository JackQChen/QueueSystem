using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TEvaluateBLL
    {
        public TEvaluateBLL()
        {
        }

        #region CommonMethods

        public List<TEvaluateModel> GetModelList()
        {
            return new TEvaluateDAL().GetModelList();
        }

        public TEvaluateModel GetModel(int id)
        {
            return new TEvaluateDAL().GetModel(id);
        }

        public TEvaluateModel Insert(TEvaluateModel model)
        {
            return new TEvaluateDAL().Insert(model);
        }

        public int Update(TEvaluateModel model)
        {
            return new TEvaluateDAL().Update(model);
        }

        public int Delete(TEvaluateModel model)
        {
            return new TEvaluateDAL().Delete(model);
        }

        #endregion
    }
}
