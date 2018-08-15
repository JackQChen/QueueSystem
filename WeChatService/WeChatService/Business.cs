using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using BLL;
using Model;

namespace WeChatService
{
    public class Business
    {
        //TWindowBLL wBll = new TWindowBLL();
        //TWindowAreaBLL waBll = new TWindowAreaBLL();
        //TBusinessAttributeBLL attBll = new TBusinessAttributeBLL();
        //TCallBLL cBll = new TCallBLL();
        //TQueueBLL qBll = new TQueueBLL();
        //TAppointmentBLL aBll = new TAppointmentBLL();
        //TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        //TOprateLogBLL oBll = new TOprateLogBLL();
        List<TBusinessAttributeModel> baList = new List<TBusinessAttributeModel>();
        List<TWindowBusinessModel> wbList = new List<TWindowBusinessModel>();
        List<TWindowModel> wList = new List<TWindowModel>();
        List<TWindowAreaModel> waList = new List<TWindowAreaModel>();
        JavaScriptSerializer script = new JavaScriptSerializer();

        public Business()
        {
            new Thread(() => { while (true) { GetAttribute(); Thread.Sleep(5 * 60 * 1000); } }) { IsBackground = true }.Start();
        }

        void GetAttribute()
        {
            try
            {
                baList = new TBusinessAttributeBLL().GetModelList();
                wbList = new TWindowBusinessBLL().GetModelList();
                waList = new TWindowAreaBLL().GetModelList();
                wList = new TWindowBLL().GetModelList();
            }
            catch (Exception ex)
            {
                WriterErrorLog("获取基础数据错误，请核查网络：" + ex.Message);
            }
        }

        //验证是否符合取票条件
        public object QueueCheck(Dictionary<string, object> json)
        {
            WriterReceiveLog("QueueCheck", script.Serialize(json));
            var idCard = json["idCard"].ToString();
            var unitSeq = json["unitSeq"].ToString();
            var busiSeq = json["busiSeq"].ToString();
            var arr = CheckLimit("QueueCheck", idCard, unitSeq, busiSeq);
            if (Convert.ToBoolean(arr[0]) == false)
            {
                return arr[1];
            }
            return new
            {
                method = "QueueCheck",
                code = 1,
                desc = "验证通过",
                idcard = idCard,
                result = new
                {
                    unitSeq = unitSeq,
                    busiSeq = busiSeq,
                    idcard = idCard,
                }
            };
        }

        //处理排队信息
        public object ProcessQueue(Dictionary<string, object> json)
        {
            var idCard = "";
            try
            {
                WriterReceiveLog("ProcessQueue", script.Serialize(json));
                var QueueInfo = json["QueueInfo"] as Dictionary<string, object>;
                var unitSeq = QueueInfo["unitSeq"].ToString();
                var unitName = QueueInfo["unitName"].ToString();
                var busiSeq = QueueInfo["busiSeq"].ToString();
                var busiName = QueueInfo["busiName"].ToString();
                var personName = QueueInfo["personName"].ToString();
                idCard = QueueInfo["idCard"].ToString();
                var wxId = QueueInfo["wxId"] == null ? "" : QueueInfo["wxId"].ToString();
                var arr = CheckLimit("ProcessQueue", idCard, unitSeq, busiSeq);
                if (Convert.ToBoolean(arr[0]) == false)
                {
                    return arr[1];
                }
                var obj = OutQueueNo(unitSeq, unitName, busiSeq, busiName, personName, idCard, wxId);
                return obj;
            }
            catch (Exception ex)
            {
                WriterErrorLog("处理排队数据出错：" + ex.Message);
                return new
                {
                    method = "ProcessQueue",
                    code = 0,
                    desc = "处理排队数据出错：" + ex.Message,
                    idcard = idCard,
                    result = new
                    {
                    }
                };
            }
        }

        //获取当前排队等候数据
        public object GetQueueInfo(Dictionary<string, object> json)
        {
            WriterReceiveLog("GetQueueInfo", script.Serialize(json));
            var id = Convert.ToInt32(json["id"]);
            return GetQueueById(id);
        }

