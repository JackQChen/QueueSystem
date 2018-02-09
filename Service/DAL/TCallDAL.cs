using System.Collections.Generic;
using Chloe;
using Model;
using System.Linq;
using System;
using System.Linq.Expressions;

namespace DAL
{
    public class TCallDAL
    {
        DbContext db;
        public TCallDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }
        public TCallDAL(string dbName)
        {
            this.db = Factory.Instance.CreateDbContext(dbName);
        }

        #region CommonMethods

        public List<TCallModel> GetModelList()
        {
            return db.Query<TCallModel>().ToList();
        }

        public List<TCallModel> GetModelList(Expression<Func<TCallModel, bool>> predicate)
        {
            return db.Query<TCallModel>().Where(predicate).ToList();
        }

        public TCallModel GetModel(int id)
        {
            return db.Query<TCallModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TCallModel Insert(TCallModel model)
        {
            return db.Insert(model);
        }

        public int Update(TCallModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TCallModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        /// <summary>
        /// 叫号 ** 已弃用
        /// </summary>
        /// <param name="unitSeq"></param>
        /// <param name="busiSeq"></param>
        /// <param name="windowNumber"></param>
        /// <param name="windowUser"></param>
        /// <returns></returns>
        public TCallModel CallNo(string unitSeq, string busiSeq, string windowNumber, string windowUser)
        {
            try
            {
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var lineQueue = db.Query<TQueueModel>().Where(q => q.busTypeSeq == busiSeq && q.unitSeq == unitSeq && q.state == 0 && q.ticketTime.Date == DateTime.Now.Date).OrderBy(o => o.id).ToList();//取到当天 窗口业务排队队列
                var line = lineQueue.FirstOrDefault();
                if (line == null)
                    return null;
                line.state = 1;
                new TQueueDAL(this.db).Update(line);
                var call = new TCallModel();
                call.busiSeq = line.busTypeSeq;
                call.handleId = DateTime.Now.ToString("yyyyMMddHHmmss");
                call.handleTime = DateTime.Now;
                call.idCard = line.idCard;
                call.qId = line.id;
                call.qNmae = line.qNmae;
                call.state = 0;
                call.ticketNumber = line.ticketNumber;
                call.ticketTime = line.ticketTime;
                call.unitSeq = line.unitSeq;
                call.windowNumber = windowNumber;
                call.windowUser = windowUser;
                var ret = this.Insert(call);
                this.db.Session.CommitTransaction();
                return ret;
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
        /// 叫号
        /// </summary>
        /// <param name="wlBusy"></param>
        /// <param name="windowNumber"></param>
        /// <param name="windowUser"></param>
        /// <returns></returns>
        public TCallModel CallNo(List<TWindowBusinessModel> wlBusy, string windowNumber, string windowUser)
        {
            try
            {
                var busyList = wlBusy.Select(w => w.busiSeq).ToList();
                var unitList = wlBusy.Select(w => w.unitSeq).ToList();
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var date = DateTime.Now;
                var lineFirst = db.Query<TQueueModel>().Where(q => busyList.Contains(q.busTypeSeq) && unitList.Contains(q.unitSeq) && q.state == 0 && q.ticketTime.Date == date.Date && q.appType == 1 && q.reserveStartTime <= date && q.reserveEndTime >= date).OrderBy(o => o.id).FirstOrDefault();
                TQueueModel line = null;
                if (lineFirst == null)
                {
                    var lineQueue = db.Query<TQueueModel>().Where(q => busyList.Contains(q.busTypeSeq) && unitList.Contains(q.unitSeq) && q.state == 0 && q.ticketTime.Date == date.Date).OrderBy(o => o.id).ToList();//取到当天 窗口业务排队队列
                    line = lineQueue.FirstOrDefault();
                    if (line == null)
                        return null;
                }
                else
                    line = lineFirst;
                line.state = 1;
                new TQueueDAL(this.db).Update(line);
                var call = new TCallModel();
                call.busiSeq = line.busTypeSeq;
                call.handleId = DateTime.Now.ToString("yyyyMMddHHmmss");
                call.handleTime = DateTime.Now;
                call.idCard = line.idCard;
                call.qId = line.id;
                call.qNmae = line.qNmae;
                call.state = 0;
                call.ticketNumber = line.ticketNumber;
                call.ticketTime = line.ticketTime;
                call.unitSeq = line.unitSeq;
                call.windowNumber = windowNumber;
                call.windowUser = windowUser;
                var ret = this.Insert(call);
                this.db.Session.CommitTransaction();
                return ret;
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
        /// 全部弃号
        /// </summary>
        public List<TCallModel> GiveUpAll()
        {
            try
            {
                List<TCallModel> tList = new List<TCallModel>();
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                var lineQueue = db.Query<TQueueModel>().Where(q => q.state == 0).ToList();
                var qDal = new TQueueDAL(this.db);
                foreach (var q in lineQueue)
                {
                    q.state = 1;
                    qDal.Update(q);
                }
                var list = db.Query<TCallModel>().Where(q => (q.state == 0 || q.state == 3)).ToList();
                foreach (var l in list)
                {
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
        /// 综合显示屏数据 ** 已弃用
        /// </summary>
        /// <returns></returns>
        public List<TCallModel> ScreenShow()
        {
            var list = db.Query<TCallModel>().Where(q => q.ticketTime.Date == DateTime.Now.Date && q.state == 0).OrderByDesc(o => o.handleTime).ToList();//按照处理时间排序
            return list;
        }
        /// <summary>
        /// 综合显示 ** 已弃用
        /// </summary>
        /// <param name="AreaNo"></param>
        /// <returns></returns>
        public List<TCallModel> ScreenShow(int AreaNo)
        {
            var window = db.Query<TWindowModel>().Where(q => q.AreaName == AreaNo).Select(s => s.Number).ToList();
            var list = db.Query<TCallModel>().Where(q => q.ticketTime.Date == DateTime.Now.Date && q.state == 0).Where(q => window.Contains(q.windowNumber))
                .OrderByDesc(o => o.handleTime).ToList();//按照处理时间排序
            var gList = from u in list
                        group u by u.windowNumber into g
                        select g;
            List<TCallModel> cList = new List<TCallModel>();
            foreach (var u in gList)
            {
                var m = list.Where(c => c.windowNumber == u.Key).OrderByDescending(o => o.handleTime).First();
                cList.Add(m);
            }
            return cList;
        }

        /// <summary>
        /// 综合显示
        /// </summary>
        /// <param name="AreaNo"></param>
        /// <returns></returns>
        public List<TCallModel> ScreenShowByArea(string AreaNo)
        {
            string[] areaList = AreaNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var window = db.Query<TWindowModel>().Where(q => areaList.Contains(q.AreaName.ToString())).Select(s => s.Number).ToList();
            var list = db.Query<TCallModel>().Where(q => q.ticketTime.Date == DateTime.Now.Date && (q.state == 0 || q.state == 3)).Where(q => window.Contains(q.windowNumber))
                .OrderByDesc(o => o.handleTime).ToList();//按照处理时间排序
            var gList = from u in list
                        group u by u.windowNumber into g
                        select g;
            List<TCallModel> cList = new List<TCallModel>();
            foreach (var u in gList)
            {
                var m = list.Where(c => c.windowNumber == u.Key).OrderByDescending(o => o.handleTime).First();
                cList.Add(m);
            }
            return cList;
        }

        public TCallModel GetModelByHandleId(string handleId)
        {
            return db.Query<TCallModel>().Where(p => p.handleId == handleId).FirstOrDefault();
        }

        /// <summary>
        /// 根据窗口号返回叫号数据
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<TCallModel> GetCall(string windowNo, string ticket)
        {
            var list = db.Query<TCallModel>().Where(q => q.ticketTime.Date == DateTime.Now.Date && (q.state == 0 || q.state == 3) && q.windowNumber == windowNo);
            if (ticket != "")
                list = list.Where(l => l.ticketNumber.Contains(ticket));
            return list.OrderByDesc(o => o.handleTime).ToList();

        }

        /// <summary>
        /// 转移，扔回呼叫资源池
        /// </summary>
        /// <param name="call"></param>
        /// <returns></returns>
        public bool Transfer(TCallModel call)
        {
            try
            {
                this.db.Session.BeginTransaction(System.Data.IsolationLevel.ReadCommitted);
                call.state = 2;
                this.Update(call);
                TQueueDAL dal = new TQueueDAL(this.db);
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
