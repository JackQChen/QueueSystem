using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using BLL;
using Model;
using ReportManager;
using System.Configuration;
using System.Collections;
using System.Printing;

namespace QueueClient
{
    public partial class frmMain3 : Form
    {
        #region
        string TimeInterval = "";
        string NumberRestriction = "";
        string ClientName = "";
        string areaSeq = System.Configuration.ConfigurationManager.AppSettings["areaSeq"];
        string GetAppointmentByID = System.Configuration.ConfigurationManager.AppSettings["GetAppointmentByID"];
        string GetWorkByID = System.Configuration.ConfigurationManager.AppSettings["GetWorkByID"];//申办
        string GetUserByID = System.Configuration.ConfigurationManager.AppSettings["GetUserByID"];//暂停使用
        string CheckUser = System.Configuration.ConfigurationManager.AppSettings["CheckUser"];
        string GetUnit = System.Configuration.ConfigurationManager.AppSettings["GetUnit"];
        string GetUnitBusiness = System.Configuration.ConfigurationManager.AppSettings["GetUnitBusiness"];
        string GetBusiness = System.Configuration.ConfigurationManager.AppSettings["GetBusiness"];
        string RegisterUser = System.Configuration.ConfigurationManager.AppSettings["RegisterUser"]; //暂停使用
        string UpdateAppoint = "";// System.Configuration.ConfigurationManager.AppSettings["UpdateAppoint"];
        string GetCard = "";
        string AppointmentOnline = System.Configuration.ConfigurationManager.AppSettings["AppointmentOnline"];
        string ExitPwd = System.Configuration.ConfigurationManager.AppSettings["ExitPwd"];
        string GetEvaluate = System.Configuration.ConfigurationManager.AppSettings["GetEvaluate"];
        string SaveEvaluate = System.Configuration.ConfigurationManager.AppSettings["SaveEvaluate"];
        string GetUserByUserCode = System.Configuration.ConfigurationManager.AppSettings["GetUserByUserCode"];
        string GetAppointmentByReserveSeq = System.Configuration.ConfigurationManager.AppSettings["GetAppointmentByReserveSeq"];
        string ExitTime = System.Configuration.ConfigurationManager.AppSettings["ExitTime"];
        string EvaluateTime = System.Configuration.ConfigurationManager.AppSettings["Evaluate"];
        string AppointTime = System.Configuration.ConfigurationManager.AppSettings["Appoint"];
        string BusyTime = System.Configuration.ConfigurationManager.AppSettings["Busy"];
        string UnitTime = System.Configuration.ConfigurationManager.AppSettings["Unit"];
        string ReadcardTime = System.Configuration.ConfigurationManager.AppSettings["Readcard"];
        string CardTime = System.Configuration.ConfigurationManager.AppSettings["Card"];
        string BidUrl1 = "";//申办
        string BidUrl2 = "";//申办
        string InvestmentUnit = "";//投资部门
        string InvestmentBusy = "";//投资业务
        string GetAppointmentLimit = "";//获取预约数
        string FilterUnitStr = "";//办事业务下过滤出证业务类型
        #endregion

        #region
        JavaScriptSerializer script = new JavaScriptSerializer();
        HttpHelper http = new HttpHelper();
        PrintHelper print = new PrintHelper();
        TEvaluateBLL eBll = new TEvaluateBLL();
        TRegisterBLL rBll = new TRegisterBLL();
        TGetCardBLL gBll = new TGetCardBLL();
        TQueueBLL qBll = new TQueueBLL();
        TWindowBLL wBll = new TWindowBLL();
        TWindowAreaBLL waBll = new TWindowAreaBLL();
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        TLineUpMaxNoBLL lineBll = new TLineUpMaxNoBLL();
        TOprateLogBLL oBll = new TOprateLogBLL();
        TBusinessAttributeBLL baBll = new TBusinessAttributeBLL();
        TAppointmentBLL aBll = new TAppointmentBLL();
        TUnitBLL uBll = new TUnitBLL();
        List<TAppointmentModel> appList = new List<TAppointmentModel>();
        List<TEvaluateModel> eList = new List<TEvaluateModel>();
        TBusinessBLL bBll = new TBusinessBLL();
        List<TWindowAreaModel> waList = new List<TWindowAreaModel>();
        List<TWindowModel> wList = new List<TWindowModel>();
        List<TWindowBusinessModel> wbList = new List<TWindowBusinessModel>();
        List<TUnitModel> uList = new List<TUnitModel>();//部门列表
        List<TBusinessModel> bList = new List<TBusinessModel>();//业务列表
        List<TBusinessAttributeModel> baList = new List<TBusinessAttributeModel>();
        AutoResetEvent are = new AutoResetEvent(false);
        PageLocation pageLocation;
        BusyType busyType;
        Person person;
        bool suppend = true;
        int pageStopTime = 100;
        int iRetUSB = 0;
        string idCard = "";//身份证号码
        string userCode = "";//
        Thread thread;
        Thread exit;
        Thread wait;
        TUnitModel selectUnit;
        TBusinessModel selectBusy;
        TAppointmentModel selectAppoomt;
        string appPhone = "X";
        string appName = "X";
        Dictionary<string, Control> uc = new Dictionary<string, Control>();
        Dictionary<string, int> ucTimer = new Dictionary<string, int>();
        #endregion

        #region 构造函数，初始化，读卡

