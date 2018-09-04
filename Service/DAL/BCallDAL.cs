using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;
using System.IO;

namespace DAL
{
    public class BCallDAL : DALBase<BCallModel>
    {
        static object obj = new object();
        public BCallDAL()
            : base()
        {
        }

        public BCallDAL(DbContext db)
            : base(db)
        {
        }

        public BCallDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public BCallDAL(string connName)
            : base(connName)
        {
        }

        public BCallDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        /// <summary>
        /// 叫号 * 有绿色通道插队
        /// </summary>
        /// <param name="wlBusy"></param>
        /// <param name="gwlBusy"></param>
        /// <param name="windowNumber"></param>
        /// <param name="windowUser"></param>
        /// <returns></returns>
        public BCallModel CallNo(List<TWindowBusinessModel> wlBusy, List<TWindowBusinessModel> gwlBusy, string windowNumber, string windowUser)
        {
            BCallModel tcModel = null;
            try
            {
                lock (obj)
                {
                    db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                    if (gwlBusy == null)
                        gwlBusy = new List<TWindowBusinessModel>();
                    var busyList = wlBusy.Select(w => w.busiSeq).ToList();
                    var unitList = wlBusy.Select(w => w.unitSeq).ToList();
                    var gbList = gwlBusy.Select(w => w.busiSeq).ToList();
                    var guList = gwlBusy.Select(w => w.unitSeq).ToList();
                    var date = DateTime.Now;
                    BQueueModel line = null;
                    var lineGreen = db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(q => gbList.Contains(q.busTypeSeq) && guList.Contains(q.unitSeq) && q.state == 0 && q.ticketTime.Date == date.Date).OrderBy(o => o.ID).FirstOrDefault();
                    if (lineGreen == null)
                    {
                        var lineQueue = db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(q => busyList.Contains(q.busTypeSeq) && unitList.Contains(q.unitSeq) && q.state == 0 && q.ticketTime.Date == date.Date).OrderBy(o => o.ID).ToList();//取到当天 窗口业务排队队列
                        line = lineQueue.FirstOrDefault();
                        if (line == null)
                            return null;
                    }
                    else
                        line = lineGreen;
                    line.state = 1;
                    new BQueueDAL(this.db, this.areaNo).Update(line);
                    var call = new BCallModel();
                    call.ID = this.GetMaxId();
                    call.AreaNo = this.areaNo;
                    call.busiSeq = line.busTypeSeq;
                    call.handleId = DateTime.Now.ToString("yyyyMMddHHmmss");
                    call.handleTime = DateTime.Now;
                    call.idCard = line.idCard;
                    call.qId = line.ID;
                    call.qNmae = line.qNmae;
                    call.reserveSeq = line.reserveSeq;
                    call.state = 0;
                    call.ticketNumber = line.ticketNumber;
                    call.ticketTime = line.ticketTime;
                    call.unitSeq = line.unitSeq;
                    call.windowNumber = windowNumber;
                    call.windowUser = windowUser;
                    call.finishTime = DateTime.MinValue;
                    var ret = this.Insert(call);
                    tcModel = ret;
                    db.Session.CommitTransaction();
                }
            }
            catch
            {
                db.Session.RollbackTransaction();
                return null;
            }
            return tcModel;
        }

        /// <summary>
        /// 全部弃号
        /// </summary>
        public List<BCallModel> GiveUpAll()
        {
            try
            {
                List<BCallModel> tList = new List<BCallModel>();
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var lineQueue = db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(q => q.state == 0).ToList();
                var qDal = new BQueueDAL(this.db, this.areaNo);
                foreach (var q in lineQueue)
                {
                    q.state = 1;
                    qDal.Update(q);
                }
                var list = db.Query<BCallModel>().Where(a => a.AreaNo == this.areaNo).Where(q => (q.state == 0 || q.state == 3)).ToList();
                foreach (var l in list)
                {
                    l.finishTime = DateTime.Now;
                    l.state = -1;
                    this.Update(l);
                    tList.Add(l);
                }
                this.db.Session.CommitTransaction();
                return tList;
            }
            catch (Exception ex)
            {
                this.db.Session.RollbackTransaction();
                if (ex.InnerException != null)
                {
                    File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "s_Exception.txt", ex.InnerException.Message + "\r\n" + ex.InnerException.StackTrace + "\r\n");
                    throw ex.InnerException;
                }
                throw ex;
            }
            finally
            {
                this.db.Dispose();
            }
        }

