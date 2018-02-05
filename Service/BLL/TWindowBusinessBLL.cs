using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class TWindowBusinessBLL
    {
        public TWindowBusinessBLL()
        {
        }

        #region CommonMethods

        public List<TWindowBusinessModel> GetModelList()
        {
            return new TWindowBusinessDAL().GetModelList();
        }

        public TWindowBusinessModel GetModel(int id)
        {
            return new TWindowBusinessDAL().GetModel(id);
        }

        public TWindowBusinessModel Insert(TWindowBusinessModel model)
        {
            return new TWindowBusinessDAL().Insert(model);
        }

        public int Update(TWindowBusinessModel model)
        {
            return new TWindowBusinessDAL().Update(model);
        }

        public int Delete(TWindowBusinessModel model)
        {
            return new TWindowBusinessDAL().Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            new TWindowBusinessDAL().ResetIndex();
        }

        public object GetGridData()
        {
            return new TWindowDAL().GetGridData();
        }

        public object GetGridDetailData(int winId)
        {
            return new TWindowBusinessDAL().GetGridDetailData(winId);
        }
    }
}