        public static void SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] == null)
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0014) // 禁掉清除背景消息
                return;
            base.WndProc(ref m);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams paras = base.CreateParams;
                paras.ExStyle |= 0x02000000;
                return paras;
            }
        }

        public frmMain3()
        {
            this.SetStyle(
                ControlStyles.AllPaintingInWmPaint | //不擦除背景 ,减少闪烁
                ControlStyles.OptimizedDoubleBuffer | //双缓冲
                ControlStyles.UserPaint, //使用自定义的重绘事件,减少闪烁
                true);
            InitializeComponent();
            busyType = BusyType.Default;

            ucpnMain main = new ucpnMain();
            main.Size = new Size(1920, 1080);
            main.Location = new Point(0, 0);
            ucpnReadCard readCard = new ucpnReadCard();
            readCard.Size = new Size(1920, 1080);
            readCard.Location = new Point(0, 0);
            ucpnCard card = new ucpnCard();
            card.Size = new Size(1920, 1080);
            card.Location = new Point(0, 0);
            ucpnSelectUnit unit = new ucpnSelectUnit();
            unit.Size = new Size(1920, 1080);
            unit.Location = new Point(0, 0);
            ucpnSelectBusy busy = new ucpnSelectBusy();
            busy.Size = new Size(1920, 1080);
            busy.Location = new Point(0, 0);
            ucpnAppointment appoint = new ucpnAppointment();
            appoint.Size = new Size(1920, 1080);
            appoint.Location = new Point(0, 0);
            ucpnEvaluate evaluate = new ucpnEvaluate();
            evaluate.Size = new Size(1920, 1080);
            evaluate.Location = new Point(0, 0);
            ucpnPwd pwd = new ucpnPwd();
            pwd.Size = new Size(1920, 1080);
            pwd.Location = new Point(0, 0);

            main.Work += new Action(Work);
            main.GetCard += new Action(GetCardAction);
            //main.Consult += new Action(Consult);
            main.Investment += new Action(Investment);
            main.Evaluate += new Action(Evaluate);
            main.UserGuide += new Action(UserGuide);

            readCard.GotoInput += new Action(GotoInput);
            card.clickAction += cardClick;
            card.ProcessIdCard += new Action<string>(ProcessIdCard);
            unit.SelectBusy += new Action(unit_SelectedUnit);
            busy.SelectedBusy += new Action(busy_SelectedBusy);

            appoint.other += new Action(appoint_other);
            appoint.enter += new Action(appoint_enter);
            appoint.previous += new Action<object>(appoint_previous);
            appoint.next += new Action<object>(appoint_previous);

            evaluate.enterEvaluate += new Action<TEvaluateModel>(evaluate_enterEvaluate);
            //evaluate.enter += new Action(evaluate_enter);
            evaluate.previous += new Action<object>(evaluate_previous);
            evaluate.next += new Action<object>(evaluate_previous);

            unit.previous += new Action<object>(unit_previous);
            unit.next += new Action<object>(unit_previous);
            busy.previous += new Action<object>(busy_previous);
            pwd.Exit += new Action(pwd_Exit);

            pnMain.Controls.Add(pwd);
            pnMain.Controls.Add(evaluate);
            pnMain.Controls.Add(appoint);
            pnMain.Controls.Add(busy);
            pnMain.Controls.Add(unit);
            pnMain.Controls.Add(card);
            pnMain.Controls.Add(readCard);
            pnMain.Controls.Add(main);

            uc.Add("pwd", pwd);
            uc.Add("evaluate", evaluate);
            uc.Add("appoint", appoint);
            uc.Add("busy", busy);
            uc.Add("unit", unit);
            uc.Add("readcard", readCard);
            uc.Add("card", card);
            uc.Add("main", main);

            //页面停留时间 单位：秒
            ucTimer.Add("pwd", Convert.ToInt32(ExitTime));
            ucTimer.Add("evaluate", Convert.ToInt32(EvaluateTime));
            ucTimer.Add("appoint", Convert.ToInt32(AppointTime));
            ucTimer.Add("busy", Convert.ToInt32(BusyTime));
            ucTimer.Add("unit", Convert.ToInt32(UnitTime));
            ucTimer.Add("readcard", Convert.ToInt32(ReadcardTime));
            ucTimer.Add("card", Convert.ToInt32(CardTime));

            uc["main"].BringToFront();

            try
            {
                Image img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "img\\title.png");
                if (img != null)
                {
                    pntitle.BackgroundImage = img;
                }
            }
            catch
            {

            }
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            SetConfigValue("NumberRestriction", "200,200");
            SetConfigValue("ClientName", "1号取票机");
            SetConfigValue("BidUrl1", "http://19.136.14.62/CommonService/api/control/controlInfoList/query.v?custCardId=@paperCode&approveStatus=-2");
            SetConfigValue("BidUrl2", "http://19.136.14.62/CommonService/api/reserve/reserveTreeList/query.v?approveItem=@approveItem");
            SetConfigValue("InvestmentUnit", "http://19.136.14.62/CommonService/api/reserve/unitTreeList/query.v?pageRowNum=1000&areaCode=23");
            SetConfigValue("InvestmentBusy", "http://19.136.14.62/CommonService/api/reserve/reserveTypeList/query.v?pageRowNum=1000&unitSeq=@unitSeq");
            SetConfigValue("GetAppLimit", "http://19.136.14.62/CommonService/api/reserve/reserveInfoList/query.v?pageRowNum=1000&reserveDate=@currentDate&unitSeq=@unitSeq&busiSeq=@busiSeq&areaSeq=@areaSeq");
            SetConfigValue("UpdateApp", "http://19.136.14.62/CommonService/api/reserve/syncReserveInfo/update.v?reserveSeq=@reserveSeq&syncStatus=1");
            SetConfigValue("GetCardNew", "http://19.136.14.62/CommonService/api/control/controlInfoList/query.v?custCardId=@custCardId&approveStatus=14");
            SetConfigValue("FilterUnitStr", "领证");
            UpdateAppoint = System.Configuration.ConfigurationManager.AppSettings["UpdateApp"];
            GetAppointmentLimit = System.Configuration.ConfigurationManager.AppSettings["GetAppLimit"];
            BidUrl1 = System.Configuration.ConfigurationManager.AppSettings["BidUrl1"];
            BidUrl2 = System.Configuration.ConfigurationManager.AppSettings["BidUrl2"];
            ClientName = System.Configuration.ConfigurationManager.AppSettings["ClientName"];
            NumberRestriction = System.Configuration.ConfigurationManager.AppSettings["NumberRestriction"];
            TimeInterval = System.Configuration.ConfigurationManager.AppSettings["TimeInterval"];
            InvestmentUnit = System.Configuration.ConfigurationManager.AppSettings["InvestmentUnit"];
            InvestmentBusy = System.Configuration.ConfigurationManager.AppSettings["InvestmentBusy"];
            GetCard = System.Configuration.ConfigurationManager.AppSettings["GetCardNew"];
            FilterUnitStr = System.Configuration.ConfigurationManager.AppSettings["FilterUnitStr"];
            int iPort;
            for (iPort = 1001; iPort <= 1016; iPort++)
            {
                iRetUSB = CVRSDK.CVR_InitComm(iPort);
                if (iRetUSB == 1)
                    break;
            }
            if (iRetUSB != 1)
            {
                frmMsg frm = new frmMsg();
                frm.msgInfo = "身份证读卡器初始化失败！";
                frm.ShowDialog();
            }
            else
            {
                thread = new Thread(new ThreadStart(ReadIDCard));
                thread.IsBackground = true;
                thread.Start();
            }
            new AsyncWork(this).Start(act =>
            {
                GetBasic();
                GetUnitAndBusiness();
                //this.Invoke(new Action(() =>
                //{
                //    var ucUnit = ((ucpnSelectUnit)uc["unit"]);
                //    ucUnit.uList = uList;
                //    ucUnit.cureentPage = 0;
                //    ucUnit.CreateUnit();
                //    pbReturn_Click(null, null);
                //}));
            }, AsyncType.Loading);
        }

        #region action

        void pwd_Exit()
        {
            ExitThread();
            Application.ExitThread();
        }

        void cardClick()
        {
            pageStopTime = ucTimer["card"];
        }

        void evaluate_previous(object sender)
        {
            var ctl = ((ucpnEvaluate)uc["evaluate"]);
            int max = eList.Count / ctl.pageCount;
            if ((eList.Count % ctl.pageCount) > 0)
                max++;
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pnPrevious")
            {
                if (ctl.cureentPage == 0)
                    return;
                else
                {
                    ctl.cureentPage--;
                    pageStopTime = ucTimer["evaluate"];
                }
            }
            else
            {
                if (max == ctl.cureentPage + 1)
                    return;
                else
                {
                    ctl.cureentPage++;
                    pageStopTime = ucTimer["evaluate"];
                }
            }
            ctl.CreateEvaluate();
        }

        void evaluate_enterEvaluate(TEvaluateModel ev)
        {
            pageStopTime = ucTimer["evaluate"];
            if (SaveeEvaluate(ev))
            {
                frmThankMsg frm = new frmThankMsg();
                frm.ShowDialog();
            }
        }

        bool SaveeEvaluate(TEvaluateModel ev)
        {
            ev.handleTime = DateTime.Now;
            ev.custCardId = (person.idcard == null || person.idcard == "") ? "" : person.idcard;
            ev.name = (person.name == null || person.name == "") ? "" : person.name;
            var rStr = SaveEvaluate.Replace("@userCode", userCode);
            rStr = rStr.Replace("@targetSeq", ev.controlSeq);
            rStr = rStr.Replace("@evalutionValue", ev.evaluateResult.ToString());
            var jString = http.HttpGet(rStr, "");
            var sevaList = script.DeserializeObject(jString) as Dictionary<string, object>;
            if (sevaList != null)
            {
                var dQuery = sevaList["data"] as Dictionary<string, object>;
                if (dQuery == null)
                {
                    frmMsg frm = new frmMsg();//无法评价
                    frm.msgInfo = "评价信息保存失败，当前业务[" + ev.approveName + "]无法评价！";
                    frm.ShowDialog();
                    return false;
                }
            }
            else
            {

                frmMsg frm = new frmMsg();//无法评价
                frm.msgInfo = "评价信息保存失败，当前业务[" + ev.approveName + "]无法评价！";
                frm.ShowDialog();
                return false;
            }
            ev.sysFlag = 0;
            eBll.Insert(ev);
            return true;
        }

        void appoint_previous(object sender)
        {
            var ctl = ((ucpnAppointment)uc["appoint"]);
            int max = appList.Count / ctl.pageCountA;
            if ((appList.Count % ctl.pageCountA) > 0)
                max++;
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pnPrevious1")
            {
                if (ctl.cureentPageA == 0)
                    return;
                else
                {
                    ctl.cureentPageA--;
                    pageStopTime = ucTimer["appoint"];
                }
            }
            else
            {
                if (max == ctl.cureentPageA + 1)
                    return;
                else
                {
                    ctl.cureentPageA++;
                    pageStopTime = ucTimer["appoint"];
                }
            }
            ctl.CreateAppointment();
        }

        void appoint_enter()
        {
            try
            {
                var aList = appList.Where(a => a.isCheck).ToList();
                if (aList.Count == 0)
                {
                    frmMsg frm = new frmMsg();//提示
                    frm.msgInfo = "请先选择预约信息然后排队！";
                    frm.ShowDialog();
                    return;
                }
                var apList = new List<TAppointmentModel>();
                bool isError = false;
                foreach (var app in aList)
                {
                    bool isIn = busyType == BusyType.Investment ? true : false;
                    selectAppoomt = app;
                    selectUnit = uList.Where(u => u.unitName == selectAppoomt.unitName && u.isInvestment == isIn).FirstOrDefault();
                    if (selectUnit == null)
                    {
                        isError = true;
                        continue;
                    }
                    selectBusy = bList.Where(b => b.unitName == selectUnit.unitName && b.busiName == selectAppoomt.busiName && b.isInvestment == isIn).FirstOrDefault();
                    if (selectBusy == null)
                    {
                        isError = true;
                        continue;
                    }
                    apList.Add(app);
                }
                if (isError)
                {
                    frmMsg frm = new frmMsg();//提示
                    frm.msgInfo = "存在部分数据部门/业务数据未找到，已忽略！";
                    frm.ShowDialog();
                }
                foreach (var ap in apList)
                {
                    if (!CheckLimit(selectUnit.unitSeq, selectBusy.busiSeq, true))
                    {
                        pbReturn_Click(null, null);
                        return;
                    }
                    OutQueueNo(ap, "0", "");
                }
                pbReturn_Click(null, null);
            }
            catch (Exception ex)
            {
                LogHelper.WriterQueueLog("预约取号出错：" + ex.Message);
                frmMsg frm = new frmMsg();//提示
                frm.msgInfo = "预约取号异常，请联系管理员！";
                frm.ShowDialog();
            }
        }

        void appoint_other()
        {
            SelectUnit();
        }

        void unit_previous(object sender)
        {
            var ctl = ((ucpnSelectUnit)uc["unit"]);
            pageStopTime = ucTimer["unit"];
            int max = ctl.uList.Count / ctl.pageCount;
            if ((ctl.uList.Count % ctl.pageCount) > 0)
                max++;
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pbPrevious")
            {
                if (ctl.cureentPage == 0)
                    return;
                else
                {
                    ctl.cureentPage--;
                }
            }
            else
            {
                if (max == ctl.cureentPage + 1)
                    return;
                else
                {
                    ctl.cureentPage++;
                }
            }
            ctl.CreateUnit();
        }

        void busy_previous(object sender)
        {
            var ctl = ((ucpnSelectBusy)uc["busy"]);
            pageStopTime = ucTimer["busy"];
            var count = bList.Where(b => b.unitSeq == selectUnit.unitSeq).Count();
            int max = count / ctl.pageCount;
            if ((count % ctl.pageCount) > 0)
                max++;
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pbPrevious")
            {
                if (ctl.cureentPage == 0)
                    return;
                else
                {
                    ctl.cureentPage--;
                }
            }
            else
            {
                if (max == ctl.cureentPage + 1)
                    return;
                else
                {
                    ctl.cureentPage++;
                }
            }
            ctl.CreateBusiness();
        }

        void unit_SelectedUnit()
        {
            var ucUnit = ((ucpnSelectUnit)uc["unit"]);
            var ucBusy = ((ucpnSelectBusy)uc["busy"]);
            selectUnit = ucUnit.selectUnit;
            ucBusy.cureentPage = 0;
            var list = new List<TBusinessModel>();
            if (busyType == BusyType.Investment)
                list = bList.Where(b => b.unitSeq == selectUnit.unitSeq && b.isInvestment).ToList();
            else
                list = bList.Where(b => b.unitSeq == selectUnit.unitSeq && !b.isInvestment).ToList();
            ucBusy.bList = list;
            ucBusy.BringToFront();
            ucBusy.CreateBusiness();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (busyType == BusyType.Work)
                pageLocation = PageLocation.WorkSelectBusy;
            else if (busyType == BusyType.Investment)
                pageLocation = PageLocation.InvestmentSelectBusy;
            else
                pageLocation = PageLocation.ConsultSelectBusy;
            pageStopTime = ucTimer["busy"];
        }

        void busy_SelectedBusy()
        {
            if (selectUnit == null)
                return;
            selectBusy = ((ucpnSelectBusy)uc["busy"]).selectBusy;
            if (!CheckLimit(selectUnit.unitSeq, selectBusy.busiSeq, false))
            {
                pbReturn_Click(null, null);
                return;
            }
            TAppointmentModel app = null;
            if (busyType == BusyType.Work)
            {
                #region
                string zName = appName;
                string zPhone = appPhone;
                if (person != null && person.name != null && person.name != "")
                    zName = person.name;
                if (userCode != "")
                {
                    var info = GetUserByUserCode.Replace("@userCode", userCode);
                    var json = http.HttpGet(info, "");
                    var wtMes = script.DeserializeObject(json) as Dictionary<string, object>;
                    if (wtMes != null)
                    {
                        var data = wtMes["data"] as Dictionary<string, object>;
                        if (data != null)
                        {
                            var userList = data["userInfo"] as Dictionary<string, object>;
                            var uinfo = userList;// userList["userInfo"] as Dictionary<string, object>;
                            if (uinfo != null)
                            {
                                zName = uinfo["name"].ToString();
                                zPhone = uinfo["mobilePhone"].ToString();
                            }
                        }
                    }
                }
                #endregion

                //在线预约接口
                var zStr = AppointmentOnline.Replace("@paperCode", idCard);
                zStr = zStr.Replace("@busiCode", selectBusy.busiCode);
                zStr = zStr.Replace("@currentDate", DateTime.Now.ToString("yyyy-MM-dd"));
                zStr = zStr.Replace("@name", zName);
                zStr = zStr.Replace("@mobilePhone", zPhone);
                var jStr = http.HttpGet(zStr, "");
                var online = script.DeserializeObject(jStr) as Dictionary<string, object>;
                if (online != null)
                {
                    var Seq = online["data"] as Dictionary<string, object>;
                    if (Seq != null)
                    {
                        var reserveSeq = Seq["reserveSeq"] == null ? "" : Seq["reserveSeq"].ToString();
                        if (reserveSeq != "")
                        {
                            var seq = GetAppointmentByReserveSeq.Replace("@reserveSeq", reserveSeq);
                            var js = http.HttpGet(seq, "");
                            var appBySeq = script.DeserializeObject(js) as Dictionary<string, object>;
                            if (appBySeq != null)
                            {
                                var data = appBySeq["data"] as Dictionary<string, object>;
                                if (data != null)
                                {
                                    #region
                                    var busiCode = data["busiCode"] == null ? "" : data["busiCode"].ToString();
                                    //var reserveSeq = data["reserveSeq"] == null ? "" : data["reserveSeq"].ToString();
                                    var busiName = data["busiName"] == null ? "" : data["busiName"].ToString();
                                    var userName = data["userName"] == null ? "" : data["userName"].ToString();
                                    var paperType = data["paperType"] == null ? "" : data["paperType"].ToString();// 10 为身份证
                                    var paperCode = data["paperCode"] == null ? "" : data["paperCode"].ToString();
                                    var mobilePhone = data["mobilePhone"] == null ? "" : data["mobilePhone"].ToString();
                                    var comName = data["comName"] == null ? "" : data["comName"].ToString();
                                    var reserveDate = data["reserveDate"] == null ? "1990-01-01" : data["reserveDate"].ToString();
                                    var reserveStartTime = data["reserveStartTime"] == null ? "00:00" : data["reserveStartTime"].ToString();
                                    var reserveEndTime = data["reserveEndTime"] == null ? "00:00" : data["reserveEndTime"].ToString();
                                    var approveSeq = data["approveSeq"] == null ? "" : data["approveSeq"].ToString();
                                    var approveName = data["approveName"] == null ? "" : data["approveName"].ToString();
                                    var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                                    var unitCode = data["unitCode"] == null ? "" : data["unitCode"].ToString();
                                    app = new TAppointmentModel();
                                    app.appType = 0;
                                    app.busiCode = busiCode;
                                    app.type = 0;
                                    app.reserveSeq = reserveSeq;
                                    app.approveName = approveName;
                                    app.approveSeq = approveSeq;
                                    app.busiName = busiName;
                                    app.comName = comName;
                                    app.custCardId = paperCode;
                                    app.mobilePhone = mobilePhone;
                                    app.paperCode = paperCode;
                                    app.paperType = paperType;
                                    app.reserveDate = Convert.ToDateTime(reserveDate);
                                    app.reserveEndTime = Convert.ToDateTime(reserveDate + " " + reserveEndTime + ":00");
                                    app.reserveStartTime = Convert.ToDateTime(reserveDate + " " + reserveStartTime + ":00");
                                    app.unitCode = unitCode;
                                    app.unitName = unitName;
                                    app.userName = userName;
                                    app.isCheck = false;
                                    #endregion
                                }
                            }
                        }
                    }
                }
            }
            OutQueueNo(app, "0", ""); //出号
            pbReturn_Click(null, null);
        }

        void GotoInput()
        {
            int type = 0;
            if (busyType == BusyType.Work)
                type = 0;
            else if (busyType == BusyType.Evaluate)
                type = 1;
            else if (busyType == BusyType.GetCard)
                type = 2;
            else if (busyType == BusyType.Investment)
                type = 3;
            GotoInputCard(type);
        }

        void Work()
        {
            if (!IsOk())
                return;
            Start(false);
            busyType = BusyType.Work;
            GotoReadCard(0);// 
        }

        void GetCardAction()
        {
            if (!IsOk())
                return;
            busyType = BusyType.GetCard;
            GotoReadCard(2);
        }

        void Consult()
        {
            busyType = BusyType.Consult;
            SelectUnit();
        }

        void Evaluate()
        {
            if (!IsOk())
                return;
            busyType = BusyType.Evaluate;
            GotoReadCard(1);
        }

        void Investment()
        {
            if (!IsOk())
                return;
            busyType = BusyType.Investment;
            GotoReadCard(3);
        }

        void UserGuide()
        {
            //SelectUnit();
        }

        bool IsOk()
        {
            //检验打印机状态
            if (BoShiPrinter.IfPaperend())
            {
                frmMsg msg = new frmMsg();
                msg.msgInfo = "打印机缺纸或者快没有纸了，请联系相关工作人员！";
                msg.ShowDialog();
                return false;
            }

            //检验时间有效
            string[] section = TimeInterval.Split('|');
            foreach (var part in section)
            {
                string[] ts = part.Split(',');
                DateTime start = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + ts[0]);
                DateTime end = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + ts[1]);
                if (DateTime.Now > start && DateTime.Now < end)
                {
                    return true;
                }
            }
            frmTimeMsg frm = new frmTimeMsg();
            frm.ShowDialog();
            return false;
        }

        ArrayList GetLimit()
        {
            ArrayList arr = new ArrayList();
            string[] section = TimeInterval.Split('|');
            string[] limits = NumberRestriction.Split(',');
            int index = 0;
            foreach (var part in section)
            {
                string[] ts = part.Split(',');
                DateTime start = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + ts[0]);
                DateTime end = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + ts[1]);
                if (DateTime.Now > start && DateTime.Now < end)
                {
                    arr.Add(Convert.ToInt32(limits[index]));
                    arr.Add(start);
                    arr.Add(end);
                    return arr;
                }
                index++;
            }
            return null;
        }

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
                LogHelper.WriterQueueLog("检测票数限制出错：" + ex.Message);
            }
            return null;
        }

        bool CheckLimit(string unitSeq, string busiSeq, bool isApp)
        {
            if (person != null && !string.IsNullOrEmpty(person.idcard))
            {
                //验证同一个身份证不能在一个部门一个业务排队2次（未处理的）
                var isCan = qBll.IsCanQueueO(person.idcard, busiSeq, unitSeq);
                if (Convert.ToBoolean(isCan[0]) == false)
                {
                    frmRePrint frm = new frmRePrint();
                    frm.qModel = isCan[1] as TQueueModel;
                    frm.enter_action += rePrint;
                    frm.cance_action += gotoFirst;
                    frm.ShowDialog();
                    return false;
                }
            }
            ArrayList arr = GetLimitBySeq(unitSeq, busiSeq);
            if (arr == null)
            {
                frmMsg frm = new frmMsg();
                frm.msgInfo = "当前时间段不能取号或未配置取号限制条件！";
                frm.ShowDialog();
                return false;
            }
            int max = Convert.ToInt32(arr[0]);
            DateTime start = Convert.ToDateTime(arr[1]);
            DateTime end = Convert.ToDateTime(arr[2]);
            var mList = qBll.GetModelList(busiSeq, unitSeq, start, end);
            if (!isApp)
            {
                var amount = GetAppCount(unitSeq, busiSeq);
                if (max <= mList.Count + amount)
                {
                    frmMsg frm = new frmMsg();
                    frm.msgInfo = "排队号已取完,若窗口空闲可直接到窗口办理！";
                    frm.ShowDialog();
                    return false;
                }
            }
            return true;
        }

        int GetAppCount(string unitSeq, string busiSeq)
        {
            int Amount = 0;
            try
            {
                var app = GetAppointmentLimit;
                app = app.Replace("@currentDate", DateTime.Now.ToString("yyyy-MM-dd"));
                app = app.Replace("@unitSeq", unitSeq);
                app = app.Replace("@busiSeq", busiSeq);
                string[] areaSeqList = areaSeq.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var aSeq in areaSeqList)
                {
                    var appLimit = app.Replace("@areaSeq", aSeq);
                    var appStr = http.HttpGet(appLimit, "");
                    var appInfos = script.DeserializeObject(appStr) as Dictionary<string, object>;
                    if (appInfos != null)
                    {
                        var status = appInfos["status"].ToString();
                        var dataQuery = appInfos["data"] as Dictionary<string, object>;
                        var page = dataQuery["pageObj"] as Dictionary<string, object>;
                        if (page != null)
                        {
                            var totalRow = page["totalRow"] == null ? 0 : Convert.ToInt32(page["totalRow"]);
                            Amount += totalRow;
                        }
                        #region
                        //var dataArr = dataQuery["dataList"] as object[];
                        //if (dataArr == null || dataArr.Count() == 0)
                        //{
                        //}
                        //else
                        //{
                        //    foreach (Dictionary<string, object> data in dataArr)
                        //    {
                        //        var uSeq = data["unitSeq"] == null ? "" : data["unitSeq"].ToString();
                        //        if (uSeq != "")
                        //        {
                        //            Amount++;
                        //        }
                        //    }
                        //}
                        #endregion
                    }
                }

            }
            catch (Exception ex)
            {
                LogHelper.WriterInterfaceLog("获取预约数量异常：" + ex.Message);
            }
            return Amount;
        }

        void rePrint(TQueueModel queue)
        {
            var area = "";
            var windowStr = "";
            var windowList = wbList.Where(w => w.unitSeq == selectUnit.unitSeq && w.busiSeq == selectBusy.busiSeq);
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
            var isGreen = "";
            var att = baList.Where(b => b.unitSeq == selectUnit.unitSeq && b.busiSeq == selectBusy.busiSeq).FirstOrDefault();
            if (att != null)
            {
                isGreen = att.isGreenChannel == 1 ? "绿色通道" : "";
            }
            var list = qBll.GetModelList(selectBusy.busiSeq, selectUnit.unitSeq, 0).Where(q => q.id < queue.id).ToList();
            int waitNo = list.Count;//计算等候人数
            string strLog = string.Format("补打已出票：部门[{0}]，业务[{1}]，票号[{2}]，预约号[{3}]，身份证号[{4}]，姓名[{5}]，时间[{6}]。",
                queue.unitName, queue.busTypeName, queue.ticketNumber, queue.reserveSeq, queue.idCard, queue.qNmae, DateTime.Now);
            LogHelper.WriterQueueLog(strLog);
            oBll.Insert(new TOprateLogModel()
            {
                oprateFlag = ClientName,
                oprateType = "排队端",
                oprateClassifyType = "补打",
                oprateTime = DateTime.Now,
                oprateLog = strLog,
                sysFlag = 0
            });
            if (queue.appType == 1 && queue.type == 0 && queue.reserveEndTime >= DateTime.Now && isGreen == "")
                isGreen = "网上预约";
            else if (queue.type == 1 && isGreen == "")
                isGreen = "网上申办";
            var isGetCard = queue.type == 2 ? "1" : "0";
            var serialNo = queue.type == 2 ? queue.reserveSeq : "";
            Print(queue, area, windowStr, waitNo, "补打", isGreen, isGetCard, serialNo);
        }

        void gotoFirst()
        {
            pbReturn_Click(null, null);
        }

        #endregion

        #region 读身份证相关
        public void Start(bool state)
        {
            if (state)
                are.Set();
            else
                suppend = true;
        }
        private void ReadIDCard()
        {
            int time = 0;
            while (true)
            {
                if (suppend)
                {
                    are.WaitOne(-1, false);
                    suppend = false;
                }
                time++;
                int isHaveCard = CVRSDK.CVR_Authenticate();
                if (isHaveCard == 1)
                {
                    int readOk = CVRSDK.CVR_Read_Content(4);
                    if (readOk == 1)
                    {
                        #region
                        byte[] name = new byte[30];
                        int length = 30;
                        CVRSDK.GetPeopleName(ref name[0], ref length);
                        byte[] number = new byte[30];
                        length = 36;
                        CVRSDK.GetPeopleIDCode(ref number[0], ref length);
                        byte[] address = new byte[30];
                        length = 70;
                        CVRSDK.GetPeopleAddress(ref address[0], ref length);
                        #region
                        //byte[] people = new byte[30];
                        //length = 3;
                        //CVRSDK.GetPeopleNation(ref people[0], ref length);
                        //byte[] validtermOfStart = new byte[30];
                        //length = 16;
                        //CVRSDK.GetStartDate(ref validtermOfStart[0], ref length);
                        //byte[] birthday = new byte[30];
                        //length = 16;
                        //CVRSDK.GetPeopleBirthday(ref birthday[0], ref length);
                        //byte[] validtermOfEnd = new byte[30];
                        //length = 16;
                        //CVRSDK.GetEndDate(ref validtermOfEnd[0], ref length);
                        //byte[] signdate = new byte[30];
                        //length = 30;
                        //CVRSDK.GetDepartment(ref signdate[0], ref length);
                        //byte[] sex = new byte[30];
                        //length = 3;
                        //CVRSDK.GetPeopleSex(ref sex[0], ref length);
                        //byte[] samid = new byte[32];
                        //CVRSDK.CVR_GetSAMID(ref samid[0]);
                        #endregion
                        var iCard = System.Text.Encoding.GetEncoding("GB2312").GetString(number).Replace("\0", "").Trim();
                        var iName = System.Text.Encoding.GetEncoding("GB2312").GetString(name).Replace("\0", "").Trim();
                        var iAdress = System.Text.Encoding.GetEncoding("GB2312").GetString(address).Replace("\0", "").Trim();
                        person = new Person();
                        person.name = iName;
                        person.address = iAdress;
                        person.idcard = iCard;
                        if (iCard != "")
                        {
                            idCard = iCard;
                            this.Invoke(new Action(() =>
                            {
                                DrawCard(idCard);
                            }));
                            //this.Invoke(new Action(() => { txtCard.Text = iCard; }));
                            Start(false);
                            LogHelper.WriterReadIdCardLog(string.Format("身份证读卡成功：本次读卡循环读取了{3}次，证件号码{0} 姓名{1} 地址{2}", idCard, iName, iAdress, time));
                            continue;
                        }
                        #endregion
                    }
                    //else
                    //    LogHelper.WriterReadIdCardLog("身份证读卡miss:读卡失败");
                }
                //else
                //    LogHelper.WriterReadIdCardLog("身份证读卡miss:无卡");
                Thread.Sleep(100);
            }
        }
        private void DrawCard(string idNo)
        {
            var uCard = ((ucpnReadCard)uc["readcard"]);
            uCard.IdCard = idNo;
            var pen = ((ucpnReadCard)uc["readcard"]).pnCard.CreateGraphics();
            Font font = new Font("黑体", 40, FontStyle.Bold);
            pen.DrawString(idNo, font, new SolidBrush(Color.White), 5, 6);
            wait = new Thread(new ThreadStart(Wait));
            wait.IsBackground = true;
            wait.Start();
        }
        private void Wait()
        {
            int index = 0;
            while (true)
            {
                if (index > 1)
                {
                    this.Invoke(new Action(() => { ProcessIdCard(idCard); }));
                    break;
                }
                index++;
                Thread.Sleep(200);
            }
        }
        #endregion

        #endregion

        #region 二级菜单

        #region 页面跳转 刷身份证 读身份证 ******** 入口

        /// <summary>
        /// 刷身份证type0：办事读卡 1：评价读卡 2：领卡读卡
        /// </summary>
        /// <param name="type"></param>
        private void GotoReadCard(int type)
        {
            var read = (ucpnReadCard)uc["readcard"];
            read.IdCard = "";
            read.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            pageStopTime = ucTimer["readcard"];
            if (type == 0)
                pageLocation = PageLocation.WorkReadCard;//办事读卡
            else if (type == 1)
                pageLocation = PageLocation.EvaluateReadCard;//评价读卡
            else if (type == 2)
                pageLocation = PageLocation.GetCardReadCard;//领卡读卡
            else if (type == 3)
                pageLocation = PageLocation.GetCardReadCard;//投资读卡
            idCard = "";
            person = new Person();
            Start(true);
        }

        /// <summary>
        /// 输入身份证type0：办事 1：评价 2：领卡
        /// </summary>
        /// <param name="type"></param>
        private void GotoInputCard(int type)
        {
            Start(false);
            var sCard = ((ucpnCard)uc["card"]);
            sCard.BringToFront();
            sCard.CardId = "";
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            pageStopTime = ucTimer["card"];
            idCard = "";
            if (type == 0)
                pageLocation = PageLocation.WorkInputIdCard;//办事
            else if (type == 1)
                pageLocation = PageLocation.EvaluateInputCard;//评价读卡
            else if (type == 2)
                pageLocation = PageLocation.GetCardInputCard;//领卡读卡
            else if (type == 3)
                pageLocation = PageLocation.InvestmentInputIdCard;//投资读卡

        }

        /// <summary>
        /// 身份证读卡或者输入卡 后续处理
        /// </summary>
        /// <param name="idNo"></param>
        private void ProcessIdCard(string idNo)
        {
            idCard = idNo;
            if (person == null || person.idcard == null || person.idcard.ToString() == "")
            {
                person = new Person();
                person.idcard = idNo;
            }
            if (busyType == BusyType.Work || busyType == BusyType.Investment)
            {
                #region 办事 读身份证

                appList = new List<TAppointmentModel>();

                #region 申办

                var bidStr = BidUrl1.Replace("@paperCode", idNo);
                var jlist = http.HttpGet(bidStr, "");
                var bids = script.DeserializeObject(jlist) as Dictionary<string, object>;
                if (bids != null)
                {
                    var status = bids["status"].ToString();
                    var dataQuery = bids["data"] as Dictionary<string, object>;
                    var dataArr = dataQuery["dataList"] as object[];
                    if (dataArr == null || dataArr.Count() == 0)
                    {
                    }
                    else
                    {
                        foreach (Dictionary<string, object> data in dataArr)
                        {
                            #region
                            var beginDate = data["beginDate"] == null ? "" : data["beginDate"].ToString();
                            var approveSeq = data["approveSeq"] == null ? "" : data["approveSeq"].ToString();
                            var approveName = data["approveName"] == null ? "" : data["approveName"].ToString();
                            var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                            var unitCode = data["unitSeq"] == null ? "" : data["unitSeq"].ToString();
                            var reserveSeq = data["controlSeq"] == null ? "" : data["controlSeq"].ToString();
                            var comName = data["custName"] == null ? "" : data["custName"].ToString();
                            var paperCode = idNo;
                            var paperType = "";
                            var mobilePhone = "";
                            var reserveDate = (beginDate == "" ? DateTime.Now : GetTime(beginDate)).ToString("yyyy-MM-dd");
                            var reserveStartTime = "00:00";
                            var reserveEndTime = "00:00";
                            var userName = "";
                            var busiCode = "";
                            var busiName = "";
                            if (approveSeq != "")
                            {
                                #region  根据事项名称获取业务类型 默认取第一条
                                var bid2 = BidUrl2.Replace("@approveItem", approveSeq);
                                var blist = http.HttpGet(bid2, "");
                                var bds = script.DeserializeObject(blist) as Dictionary<string, object>;
                                if (bds != null)
                                {
                                    var dQuery = bds["data"] as Dictionary<string, object>;
                                    var dArr = dQuery["dataList"] as object[];
                                    if (dArr == null || dArr.Count() == 0)
                                    {
                                        continue;
                                    }
                                    else
                                    {
                                        foreach (Dictionary<string, object> dt in dArr)
                                        {
                                            busiCode = dt["businessSeq"] == null ? "" : dt["businessSeq"].ToString();
                                            busiName = dt["businessName"] == null ? "" : dt["businessName"].ToString();
                                            unitCode = dt["unitSeq"] == null ? "" : dt["unitSeq"].ToString();
                                            var u = uList.FirstOrDefault(f => f.unitSeq == unitCode);
                                            unitName = unitCode == "" ? "" : u != null ? u.unitName : "";
                                            if (busiCode != "" && busiName != "")
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            if (busiCode != "" && busiName != "")
                            {
                                TAppointmentModel app = new TAppointmentModel();
                                app.type = 1;
                                app.appType = 0;
                                app.busiCode = busiCode;
                                app.reserveSeq = reserveSeq;
                                app.approveName = approveName;
                                app.approveSeq = approveSeq;
                                app.busiName = busiName;
                                app.comName = comName;
                                app.custCardId = paperCode;
                                app.mobilePhone = mobilePhone;
                                app.paperCode = paperCode;
                                app.paperType = paperType;
                                app.reserveDate = Convert.ToDateTime(reserveDate);
                                app.reserveEndTime = Convert.ToDateTime(reserveDate + " " + reserveEndTime + ":00");
                                app.reserveStartTime = Convert.ToDateTime(reserveDate + " " + reserveStartTime + ":00");
                                app.unitCode = unitCode;
                                app.unitName = unitName;
                                app.userName = person == null ? userName : person.name;
                                app.isCheck = false;
                                app.sysFlag = 0;
                                appList.Add(app);
                            }
                            #endregion
                        }
                    }
                }

                #endregion

                #region 预约
                var requestStr = GetAppointmentByID.Replace("@paperCode", idNo);
                var jsonString = http.HttpGet(requestStr, "");
                var Appointment = script.DeserializeObject(jsonString) as Dictionary<string, object>;
                if (Appointment != null)
                {
                    var status = Appointment["status"].ToString();
                    var dataQuery = Appointment["data"] as Dictionary<string, object>;
                    var dataArr = dataQuery["dataList"] as object[];

                    #region 查询usercode
                    userCode = "";
                    var uString = CheckUser.Replace("@paperCode", idNo);
                    var jString = http.HttpGet(uString, "");
                    var isexist = script.DeserializeObject(jString) as Dictionary<string, object>;
                    if (isexist != null)
                    {
                        var data = isexist["data"] as Dictionary<string, object>;
                        if (data != null)
                        {
                            var uCode = data["userCode"] == null ? "" : data["userCode"].ToString();
                            var exist = data["exist"].ToString();
                            if (exist == "1")
                            {
                                userCode = uCode;
                            }
                        }
                    }
                    #endregion

                    if (dataArr == null || dataArr.Count() == 0)
                    {
                        //SelectUnit();
                    }
                    else
                    {
                        #region 有预约
                        foreach (Dictionary<string, object> data in dataArr)
                        {
                            #region
                            var busiCode = data["busiCode"] == null ? "" : data["busiCode"].ToString();
                            var reserveSeq = data["reserveSeq"] == null ? "" : data["reserveSeq"].ToString();
                            var busiName = data["busiName"] == null ? "" : data["busiName"].ToString();
                            var userName = data["userName"] == null ? "" : data["userName"].ToString();
                            var paperType = data["paperType"] == null ? "" : data["paperType"].ToString();// 10 为身份证
                            var paperCode = data["paperCode"] == null ? "" : data["paperCode"].ToString();
                            var mobilePhone = data["mobilePhone"] == null ? "" : data["mobilePhone"].ToString();
                            var comName = data["comName"] == null ? "" : data["comName"].ToString();
                            var reserveDate = data["reserveDate"] == null ? "1990-01-01" : data["reserveDate"].ToString();
                            var reserveStartTime = data["reserveStartTime"] == null ? "00:00" : data["reserveStartTime"].ToString();
                            var reserveEndTime = data["reserveEndTime"] == null ? "00:00" : data["reserveEndTime"].ToString();
                            var approveSeq = data["approveSeq"] == null ? "" : data["approveSeq"].ToString();
                            var approveName = data["approveName"] == null ? "" : data["approveName"].ToString();
                            var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                            var unitCode = data["unitCode"] == null ? "" : data["unitCode"].ToString();
                            TAppointmentModel app = new TAppointmentModel();
                            app.appType = 1;
                            app.busiCode = busiCode;
                            app.type = 0;
                            app.reserveSeq = reserveSeq;
                            app.approveName = approveName;
                            app.approveSeq = approveSeq;
                            app.busiName = busiName;
                            app.comName = comName;
                            app.custCardId = paperCode;
                            app.mobilePhone = mobilePhone;
                            app.paperCode = paperCode;
                            app.paperType = paperType;
                            app.reserveDate = Convert.ToDateTime(reserveDate);
                            app.reserveEndTime = Convert.ToDateTime(reserveDate + " " + reserveEndTime + ":00");
                            app.reserveStartTime = Convert.ToDateTime(reserveDate + " " + reserveStartTime + ":00");
                            app.unitCode = unitCode;
                            app.unitName = unitName;
                            app.userName = userName;
                            app.isCheck = false;
                            app.sysFlag = 0;
                            appList.Add(app);
                            #endregion
                        }
                        #endregion
                    }
                }
                else
                {
                    //frmMsg frm = new frmMsg();//提示
                    //frm.msgInfo = "获取用户预约接口错误！";
                    //frm.ShowDialog();
                    //return;
                }
                #endregion

                #region  校验

                var ali = appList.Where(a => a.type == 1 || (a.type == 0 && a.reserveEndTime >= DateTime.Now)).ToList();
                if (ali == null || ali.Count == 0)
                {
                    SelectUnit();//如果预约失效 则不显示预约界面
                }
                else
                {
                    appList = ali;
                    ShowAppointment();
                }

                #endregion

                #endregion
            }
            else if (busyType == BusyType.GetCard)
            {
                #region 领卡读身份证
                var requestStr = GetCard.Replace("@custCardId", idNo);
                var jsonString = http.HttpGet(requestStr, "");
                var Card = script.DeserializeObject(jsonString) as Dictionary<string, object>;
                if (Card != null)
                {
                    var status = Card["status"].ToString();
                    var dataQuery = Card["data"] as Dictionary<string, object>;
                    var dataArr = dataQuery["dataList"] as object[];
                    if (dataArr == null || dataArr.Count() == 0)
                    {
                        //没有出证记录
                        frmMsg frm = new frmMsg();//提示
                        frm.msgInfo = "该身份证暂无出证记录！";
                        frm.ShowDialog();
                        return;
                    }
                    else
                    {
                        Dictionary<string, object> data = dataArr.FirstOrDefault() as Dictionary<string, object>;
                        var approveSeq = data["approveSeq"] == null ? "" : data["approveSeq"].ToString();
                        var approveName = data["approveName"] == null ? "" : data["approveName"].ToString();
                        var unitSeq = data["unitSeq"] == null ? "" : data["unitSeq"].ToString();
                        var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                        var controlSeq = data["controlSeq"] == null ? "" : data["controlSeq"].ToString();
                        var beginDate = data["beginDate"] == null ? "" : data["beginDate"].ToString();
                        var busiSeq = "";
                        var busiName = "";
                        selectUnit = uList.Where(u => u.unitSeq == unitSeq).FirstOrDefault();
                        if (selectUnit != null)
                        {
                            if (approveSeq != "")
                            {
                                #region  根据事项名称获取业务类型 默认取第一条
                                var bid2 = BidUrl2.Replace("@approveItem", approveSeq);
                                var blist = http.HttpGet(bid2, "");
                                var bds = script.DeserializeObject(blist) as Dictionary<string, object>;
                                if (bds != null)
                                {
                                    var dQuery = bds["data"] as Dictionary<string, object>;
                                    var dArr = dQuery["dataList"] as object[];
                                    if (dArr == null || dArr.Count() == 0)
                                    {

                                    }
                                    else
                                    {
                                        foreach (Dictionary<string, object> dt in dArr)
                                        {
                                            busiSeq = dt["businessSeq"] == null ? "" : dt["businessSeq"].ToString();
                                            busiName = dt["businessName"] == null ? "" : dt["businessName"].ToString();
                                            unitSeq = dt["unitSeq"] == null ? "" : dt["unitSeq"].ToString();
                                            var u = uList.FirstOrDefault(f => f.unitSeq == unitSeq);
                                            unitName = unitSeq == "" ? "" : u != null ? u.unitName : "";
                                            if (busiSeq != "" && busiName != "")
                                            {
                                                break;
                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                            if (busiSeq != "" && busiName != "")
                            {
                                //selectBusy = bList.Where(b => b.unitSeq == unitSeq).Where(b => 1 == 1).FirstOrDefault();//根据
                                selectBusy = bList.Where(b => b.busiSeq == busiSeq).FirstOrDefault();
                                if (selectBusy != null)
                                {
                                    TGetCardModel model = new TGetCardModel();
                                    model.controlSeq = controlSeq;
                                    model.unitName = unitName;
                                    model.unitSeq = unitSeq;
                                    model.busTypeSeq = busiSeq;
                                    model.busTypeName = busiName;
                                    model.custCardId = idNo;
                                    model.outCardTime = DateTime.Now;
                                    model.sysFlag = 0;
                                    if (!CheckLimit(selectUnit.unitSeq, selectBusy.busiSeq, false))
                                    {
                                        pbReturn_Click(null, null);
                                        return;
                                    }
                                    gBll.Insert(model);
                                    OutQueueNo(null, "1", approveSeq);
                                    pbReturn_Click(null, null);
                                }
                                else
                                {
                                    frmMsg frm = new frmMsg();//提示
                                    frm.msgInfo = "当前办证业务未找到！";
                                    frm.ShowDialog();
                                    return;
                                }
                            }
                            else
                            {
                                frmMsg frm = new frmMsg();//提示
                                frm.msgInfo = "当前办证业务未找到！";
                                frm.ShowDialog();
                                return;
                            }
                        }
                        else
                        {
                            frmMsg frm = new frmMsg();//提示
                            frm.msgInfo = "当前办证部门未找到！";
                            frm.ShowDialog();
                            return;
                        }
                    }
                }
                #endregion
            }
            else if (busyType == BusyType.Evaluate)
            {
                #region 评价读身份证
                var requestStr = CheckUser.Replace("@paperCode", idNo);
                var jsonString = http.HttpGet(requestStr, "");
                var Card = script.DeserializeObject(jsonString) as Dictionary<string, object>;
                if (Card != null)
                {
                    var status = Card["status"].ToString();
                    var data = Card["data"] as Dictionary<string, object>;
                    if (data == null)
                    {
                        frmMsg frm = new frmMsg();
                        frm.msgInfo = "当前身份证未注册，获取不到用户编号！";
                        frm.ShowDialog();
                        return;
                    }
                    else
                    {
                        var uCode = data["userCode"] == null ? "" : data["userCode"].ToString();
                        var exist = data["exist"].ToString();
                        if (exist == "0")
                        {
                            frmMsg frm = new frmMsg();
                            frm.msgInfo = "当前身份证未注册平台账户！";
                            frm.ShowDialog();
                            return;
                        }
                        userCode = uCode;
                        var rStr = GetEvaluate.Replace("@custCardId", idNo);
                        var jString = http.HttpGet(rStr, "");
                        var evaList = script.DeserializeObject(jString) as Dictionary<string, object>;
                        if (evaList != null)
                        {
                            var dQuery = evaList["data"] as Dictionary<string, object>;
                            var dArr = dQuery["dataList"] as object[];
                            if (dArr == null || dArr.Count() == 0)
                            {
                                frmMsg frm = new frmMsg();
                                frm.msgInfo = "该身份证用户没有可评价的业务信息！";
                                frm.ShowDialog();
                                return;
                            }
                            else
                            {
                                eList = new List<TEvaluateModel>();
                                foreach (Dictionary<string, object> dat in dArr)
                                {
                                    var controlSeq = dat["controlSeq"] == null ? "" : dat["controlSeq"].ToString();
                                    var approveName = dat["approveName"] == null ? "" : dat["approveName"].ToString();
                                    var approveSeq = dat["approveSeq"] == null ? "" : dat["approveSeq"].ToString();
                                    var unitSeq = dat["unitSeq"] == null ? "" : dat["unitSeq"].ToString();
                                    var unitName = dat["unitName"] == null ? "" : dat["unitName"].ToString();
                                    eList.Add(new TEvaluateModel()
                                    {
                                        type = 0,
                                        approveName = approveName,
                                        controlSeq = controlSeq,
                                        approveSeq = approveSeq,
                                        custCardId = idCard,
                                        unitSeq = unitSeq,
                                        unitName = unitName,
                                    });

                                }
                                ShowEvaluate();
                            }
                        }

                    }
                }
                #endregion
            }
            Start(false);
        }

        #endregion

        #region 回退、主页
        //主页
        private void pbReturn_Click(object sender, EventArgs e)
        {
            var sUnit = ((ucpnSelectUnit)uc["unit"]);
            var sBusy = ((ucpnSelectBusy)uc["busy"]);
            var sCard = ((ucpnCard)uc["card"]);
            var uCard = ((ucpnReadCard)uc["readcard"]);
            uCard.IdCard = "";
            sCard.CardId = "";
            sUnit.cureentPage = 0;
            sBusy.cureentPage = 0;
            //sUnit.CreateUnit();
            Start(false);
            uc["main"].BringToFront();
            lblMes.Visible = false;
            idCard = "";
            selectAppoomt = null;
            selectBusy = null;
            selectUnit = null;
            busyType = BusyType.Default;
            pageLocation = PageLocation.Main;
            person = new Person();
        }
        //返回上一页面
        private void pbLastPage_Click(object sender, EventArgs e)
        {
            if (pageLocation == PageLocation.Main)
                return;
            if (pageLocation == PageLocation.WorkReadCard || pageLocation == PageLocation.EvaluateReadCard || pageLocation == PageLocation.GetCardReadCard || pageLocation == PageLocation.ConsultSelectUnit || pageLocation == PageLocation.InvestmentReadCard)
            {
                pbReturn_Click(null, null);
            }
            else if (pageLocation == PageLocation.WorkInputIdCard || pageLocation == PageLocation.WorkSelectUnit || pageLocation == PageLocation.InvestmentSelectUnit || pageLocation == PageLocation.WorkAppointment || pageLocation == PageLocation.InvestmentInputIdCard || pageLocation == PageLocation.InvestmentAppointment)
            {
                Work();
            }
            else if (pageLocation == PageLocation.GetCardInputCard)
            {
                GetCardAction();
            }
            else if (pageLocation == PageLocation.EvaluateInputCard || pageLocation == PageLocation.Evaluate)
            {
                Evaluate();
            }
            else if (pageLocation == PageLocation.WorkSelectBusy || pageLocation == PageLocation.ConsultSelectBusy || pageLocation == PageLocation.InvestmentSelectBusy)
            {
                SelectUnit();
            }
        }

        #endregion

        #region 退出密码框

        private void pnexit_Click(object sender, EventArgs e)
        {
            var pwd = (ucpnPwd)uc["pwd"];
            pwd.InputPwd = "";
            pwd.ExitPwd = ExitPwd;
            pwd.BringToFront();
            pageLocation = PageLocation.Exit;
            pageStopTime = ucTimer["pwd"];

        }

        void ExitThread()
        {
            try
            {
                if (thread != null)
                    thread.Abort();
            }
            catch
            {
            }
            try
            {
                if (exit != null)
                    exit.Abort();
            }
            catch
            {
            }
            try
            {
                CVRSDK.CVR_CloseComm();
            }
            catch
            {

            }
        }

        #endregion

        #endregion

        #region 通过接口获取部门和业务
        private void timer1_Tick(object sender, EventArgs e)
        {
            new AsyncWork(this).Start(act =>
            {
                GetUnitAndBusiness();
                //this.Invoke(new Action(() =>
                //{
                //    var ucUnit = ((ucpnSelectUnit)uc["unit"]);
                //    ucUnit.uList = uList;
                //    ucUnit.cureentPage = 0;
                //    ucUnit.CreateUnit();
                //}));
            }, AsyncType.Loading);
        }
        private void GetBasic()
        {
            waList = waBll.GetModelList();
            wList = wBll.GetModelList();
            baList = baBll.GetModelList();
            wbList = wbBll.GetModelList();
        }
        //获取部门和业务
        private void GetUnitAndBusiness()
        {
            #region
            //uList.Clear();
            //bList.Clear();
            //var unitList = uBll.GetModelList().OrderBy(o => o.orderNum).ToList();
            //var businessList = bBll.GetModelList();
            //var tbList = new List<TBusinessModel>();
            ////uList = unitList;
            ////bList = businessList;

            //var depts = new List<TUnitModel>();
            //var busies = new List<TBusinessModel>();
            //string[] areaSeqList = areaSeq.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (var aSeq in areaSeqList)
            //{
            //    #region  unit
            //    var areaStr = GetUnit.Replace("@areaSeq", aSeq);
            //    var unitString = http.HttpGet(areaStr, "");
            //    var units = script.DeserializeObject(unitString) as Dictionary<string, object>;
            //    if (units != null)
            //    {
            //        var du = units["data"] == null ? null : units["data"] as Dictionary<string, object>;
            //        if (du != null)
            //        {
            //            var dataArr = du["dataList"] == null ? null : du["dataList"] as object[];
            //            foreach (var item in dataArr)
            //            {
            //                var data = item as Dictionary<string, object>;
            //                if (data != null)
            //                {
            //                    var unitSeq = data["unitCode"] == null ? "" : data["unitCode"].ToString();
            //                    var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
            //                    var sortNum = data["sortNum"] == null ? "999" : data["sortNum"].ToString();
            //                    TUnitModel unit = new TUnitModel { unitSeq = unitSeq, unitName = unitName, orderNum = Convert.ToInt32(sortNum), sysFlag = 0 };
            //                    if (unitList.Where(u => u.unitSeq == unitSeq && u.unitName == unitName).Count() == 0)
            //                    {
            //                        uBll.Insert(unit);
            //                    }
            //                    uList.Add(unit);
            //                    depts.Add(unit);
            //                }
            //            }
            //        }
            //    }
            //    #endregion
            //}
            //uList = uList.OrderBy(o => o.orderNum).ToList();

            //#region busy
            //var busys = http.HttpGet(GetBusiness, "");
            //var busyJson = script.DeserializeObject(busys) as Dictionary<string, object>;
            //if (busyJson != null)
            //{
            //    var data = busyJson["data"] == null ? null : busyJson["data"] as Dictionary<string, object>;
            //    if (data != null)
            //    {
            //        var dataList = data["dataList"] == null ? null : data["dataList"] as object[];
            //        foreach (var item in dataList)
            //        {
            //            var busiData = item as Dictionary<string, object>;
            //            var busiSeq = busiData["busiSeq"] == null ? "" : busiData["busiSeq"].ToString();
            //            var busiCode = busiData["busiCode"] == null ? "" : busiData["busiCode"].ToString();
            //            var busiName = busiData["busiName"] == null ? "" : busiData["busiName"].ToString();
            //            var busiType = busiData["busiType"] == null ? "0" : busiData["busiType"].ToString();
            //            var acceptBusi = busiData["acceptBusi"] == null ? "0" : busiData["acceptBusi"].ToString();
            //            var getBusi = busiData["getBusi"] == null ? "0" : busiData["getBusi"].ToString();
            //            var askBusi = busiData["askBusi"] == null ? "0" : busiData["askBusi"].ToString();
            //            var unitSeq = busiData["unitSeq"] == null ? "0" : busiData["unitSeq"].ToString();
            //            var unitName = busiData["unitName"] == null ? "0" : busiData["unitName"].ToString();
            //            TBusinessModel buss = new TBusinessModel
            //            {
            //                acceptBusi = Convert.ToBoolean(Convert.ToInt32(acceptBusi)),
            //                busiCode = busiCode,
            //                busiSeq = busiSeq,
            //                askBusi = Convert.ToBoolean(Convert.ToInt32(askBusi)),
            //                busiName = busiName,
            //                busiType = busiType,
            //                getBusi = Convert.ToBoolean(Convert.ToInt32(getBusi)),
            //                unitSeq = unitSeq,
            //                unitName = unitName,
            //                sysFlag = 0
            //            };
            //            tbList.Add(buss);
            //            busies.Add(buss);
            //        }
            //    }
            //}
            //#endregion
            //#region insert
            //List<TBusinessModel> insertList = new List<TBusinessModel>();
            //foreach (var uSeq in uList)
            //{
            //    var unitBusy = tbList.Where(b => b.unitSeq == uSeq.unitSeq && b.unitName == uSeq.unitName).ToList();
            //    if (unitBusy != null)
            //    {
            //        insertList.AddRange(unitBusy);
            //    }
            //}
            //foreach (var i in insertList)
            //{
            //    if (businessList.Where(b => b.unitSeq == i.unitSeq && b.unitName == i.unitName && b.busiCode == i.busiCode && b.busiName == i.busiName).Count() == 0)
            //    {
            //        bBll.Insert(i);
            //    }
            //    bList.Add(i);
            //}
            //#endregion

            //#region 进行匹配对比 然后删除数据库里面 接口中没有的数据
            //List<TBusinessModel> deleteList = new List<TBusinessModel>();
            //List<TUnitModel> deleteUnit = new List<TUnitModel>();
            //foreach (var busy in businessList)
            //{
            //    if (bList.Where(b => b.unitSeq == busy.unitSeq && b.unitName == busy.unitName && b.busiCode == busy.busiCode && b.busiName == busy.busiName).Count() == 0)
            //    {
            //        deleteList.Add(busy);
            //    }
            //}
            //foreach (var d in deleteList)
            //{
            //    bBll.Delete(d);
            //}
            //foreach (var unit in unitList)
            //{
            //    if (uList.Where(u => u.unitSeq == unit.unitSeq && u.unitName == unit.unitName).Count() == 0)
            //    {
            //        deleteUnit.Add(unit);
            //    }
            //}
            //foreach (var u in deleteUnit)
            //{
            //    uBll.Delete(u);
            //}
            //#endregion
            #endregion

            var depts = new List<TUnitModel>();
            var busies = new List<TBusinessModel>();

            #region 按区域查询单位
            string[] areaSeqList = areaSeq.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var aSeq in areaSeqList)
            {
                #region  unit
                var areaStr = GetUnit.Replace("@areaSeq", aSeq);
                var unitString = http.HttpGet(areaStr, "");
                var units = script.DeserializeObject(unitString) as Dictionary<string, object>;
                if (units != null)
                {
                    var du = units["data"] == null ? null : units["data"] as Dictionary<string, object>;
                    if (du != null)
                    {
                        var dataArr = du["dataList"] == null ? null : du["dataList"] as object[];
                        foreach (var item in dataArr)
                        {
                            var data = item as Dictionary<string, object>;
                            if (data != null)
                            {
                                var unitSeq = data["unitCode"] == null ? "" : data["unitCode"].ToString();
                                var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                                var sortNum = data["sortNum"] == null ? "999" : data["sortNum"].ToString();
                                TUnitModel unit = new TUnitModel { unitSeq = unitSeq, unitName = unitName, orderNum = Convert.ToInt32(sortNum), sysFlag = 0, isInvestment = false };
                                depts.Add(unit);
                            }
                        }
                    }
                }
                #endregion
            }
            #region 查询投资项目部门
            var investment = http.HttpGet(InvestmentUnit, "");
            var uts = script.DeserializeObject(investment) as Dictionary<string, object>;
            if (uts != null)
            {
                var du = uts["data"] == null ? null : uts["data"] as Dictionary<string, object>;
                if (du != null)
                {
                    var dataArr = du["dataList"] == null ? null : du["dataList"] as object[];
                    foreach (var item in dataArr)
                    {
                        var data = item as Dictionary<string, object>;
                        if (data != null)
                        {
                            var unitSeq = data["unitCode"] == null ? "" : data["unitCode"].ToString();
                            var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                            var sortNum = data["sortNum"] == null ? "999" : data["sortNum"].ToString();
                            TUnitModel unit = new TUnitModel { unitSeq = unitSeq, unitName = unitName, orderNum = Convert.ToInt32(sortNum), sysFlag = 0, isInvestment = true };
                            if (depts.Where(d => d.unitSeq == unitSeq && d.unitName == unitName).Count() == 0)
                                depts.Add(unit);
                        }
                    }
                }
            }
            #endregion


            #endregion

            #region 查询业务类型
            var busys = http.HttpGet(GetBusiness, "");
            var busyJson = script.DeserializeObject(busys) as Dictionary<string, object>;
            if (busyJson != null)
            {
                var data = busyJson["data"] == null ? null : busyJson["data"] as Dictionary<string, object>;
                if (data != null)
                {
                    var dataList = data["dataList"] == null ? null : data["dataList"] as object[];
                    foreach (var item in dataList)
                    {
                        var busiData = item as Dictionary<string, object>;
                        var busiSeq = busiData["busiSeq"] == null ? "" : busiData["busiSeq"].ToString();
                        var busiCode = busiData["busiCode"] == null ? "" : busiData["busiCode"].ToString();
                        var busiName = busiData["busiName"] == null ? "" : busiData["busiName"].ToString();
                        var busiType = busiData["busiType"] == null ? "0" : busiData["busiType"].ToString();
                        var acceptBusi = busiData["acceptBusi"] == null ? "0" : busiData["acceptBusi"].ToString();
                        var getBusi = busiData["getBusi"] == null ? "0" : busiData["getBusi"].ToString();
                        var askBusi = busiData["askBusi"] == null ? "0" : busiData["askBusi"].ToString();
                        var unitSeq = busiData["unitSeq"] == null ? "0" : busiData["unitSeq"].ToString();
                        var unitName = busiData["unitName"] == null ? "0" : busiData["unitName"].ToString();
                        TBusinessModel buss = new TBusinessModel
                        {
                            acceptBusi = Convert.ToBoolean(Convert.ToInt32(acceptBusi)),
                            busiCode = busiCode,
                            busiSeq = busiSeq,
                            askBusi = Convert.ToBoolean(Convert.ToInt32(askBusi)),
                            busiName = busiName,
                            busiType = busiType,
                            getBusi = Convert.ToBoolean(Convert.ToInt32(getBusi)),
                            unitSeq = unitSeq,
                            unitName = unitName,
                            isInvestment = false,
                            sysFlag = 0
                        };
                        busies.Add(buss);
                    }
                }
            }

            #region 投资项目业务类型
            foreach (var unit in depts.Where(d => d.isInvestment).ToList())
            {
                var busyS = InvestmentBusy.Replace("@unitSeq", unit.unitSeq);
                var inbusys = http.HttpGet(busyS, "");
                var inbusyJson = script.DeserializeObject(inbusys) as Dictionary<string, object>;
                if (inbusyJson != null)
                {
                    var data = inbusyJson["data"] == null ? null : inbusyJson["data"] as Dictionary<string, object>;
                    if (data != null)
                    {
                        var dataList = data["dataList"] == null ? null : data["dataList"] as object[];
                        foreach (var item in dataList)
                        {
                            var busiData = item as Dictionary<string, object>;
                            var busiSeq = busiData["busiSeq"] == null ? "" : busiData["busiSeq"].ToString();
                            var busiCode = busiData["busiCode"] == null ? "" : busiData["busiCode"].ToString();
                            var busiName = busiData["busiName"] == null ? "" : busiData["busiName"].ToString();
                            var busiType = busiData["busiType"] == null ? "0" : busiData["busiType"].ToString();
                            var acceptBusi = busiData["acceptBusi"] == null ? "0" : busiData["acceptBusi"].ToString();
                            var getBusi = busiData["getBusi"] == null ? "0" : busiData["getBusi"].ToString();
                            var askBusi = busiData["askBusi"] == null ? "0" : busiData["askBusi"].ToString();
                            var unitSeq = busiData["unitSeq"] == null ? "0" : busiData["unitSeq"].ToString();
                            var unitName = busiData["unitName"] == null ? "0" : busiData["unitName"].ToString();
                            TBusinessModel buss = new TBusinessModel
                            {
                                acceptBusi = Convert.ToBoolean(Convert.ToInt32(acceptBusi)),
                                busiCode = busiCode,
                                busiSeq = busiSeq,
                                askBusi = Convert.ToBoolean(Convert.ToInt32(askBusi)),
                                busiName = busiName,
                                busiType = busiType,
                                getBusi = Convert.ToBoolean(Convert.ToInt32(getBusi)),
                                unitSeq = unitSeq,
                                unitName = unitName,
                                sysFlag = 0,
                                isInvestment = true
                            };
                            if (busies.Where(b => b.unitName == unitName && b.unitSeq == unitSeq && b.busiSeq == busiSeq && b.busiName == b.busiName).Count() == 0)
                                busies.Add(buss);
                        }
                    }
                }
            }
            #endregion

            #endregion

            var arr = uBll.UploadUnitAndBusy(depts, busies);
            if (arr != null)
            {
                uList.Clear();
                bList.Clear();
                uList = arr[0] as List<TUnitModel>;
                bList = arr[1] as List<TBusinessModel>;
            }
        }
        #endregion

        #region  显示预约列表、显示评价列表、选择部门、选择业务

        private DateTime GetTime(string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        private void ShowAppointment()
        {
            var appointment = ((ucpnAppointment)uc["appoint"]);
            appointment.appList = appList;
            appointment.cureentPageA = 0;
            appointment.CreateAppointment();
            appointment.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (busyType == BusyType.Investment)
                pageLocation = PageLocation.InvestmentAppointment;
            else
                pageLocation = PageLocation.WorkAppointment;
            pageStopTime = ucTimer["appoint"];
        }
        private void ShowEvaluate()
        {
            var evaluate = (ucpnEvaluate)uc["evaluate"];
            evaluate.eList = eList;
            evaluate.cureentPage = 0;
            evaluate.CreateEvaluate();
            evaluate.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            pageLocation = PageLocation.Evaluate;
            pageStopTime = ucTimer["evaluate"];
        }
        private void SelectUnit()
        {
            Start(false);
            var ucUnit = ((ucpnSelectUnit)uc["unit"]);
            var list = new List<TUnitModel>();
            if (busyType == BusyType.Investment)
            {
                list = uList.Where(u => u.isInvestment && u.unitName != FilterUnitStr).ToList();
            }
            else
            {
                list = uList.Where(u => !u.isInvestment && u.unitName != FilterUnitStr).ToList();
            }
            ucUnit.uList = list;
            ucUnit.cureentPage = 0;
            ucUnit.CreateUnit();
            ucUnit.BringToFront(); //选择 部门，选择业务， 取号
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (busyType == BusyType.Work)
            {
                pageLocation = PageLocation.WorkSelectUnit;
            }
            else if (busyType == BusyType.Consult)
            {
                pageLocation = PageLocation.ConsultSelectUnit;
            }
            else if (busyType == BusyType.Investment)
            {
                pageLocation = PageLocation.InvestmentSelectUnit;
            }
            pageStopTime = ucTimer["unit"];
        }
        private void SelectBusy()
        {
            uc["busy"].BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (busyType == BusyType.Work)
                pageLocation = PageLocation.WorkSelectBusy;
            else if (busyType == BusyType.Investment)
                pageLocation = PageLocation.InvestmentSelectBusy;
            else
                pageLocation = PageLocation.ConsultSelectBusy;
            pageStopTime = ucTimer["busy"];
        }
        #endregion

        #region  出号
        private void OutQueueNo(TAppointmentModel app, string isGetCard, string serialNo)
        {
            //try
            //{
            if (app != null)
            {
                bool isIn = busyType == BusyType.Investment ? true : false;
                selectAppoomt = app; //目前默认取第一条直接出号，暂时不做让选择预约号功能
                selectUnit = uList.Where(u => u.unitName == selectAppoomt.unitName && u.isInvestment == isIn).FirstOrDefault();
                selectBusy = bList.Where(b => b.unitName == selectUnit.unitName && b.busiName == selectAppoomt.busiName && b.isInvestment == isIn).FirstOrDefault();
            }
            //验证业务扩展属性
            var isGreen = "";
            var ticketStart = "";
            var handleStartTime = "";
            var handleEndTime = "";
            int lineUpMax = 0;
            int lineUpWarningMax = 0;
            var att = baList.Where(b => b.unitSeq == selectUnit.unitSeq && b.busiSeq == selectBusy.busiSeq).FirstOrDefault();
            var list = qBll.GetModelList(selectBusy.busiSeq, selectUnit.unitSeq, 0);
            int waitNo = list.Count;//计算等候人数
            if (att != null)
            {
                isGreen = att.isGreenChannel == 1 ? "绿色通道" : "";
                ticketStart = att.ticketPrefix;
                handleStartTime = att.handleStartTime;
                handleEndTime = att.handleEndTime;
                lineUpMax = att.lineUpMax;
                lineUpWarningMax = att.lineUpWarningMax;
            }
            if (lineUpWarningMax != 0 && waitNo >= lineUpWarningMax)
            {
                //预留 发送 等候人数预警值
            }
            var queue = InsertQueue(app, ticketStart, isGetCard, serialNo);
            var area = "";
            var windowStr = "";
            var windowList = wbList.Where(w => w.unitSeq == selectUnit.unitSeq && w.busiSeq == selectBusy.busiSeq);
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
            string strLog = string.Format("已出票：部门[{0}]，业务[{1}]，票号[{2}]，预约号[{3}]，身份证号[{4}]，姓名[{5}]，时间[{6}]。",
                queue.unitName, queue.busTypeName, queue.ticketNumber, queue.reserveSeq, person == null ? "" : person.idcard, person == null ? "" : person.name, DateTime.Now);
            LogHelper.WriterQueueLog(strLog);
            oBll.Insert(new TOprateLogModel()
            {
                oprateFlag = ClientName,
                oprateType = "排队端",
                oprateClassifyType = "出票",
                oprateTime = DateTime.Now,
                oprateLog = strLog,
                sysFlag = 0
            });
            if (queue.appType == 1 && queue.type == 0 && queue.reserveEndTime >= DateTime.Now && isGreen == "")
                isGreen = "网上预约";
            else if (queue.type == 1 && isGreen == "")
                isGreen = "网上申办";
            Print(queue, area, windowStr, waitNo, "", isGreen, isGetCard, serialNo);
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.WriterQueueLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":出票过程中发生错误," + ex.Message);
            //    frmMsg frm = new frmMsg();//提示
            //    frm.msgInfo = "出票过程中发生错误，请联系管理员！";
            //    frm.ShowDialog();
            //}
        }
        //出票
        private void Print(TQueueModel model, string area, string windowStr, int wait, string flag, string vip, string isGetCard, string serialNo)
        {
            try
            {
                DataTable table = GetQueue(model, area, windowStr, wait, flag, vip, isGetCard, serialNo);
                PrintManager print = new PrintManager();
                //PrintManager.CanDesign = true;
                print.InitReport("排队小票");
                print.AddData(table, "QueueBill");
                print.Print();
                print.Dispose();
            }
            catch (Exception ex)
            {
                frmMsg frm = new frmMsg();
                frm.msgInfo = "出票打印错误：" + ex.Message;
                frm.ShowDialog();
            }
        }
        //组织票数据
        private DataTable GetQueue(TQueueModel model, string area, string windowStr, int wait, string flag, string vip, string isGetCard, string serialNo)
        {
            DataTable table = new DataTable("table");
            table.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn ("isGetCard",typeof(string)),
                new DataColumn ("flag",typeof(string)),
                new DataColumn ("area",typeof(string)),
                new DataColumn ("windowStr",typeof(string)),
                new DataColumn ("waitCount",typeof(string)),
                new DataColumn ("unitName",typeof(string)),
                new DataColumn ("busyName",typeof(string)),
                new DataColumn ("ticketNumber",typeof(string)),
                new DataColumn ("cardId",typeof(string)),
                new DataColumn ("reserveSeq",typeof(string)),
                new DataColumn ("vip",typeof(string)),
            });
            DataRow row = table.NewRow();
            row["isGetCard"] = isGetCard;
            row["area"] = area;
            row["windowStr"] = windowStr;
            row["waitCount"] = wait.ToString();
            row["unitName"] = model.unitName;
            row["busyName"] = model.busTypeName;
            row["ticketNumber"] = model.ticketNumber;
            row["flag"] = flag;
            row["cardId"] = string.IsNullOrEmpty(model.idCard) ? "" : model.idCard.Length > 6 ? model.idCard.Substring(model.idCard.Length - 6, 6) : model.idCard;
            if (isGetCard == "1")
            {
                row["reserveSeq"] = serialNo;
            }
            else
            {
                row["reserveSeq"] = model.reserveSeq;
            }
            row["vip"] = vip;
            table.Rows.Add(row);
            return table;
        }
        //插入排队数据
        private TQueueModel InsertQueue(TAppointmentModel app, string ticketStart, string isGetCard, string serialNo)
        {
            string idCard = person == null ? "" : person.idcard;
            string qNmae = person == null ? "" : person.name;
            string reserveSeq = app == null ? "" : app.reserveSeq;
            var line = qBll.QueueLine(selectBusy, selectUnit, ticketStart, idCard, qNmae, app, isGetCard, serialNo);
            if (app != null)
            {
                app.sysFlag = 0;
                aBll.Insert(app);
                if (app.appType == 1)
                {
                    var updateStr = UpdateAppoint.Replace("@reserveSeq", app.reserveSeq);
                    var jsonString = http.HttpGet(updateStr, "");
                    var syncReserve = script.DeserializeObject(jsonString) as Dictionary<string, object>;
                    if (syncReserve != null)
                    {
                        var status = syncReserve["status"].ToString();
                        var dataArr = syncReserve["data"] as Dictionary<string, object>;
                        var desc = syncReserve["desc"].ToString();
                        if (status == "200")
                        {
                            //成功
                        }
                    }
                }
            }
            return line;
        }
        #endregion

        #region 关闭

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitThread();
        }

        #endregion

        #region 其他
        private void timer2_Tick(object sender, EventArgs e)
        {
            lblTime.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss dddd");
        }
        //页面停留
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (pageLocation == PageLocation.Main)
            {
                lblMes.Visible = false;

                return;
            }
            else
            {
                if (pageStopTime <= 1)
                {
                    pbReturn_Click(null, null);
                }
                else
                {
                    lblMes.Visible = true;
                    lblMes.Text = string.Format("剩余操作时间：{0}秒", pageStopTime.ToString("00"));
                    pageStopTime--;
                }
            }
        }

        #region 暂时不用
        SolidBrush mBrush = new SolidBrush(Color.White);
        private void pbReturnMain_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            Font fontS = new Font("黑体", 24, FontStyle.Bold);
            if (pic.Name == "pbReturnMain")
            {
                e.Graphics.DrawString("首页", fontS, mBrush, 70, 12);
            }
            else if (pic.Name == "pbLastPage")
            {
                e.Graphics.DrawString("返回", fontS, mBrush, 70, 12);
            }
        }

        private void pbReturnMain_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            mBrush = new SolidBrush(Color.Black);
            if (pb.Name == "pbReturnMain")
            {
                pb.Image = Properties.Resources.首页按钮2;
            }
            else
            {
                pb.Image = Properties.Resources.返回按钮2;
            }
        }

        private void pbReturnMain_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            mBrush = new SolidBrush(Color.White);
            if (pb.Name == "pbReturnMain")
            {
                pb.Image = Properties.Resources.首页按钮;
            }
            else
            {
                pb.Image = Properties.Resources.返回按钮;
            }
        }
        #endregion

        #endregion
    }
}
