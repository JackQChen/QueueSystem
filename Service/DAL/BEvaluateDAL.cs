using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{
    public class BEvaluateDAL : DALBase<BEvaluateModel>
    {
        public BEvaluateDAL()
            : base()
        {
        }

        public BEvaluateDAL(DbContext db)
            : base(db)
        {
        }

        public BEvaluateDAL(string connName)
            : base(connName)
        {
        }

        public BEvaluateDAL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        /// <summary>
        /// 满意率
        /// </summary>
        /// <param name="type">1:服务态度  2:完成质量 3:处理效率 4:廉洁自律 </param>
        /// <returns></returns>
        public object GetSatisfied(int type,DateTime start,DateTime end)
        {

            try
            {
                var strtype = type == 1 ? "a.evaluateAttitude" : type == 2 ? "a.evaluateQuality" : type == 3 ? "a.evaluateEfficiency" : "a.evaluateHonest";
                db.Session.BeginTransaction();
                var obj = db.Session.ExecuteScalar(@"select a.unitSeq,a.unitName,a.ID,a.`Name`,count5+count4+count3+count2+count1 count,count5,count4,count3,count2,count1,
                                                    (count5+count4)*100/(count5+count4+count3+count2+count1) satisfied   from 
                                                    (
                                                    select a.unitSeq,a.unitName,a.ID,a.`Name`,sum(count5) count5,sum(count4) count4,sum(count3) count3,sum(count2) count2,sum(count1) count1
                                                    FROM
                                                    (
                                                    select unitSeq,unitName,ID,`Name`,count(*) count5,0 count4,0 count3,0 count2,0 count1 from 
                                                    (
                                                    select a.unitSeq,c.unitName,A.windowUser ID, b.`Name` from b_evaluate a 
                                                    LEFT JOIN t_user b on a.areaNo=b.areaNo and a.windowUser=b.ID
                                                    LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                    where " + strtype + @"=5  and a.handleTime>=@start and a.handleTime<=@end
                                                    ) a  group by unitSeq,unitName,ID,`Name` HAVING count(*) > 0
                                                    UNION ALL
                                                    select unitSeq,unitName,ID,`Name`,0 count5,count(*) count4,0 count3,0 count2,0 count1 from 
                                                    (
                                                    select a.unitSeq,c.unitName,A.windowUser ID, b.`Name` from b_evaluate a 
                                                    LEFT JOIN t_user b on a.areaNo=b.areaNo and a.windowUser=b.ID
                                                    LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                    where " + strtype + @"=4  and a.handleTime>=@start and a.handleTime<=@end
                                                    ) a  group by unitSeq,unitName,ID,`Name` HAVING count(*) > 0
                                                    UNION ALL
                                                    select unitSeq,unitName,ID,`Name`,0 count5,0 count4,count(*) count3,0 count2,0 count1 from 
                                                    (
                                                    select a.unitSeq,c.unitName,A.windowUser ID, b.`Name` from b_evaluate a 
                                                    LEFT JOIN t_user b on a.areaNo=b.areaNo and a.windowUser=b.ID
                                                    LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                    where " + strtype + @"=3 and a.handleTime>=@start and a.handleTime<=@end
                                                    ) a group by unitSeq,unitName,ID,`Name` HAVING count(*) > 0
                                                    UNION ALL
                                                    select unitSeq,unitName,ID,`Name`,0 count5,0 count4,0 count3,count(*) count2,0 count1 from 
                                                    (
                                                    select a.unitSeq,c.unitName,A.windowUser ID, b.`Name` from b_evaluate a 
                                                    LEFT JOIN t_user b on a.areaNo=b.areaNo and a.windowUser=b.ID
                                                    LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                    where " + strtype + @"=2 and a.handleTime>=@start and a.handleTime<=@end
                                                    ) a  group by unitSeq,unitName,ID,`Name` HAVING count(*) > 0
                                                    UNION ALL
                                                    select unitSeq,unitName,ID,`Name`,0 count5,0 count4,0 count3,0 count2,count(*) count1 from 
                                                    (
                                                    select a.unitSeq,c.unitName,A.windowUser ID, b.`Name` from b_evaluate a 
                                                    LEFT JOIN t_user b on a.areaNo=b.areaNo and a.windowUser=b.ID
                                                    LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                    where " + strtype + @"=1 and a.handleTime>=@start and a.handleTime<=@end
                                                    ) a  group by unitSeq,unitName,ID,`Name` HAVING count(*) > 0
                                                    ) a 
                                                    GROUP BY a.unitSeq,a.unitName,a.ID,a.`Name`
                                                    ) a ", 
                          new DbParam[] 
                          { 
                              new DbParam("start", start),
                              new DbParam("end", start)
                          });
                db.Session.CommitTransaction();
                return obj;
            }
            catch
            {
                return null;
            }
        }
    }
}
