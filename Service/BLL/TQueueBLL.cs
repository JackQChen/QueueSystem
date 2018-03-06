using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TQueueBLL : IUploadData
    {

        private TQueueDAL dal;

        public TQueueBLL()
        {
            this.dal = new TQueueDAL();
        }

        public TQueueBLL(string dbKey)
        {
            this.dal = new TQueueDAL(dbKey: dbKey);
        }

        #region CommonMethods

        public List<TQueueModel> GetModelList()
        {
            return this.dal.GetModelList();
        }

        public List<TQueueModel> GetModelList(Expression<Func<TQueueModel, bool>> predicate)
        {
            return this.dal.GetModelList(predicate);
        }

        public TQueueModel GetModel(int id)
        {
            return this.dal.GetModel(id);
        }

        public TQueueModel GetModel(Expression<Func<TQueueModel, bool>> predicate)
        {
            return this.dal.GetModel(predicate);
        }

        public TQueueModel Insert(TQueueModel model)
        {
            return this.dal.Insert(model);
        }

        public int Update(TQueueModel model)
        {
            return this.dal.Update(model);
        }

        public int Delete(TQueueModel model)
        {
            return this.dal.Delete(model);
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
            return new TQueueDAL().IsCanQueue(idCard, busiSeq, unitSeq);
        }

        public ArrayList IsCanQueueO(string idCard, string busiSeq, string unitSeq)
        {
            return new TQueueDAL().IsCanQueueO(idCard, busiSeq, unitSeq);
        }



        public bool IsBasic
        {
            get { return false; }
        }

        public int ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TQueueDAL(dbKey: areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TQueueDAL(dbKey: targetDbName);
                var odal = new TQueueDAL(dbKey: areaCode.ToString());
                foreach (var s in sList)
                {
                    dal.Insert(s);
                    s.id = s.areaId;
                    s.sysFlag = 2;
                    odal.Update(s);
                }
                return sList.Count;
            }
            catch
            {
                return -1;
            }
        }

        public int ProcessUpdateData(int areaCode, string targetDbName)
        {
            try
            {
                var sdal = new TQueueDAL(dbKey: areaCode.ToString());
                var tdal = new TQueueDAL(dbKey: targetDbName);
                var sList = sdal.GetModelList(p => p.sysFlag == 1);
                foreach (var s in sList)
                {
                    var id = s.id;
                    var nData = tdal.GetModelList(p => p.areaCode == areaCode && p.areaId == s.id).FirstOrDefault();
                    var data = s;
                    data.id = nData.id;
                    data.areaCode = nData.areaCode;
                    data.areaId = nData.areaId;
                    tdal.Update(data);
                    s.sysFlag = 2;
                    s.id = id;
                    sdal.Update(s);
                }
                return sList.Count;
            }
            catch
            {
                return -1;
            }
        }

        public int ProcessDeleteData(int areaCode, string targetDbName)
        {
            return 0;
        }
    }
}
