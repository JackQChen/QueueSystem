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
    }
}
