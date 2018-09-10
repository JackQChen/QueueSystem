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
using System.Linq.Expressions;
using System.Xml;
using System.IO;
using MessageClient;
using QueueMessage;

namespace QueueClient
{
    public partial class frmMain : Form
    {
        #region
        string ClientName = System.Configuration.ConfigurationManager.AppSettings["ClientName"];
        string NumberRestriction = System.Configuration.ConfigurationManager.AppSettings["NumberRestriction"];
        string TimeInterval = System.Configuration.ConfigurationManager.AppSettings["TimeInterval"];
        string ExitPwd = System.Configuration.ConfigurationManager.AppSettings["ExitPwd"];
        string ExitTime = System.Configuration.ConfigurationManager.AppSettings["ExitTime"];
        string EvaluateTime = System.Configuration.ConfigurationManager.AppSettings["Evaluate"];
        string AppointTime = System.Configuration.ConfigurationManager.AppSettings["Appoint"];
        string BusyTime = System.Configuration.ConfigurationManager.AppSettings["Busy"];
        string UnitTime = System.Configuration.ConfigurationManager.AppSettings["Unit"];
        string ReadcardTime = System.Configuration.ConfigurationManager.AppSettings["Readcard"];
        string CardTime = System.Configuration.ConfigurationManager.AppSettings["Card"];
        string MainTime = "";// System.Configuration.ConfigurationManager.AppSettings["MainTime"];
        int CanNotUseCard = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["CanNotUseCard"]);
        string ip;
        string port;
        #endregion

        #region
        Client client = new Client();
        JavaScriptSerializer script = new JavaScriptSerializer();
        HttpHelper http = new HttpHelper();
        BEvaluateBLL eBll = new BEvaluateBLL();
        BQueueBLL qBll = new BQueueBLL();
        TWindowBLL wBll = new TWindowBLL();
        TWindowAreaBLL waBll = new TWindowAreaBLL();
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        BLineUpMaxNoBLL lineBll = new BLineUpMaxNoBLL();
        TOprateLogBLL oBll = new TOprateLogBLL();
        TBusinessAttributeBLL baBll = new TBusinessAttributeBLL();
        TUnitBLL uBll = new TUnitBLL();
        List<BEvaluateModel> eList = new List<BEvaluateModel>();
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
        int pageStopTime = 99;
        int iRetUSB = 0;
        int FloorImgCount = 2;
        string idCard = "";//身份证号码
        Thread thread;
        Thread wait;
        TUnitModel selectUnit;
        TBusinessModel selectBusy;
        Dictionary<string, Control> uc = new Dictionary<string, Control>();
        Dictionary<string, int> ucTimer = new Dictionary<string, int>();
        #endregion

        #region 构造函数，初始化

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

        //保留注释
        public static void SaveAppSettingsMethod2(string key, string value)
        {
            XmlDocument xml = new XmlDocument();
            string configPath = Application.ExecutablePath + ".config";
            xml.Load(configPath);
            XmlNodeList nodeList = xml.GetElementsByTagName("appSettings");
            if (nodeList != null)
            {
                if (nodeList.Count >= 1)
                {
                    XmlNode node = nodeList[0];
                    foreach (XmlNode item in node)
                    {
                        if (item.NodeType == XmlNodeType.Comment)
                        {
                            continue;
                        }
                        XmlAttribute xaKey = item.Attributes["key"];
                        XmlAttribute xaValue = item.Attributes["value"];
                        if (xaKey != null && xaValue != null && xaKey.Value.Equals(key))
                        {
                            xaValue.Value = value;
                        }
                    }
                }
            }
            xml.Save(configPath);
        }

        protected override void WndProc(ref System.Windows.Forms.Message m)
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

