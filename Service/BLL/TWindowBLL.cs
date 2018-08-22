using DAL;
using Model;
using System.Collections.Generic;

namespace BLL
{
    public class TWindowBLL : BLLBase<TWindowDAL, TWindowModel>
    {
        public TWindowBLL()
            : base()
        {
        }

        public TWindowBLL(string connName)
            : base(connName)
        {
        }

        public TWindowBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public object GetGridData()
        {
            return this.CreateDAL().GetGridData();
        }

        /// <summary>
        /// 根据窗口区域获取窗口号列表
        /// </summary>
        /// <param name="aList"></param>
        /// <returns></returns>
        public List<string> GetWindowByArea(string aList)
        {
            return this.CreateDAL().GetWindowByArea(aList);
        }

        //RateService相关

        public object RS_GetWindowList()
        {
            return this.CreateDAL().RS_GetWindowList();
        }

        public object RS_GetUserListByWindowNo(string winNum)
        {
            return this.CreateDAL().RS_GetUserListByWindowNo(winNum);
        }

        public string RS_GetUserPhoto(string userCode)
        {
            return this.CreateDAL().RS_GetUserPhoto(userCode);
        }

        public object RS_GetModel(string winNum, string userCode)
        {
            return this.CreateDAL().RS_GetModel(winNum, userCode);
        }
    }
}