        /// <summary>
        /// 按窗口全部弃号
        /// </summary>
        /// <returns></returns>
        public List<BCallModel> GiveUpAll(List<TWindowBusinessModel> windowBusys)
        {
            try
            {
                List<BCallModel> tList = new List<BCallModel>();
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var busyList = windowBusys.Select(w => w.busiSeq).ToList();
                var unitList = windowBusys.Select(w => w.unitSeq).ToList();
                var lineQueue = db.Query<BQueueModel>().Where(a => a.AreaNo == this.areaNo).Where(q => busyList.Contains(q.busTypeSeq) && unitList.Contains(q.unitSeq) && q.state == 0).ToList();
                var qDal = new BQueueDAL(this.db, this.areaNo);
                foreach (var q in lineQueue)
                {
                    q.state = 1;
                    qDal.Update(q);
                }
                var list = db.Query<BCallModel>().Where(a => a.AreaNo == this.areaNo).Where(q => busyList.Contains(q.busiSeq) && unitList.Contains(q.unitSeq) && (q.state == 0 || q.state == 3)).ToList();
                foreach (var l in list)
                {
                    l.finishTime = DateTime.Now;
                    l.state = -1;
                    this.Update(l);
                    tList.Add(l);
                }
                this.db.Session.CommitTransaction();
                return tList;
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
        /// 综合显示：按照区域查询
        /// </summary>
        /// <param name="AreaNo"></param>
        /// <returns></returns>
        public List<BCallModel> ScreenShowByArea(string AreaNo)
        {
            string[] areaList = AreaNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var window = db.Query<TWindowModel>().Where(a => a.AreaNo == this.areaNo).Where(q => areaList.Contains(q.AreaName.ToString())).Select(s => s.Number).ToList();
            var list = db.Query<BCallModel>().Where(a => a.AreaNo == this.areaNo).Where(q => q.ticketTime.Date == DateTime.Now.Date && (q.state == 0 || q.state == 3)).Where(q => window.Contains(q.windowNumber))
                .OrderByDesc(o => o.handleTime).ToList();//按照处理时间排序
            var gList = from u in list
                        group u by u.windowNumber into g
                        select g;
            List<BCallModel> cList = new List<BCallModel>();
            foreach (var u in gList)
            {
                var m = list.Where(c => c.windowNumber == u.Key).OrderByDescending(o => o.handleTime).First();
                cList.Add(m);
            }
            return cList;
        }

        /// <summary>
        /// 综合显示屏：查询所有需要显示的叫号信息
        /// </summary>
        /// <returns></returns>
        public List<BCallModel> ScreenAllList()
        {
            var list = db.Query<BCallModel>().Where(a => a.AreaNo == this.areaNo).Where(q => q.ticketTime.Date == DateTime.Now.Date && (q.state == 0 || q.state == 3)).OrderByDesc(o => o.handleTime).ToList();//按照处理时间排序
            var gList = from u in list
                        group u by u.windowNumber into g
                        select g;
            List<BCallModel> cList = new List<BCallModel>();
            foreach (var u in gList)
            {
                var m = list.Where(c => c.windowNumber == u.Key).OrderByDescending(o => o.handleTime).First();
                cList.Add(m);
            }
            return cList;
        }


        public BCallModel GetModelByHandleId(string handleId)
        {
            return db.Query<BCallModel>().Where(a => a.AreaNo == this.areaNo).Where(p => p.handleId == handleId).FirstOrDefault();
        }

        /// <summary>
        /// 根据窗口号返回叫号数据
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<BCallModel> GetCall(string windowNo, string ticket)
        {
            var list = db.Query<BCallModel>().Where(a => a.AreaNo == this.areaNo).Where(q => q.ticketTime.Date == DateTime.Now.Date && (q.state == 0 || q.state == 3) && q.windowNumber == windowNo);
            if (ticket != "")
                list = list.Where(l => l.ticketNumber.Contains(ticket));
            return list.OrderByDesc(o => o.handleTime).ToList();

        }

        /// <summary>
        /// 转移，扔回呼叫资源池
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public bool Transfer(BCallModel call)
        {
            try
            {
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                call.state = 2;
                call.finishTime = DateTime.Now;
                this.Update(call);
                BQueueDAL dal = new BQueueDAL(this.db, this.areaNo);
                var model = dal.GetModel(call.qId);
                model.state = 0;
                dal.Update(model);
                this.db.Session.CommitTransaction();
                return true;
            }
            catch
            {
                this.db.Session.RollbackTransaction();
                return false;
            }
            finally
            {
                this.db.Dispose();
            }
        }
    }
}
