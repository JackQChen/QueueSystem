using System;
using System.Collections;
using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{
    public class BQueueBLL : BLLBase<BQueueDAL, BQueueModel>
    {
        public BQueueBLL()
            : base()
        {
        }

        public BQueueBLL(string connName)
            : base(connName)
        {
        }

        public BQueueBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public BQueueModel QueueLine(TBusinessModel selectBusy, TUnitModel selectUnit, string ticketStart, string idCard, string name)
        {
            return new BQueueDAL().QueueLine(selectBusy, selectUnit, ticketStart, idCard, name);
        }
        public BQueueModel QueueLine(string unitSeq, string unitName, string busiSeq, string busiName, string ticketStart, string idCard, string name, string wxId)
        {
            return new BQueueDAL().QueueLine(unitSeq, unitName, busiSeq, busiName, ticketStart, idCard, name, wxId);
        }
        public List<BQueueModel> GetModelList(List<TWindowBusinessModel> wlBusy, int state)
        {
            return new BQueueDAL().GetModelList(wlBusy, state);
        }
        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq)
        {
            return new BQueueDAL().GetModelList(busiSeq, unitSeq);
        }
        public List<BQueueModel> GetModelList(string unitSeq, int state)
        {
            return new BQueueDAL().GetModelList(unitSeq, state);
        }
        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq, int state)
        {
            return new BQueueDAL().GetModelList(busiSeq, unitSeq, state);
        }

        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end)
        {
            return new BQueueDAL().GetModelList(busiSeq, unitSeq, start, end);
        }

        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end, int state)
        {
            return new BQueueDAL().GetModelList(busiSeq, unitSeq, start, end, state);
        }

        public bool IsCanQueue(string idCard, string busiSeq, string unitSeq)
        {
            return new BQueueDAL().IsCanQueue(idCard, busiSeq, unitSeq);
        }

        public ArrayList IsCanQueueO(string idCard, string busiSeq, string unitSeq)
        {
            return new BQueueDAL().IsCanQueueO(idCard, busiSeq, unitSeq);
        }
    }
}
