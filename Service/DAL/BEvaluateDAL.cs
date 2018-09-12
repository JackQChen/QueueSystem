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
        public object GetSatisfied(int type, DateTime start, DateTime end)
        {
            ArrayList list = new ArrayList();
            try
            {
                var strtype = type == 1 ? "a.evaluateAttitude" : type == 2 ? "a.evaluateQuality" : type == 3 ? "a.evaluateEfficiency" : "a.evaluateHonest";
                using (var reader = db.Session.ExecuteReader(@"select a.unitSeq,a.unitName,a.ID,a.`Name`,count5+count4+count3+count2+count1 count,count5,count4,count3,count2,count1,
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
                              new DbParam("end", end)
                          }))
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            unitSeq = reader["unitSeq"],
                            unitName = reader["unitName"],
                            ID = reader["ID"],
                            Name = reader["Name"],
                            count = reader["count"],
                            count5 = reader["count5"],
                            count4 = reader["count4"],
                            count3 = reader["count3"],
                            count2 = reader["count2"],
                            count1 = reader["count1"],
                            satisfied = Convert.ToDecimal(reader["satisfied"]).ToString("0.00") + "%"
                        });
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
            finally
            {
                db.Session.Dispose();
            }

        }

        /// <summary>
        /// 获取好评率与量化分数
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public object GetFavorableComment(DateTime start, DateTime end)
        {
            ArrayList list = new ArrayList();
            try
            {
                using (var reader = db.Session.ExecuteReader(@"select unitSeq,unitName,sum(ahcount)*100/sum(atcount) AttitudePercent,sum(qhcount)*100/sum(qtcount) QualityPercent, 
                                                              sum(ehcount)*100/sum(etcount) EfficiencyPercent, sum(hhcount)*100/sum(htcount) HonestPercent from 
                                                              (
                                                              select a.unitSeq,c.unitName,count(evaluateAttitude) atcount,0 ahcount,0 qtcount,0 qhcount,0 etcount,0 ehcount, 0 htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,count(evaluateAttitude) ahcount,0 qtcount,0 qhcount,0 etcount,0 ehcount, 0 htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              and evaluateAttitude>3
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,0 ahcount,count(evaluateQuality) qtcount,0 qhcount,0 etcount,0 ehcount, 0 htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,0 ahcount,0 qtcount,count(evaluateQuality) qhcount ,0 etcount,0 ehcount, 0 htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              and evaluateQuality>3
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,0 ahcount,0 qtcount,0 qhcount,count(evaluateEfficiency) etcount,0 ehcount, 0 htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,0 ahcount,0 qtcount,0 qhcount,0 etcount,count(evaluateEfficiency) ehcount, 0 htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              and evaluateEfficiency>3
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,0 ahcount,0 qtcount,0 qhcount,0 etcount,0 ehcount, count(evaluateHonest) htcount,0 hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              group by unitSeq,unitName 
                                                              union ALL
                                                              select a.unitSeq,c.unitName,0 atcount,0 ahcount,0 qtcount,0 qhcount,0 etcount,0 ehcount, 0 htcount,count(evaluateHonest) hhcount from b_evaluate a 
                                                              LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                              where a.handleTime>=@start and a.handleTime<=@end
                                                              and evaluateHonest>3
                                                              group by unitSeq,unitName 
                                                              ) a  group by unitSeq,unitName ",
                   new DbParam[] 
                          { 
                              new DbParam("start", start),
                              new DbParam("end", end)
                          }))
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            unitSeq = reader["unitSeq"],
                            unitName = reader["unitName"],
                            AttitudePercent = Convert.ToDecimal(reader["AttitudePercent"]).ToString("0.00") + "%",
                            QualityPercent = Convert.ToDecimal(reader["QualityPercent"]).ToString("0.00") + "%",
                            EfficiencyPercent = Convert.ToDecimal(reader["EfficiencyPercent"]).ToString("0.00") + "%",
                            HonestPercent = Convert.ToDecimal(reader["HonestPercent"]).ToString("0.00") + "%"
                        });
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
            finally
            {
                db.Session.Dispose();
            }
        }

        /// <summary>
        /// 获取评价数据以及评价率
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public object GetEvaluate(DateTime start, DateTime end)
        {
            ArrayList list = new ArrayList();
            try
            {
                using (var reader = db.Session.ExecuteReader(@"select a.unitSeq,a.unitName,a.ID,a.`Name` ,
                                                               CONVERT((sum(a1)/1),SIGNED) a1,CONVERT((sum(a2)/2),SIGNED) a2,CONVERT((sum(a3)/3),SIGNED) a3,CONVERT((sum(a4)/4),SIGNED) a4,CONVERT((sum(a5)/5),SIGNED) a5,
                                                               CONVERT((sum(q1)/1),SIGNED) q1,CONVERT((sum(q2)/2),SIGNED) q2,CONVERT((sum(q3)/3),SIGNED) q3,CONVERT((sum(q4)/4),SIGNED) q4,CONVERT((sum(q5)/5),SIGNED) q5,
                                                               CONVERT((sum(e1)/1),SIGNED) e1,CONVERT((sum(e2)/2),SIGNED) e2,CONVERT((sum(e3)/3),SIGNED) e3,CONVERT((sum(e4)/4),SIGNED) e4,CONVERT((sum(e5)/5),SIGNED) e5,
                                                               CONVERT((sum(h1)/1),SIGNED) h1,CONVERT((sum(h2)/2),SIGNED) h2,CONVERT((sum(h3)/3),SIGNED) h3,CONVERT((sum(h4)/4),SIGNED) h4,CONVERT((sum(h5)/5),SIGNED) h5,
                                                               100 evaluate 
                                                               FROM(
                                                               select 
                                                               case when a.evaluateAttitude=1 then evaluateAttitude ELSe 0 end a1 ,
                                                               case when a.evaluateAttitude=2 then evaluateAttitude ELSe 0 end a2 ,
                                                               case when a.evaluateAttitude=3 then evaluateAttitude ELSe 0 end a3 ,
                                                               case when a.evaluateAttitude=4 then evaluateAttitude ELSe 0 end a4 ,
                                                               case when a.evaluateAttitude=5 then evaluateAttitude ELSe 0 end a5 ,
                                                               case when a.evaluateQuality=1 then evaluateQuality ELSe 0 end q1 ,
                                                               case when a.evaluateQuality=2 then evaluateQuality ELSe 0 end q2 ,
                                                               case when a.evaluateQuality=3 then evaluateQuality ELSe 0 end q3 ,
                                                               case when a.evaluateQuality=4 then evaluateQuality ELSe 0 end q4 ,
                                                               case when a.evaluateQuality=5 then evaluateQuality ELSe 0 end q5 ,
                                                               case when a.evaluateEfficiency=1 then evaluateEfficiency ELSe 0 end e1 ,
                                                               case when a.evaluateEfficiency=2 then evaluateEfficiency ELSe 0 end e2 ,
                                                               case when a.evaluateEfficiency=3 then evaluateEfficiency ELSe 0 end e3 ,
                                                               case when a.evaluateEfficiency=4 then evaluateEfficiency ELSe 0 end e4 ,
                                                               case when a.evaluateEfficiency=5 then evaluateEfficiency ELSe 0 end e5 ,
                                                               case when a.evaluateHonest=1 then evaluateHonest ELSe 0 end h1 ,
                                                               case when a.evaluateHonest=2 then evaluateHonest ELSe 0 end h2 ,
                                                               case when a.evaluateHonest=3 then evaluateHonest ELSe 0 end h3 ,
                                                               case when a.evaluateHonest=4 then evaluateHonest ELSe 0 end h4 ,
                                                               case when a.evaluateHonest=5 then evaluateHonest ELSe 0 end h5 ,
                                                               a.unitSeq,c.unitName,A.windowUser ID, b.`Name` from b_evaluate a
                                                               LEFT JOIN t_user b on a.areaNo=b.areaNo and a.windowUser=b.ID
                                                               LEFT JOIN t_unit c on a.areaNo=c.areaNo and a.unitSeq=c.unitSeq
                                                               where a.handleTime>=@start and a.handleTime<=@end 
                                                                ) a group by a.unitSeq,a.unitName,a.ID,a.`Name`",
                   new DbParam[] 
                          { 
                              new DbParam("start", start),
                              new DbParam("end", end)
                          }))
                {
                    while (reader.Read())
                    {
                        list.Add(new
                        {
                            unitSeq = reader["unitSeq"],
                            unitName = reader["unitName"],
                            ID = reader["ID"],
                            Name = reader["Name"],
                            a1 = reader["a1"],
                            a2 = reader["a2"],
                            a3 = reader["a3"],
                            a4 = reader["a4"],
                            a5 = reader["a5"],
                            q1 = reader["q1"],
                            q2 = reader["q2"],
                            q3 = reader["q3"],
                            q4 = reader["q4"],
                            q5 = reader["q5"],
                            e1 = reader["e1"],
                            e2 = reader["e2"],
                            e3 = reader["e3"],
                            e4 = reader["e4"],
                            e5 = reader["e5"],
                            h1 = reader["h1"],
                            h2 = reader["h2"],
                            h3 = reader["h3"],
                            h4 = reader["h4"],
                            h5 = reader["h5"],
                            evaluate = reader["evaluate"],
                        });
                    }
                }
                return list;
            }
            catch
            {
                return null;
            }
            finally
            {
                db.Session.Dispose();
            }

        }
    }
}
