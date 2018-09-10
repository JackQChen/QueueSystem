using System.Collections.Generic;
using DAL;
using Model;
using System;

namespace BLL
{
    public class BCallBLL : BLLBase<BCallDAL, BCallModel>
    {
        public BCallBLL()
            : base()
        {
        }

        public BCallBLL(string connName)
            : base(connName)
        {
        }

        public BCallBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public BCallModel CallNo(List<TWindowBusinessModel> wlBusy, List<TWindowBusinessModel> gwlBusy, string windowNumber, string windowUser)
        {
            return new BCallDAL().CallNo(wlBusy, gwlBusy, windowNumber, windowUser);
        }

        /// <summary>
        /// 综合显示屏：查询所有需要显示的叫号信息
        /// </summary>
        /// <returns></returns>
        public List<BCallModel> ScreenAllList()
        {
            return new BCallDAL().ScreenAllList();
        }
        public List<BCallModel> ScreenShowByArea(string AreaNo)
        {
            return new BCallDAL().ScreenShowByArea(AreaNo);
        }
        public BCallModel GetModelByHandleId(string handleId)
        {
            return new BCallDAL().GetModelByHandleId(handleId);
        }

        /// <summary>
        /// 根据窗口号返回叫号数据
        /// </summary>
        /// <param name="windowNo"></param>
        /// <param name="ticket"></param>
        /// <returns></returns>
        public List<BCallModel> GetCall(string windowNo, string ticket)
        {
            return new BCallDAL().GetCall(windowNo, ticket);
        }

        public List<BCallModel> GiveUpAll()
        {
            return new BCallDAL().GiveUpAll();
        }

        /// <summary>
        /// 按窗口全部弃号
        /// </summary>
        /// <returns></returns>
        public List<BCallModel> GiveUpAll(List<TWindowBusinessModel> windowBusys)
        {
            return new BCallDAL().GiveUpAll(windowBusys);
        }

        public bool Transfer(BCallModel call)
        {
            return new BCallDAL().Transfer(call);
        }

        public object GetWaitFor(DateTime start, DateTime end)
        {
            return new BCallDAL().GetWaitFor(start,end);
        }
    }
}