        //推送提醒
        public object PushNotify(string Id)
        {
            var qBll = new BQueueBLL();
            var oId = Convert.ToInt32(Id);
            object obj = new object();
            var model = qBll.GetModel(oId);
            if (model == null)
            {
                return new
                {
                    method = "PushNotify",
                    code = 0,
                    desc = "无此编号的排队数据，排队已失效，请核查",
                    result = new
                    {
                    }
                };

            }
            else
            {
                if (model.state == 0)
                {
                    return new
                    {
                        method = "PushNotify",
                        code = 0,
                        desc = "该排队数据已失效，请核查",
                        result = new
                        {
                        }
                    };
                }
            }
            var list = qBll.GetModelList(model.busTypeSeq, model.unitSeq, 0);
            var cModel = new BCallBLL().GetModel(f => f.qId == oId && f.state != 2);
            var areaWindowStr = GetAreaWindowsStr(model.unitSeq, model.busTypeSeq);
            var waitNo = 1;
            //返回该条数据以及三条待叫号数据
            var objresult = new
            {
                method = "PushNotify",
                code = 1,
                desc = "处理成功",
                result = new
                {
                    currentQueue = new
                    {
                        state = "已叫号",
                        id = model.ID,
                        ticketNumber = model.ticketNumber,
                        windowName = cModel.windowNumber,
                        unitSeq = model.unitSeq,
                        unitName = model.unitName,
                        busySeq = model.busTypeSeq,
                        busyName = model.busTypeName,
                        reserveSeq = model.reserveSeq,
                        area = areaWindowStr[0],
                        windowStr = areaWindowStr[1],
                        cardId = model.idCard,
                        vip = GetVipLever(model),
                        wxId = model.wxId,
                    },
                    waitQueue = list.OrderBy(o => o.ID).Take(3).Select(s => new
                    {
                        id = s.ID,
                        area = areaWindowStr[0],
                        windowStr = areaWindowStr[1],
                        currentState = "排队中",
                        windowNo = "",
                        waitCount = waitNo++,
                        unitSeq = s.unitSeq,
                        unitName = s.unitName,
                        busySeq = s.busTypeSeq,
                        busyName = s.busTypeName,
                        ticketNumber = s.ticketNumber,
                        ticketTime = s.ticketTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        reserveSeq = s.reserveSeq,
                        cardId = s.idCard,
                        vip = GetVipLever(s),
                        wxId = s.wxId
                    }).ToList()
                }
            };
            return objresult;
        }

        //获取排队等候人数
        public object GetWaitInfo(Dictionary<string, object> json)
        {
            WriterReceiveLog("GetWaitInfo", script.Serialize(json));
            var unitSeq = json["unitSeq"].ToString();
            var busiSeq = json["busiSeq"].ToString();
            var list = new BQueueBLL().GetModelList(busiSeq, unitSeq, 0);
            return new
            {
                method = "GetWaitInfo",
                code = 1,
                desc = "处理成功",
                result = list.Select(s => new
                {
                    unitSeq = unitSeq,
                    busiSeq = busiSeq,
                    waitCount = list.Count,
                })
            };
        }

        //获取排队等候人数ByUnit
        public object GetWaitInfoByUnit(Dictionary<string, object> json)
        {
            WriterReceiveLog("GetWaitInfoByUnit", script.Serialize(json));
            var unitSeq = json["unitSeq"].ToString();
            var list = new BQueueBLL().GetModelList(unitSeq, 0);
            return new
            {
                method = "GetWaitInfoByUnit",
                code = 1,
                desc = "处理成功",
                result = list.Select(s => new
                {
                    unitSeq = unitSeq,
                    waitCount = list.Count,
                })
            };
        }

        //获取排队等候人数 按部门业务分组
        public object GetWaitInfoAll()
        {
            var list = new BQueueBLL().GetModelList(c => c.ticketTime.Date == DateTime.Now.Date && c.state == 0);
            var glist = list.GroupBy(g => g.unitSeq).ToList();
            var grlist = list.GroupBy(g => new { g.unitSeq, g.busTypeSeq }).ToList();
            return new
            {
                method = "GetWaitInfoAll",
                code = 1,
                desc = "处理成功",
                result = glist.Select(s => new
                {
                    unitSeq = s.Key,
                    waitCount = s.Count(),
                    unitBusi = grlist.Where(g => g.Key.unitSeq == s.Key).Select(k => new
                    {
                        unitSeq = k.Key.unitSeq,
                        busiSeq = k.Key.busTypeSeq,
                        waitCount = k.Count()
                    }).ToList()

                }).ToList()
            };
        }

