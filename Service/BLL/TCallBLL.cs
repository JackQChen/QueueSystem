using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{
    public class TCallBLL : IUploadData
    {
        public TCallBLL()
        {
        }

        #region CommonMethods


        public List<TCallModel> GetModelList()
        {
            return new TCallDAL().GetModelList();
        }

        public List<TCallModel> GetModelList(Expression<Func<TCallModel, bool>> predicate)
        {
            return new TCallDAL().GetModelList(predicate);
        }

        public TCallModel GetModel(int id)
        {
            return new TCallDAL().GetModel(id);
        }

        public TCallModel GetModel(Expression<Func<TCallModel, bool>> predicate)
        {
            return new TCallDAL().GetModel(predicate);
        }

        public TCallModel Insert(TCallModel model)
        {
            return new TCallDAL().Insert(model);
        }

        public int Update(TCallModel model)
        {
            return new TCallDAL().Update(model);
        }

        public int Delete(TCallModel model)
        {
            return new TCallDAL().Delete(model);
        }

        #endregion

        public TCallModel CallNo(string unitSeq, string busiSeq, string windowNumber, string windowUser)
        {
            return new TCallDAL().CallNo(unitSeq, busiSeq, windowNumber, windowUser);
        }

        public TCallModel CallNo(List<TWindowBusinessModel> wlBusy, string windowNumber, string windowUser)
        {
            return new TCallDAL().CallNo(wlBusy, windowNumber, windowUser);
        }

        public List<TCallModel> ScreenShow()
        {
            return new TCallDAL().ScreenShow();
        }

        public List<TCallModel> ScreenShow(int AreaNo)
        {
            return new TCallDAL().ScreenShow(AreaNo);
        }
        public List<TCallModel> ScreenShowByArea(string AreaNo)
        {
            return new TCallDAL().ScreenShowByArea(AreaNo);
        }
        public TCallModel GetModelByHandleId(string handleId)
        {
            return new TCallDAL().GetModelByHandleId(handleId);
        }

        /// <summary>
        /// 根据窗口号返回叫号数据
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<TCallModel> GetCall(string windowNo, string ticket)
        {
            return new TCallDAL().GetCall(windowNo, ticket);
        }

        public List<TCallModel> GiveUpAll()
        {
            return new TCallDAL().GiveUpAll();
        }

        public bool Transfer(TCallModel call)
        {
            return new TCallDAL().Transfer(call);
        }



        public bool IsBasic
        {
            get { return false; }
        }

        public bool ProcessInsertData(int areaCode, string targetDbName)
        {
            try
            {
                var sList = new TCallDAL(areaCode.ToString()).GetModelList(c => c.sysFlag == 0).ToList();
                sList.ForEach(s =>
                {
                    s.areaCode = areaCode;
                    s.areaId = s.id;
                });
                var dal = new TCallDAL(targetDbName);
                var odal = new TCallDAL(areaCode.ToString());
                foreach (var s in sList)
                {
                    dal.Insert(s);
                    s.id = s.areaId;
                    s.sysFlag = 2;
                    odal.Update(s);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessUpdateData(int areaCode, string targetDbName)
        {
            try
            {
                var sdal = new TCallDAL(areaCode.ToString());
                var tdal = new TCallDAL(targetDbName);
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
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ProcessDeleteData(int areaCode, string targetDbName)
        {
            return true;
        }
    }
}
