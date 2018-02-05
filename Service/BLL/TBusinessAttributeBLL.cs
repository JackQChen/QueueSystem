using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TBusinessAttributeBLL
    {
        public TBusinessAttributeBLL()
        {
        }

        #region CommonMethods

        public List<TBusinessAttributeModel> GetModelList()
        {
            return new TBusinessAttributeDAL().GetModelList();
        }

        public TBusinessAttributeModel GetModel(int id)
        {
            return new TBusinessAttributeDAL().GetModel(id);
        }

        public TBusinessAttributeModel Insert(TBusinessAttributeModel model)
        {
            return new TBusinessAttributeDAL().Insert(model);
        }

        public int Update(TBusinessAttributeModel model)
        {
            return new TBusinessAttributeDAL().Update(model);
        }

        public int Delete(TBusinessAttributeModel model)
        {
            return new TBusinessAttributeDAL().Delete(model);
        }

        #endregion
        public void ResetIndex()
        {
            new TBusinessAttributeDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TBusinessAttributeDAL().GetGridData();
        }
        public object GetGridDataByUnitSeq(string unitSeq)
        {
            return new TBusinessAttributeDAL().GetGridDataByUnitSeq(unitSeq);
        }

        public object GetGridDetailData(string unitSeq, string busiSeq)
        {
            return new TBusinessAttributeDAL().GetGridDetailData(unitSeq, busiSeq);
        }
    }
}