        //取票时间段检测
        ArrayList GetLimitBySeq(string unitSeq, string busiSeq)
        {
            try
            {
                var busiAtt = baList.Where(b => b.unitSeq == unitSeq && b.busiSeq == busiSeq).FirstOrDefault();
                if (busiAtt == null)
                {
                    return null;
                }
                ArrayList arr = new ArrayList();
                string[] section = busiAtt.timeInterval.Split('|');
                string[] limits = busiAtt.ticketRestriction.Split(',');
                int index = 0;
                foreach (var part in section)
                {
                    string[] ts = part.Split(',');
                    if (ts.Length > 1)
                    {
                        DateTime start = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + ts[0]);
                        DateTime end = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + ts[1]);
                        if (DateTime.Now > start && DateTime.Now < end)
                        {
                            arr.Add(Convert.ToInt32(limits[index]));
                            arr.Add(start);
                            arr.Add(end);
                            return arr;
                        }
                    }
                    index++;
                }
            }
            catch (Exception ex)
            {
                WriterErrorLog("检测票数限制出错：" + ex.Message);
            }
            return null;
        }

        //1.身份证验证
        //2.取号时间段验证
        //3.取号数量验证
        ArrayList CheckLimit(string methodName, string idCard, string unitSeq, string busiSeq)
        {
            var qBll = new BQueueBLL();
            ArrayList arry = new ArrayList();
            arry.Add(true);
            //验证同一个身份证不能在一个部门一个业务排队2次（未处理的）
            var isCan = qBll.IsCanQueueO(idCard, busiSeq, unitSeq);
            if (Convert.ToBoolean(isCan[0]) == false)
            {
                var err = new
                {
                    method = methodName,
                    code = 0,
                    desc = "此身份证同类业务未办理，请办理完成后再次取号",
                    idcard = idCard,
                    result = new
                    {
                    }
                };
                arry.Clear();
                arry.Add(false);
                arry.Add(err);
                return arry;
            }

            ArrayList arr = GetLimitBySeq(unitSeq, busiSeq);
            if (arr == null)
            {
                var err = new
                {
                    method = methodName,
                    code = 0,
                    desc = "该业务类型当前时间段不能取号或未配置取号限制条件",
                    idcard = idCard,
                    result = new
                    {
                    }
                };
                arry.Clear();
                arry.Add(false);
                arry.Add(err);
                return arry;
            }
            int max = Convert.ToInt32(arr[0]);
            DateTime start = Convert.ToDateTime(arr[1]);
            DateTime end = Convert.ToDateTime(arr[2]);
            var mList = qBll.GetModelList(busiSeq, unitSeq, start, end);
            if (max <= mList.Count)
            {
                var err = new
                {
                    method = methodName,
                    code = 0,
                    desc = "该业务类型当前时间段排队数量已达上限",
                    idcard = idCard,
                    result = new
                    {
                    }
                };
                arry.Clear();
                arry.Add(false);
                arry.Add(err);
                return arry;
            }
            return arry;
        }

        //获取业务所属区域以及窗口
        string[] GetAreaWindowsStr(string unitSeq, string busTypeSeq)
        {
            var area = "";
            var windowStr = "";
            var windowList = wbList.Where(w => w.unitSeq == unitSeq && w.busiSeq == busTypeSeq);
            if (windowList != null)
            {
                foreach (var win in windowList)
                {
                    var window = wList.Where(w => w.ID == win.WindowID).FirstOrDefault();
                    if (window != null)
                    {
                        if (area == "")
                        {
                            var windoware = waList.Where(w => w.ID == window.AreaName).FirstOrDefault();
                            if (windoware != null)
                            {
                                area = windoware.areaName;
                            }
                        }
                        windowStr += (window.Name + "、");
                    }
                }
                if (windowStr.Length > 0)
                    windowStr = windowStr.Substring(0, windowStr.Length - 1);
            }
            return new string[] { area, windowStr };
        }

        //获取业务属性类型
        string GetVipLever(BQueueModel model)
        {
            var isGreen = "";
            var att = baList.Where(b => b.unitSeq == model.unitSeq && b.busiSeq == model.busTypeSeq).FirstOrDefault();
            if (att != null)
            {
                isGreen = att.isGreenChannel == 1 ? "绿色通道" : "";
            }
            if (model.appType == 1 && model.type == 0 && model.reserveEndTime >= DateTime.Now && isGreen == "")
                isGreen = "网上预约";
            else if (model.type == 1 && isGreen == "")
                isGreen = "网上申办";
            return isGreen;
        }

        //进行排队
        private object OutQueueNo(string unitSeq, string unitName, string busiSeq, string busiName, string personName, string idCard, string wxId)
        {
            #region 验证业务扩展属性
            var qBll = new BQueueBLL();
            var ticketStart = "";
            var att = baList.Where(b => b.unitSeq == unitSeq && b.busiSeq == busiSeq).FirstOrDefault();
            var list = qBll.GetModelList(busiSeq, unitSeq, 0);
            int waitNo = list.Count;//计算等候人数
            if (att != null)
            {
                ticketStart = att.ticketPrefix;
            }
            else
            {
                return new
                {
                    method = "ProcessQueue",
                    code = 0,
                    desc = "当前部门以及业务类型未获取到扩展属性，无法排队",
                    idcard = idCard,
                    result = new
                    {
                    }
                };
            }
            #endregion

            #region 排队
            var queue = qBll.QueueLine(unitSeq, unitName, busiSeq, busiName, ticketStart, idCard, personName, wxId);
            #endregion

            #region 日志相关
            string strLog = string.Format("已出票：部门[{0}]，业务[{1}]，票号[{2}]，预约号[{3}]，身份证号[{4}]，姓名[{5}]，时间[{6}]。",
                queue.unitName, queue.busTypeName, queue.ticketNumber, queue.reserveSeq, idCard, personName, DateTime.Now);
            WriterQueueLog(strLog);
            new TOprateLogBLL().Insert(new TOprateLogModel()
            {
                oprateFlag = wxId,
                oprateType = "微信端排队",
                oprateClassifyType = "出票",
                oprateTime = DateTime.Now,
                oprateLog = strLog
            });
            #endregion

            #region  返回数据组织
            var areaWindowStr = GetAreaWindowsStr(unitSeq, busiSeq);
            var isGreen = GetVipLever(queue);
            object obj = new
            {
                method = "ProcessQueue",
                code = 1,
                desc = "处理成功",
                idcard = idCard,
                result = new
                {
                    id = queue.ID,
                    area = areaWindowStr[0],
                    windowStr = areaWindowStr[1],
                    waitCount = waitNo,
                    unitSeq = queue.unitSeq,
                    unitName = queue.unitName,
                    busySeq = queue.busTypeSeq,
                    busyName = queue.busTypeName,
                    ticketNumber = queue.ticketNumber,
                    ticketTime = queue.ticketTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    cardId = queue.idCard,
                    reserveSeq = queue.reserveSeq,
                    vip = isGreen,
                }
            };
            return obj;
            #endregion
        }

        //组织票数据 * 暂时不用了
        private DataTable GetQueue(BQueueModel model, string area, string windowStr, int wait, string vip)
        {
            DataTable table = new DataTable("table");
            table.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn ("id",typeof(string)),
                new DataColumn ("area",typeof(string)),
                new DataColumn ("windowStr",typeof(string)),
                new DataColumn ("waitCount",typeof(string)),
                new DataColumn ("unitSeq",typeof(string)),
                new DataColumn ("busySeq",typeof(string)),
                new DataColumn ("unitName",typeof(string)),
                new DataColumn ("busyName",typeof(string)),
                new DataColumn ("ticketNumber",typeof(string)),
                new DataColumn ("cardId",typeof(string)),
                new DataColumn ("reserveSeq",typeof(string)),
                new DataColumn ("vip",typeof(string)),
            });
            DataRow row = table.NewRow();
            row["id"] = model.ID;
            row["area"] = area;
            row["windowStr"] = windowStr;
            row["waitCount"] = wait.ToString();
            row["unitSeq"] = model.unitSeq;
            row["busySeq"] = model.busTypeSeq;
            row["unitName"] = model.unitName;
            row["busyName"] = model.busTypeName;
            row["ticketNumber"] = model.ticketNumber;
            row["cardId"] = string.IsNullOrEmpty(model.idCard) ? "" : model.idCard.Length > 6 ? model.idCard.Substring(model.idCard.Length - 6, 6) : model.idCard;
            row["reserveSeq"] = model.reserveSeq;
            row["vip"] = vip;
            table.Rows.Add(row);
            return table;
        }

        //写排队日志
        public static void WriterQueueLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\QueueLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " : " + logString);
            }
        }

        //写收数据日志
        public static void WriterReceiveLog(string method, string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\ReciveLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(string.Format("{0} : 方法【{1}】值【{2}】", DateTime.Now.ToString(), method, logString));
            }
        }

        //写错误日志
        public static void WriterErrorLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\ErrorLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " : " + logString);
            }
        }

        //获取当前排队信息
        private object GetQueueById(int Id)
        {
            var qBll = new BQueueBLL();
            object obj = new object();
            var model = qBll.GetModel(Id);
            if (model == null)
            {
                return new
                {
                    method = "GetQueueInfo",
                    code = 0,
                    desc = "无此编号的排队数据",
                    result = new
                    {
                    }
                };

            }
            var areaWindowStr = GetAreaWindowsStr(model.unitSeq, model.busTypeSeq);
            var isGreen = GetVipLever(model);
            var list = qBll.GetModelList(model.busTypeSeq, model.unitSeq, 0).Where(q => q.ID < model.ID).ToList();
            int waitNo = list.Count;//计算等候人数
            if (model.state == 1)
            {
                //已叫号/已处理
                var call = new BCallBLL().GetModel(f => f.qId == Id && f.state != 2);
                if (call == null)
                {
                    return new
                    {
                        method = "GetQueueInfo",
                        code = 0,
                        desc = "无此编号的叫号数据",
                        result = new
                        {
                        }
                    };
                }
                else
                {
                    var currentState = call.ticketTime.Date != DateTime.Now.Date ? "已过期" : (call.state == -1 || call.state == 1) ? "已完成" : "已叫号";
                    obj = new
                    {
                        method = "GetQueueInfo",
                        code = 1,
                        desc = "处理成功",
                        result = new
                        {
                            id = model.ID,
                            area = areaWindowStr[0],
                            windowStr = areaWindowStr[1],
                            currentState = currentState,
                            windowNo = "",
                            waitCount = waitNo,
                            unitSeq = model.unitSeq,
                            unitName = model.unitName,
                            busySeq = model.busTypeSeq,
                            busyName = model.busTypeName,
                            ticketNumber = model.ticketNumber,
                            ticketTime = model.ticketTime.ToString("yyyy-MM-dd HH:mm:ss"),
                            reserveSeq = model.reserveSeq,
                            cardId = model.idCard,
                            vip = isGreen,
                            wxId = model.wxId
                        }
                    };
                }
            }
            else
            {
                //排队中
                obj = new
                {
                    method = "GetQueueInfo",
                    code = 1,
                    desc = "处理成功",
                    result = new
                    {
                        id = model.ID,
                        area = areaWindowStr[0],
                        windowStr = areaWindowStr[1],
                        currentState = "排队中",
                        windowNo = "",
                        waitCount = waitNo,
                        unitSeq = model.unitSeq,
                        unitName = model.unitName,
                        busySeq = model.busTypeSeq,
                        busyName = model.busTypeName,
                        ticketNumber = model.ticketNumber,
                        ticketTime = model.ticketTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        reserveSeq = model.reserveSeq,
                        cardId = model.idCard,
                        vip = isGreen,
                        wxId = model.wxId
                    }
                };
            }
            return obj;
        }
    }
}
