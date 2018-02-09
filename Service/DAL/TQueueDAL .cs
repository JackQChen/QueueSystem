using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TQueueDAL
    {
        DbContext db;
        public TQueueDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TQueueDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        public TQueueDAL(DbContext db)
        {
            this.db = db;
        }

        #region CommonMethods

        public List<TQueueModel> GetModelList()
        {
            return db.Query<TQueueModel>().ToList();
        }

        public List<TQueueModel> GetModelList(Expression<Func<TQueueModel, bool>> predicate)
        {
            return db.Query<TQueueModel>().Where(predicate).ToList();
        }

        public TQueueModel GetModel(int id)
        {
            return db.Query<TQueueModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TQueueModel GetModel(Expression<Func<TQueueModel, bool>> predicate)
        {
            return db.Query<TQueueModel>().Where(predicate).FirstOrDefault();
        }

        public TQueueModel Insert(TQueueModel model)
        {
            return db.Insert(model);
        }

        public int Update(TQueueModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TQueueModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        //public List<TQueueModel> GetModelListByFunc(Func<IQuery<TQueueModel>, IQuery<TQueueModel>> func)
        //{
        //    return func(db.Query<TQueueModel>()).ToList();
        //}

        public List<TQueueModel> GetModelList(List<TWindowBusinessModel> wlBusy, int state)
        {
            var busyList = wlBusy.Select(w => w.busiSeq).ToList();
            var unitList = wlBusy.Select(w => w.unitSeq).ToList();
            return db.Query<TQueueModel>().Where(c => busyList.Contains(c.busTypeSeq) && unitList.Contains(c.unitSeq) && c.ticketTime.Date == DateTime.Now.Date && c.state == state).ToList();
        }

        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq, int state)
        {
            return db.Query<TQueueModel>().Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime.Date == DateTime.Now.Date && c.state == state).ToList();
        }

        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq)
        {
            return db.Query<TQueueModel>().Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime.Date == DateTime.Now.Date).ToList();
        }

        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end)
        {
            return db.Query<TQueueModel>().Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime >= start && c.ticketTime <= end).ToList();
        }

        public List<TQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end, int state)
        {
            return db.Query<TQueueModel>().Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime >= start && c.ticketTime <= end && c.state == state).ToList();
        }

        /// <summary>
        /// 叫号 ***** 已作废不用
        /// </summary>
        /// <param name="unitSeq"></param>
        /// <param name="busiSeq"></param>
        /// <returns></returns>
        public TQueueModel CallNo(string unitSeq, string busiSeq)
        {
            try
            {
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var lineQueue = db.Query<TQueueModel>().Where(q => q.busTypeSeq == busiSeq && q.unitSeq == unitSeq && q.state == 0 && q.ticketTime.Date == DateTime.Now.Date).OrderBy(o => o.ticketTime).ToList();//取到当天 窗口业务排队队列
                var line = lineQueue.FirstOrDefault();
                line.state = 1;
                this.Update(line);
                this.db.Session.CommitTransaction();
                return line;
            }
            catch
            {
                this.db.Session.RollbackTransaction();
                return null;
            }
            finally
            {
                this.db.Dispose();
            }
        }

        /// <summary>
        /// 排队 ** 已弃用
        /// </summary>
        /// <param name="selectBusy"></param>
        /// <param name="selectUnit"></param>
        /// <param name="ticketStart"></param>
        /// <param name="idCard"></param>
        /// <param name="name"></param>
        /// <param name="reserveSeq"></param>
        /// <returns></returns>
        public TQueueModel QueueLine(TBusinessModel selectBusy, TUnitModel selectUnit, string ticketStart, string idCard, string name, string reserveSeq)
        {
            try
            {
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var maxNo = new TLineUpMaxNoDAL(this.db).GetModelList().Where(l => l.unitSeq == selectUnit.unitSeq && l.busiSeq == selectBusy.busiSeq).FirstOrDefault();
                int ticketNo = maxNo == null ? 1 : maxNo.lineDate.Date != DateTime.Now.Date ? 1 : maxNo.maxNo + 1;
                TQueueModel line = new TQueueModel();
                line.busTypeName = selectBusy.busiName;
                line.busTypeSeq = selectBusy.busiSeq;
                line.qNumber = ticketNo.ToString();
                line.state = 0;
                line.ticketNumber = ticketStart + ticketNo.ToString("000");
                line.ticketTime = DateTime.Now;
                line.unitName = selectUnit.unitName;
                line.unitSeq = selectUnit.unitSeq;
                line.vipLever = "";
                line.windowName = "";
                line.windowNumber = "";
                line.idCard = idCard;
                line.qNmae = name;
                line.reserveSeq = reserveSeq;
                line = this.Insert(line);
                if (maxNo == null)
                {
                    maxNo = new TLineUpMaxNoModel();
                    maxNo.areaSeq = "";
                    maxNo.busiSeq = selectBusy.busiSeq;
                    maxNo.lineDate = DateTime.Now;
                    maxNo.maxNo = 1;
                    maxNo.unitSeq = selectUnit.unitSeq;
                    new TLineUpMaxNoDAL(this.db).Insert(maxNo);
                }
                else
                {
                    if (maxNo.lineDate.Date != DateTime.Now.Date)
                        maxNo.maxNo = 1;
                    else
                        maxNo.maxNo = maxNo.maxNo + 1;
                    maxNo.lineDate = DateTime.Now;
                    new TLineUpMaxNoDAL(this.db).Update(maxNo);
                }
                this.db.Session.CommitTransaction();
                return line;
            }
            catch
            {
                this.db.Session.RollbackTransaction();
                return null;
            }
            finally
            {
                this.db.Dispose();
            }
        }

        /// <summary>
        /// 排队
        /// </summary>
        /// <param name="selectBusy"></param>
        /// <param name="selectUnit"></param>
        /// <param name="ticketStart"></param>
        /// <param name="idCard"></param>
        /// <param name="name"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public TQueueModel QueueLine(TBusinessModel selectBusy, TUnitModel selectUnit, string ticketStart, string idCard, string name, TAppointmentModel app)
        {
            try
            {

                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var maxNo = new TLineUpMaxNoDAL(this.db).GetModelList().Where(l => l.unitSeq == selectUnit.unitSeq && l.busiSeq == selectBusy.busiSeq).FirstOrDefault();
                int ticketNo = maxNo == null ? 1 : maxNo.lineDate.Date != DateTime.Now.Date ? 1 : maxNo.maxNo + 1;
                TQueueModel line = new TQueueModel();
                line.busTypeName = selectBusy.busiName;
                line.busTypeSeq = selectBusy.busiSeq;
                line.qNumber = ticketNo.ToString();
                line.state = 0;
                line.ticketNumber = ticketStart + ticketNo.ToString("000");
                line.ticketTime = DateTime.Now;
                line.unitName = selectUnit.unitName;
                line.unitSeq = selectUnit.unitSeq;
                line.vipLever = "";
                line.windowName = "";
                line.windowNumber = "";
                line.idCard = idCard;
                line.qNmae = name;
                line.sysFlag = 0;
                if (app != null)
                {
                    line.appType = app.appType;
                    line.reserveSeq = app.reserveSeq;
                    line.reserveStartTime = app.reserveStartTime;
                    line.reserveEndTime = app.reserveEndTime;
                }
                line = this.Insert(line);
                if (maxNo == null)
                {
                    maxNo = new TLineUpMaxNoModel();
                    maxNo.areaSeq = "";
                    maxNo.busiSeq = selectBusy.busiSeq;
                    maxNo.lineDate = DateTime.Now;
                    maxNo.maxNo = 1;
                    maxNo.unitSeq = selectUnit.unitSeq;
                    maxNo.sysFlag = 0;
                    new TLineUpMaxNoDAL(this.db).Insert(maxNo);
                }
                else
                {
                    if (maxNo.lineDate.Date != DateTime.Now.Date)
                        maxNo.maxNo = 1;
                    else
                        maxNo.maxNo = maxNo.maxNo + 1;
                    maxNo.lineDate = DateTime.Now;
                    maxNo.sysFlag = 1;
                    new TLineUpMaxNoDAL(this.db).Update(maxNo);
                }
                this.db.Session.CommitTransaction();
                return line;
            }
            catch
            {
                this.db.Session.RollbackTransaction();
                return null;
            }
            finally
            {
                this.db.Dispose();
            }
        }


        public bool IsCanQueue(string idCard, string busiSeq, string unitSeq)
        {
            var list = db.Query<TQueueModel>().Where(c => c.idCard == idCard && c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.state == 0).ToList();
            if (list.Count > 0)
                return false;
            else
                return true;
        }

        public ArrayList IsCanQueueO(string idCard, string busiSeq, string unitSeq)
        {
            ArrayList arr = new ArrayList();
            var list = db.Query<TQueueModel>().Where(c => c.idCard == idCard && c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.state == 0).ToList();
            if (list.Count > 0)
            {
                arr.Add(false);
                arr.Add(list.FirstOrDefault());
            }
            else
                arr.Add(true);
            return arr;
        }

        public void Trans()
        {
            try
            {
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                this.db.Insert<TQueueModel>(new TQueueModel());
                this.db.Update<TQueueModel>(new TQueueModel());
                this.db.Session.CommitTransaction();
            }
            catch
            {
                this.db.Session.RollbackTransaction();
            }
            finally
            {
                this.db.Dispose();
            }
        }
    }
}
