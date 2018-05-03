using System;
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
        TWindowBLL wBll = new TWindowBLL();
        TWindowAreaBLL waBll = new TWindowAreaBLL();
        TBusinessAttributeBLL attBll = new TBusinessAttributeBLL();
        TCallBLL cBll = new TCallBLL();
        TQueueBLL qBll = new TQueueBLL();
        TAppointmentBLL aBll = new TAppointmentBLL();
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        List<TBusinessAttributeModel> baList = new List<TBusinessAttributeModel>();
        List<TWindowBusinessModel> wbList = new List<TWindowBusinessModel>();
        List<TWindowModel> wList = new List<TWindowModel>();
        List<TWindowAreaModel> waList = new List<TWindowAreaModel>();
        TOprateLogBLL oBll = new TOprateLogBLL();
        JavaScriptSerializer script = new JavaScriptSerializer();

        public Business()
        {
            new Thread(() => { while (true) { GetAttribute(); Thread.Sleep(5 * 60 * 1000); } }) { IsBackground = true }.Start();
        }

        void GetAttribute()
        {
            baList = attBll.GetModelList();
            wbList = wbBll.GetModelList();
            waList = waBll.GetModelList();
            wList = wBll.GetModelList();
        }

        public object ProcessQueue(Dictionary<string, object> json)
        {
            try
            {
                WriterReciveLog("ProcessQueue", script.Serialize(json));
                var QueueInfo = json["QueueInfo"] as Dictionary<string, object>;
                var unitSeq = QueueInfo["unitSeq"].ToString();
                var unitName = QueueInfo["unitName"].ToString();
                var busiSeq = QueueInfo["busiSeq"].ToString();
                var busiName = QueueInfo["busiName"].ToString();
                var personName = QueueInfo["personName"].ToString();
                var idCard = QueueInfo["idCard"].ToString();
                var wxId = QueueInfo["wxId"] == null ? "" : QueueInfo["wxId"].ToString();
                var Appointment = script.Deserialize<TAppointmentModel>(script.Serialize(json["Appointment"]));
                var obj = OutQueueNo(Appointment, unitSeq, unitName, busiSeq, busiName, personName, idCard, wxId);
                return obj;
            }
            catch (Exception ex)
            {
                WriterErrorLog("处理排队数据出错：" + ex.Message);
                return new
                {
                    code = 0,
                    desc = "处理排队数据出错：" + ex.Message,
                    result = new
                    {
                    }
                };
            }
        }

        public object GetQueueInfo(Dictionary<string, object> json)
        {
            var id = Convert.ToInt32(json["id"]);
            return GetQueueById(id);
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
                            var windoware = waList.Where(w => w.id == window.AreaName).FirstOrDefault();
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
        string GetVipLever(TQueueModel model)
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
        private object OutQueueNo(TAppointmentModel app, string unitSeq, string unitName, string busiSeq, string busiName, string personName, string idCard, string wxId)
        {
            #region 验证业务扩展属性
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
                    code = 0,
                    desc = "当前部门以及业务类型未获取到扩展属性，无法排队",
                    result = new
                    {
                    }
                };
            }
            #endregion

            #region 排队
            var queue = qBll.QueueLine(unitSeq, unitName, busiSeq, busiName, ticketStart, idCard, personName, app);
            if (app != null)
            {
                app.sysFlag = 0;
                aBll.Insert(app);
            }
            #endregion

            #region 日志相关
            string strLog = string.Format("已出票：部门[{0}]，业务[{1}]，票号[{2}]，预约号[{3}]，身份证号[{4}]，姓名[{5}]，时间[{6}]。",
                queue.unitName, queue.busTypeName, queue.ticketNumber, queue.reserveSeq, idCard, personName, DateTime.Now);
            WriterQueueLog(strLog);
            oBll.Insert(new TOprateLogModel()
            {
                oprateFlag = wxId,
                oprateType = "微信端排队",
                oprateClassifyType = "出票",
                oprateTime = DateTime.Now,
                oprateLog = strLog,
                sysFlag = 0
            });
            #endregion

            #region  返回数据组织
            var areaWindowStr = GetAreaWindowsStr(unitSeq, busiSeq);
            var isGreen = GetVipLever(queue);
            object obj = new
            {
                code = 1,
                desc = "处理成功",
                result = new
                {
                    id = queue.id,
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
        private DataTable GetQueue(TQueueModel model, string area, string windowStr, int wait, string vip)
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
            row["id"] = model.id;
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
        public static void WriterReciveLog(string method, string logString)
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
            object obj = new object();
            var model = qBll.GetModel(Id);
            if (model == null)
            {
                return new
                {
                    code = 0,
                    desc = "无此编号的排队数据",
                    result = new
                    {
                    }
                };

            }
            var areaWindowStr = GetAreaWindowsStr(model.unitSeq, model.busTypeSeq);
            var isGreen = GetVipLever(model);
            var list = qBll.GetModelList(model.busTypeSeq, model.unitSeq, 0);
            int waitNo = list.Count - 1;//计算等候人数
            if (model.state == 1)
            {
                //已叫号/已处理
                var call = cBll.GetModel(f => f.qId == Id && f.state != 2);
                if (call == null)
                {
                    return new
                    {
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
                        code = 1,
                        desc = "处理成功",
                        result = new
                        {
                            id = model.id,
                            area = areaWindowStr[0],
                            windowStr = areaWindowStr[2],
                            currentState = currentState,
                            windowNo = "",
                            waitCount = waitNo,
                            unitSeq = model.unitSeq,
                            unitName = model.unitName,
                            busySeq = model.busTypeSeq,
                            busyName = model.busTypeName,
                            ticketNumber = model.ticketNumber,
                            ticketTime = model.ticketTime,
                            reserveSeq = model.reserveSeq,
                            cardId = model.idCard,
                            vip = isGreen,

                        }
                    };
                }
            }
            else
            {
                //排队中
                obj = new
                {
                    code = 1,
                    desc = "处理成功",
                    result = new
                    {
                        id = model.id,
                        area = areaWindowStr[0],
                        windowStr = areaWindowStr[2],
                        currentState = "排队中",
                        windowNo = "",
                        waitCount = waitNo,
                        unitSeq = model.unitSeq,
                        unitName = model.unitName,
                        busySeq = model.busTypeSeq,
                        busyName = model.busTypeName,
                        ticketNumber = model.ticketNumber,
                        ticketTime = model.ticketTime,
                        reserveSeq = model.reserveSeq,
                        cardId = model.idCard,
                        vip = isGreen,

                    }
                };
            }
            return obj;
        }
    }
}
