using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;
using System.Threading;

namespace DAL
{
    public class BQueueDAL : DALBase<BQueueModel>
    {
        static object obj = new object();
        public BQueueDAL()
            : base()
        {
        }

        public BQueueDAL(DbContext db)
            : base(db)
        {
        }
        public BQueueDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }
        public BQueueDAL(string connName)
            : base(connName)
        {
        }

        public BQueueDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public List<BQueueModel> GetModelList(List<TWindowBusinessModel> wlBusy, int state)
        {
            var busyList = wlBusy.Select(w => w.busiSeq).ToList();
            var unitList = wlBusy.Select(w => w.unitSeq).ToList();
            return db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => busyList.Contains(c.busTypeSeq) && unitList.Contains(c.unitSeq) && c.ticketTime.Date == DateTime.Now.Date && c.state == state).ToList();
        }

        public List<BQueueModel> GetModelList(string unitSeq, int state)
        {
            return db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.unitSeq == unitSeq && c.ticketTime.Date == DateTime.Now.Date && c.state == state).ToList();
        }

        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq, int state)
        {
            return db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime.Date == DateTime.Now.Date && c.state == state).ToList();
        }

        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq)
        {
            return db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime.Date == DateTime.Now.Date).ToList();
        }

        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end)
        {
            return db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime >= start && c.ticketTime <= end).ToList();
        }

        public List<BQueueModel> GetModelList(string busiSeq, string unitSeq, DateTime start, DateTime end, int state)
        {
            return db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.ticketTime >= start && c.ticketTime <= end && c.state == state).ToList();
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
        public BQueueModel QueueLine(TBusinessModel selectBusy, TUnitModel selectUnit, string ticketStart, string idCard, string name)
        {
            BQueueModel qModel = null;
            try
            {
                lock (obj)
                {
                    db.Session.BeginTransaction();
                    var dal = new BLineUpMaxNoDAL(this.db, this.areaNo);
                    var maxNo = dal.GetModelList().Where(a => a.AreaNo == this.areaNo).Where(l => l.unitSeq == selectUnit.unitSeq && l.busiSeq == selectBusy.busiSeq).FirstOrDefault();
                    int ticketNo = maxNo == null ? 1 : maxNo.lineDate.Date != DateTime.Now.Date ? 1 : maxNo.maxNo + 1;
                    BQueueModel line = new BQueueModel();
                    line.ID = this.GetMaxId();
                    line.AreaNo = this.areaNo;
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
                    line.qType = 0;
                    line = this.Insert(line);
                    if (maxNo == null)
                    {
                        maxNo = new BLineUpMaxNoModel();
                        maxNo.ID = dal.GetMaxId();
                        maxNo.AreaNo = this.areaNo;
                        maxNo.areaSeq = "";
                        maxNo.busiSeq = selectBusy.busiSeq;
                        maxNo.lineDate = DateTime.Now;
                        maxNo.maxNo = 1;
                        maxNo.unitSeq = selectUnit.unitSeq;
                        dal.Insert(maxNo);
                    }
                    else
                    {
                        if (maxNo.lineDate.Date != DateTime.Now.Date)
                            maxNo.maxNo = 1;
                        else
                            maxNo.maxNo = maxNo.maxNo + 1;
                        maxNo.lineDate = DateTime.Now;
                        dal.Update(maxNo);
                    }
                    qModel = line;
                    db.Session.CommitTransaction();
                }
            }
            catch
            {
                db.Session.RollbackTransaction();
                return null;
            }
            finally
            {
                db.Session.Dispose();
            }
            return qModel;
        }

        /// <summary>
        /// 排队 微信接口使用
        /// </summary>
        /// <param name="selectBusy"></param>
        /// <param name="selectUnit"></param>
        /// <param name="ticketStart"></param>
        /// <param name="idCard"></param>
        /// <param name="name"></param>
        /// <param name="app"></param>
        /// <returns></returns>
        public BQueueModel QueueLine(string unitSeq, string unitName, string busiSeq, string busiName, string ticketStart, string idCard, string name, string wxId)
        {
            BQueueModel qModel = null;
            try
            {
                lock (obj)
                {
                    db.Session.BeginTransaction();
                    var dal = new BLineUpMaxNoDAL(this.db, this.areaNo);
                    var maxNo = dal.GetModelList().Where(a => a.AreaNo == this.areaNo).Where(l => l.unitSeq == unitSeq && l.busiSeq == busiSeq).FirstOrDefault();
                    int ticketNo = maxNo == null ? 1 : maxNo.lineDate.Date != DateTime.Now.Date ? 1 : maxNo.maxNo + 1;
                    BQueueModel line = new BQueueModel();
                    line.ID = this.GetMaxId();
                    line.AreaNo = this.areaNo;
                    line.busTypeName = busiName;
                    line.busTypeSeq = busiSeq;
                    line.qNumber = ticketNo.ToString();
                    line.state = 0;
                    line.ticketNumber = ticketStart + ticketNo.ToString("000");
                    line.ticketTime = DateTime.Now;
                    line.unitName = unitName;
                    line.unitSeq = unitSeq;
                    line.vipLever = "";
                    line.windowName = "";
                    line.windowNumber = "";
                    line.idCard = idCard;
                    line.qNmae = name;
                    line.wxId = wxId;
                    line.qType = 1;
                    line = this.Insert(line);
                    if (maxNo == null)
                    {
                        maxNo = new BLineUpMaxNoModel();
                        maxNo.ID = dal.GetMaxId();
                        maxNo.AreaNo = this.areaNo;
                        maxNo.areaSeq = "";
                        maxNo.busiSeq = busiSeq;
                        maxNo.lineDate = DateTime.Now;
                        maxNo.maxNo = 1;
                        maxNo.unitSeq = unitSeq;
                        dal.Insert(maxNo);
                    }
                    else
                    {
                        if (maxNo.lineDate.Date != DateTime.Now.Date)
                            maxNo.maxNo = 1;
                        else
                            maxNo.maxNo = maxNo.maxNo + 1;
                        maxNo.lineDate = DateTime.Now;
                        dal.Update(maxNo);
                    }
                    qModel = line;
                    db.Session.CommitTransaction();
                }
            }
            catch
            {
                db.Session.RollbackTransaction();
                return null;
            }
            finally
            {
                db.Session.Dispose();
            }
            return qModel;
        }

        public bool IsCanQueue(string idCard, string busiSeq, string unitSeq)
        {
            var list = db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.idCard == idCard && c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.state == 0).ToList();
            if (list.Count > 0)
                return false;
            else
                return true;
        }

        public ArrayList IsCanQueueO(string idCard, string busiSeq, string unitSeq)
        {
            ArrayList arr = new ArrayList();
            var list = db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(c => c.idCard == idCard && c.busTypeSeq == busiSeq && c.unitSeq == unitSeq && c.state == 0).ToList();
            if (list.Count > 0)
            {
                arr.Add(false);
                arr.Add(list.FirstOrDefault());
            }
            else
                arr.Add(true);
            return arr;
        }
    }
}