        public frmMain()
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
            ucpnSelectUnitArea unit = new ucpnSelectUnitArea();
            unit.Size = new Size(1920, 1080);
            unit.Location = new Point(0, 0);
            ucpnSelectBusy busy = new ucpnSelectBusy();
            busy.Size = new Size(1920, 1080);
            busy.Location = new Point(0, 0);
            ucpnSelectBusyPhoto busyn = new ucpnSelectBusyPhoto();
            busyn.Size = new Size(1920, 1080);
            busyn.Location = new Point(0, 0);
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
            busyn.SelectedBusy += new Action(busyn_SelectedBusy);
            evaluate.enterEvaluate += new Action<BEvaluateModel>(evaluate_enterEvaluate);
            //evaluate.enter += new Action(evaluate_enter);
            evaluate.previous += new Action<object>(evaluate_previous);
            evaluate.next += new Action<object>(evaluate_previous);

            unit.previous += new Action<object>(unit_previous);
            unit.next += new Action<object>(unit_previous);
            busy.previous += new Action<object>(busy_previous);
            busy.next += new Action<object>(busy_previous);
            busyn.next += new Action<object>(busyn_previous);
            busyn.previous += new Action<object>(busyn_previous);
            pwd.Exit += new Action(pwd_Exit);

            pnMain.Controls.Add(pwd);
            pnMain.Controls.Add(evaluate);
            pnMain.Controls.Add(busy);
            pnMain.Controls.Add(busyn);
            pnMain.Controls.Add(unit);
            pnMain.Controls.Add(card);
            pnMain.Controls.Add(readCard);
            pnMain.Controls.Add(main);

