using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TDictionaryBLL
    {
        public TDictionaryBLL()
        {
        }

        #region CommonMethods

        public List<TDictionaryModel> GetModelList()
        {
            return new TDictionaryDAL().GetModelList();
        }

        public TDictionaryModel GetModel(int id)
        {
            return new TDictionaryDAL().GetModel(id);
        }

        public TDictionaryModel Insert(TDictionaryModel model)
        {
            return new TDictionaryDAL().Insert(model);
        }

        public int Update(TDictionaryModel model)
        {
            return new TDictionaryDAL().Update(model);
        }

        public int Delete(TDictionaryModel model)
        {
            return new TDictionaryDAL().Delete(model);
        }

        #endregion

        public List<TDictionaryModel> GetModelList(string name)
        {
            return new TDictionaryDAL().GetModelList(name);
        }
    }
}
