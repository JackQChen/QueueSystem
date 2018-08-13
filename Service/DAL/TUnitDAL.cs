using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class TUnitDAL : DALBase<TUnitModel>
    {
        public TUnitDAL()
            : base()
        {
        }

        public TUnitDAL(string connName)
            : base(connName)
        {
        }

        public TUnitDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public TUnitDAL(DbContext db)
            : base(db)
        {
        }

        public TUnitDAL(DbContext db, string areaNo)
            : base(db, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.GetQuery().Select(s => new
            {
                s.ID,
                s.unitSeq,
                s.unitName,
                s.orderNum,
                Model = s
            })
            .OrderBy(k => k.ID)
            .ToList();
        }

        public TUnitModel GetModel(int areaCode, int areaId)
        {
            return db.Query<TUnitModel>().Where(p => 1 == 1).FirstOrDefault();
        }

        public ArrayList UploadUnitAndBusy(List<TUnitModel> uList, List<TBusinessModel> bList)
        {
            ArrayList arr = null;
            try
            {
                LockAction.Run(FLockKey.Upload, () =>
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
                        //else
                        //    uSeq.orderNum = unit.FirstOrDefault().orderNum;
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
                    arr.Add(uList.OrderBy(o => o.orderNum).ToList());
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