            uc.Add("pwd", pwd);
            uc.Add("evaluate", evaluate);
            uc.Add("busy", busy);
            uc.Add("busyn", busyn);
            uc.Add("unit", unit);
            uc.Add("readcard", readCard);
            uc.Add("card", card);
            uc.Add("main", main);
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
            SetConfigValue("IP", "192.168.0.253");
            SetConfigValue("Port", "3347");
            SetConfigValue("MainTime", "20");//
            SetConfigValue("BroadcastInterval", "5");
            SetConfigValue("Units", "1,2,3,4,5|6,7,8,9");
            SetConfigValue("Position", "390,31|208,119|646,137|764,326|555,326#390,31|208,119|646,137|764,326|555,326");
            SetConfigValue("Size", "300,237,324,440,400|300,237,324,440");
            FloorImgCount = GetMaxFloorCount();
            MainTime = System.Configuration.ConfigurationManager.AppSettings["MainTime"];
            ip = System.Configuration.ConfigurationManager.AppSettings["IP"];
            port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            //页面停留时间 单位：秒
            ucTimer.Add("main", Convert.ToInt32(MainTime));
            ucTimer.Add("pwd", Convert.ToInt32(ExitTime));
            ucTimer.Add("evaluate", Convert.ToInt32(EvaluateTime));
            ucTimer.Add("appoint", Convert.ToInt32(AppointTime));
            ucTimer.Add("busy", Convert.ToInt32(BusyTime));
            ucTimer.Add("busyn", Convert.ToInt32(BusyTime));
            ucTimer.Add("unit", Convert.ToInt32(UnitTime));
            ucTimer.Add("readcard", Convert.ToInt32(ReadcardTime));
            ucTimer.Add("card", Convert.ToInt32(CardTime));
            pageStopTime = ucTimer["main"];
            uc["main"].BringToFront();
            timer3.Enabled = true;
            timer3.Start();
            if (CanNotUseCard == 0)
            {
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
            }
            AsyncGetBasic();
            client.ServerIP = ip;
            client.ServerPort = ushort.Parse(port);
            client.ClientType = ClientType.QueueClient;
            client.ClientName = ClientName;
            this.client.OnMessage += new Action<QueueMessage.Message>(client_OnMessage);
            client.Start();
        }
        void client_OnMessage(QueueMessage.Message obj)
        {
            
        }

        void AsyncGetBasic()
        {
            try
            {
                new AsyncWork(this).Start(act =>
                {
                    GetBasic();
                }, AsyncType.Loading);
            }
            catch (Exception ex)
            {
                frmMsg frm = new frmMsg();
                frm.msgInfo = "数据更新失败，请核查网络！";
                frm.ShowDialog();
            }
        }

        #endregion

        #region action 回调

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

        void evaluate_enterEvaluate(BEvaluateModel ev)
        {
            pageStopTime = ucTimer["evaluate"];
            if (SaveeEvaluate(ev))
            {
                frmThankMsg frm = new frmThankMsg();
                frm.ShowDialog();
            }
        }

        bool SaveeEvaluate(BEvaluateModel ev)
        {
            //ev.handleTime = DateTime.Now;
            //ev.custCardId = (person.idcard == null || person.idcard == "") ? "" : person.idcard;
            //ev.name = (person.name == null || person.name == "") ? "" : person.name;
            //var rStr = SaveEvaluate.Replace("@userCode", userCode);
            //rStr = rStr.Replace("@targetSeq", ev.controlSeq);
            //rStr = rStr.Replace("@evalutionValue", ev.evaluateResult.ToString());
            //var jString = http.HttpGet(rStr, "");
            //var sevaList = script.DeserializeObject(jString) as Dictionary<string, object>;
            //if (sevaList != null)
            //{
            //    var dQuery = sevaList["data"] as Dictionary<string, object>;
            //    if (dQuery == null)
            //    {
            //        frmMsg frm = new frmMsg();//无法评价
            //        frm.msgInfo = "评价信息保存失败，当前业务[" + ev.approveName + "]无法评价！";
            //        frm.ShowDialog();
            //        return false;
            //    }
            //}
            //else
            //{

            //    frmMsg frm = new frmMsg();//无法评价
            //    frm.msgInfo = "评价信息保存失败，当前业务[" + ev.approveName + "]无法评价！";
            //    frm.ShowDialog();
            //    return false;
            //}
            //eBll.Insert(ev);
            return true;
        }

        void appoint_previous(object sender)
        {
            //var ctl = ((ucpnAppointment)uc["appoint"]);
            //int max = appList.Count / ctl.pageCountA;
            //if ((appList.Count % ctl.pageCountA) > 0)
            //    max++;
            //PictureBox pb = sender as PictureBox;
            //if (pb.Name == "pnPrevious1")
            //{
            //    if (ctl.cureentPageA == 0)
            //        return;
            //    else
            //    {
            //        ctl.cureentPageA--;
            //        pageStopTime = ucTimer["appoint"];
            //    }
            //}
            //else
            //{
            //    if (max == ctl.cureentPageA + 1)
            //        return;
            //    else
            //    {
            //        ctl.cureentPageA++;
            //        pageStopTime = ucTimer["appoint"];
            //    }
            //}
            //ctl.CreateAppointment();
        }

        void appoint_enter()
        {
            #region
            //try
            //{
            //    var aList = appList.Where(a => a.isCheck).ToList();
            //    if (aList.Count == 0)
            //    {
            //        frmMsg frm = new frmMsg();//提示
            //        frm.msgInfo = "请先选择预约信息然后排队！";
            //        frm.ShowDialog();
            //        return;
            //    }
            //    var apList = new List<TAppointmentModel>();
            //    bool isError = false;
            //    foreach (var app in aList)
            //    {
            //        bool isIn = busyType == BusyType.Investment ? true : false;
            //        selectAppoomt = app;
            //        selectUnit = uList.Where(u => u.unitName == selectAppoomt.unitName && u.isInvestment == isIn).FirstOrDefault();
            //        if (selectUnit == null)
            //        {
            //            isError = true;
            //            continue;
            //        }
            //        selectBusy = bList.Where(b => b.unitName == selectUnit.unitName && b.busiName == selectAppoomt.busiName && b.isInvestment == isIn).FirstOrDefault();
            //        if (selectBusy == null)
            //        {
            //            isError = true;
            //            continue;
            //        }
            //        apList.Add(app);
            //    }
            //    if (isError)
            //    {
            //        frmMsg frm = new frmMsg();//提示
            //        frm.msgInfo = "存在部分数据部门/业务数据未找到，已忽略！";
            //        frm.ShowDialog();
            //    }
            //    foreach (var ap in apList)
            //    {
            //        if (!CheckLimit(selectUnit.unitSeq, selectBusy.busiSeq, true))
            //        {
            //            pbReturn_Click(null, null);
            //            return;
            //        }
            //        OutQueueNo(ap, "0", "");
            //    }
            //    pbReturn_Click(null, null);
            //}
            //catch (Exception ex)
            //{
            //    LogHelper.WriterQueueLog("预约取号出错：" + ex.Message);
            //    frmMsg frm = new frmMsg();//提示
            //    frm.msgInfo = "预约取号异常，请联系管理员！";
            //    frm.ShowDialog();
            //}
            #endregion
        }

        void appoint_other()
        {
            SelectUnit();
        }

        void unit_previous(object sender)
        {
            var ctl = ((ucpnSelectUnitArea)uc["unit"]);
            pageStopTime = ucTimer["unit"];
            int max = FloorImgCount;
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

        int GetMaxFloorCount()
        {
            var units = System.Configuration.ConfigurationManager.AppSettings["Units"].ToString().Split('|');
            return units.Count();
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

        void busyn_previous(object sender)
        {
            var ctl = ((ucpnSelectBusyPhoto)uc["busyn"]);
            pageStopTime = ucTimer["busyn"];
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
            var ucUnit = ((ucpnSelectUnitArea)uc["unit"]);
            selectUnit = ucUnit.selectUnit;
            var list = new List<TBusinessModel>();
            list = bList.Where(b => b.unitSeq == selectUnit.unitSeq).ToList();
            if (isPhoto(selectUnit.unitSeq))
            {
                var ucBusyn = ((ucpnSelectBusyPhoto)uc["busyn"]);
                ucBusyn.cureentPage = 0;
                ucBusyn.bList = list;
                ucBusyn.BringToFront();
                ucBusyn.CreateBusiness();
                pageStopTime = ucTimer["busyn"];
            }
            else
            {
                var ucBusy = ((ucpnSelectBusy)uc["busy"]);
                ucBusy.cureentPage = 0;
                ucBusy.bList = list;
                ucBusy.BringToFront();
                ucBusy.CreateBusiness();
                pageStopTime = ucTimer["busy"];
            }
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (busyType == BusyType.Work)
                pageLocation = PageLocation.WorkSelectBusy;
            else if (busyType == BusyType.Investment)
                pageLocation = PageLocation.InvestmentSelectBusy;
            else
                pageLocation = PageLocation.ConsultSelectBusy;

        }

        bool isPhoto(string unitSeq)
        {
            try
            {
                var path = AppDomain.CurrentDomain.BaseDirectory + "img\\ItemPhoto\\" + unitSeq;
                if (Directory.Exists(path))
                {
                    string[] files = Directory.GetFiles(path);
                    if (files.Length > 0)
                        return true;
                    else
                        return false;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        void busy_SelectedBusy()
        {
            if (selectUnit == null)
                return;
            selectBusy = ((ucpnSelectBusy)uc["busy"]).selectBusy;
            if (!CheckLimit(selectUnit.unitSeq, selectBusy.busiSeq))
            {
                pbReturn_Click(null, null);
                return;
            }
            OutQueueNo(); //出号
            pbReturn_Click(null, null);
        }
        void busyn_SelectedBusy()
        {
            if (selectUnit == null)
                return;
            selectBusy = ((ucpnSelectBusyPhoto)uc["busyn"]).selectBusy;
            if (!CheckLimit(selectUnit.unitSeq, selectBusy.busiSeq))
            {
                pbReturn_Click(null, null);
                return;
            }
            OutQueueNo(); //出号
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
            if (CanNotUseCard == 1)
            {
                SelectUnit();
            }
            else
            {
                GotoReadCard(0);// 
            }
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

        bool CheckLimit(string unitSeq, string busiSeq)
        {
            if (person != null && !string.IsNullOrEmpty(person.idcard))
            {
                //验证同一个身份证不能在一个部门一个业务排队2次（未处理的）
                var isCan = qBll.IsCanQueueO(person.idcard, busiSeq, unitSeq);
                if (Convert.ToBoolean(isCan[0]) == false)
                {
                    frmRePrint frm = new frmRePrint();
                    frm.qModel = isCan[1] as BQueueModel;
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
            if (max <= mList.Count)
            {
                frmMsg frm = new frmMsg();
                frm.msgInfo = "排队号已取完,若窗口空闲可直接到窗口办理！";
                frm.ShowDialog();
                return false;
            }
            return true;
        }

        void rePrint(BQueueModel queue)
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
            var isGreen = "";
            var att = baList.Where(b => b.unitSeq == selectUnit.unitSeq && b.busiSeq == selectBusy.busiSeq).FirstOrDefault();
            if (att != null)
            {
                isGreen = att.isGreenChannel == 1 ? "绿色通道" : "";
            }
            var list = qBll.GetModelList(selectBusy.busiSeq, selectUnit.unitSeq, 0).ToList().Where(q => q.ID < queue.ID).ToList();
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
            });
            if (queue.appType == 1 && queue.type == 0 && queue.reserveEndTime >= DateTime.Now && isGreen == "")
                isGreen = "网上预约";
            else if (queue.type == 1 && isGreen == "")
                isGreen = "网上申办";
            var isGetCard = queue.type == 2 ? "1" : "0";
            var serialNo = queue.type == 2 ? queue.reserveSeq : "";
            Print(queue, area, windowStr, waitNo, "补打", isGreen);
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
                SelectUnit();//
            }
            else if (busyType == BusyType.Evaluate)
            {
                //#region 评价读身份证
                //var requestStr = CheckUser.Replace("@paperCode", idNo);
                //var jsonString = http.HttpGet(requestStr, "");
                //var Card = script.DeserializeObject(jsonString) as Dictionary<string, object>;
                //if (Card != null)
                //{
                //    var status = Card["status"].ToString();
                //    var data = Card["data"] as Dictionary<string, object>;
                //    if (data == null)
                //    {
                //        frmMsg frm = new frmMsg();
                //        frm.msgInfo = "当前身份证未注册，获取不到用户编号！";
                //        frm.ShowDialog();
                //        return;
                //    }
                //    else
                //    {
                //        var uCode = data["userCode"] == null ? "" : data["userCode"].ToString();
                //        var exist = data["exist"].ToString();
                //        if (exist == "0")
                //        {
                //            frmMsg frm = new frmMsg();
                //            frm.msgInfo = "当前身份证未注册平台账户！";
                //            frm.ShowDialog();
                //            return;
                //        }
                //        userCode = uCode;
                //        var rStr = GetEvaluate.Replace("@custCardId", idNo);
                //        var jString = http.HttpGet(rStr, "");
                //        var evaList = script.DeserializeObject(jString) as Dictionary<string, object>;
                //        if (evaList != null)
                //        {
                //            var dQuery = evaList["data"] as Dictionary<string, object>;
                //            var dArr = dQuery["dataList"] as object[];
                //            if (dArr == null || dArr.Count() == 0)
                //            {
                //                frmMsg frm = new frmMsg();
                //                frm.msgInfo = "该身份证用户没有可评价的业务信息！";
                //                frm.ShowDialog();
                //                return;
                //            }
                //            else
                //            {
                //                eList = new List<BEvaluateModel>();
                //                foreach (Dictionary<string, object> dat in dArr)
                //                {
                //                    var controlSeq = dat["controlSeq"] == null ? "" : dat["controlSeq"].ToString();
                //                    var approveName = dat["approveName"] == null ? "" : dat["approveName"].ToString();
                //                    var approveSeq = dat["approveSeq"] == null ? "" : dat["approveSeq"].ToString();
                //                    var unitSeq = dat["unitSeq"] == null ? "" : dat["unitSeq"].ToString();
                //                    var unitName = dat["unitName"] == null ? "" : dat["unitName"].ToString();
                //                    eList.Add(new BEvaluateModel()
                //                    {
                //                        type = 0,
                //                        approveName = approveName,
                //                        controlSeq = controlSeq,
                //                        approveSeq = approveSeq,
                //                        custCardId = idCard,
                //                        unitSeq = unitSeq,
                //                        unitName = unitName,
                //                    });

                //                }
                //                ShowEvaluate();
                //            }
                //        }

                //    }
                //}
                //#endregion
            }
            Start(false);
        }

        #endregion

        #region 回退、主页
        //主页
        private void pbReturn_Click(object sender, EventArgs e)
        {
            var sUnit = ((ucpnSelectUnitArea)uc["unit"]);
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
            selectBusy = null;
            selectUnit = null;
            busyType = BusyType.Default;
            pageLocation = PageLocation.Main;
            pageStopTime = ucTimer["main"]; ;
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
                if (CanNotUseCard == 1)
                {
                    pbReturn_Click(null, null);
                }
                else
                {
                    Work();
                }
            }
            else if (pageLocation == PageLocation.GetCardInputCard)
            {
                GetCardAction();
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
                CVRSDK.CVR_CloseComm();
            }
            catch
            {

            }
        }

        #endregion

        #region 更新基础数据

        private void timer1_Tick(object sender, EventArgs e)
        {
            AsyncGetBasic();
        }

        private void GetBasic()
        {
            waList = waBll.GetModelList();
            wList = wBll.GetModelList();
            baList = baBll.GetModelList();
            wbList = wbBll.GetModelList();
            uList = uBll.GetModelList();
            bList = bBll.GetModelList();
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
            //var appointment = ((ucpnAppointment)uc["appoint"]);
            //appointment.appList = appList;
            //appointment.cureentPageA = 0;
            //appointment.CreateAppointment();
            //appointment.BringToFront();
            //pbReturnMain.BringToFront();
            //pbLastPage.BringToFront();
            //if (busyType == BusyType.Investment)
            //    pageLocation = PageLocation.InvestmentAppointment;
            //else
            //    pageLocation = PageLocation.WorkAppointment;
            //pageStopTime = ucTimer["appoint"];
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
            var ucUnit = ((ucpnSelectUnitArea)uc["unit"]);
            ucUnit.uList = uList;
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

        #region  出号、出票
        private void OutQueueNo()
        {
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
            var queue = InsertQueue(ticketStart);
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
            });
            Print(queue, area, windowStr, waitNo, "", isGreen);
        }

        //出票
        private void Print(BQueueModel model, string area, string windowStr, int wait, string flag, string vip)
        {
            try
            {
                DataTable table = GetQueue(model, area, windowStr, wait, flag, vip);
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
        private DataTable GetQueue(BQueueModel model, string area, string windowStr, int wait, string flag, string vip)
        {
            DataTable table = new DataTable("table");
            table.Columns.AddRange(new DataColumn[] 
            {
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
            row["area"] = area;
            row["windowStr"] = windowStr;
            row["waitCount"] = wait.ToString();
            row["unitName"] = model.unitName;
            row["busyName"] = model.busTypeName;
            row["ticketNumber"] = model.ticketNumber;
            row["flag"] = flag;
            row["cardId"] = string.IsNullOrEmpty(model.idCard) ? "" : model.idCard.Length > 6 ? model.idCard.Substring(model.idCard.Length - 6, 6) : model.idCard;
            //row["reserveSeq"] = model.reserveSeq;
            row["vip"] = vip;
            table.Rows.Add(row);
            return table;
        }
        //插入排队数据
        private BQueueModel InsertQueue(string ticketStart)
        {
            string idCard = person == null ? "" : person.idcard;
            string qNmae = person == null ? "" : person.name;
            var line = qBll.QueueLine(selectBusy, selectUnit, ticketStart, idCard, qNmae);
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
        frmBroadcast frmB = new frmBroadcast();
        //页面停留
        private void timer3_Tick(object sender, EventArgs e)
        {
            if (pageStopTime <= 1)
            {
                if (pageLocation == PageLocation.Main)
                {
                    if (!frmB.Visible)
                    {
                        frmB.ShowDialog();
                        pageStopTime = ucTimer["main"]; ;
                    }
                }
                else
                {
                    pbReturn_Click(null, null);
                }
            }
            else
            {
                lblMes.Visible = true;
                lblMes.Text = string.Format("剩余操作时间：{0}秒", pageStopTime.ToString("00"));
                pageStopTime--;
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
