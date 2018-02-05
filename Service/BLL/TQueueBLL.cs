using System.Collections.Generic;
using DAL;
using Model;
using System;
using System.Linq;
using System.Collections;

namespace BLL
{
    public class TQueueBLL
    {
        public TQueueBLL()
        {
        }

        #region CommonMethods

        public List<TQueueModel> GetModelList()
        {
            return new TQueueDAL().GetModelList();
        }

        public TQueueModel GetModel(int id)
        {
            return new TQueueDAL().GetModel(id);
        }

        public TQueueModel Insert(TQueueModel model)
        {
            return new TQueueDAL().Insert(model);
        }

        public int Update(TQueueModel model)
        {
            return new TQueueDAL().Update(model);
        }

        public int Delete(TQueueModel model)
        {
            return new TQueueDAL().Delete(model);
        }

        #endregion

        public TQueueModel QueueLine(TBusinessModel selectBusy, TUnitModel selectUnit, string ticketStart, string idCard, string name, string reserveSeq)
        {
            return new TQueueDAL().QueueLine(selectBusy, selectUnit, ticketStart, idCard, name, reserveSeq);
        }
        public TQueueModel QueueLine(TBusinessModel selectBusy, TUnitModel selectUnit, string ticketStart, string idCard, string name, TAppointmentModel app)
        {
            return new TQueueDAL().QueueLine(selectBusy, selectUnit, ticketStart, idCard, name, app);
        }
        public List<TQueueModel> GetModelList(List<TWindowBusinessModel> wlBusy, int state)
        {
            return new TQueueDAL().GetModelList(wlBusy, state);
        }
        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq)
        {
            return new TQueueDAL().GetModelList(busiSeq, unitSeq);
        }
        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq, int state)
        {
            return new TQueueDAL().GetModelList(busiSeq, unitSeq, state);
        }

        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end)
        {
            return new TQueueDAL().GetModelList(busiSeq, unitSeq, start, end);
        }

        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end, int state)
        {
            return new TQueueDAL().GetModelList(busiSeq, unitSeq, start, end, state);
        }

        public TQueueModel CallNo(string unitSeq, string busiSeq)
        {
            return new TQueueDAL().CallNo(unitSeq, busiSeq);
        }

        public bool IsCanQueue(string idCard, string busiSeq, string unitSeq)
        {
            return new TQueueDAL().IsCanQueue(idCard, busiSeq,unitSeq);
        }

        public ArrayList IsCanQueueO(string idCard, string busiSeq, string unitSeq)
        {
            return new TQueueDAL().IsCanQueueO(idCard, busiSeq, unitSeq);
        }
    }
}
