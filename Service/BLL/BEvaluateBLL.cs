using DAL;
using Model;
using System;

namespace BLL
{
    public class BEvaluateBLL : BLLBase<BEvaluateDAL, BEvaluateModel>
    {
        public BEvaluateBLL()
            : base()
        {
        }

        public BEvaluateBLL(string connName)
            : base(connName)
        {
        }

        public BEvaluateBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        /// <summary>
        /// 满意率
        /// </summary>
        /// <param name="type">1:服务态度  2:完成质量 3:处理效率 4:廉洁自律 </param>
        /// <param name="start"></param>
        /// <param name="end"
        /// <returns></returns>
        public object GetSatisfied(int type, DateTime start, DateTime end)
        {
            return new BEvaluateDAL().GetSatisfied(type, start, end);
        }

        /// <summary>
        /// 获取好评率与量化分数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public object GetFavorableComment(DateTime start, DateTime end)
        {
            return new BEvaluateDAL().GetFavorableComment(start, end);
        }

        /// <summary>
        /// 获取评价数据以及评价率
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public object GetEvaluate(DateTime start, DateTime end)
        {
            return new BEvaluateDAL().GetEvaluate(start, end);
        }

        /// <summary>
        /// 获取评价星级百分比（柱状图、饼状图）
        /// </summary>
        /// <param name="type">1:服务态度  2:完成质量 3:处理效率 4:廉洁自律</param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="unitSeq">部门编号（可空）</param>
        /// <param name="userId">用户id（可空）</param>
        /// <returns></returns>
        public object GetEvaluatePercent(int type, DateTime start, DateTime end, string unitSeq, string userId)
        {
            return new BEvaluateDAL().GetEvaluatePercent(type, start, end, unitSeq, userId);
        }

        /// <summary>
        /// 获取已评价的办件数比例（柱状、饼状）
        /// </summary>
        /// <param name="type">
        /// 1：按部门统计         （此时X轴为部门）
        /// 2：按部门业务类型     （此时X轴为业务类型，此时部门不能为空,否则数据量大展示不全，并且跨部门的同样名称业务类型也无法区分）
        /// </param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="unitSeq"></param>
        /// <param name="busiSeq"></param>
        /// <returns></returns>
        public object GetWorkPercent(int type, DateTime start, DateTime end, string unitSeq, string busiSeq)
        {
            return new BEvaluateDAL().GetWorkPercent(type, start, end, unitSeq, busiSeq);
        }
    }
}
