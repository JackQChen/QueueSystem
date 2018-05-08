using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TUnitDAL
    {
        DbContext db;
        public TUnitDAL()
        {
            this.db = Factory.Instance.CreateDbContext();
        }

        public TUnitDAL(string dbKey)
        {
            this.db = Factory.Instance.CreateDbContext(dbKey);
        }

        #region CommonMethods

        public List<TUnitModel> GetModelList()
        {
            return db.Query<TUnitModel>().ToList();
        }

        public List<TUnitModel> GetModelList(Expression<Func<TUnitModel, bool>> predicate)
        {
            return db.Query<TUnitModel>().Where(predicate).ToList();
        }

        public TUnitModel GetModel(int id)
        {
            return db.Query<TUnitModel>().Where(p => p.id == id).FirstOrDefault();
        }

        public TUnitModel GetModel(Expression<Func<TUnitModel, bool>> predicate)
        {
            return db.Query<TUnitModel>().Where(predicate).FirstOrDefault();
        }

        public TUnitModel Insert(TUnitModel model)
        {
            return db.Insert(model);
        }

        public int Update(TUnitModel model)
        {
            return this.db.Update(model);
        }

        public int Delete(TUnitModel model)
        {
            return this.db.Delete(model);
        }

        #endregion

        public void ResetIndex()
        {
            this.db.Session.ExecuteNonQuery("alter table t_unit AUTO_INCREMENT=1", new DbParam[] { });
        }

        public object GetGridData()
        {
            return db.Query<TUnitModel>().Select(s => new
            {
                s.id,
                s.unitSeq,
                s.unitName,
                s.orderNum,
                Model = s
            })
            .OrderBy(k => k.id)
            .ToList();
        }

        public TUnitModel GetModel(int areaCode, int areaId)
        {
            return db.Query<TUnitModel>().Where(p => p.areaCode == areaCode && p.areaId == areaId).FirstOrDefault();
        }

        public ArrayList UploadUnitAndBusy(List<TUnitModel> uList, List<TBusinessModel> bList)
        {
            ArrayList arr = null;
            try
            {
                LockAction.Run(LockKey.Upload, () =>
                {
                    var businessList = new TBusinessDAL().GetModelList();
                    var unitList = new TUnitDAL().GetModelList();
                    var serchBlist = new List<TBusinessModel>();//循环接口返回的部门，按照部门获取到业务类型
                    var insertBlist = new List<TBusinessModel>();//筛选获取到的业务类型。把需要添加的列表整理出来
                    var inserUlist = new List<TUnitModel>();
                    var busyBll = new TBusinessDAL(this.db);
                    foreach (var uSeq in uList)
                    {
                        var unitBusy = bList.Where(b => b.unitSeq == uSeq.unitSeq && b.unitName == uSeq.unitName).ToList();
                        if (unitBusy != null)
                            serchBlist.AddRange(unitBusy);
                        var unit = unitList.Where(b => b.unitSeq == uSeq.unitSeq && b.unitName == uSeq.unitName);
                        if (unit.Count() == 0)
                            inserUlist.Add(uSeq);
                        else
                            uSeq.orderNum = unit.FirstOrDefault().orderNum;
                    }
                    foreach (var i in serchBlist)
                    {
                        if (businessList.Where(b => b.unitSeq == i.unitSeq && b.unitName == i.unitName && b.busiSeq == i.busiSeq && b.busiName == i.busiName).Count() == 0)
                            insertBlist.Add(i);
                    }

                    var deleteBusy = new List<TBusinessModel>();
                    var deleteUnit = new List<TUnitModel>();
                    foreach (var busy in businessList)
                    {
                        if (bList.Where(b => b.unitSeq == busy.unitSeq && b.unitName == busy.unitName && b.busiSeq == busy.busiSeq && b.busiName == busy.busiName).Count() == 0)
                            deleteBusy.Add(busy);
                    }
                    foreach (var unit in unitList)
                    {
                        if (uList.Where(u => u.unitSeq == unit.unitSeq && u.unitName == unit.unitName).Count() == 0)
                            deleteUnit.Add(unit);
                    }
                    
                    foreach (var u in inserUlist)
                        this.Insert(u);
                    foreach (var d in deleteUnit)
                        this.Delete(d);
                    foreach (var i in insertBlist)
                        busyBll.Insert(i);
                    foreach (var d in deleteBusy)
                        busyBll.Delete(d);
                    arr = new ArrayList();
                    arr.Add(uList.OrderBy(o=>o.orderNum).ToList());
                    arr.Add(serchBlist);
                });
                return arr;
            }
            catch (Exception ex)
            {
                return arr;
            }
        }
    }
}
