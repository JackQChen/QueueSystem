using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using BLL;
using Model;
using ReportManager;

namespace QueueClient
{
    public partial class frmMain2 : Form
    {
        PageLocation pageLocation;
        BusyType busyType;
        Person person;
        int pageCount = 4;//评价数据一页显示4个
        int cureentPage = 0;//评价数据页码。从1开始
        int pageCountA = 6;
        int cureentPageA = 0;
        bool suppend = true;
        int iRetUSB = 0;
        string idCard = "";//身份证号码
        string mobilePhone = "";//手机号码
        string userCode = "";//
        string inputPwd;
        JavaScriptSerializer script = new JavaScriptSerializer();
        string areaSeq = System.Configuration.ConfigurationManager.AppSettings["areaSeq"];
        string GetAppointmentByID = System.Configuration.ConfigurationManager.AppSettings["GetAppointmentByID"];
        string GetUserByID = System.Configuration.ConfigurationManager.AppSettings["GetUserByID"];//暂停使用
        string CheckUser = System.Configuration.ConfigurationManager.AppSettings["CheckUser"];
        string GetUnit = System.Configuration.ConfigurationManager.AppSettings["GetUnit"];
        string GetUnitBusiness = System.Configuration.ConfigurationManager.AppSettings["GetUnitBusiness"];
        string RegisterUser = System.Configuration.ConfigurationManager.AppSettings["RegisterUser"]; //暂停使用
        string UpdateAppoint = System.Configuration.ConfigurationManager.AppSettings["UpdateAppoint"];
        string GetCard = System.Configuration.ConfigurationManager.AppSettings["GetCard"];
        string AppointmentOnline = System.Configuration.ConfigurationManager.AppSettings["AppointmentOnline"];
        string ExitPwd = System.Configuration.ConfigurationManager.AppSettings["ExitPwd"];
        string Sencod = System.Configuration.ConfigurationManager.AppSettings["Sencod"];
        string GetEvaluate = System.Configuration.ConfigurationManager.AppSettings["GetEvaluate"];
        string SaveEvaluate = System.Configuration.ConfigurationManager.AppSettings["SaveEvaluate"];
        string GetUserByUserCode = System.Configuration.ConfigurationManager.AppSettings["GetUserByUserCode"];
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
        Thread thread;
        Thread exit;
        Thread wait;
        TUnitModel selectUnit;
        TBusinessModel selectBusy;
        TAppointmentModel selectAppoomt;
        string appPhone = "13555555555";
        string appName = "李青云";

        #region 构造函数，初始化，读卡

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

        public frmMain2()
        {
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | //不擦除背景 ,减少闪烁
            ControlStyles.OptimizedDoubleBuffer | //双缓冲
            ControlStyles.UserPaint, //使用自定义的重绘事件,减少闪烁
            true);

            InitializeComponent();
            pnAppointment.Size = new Size(1920, 1080);
            pnAppointment.Location = new Point(0, 0);
            pnReadCard.Size = new Size(1920, 1080);
            pnReadCard.Location = new Point(0, 0);
            pnEvaluate.Size = new Size(1920, 1080);
            pnEvaluate.Location = new Point(0, 0);
            pnPwd.Size = new Size(1920, 1080);
            pnPwd.Location = new Point(0, 0);
            pnMain.Size = new Size(1920, 1080);
            pnMain.Location = new Point(0, 0);
           
            pncard.Size = new Size(1920, 1080);
            pncard.Location = new Point(0, 0);
            pnSelectUnit.Size = new Size(1920, 1080);
            pnSelectUnit.Location = new Point(0, 0);
            pnSelectBusy.Size = new Size(1920, 1080);
            pnSelectBusy.Location = new Point(0, 0);
           
          
            pbReturnMain.Location = new Point(200, 75);
            pbLastPage.Location = new Point(1550, 75);
          
            busyType = BusyType.Default;
        }

        private void frmMain_Load(object sender, EventArgs e)
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
            new AsyncWork(this).Start(act =>
            {
                GetBasic();
                GetUnitAndBusiness();
                this.Invoke(new Action(() =>
                {
                    CreateUnit();
                    pnMain.BringToFront();
                }));
            }, AsyncType.Loading);

        }

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
                            this.Invoke(new Action(() => { DrawCard(idCard); }));
                            //this.Invoke(new Action(() => { txtCard.Text = iCard; }));
                            Start(false);
                            LogHelper.WriterReadIdCardLog(string.Format("身份证读卡成功：本次读卡循环读取了{3}次，证件号码{0} 姓名{1} 地址{2}", idCard, iName, iAdress, time));
                            continue;
                        }
                        #endregion
                    }
                    else
                        LogHelper.WriterReadIdCardLog("身份证读卡miss:读卡失败");
                }
                else
                    LogHelper.WriterReadIdCardLog("身份证读卡miss:无卡");
                Thread.Sleep(100);
            }
        }

        private void DrawCard(string idNo)
        {
            var pen = pnWriterCard.CreateGraphics();
            Font font = new Font("黑体", 40, FontStyle.Bold);
            pen.DrawString(idNo, font, new SolidBrush(Color.White), 5, 3);
            wait = new Thread(new ThreadStart(Wait));
            wait.IsBackground = true;
            wait.Start();
        }

        private void Wait()
        {
            int index = 0;
            while (true)
            {
                if (index > 5)
                {
                    this.Invoke(new Action(() => { ProcessIdCard(idCard); }));
                    break;
                }
                index++;
                Thread.Sleep(500);
            }
        }

        #endregion

        #region 主界面菜单 以及二级菜单

        #region 办事
        private void pbWork_Click(object sender, EventArgs e)
        {
            Start(false);
            busyType = BusyType.Work;
            GotoReadCard(0);// 
        }

        #region 这两个功能废弃
        private void pbPersonal_Click(object sender, EventArgs e)
        {
            GotoReadCard(0);
            //pnReadCard.BringToFront();
            //pbReturnMain.BringToFront();
            //pbLastPage.BringToFront();
            //pageLocation = PageLocation.WorkReadCard;
            //txtCard.Text = "";
            //idCard = "";
            //txtCard.Focus();
            //person = new Person();
            //Start(true);

            //Start(false);
            //pncard.BringToFront();
            //pbReturnMain.BringToFront();
            //pbLastPage.BringToFront();
            //pageLocation = PageLocation.WorkInputIdCard;

        }

        private void pbCompany_Click(object sender, EventArgs e)
        {

        }
        #endregion
        #endregion
        #region 领卡
        private void pbGetCard_Click(object sender, EventArgs e)
        {
            busyType = BusyType.GetCard;
            GotoReadCard(2);
            //this.pncard.BringToFront();
            //this.pbReturnMain.BringToFront();
            //this.pbLastPage.BringToFront();
            //pageLocation = PageLocation.GetCardSecond;
            //txtCard.Text = "";
            //idCard = "";
            //person = new Person();
            //Start(true);
        }
        #endregion
        #region  咨询
        private void pbConsult_Click(object sender, EventArgs e)
        {
            busyType = BusyType.Consult;
            SelectUnit();
            //this.pnConsult.BringToFront();
            //this.pbReturnMain.BringToFront();
            //this.pbLastPage.BringToFront();
            //pageLocation = PageLocation.ConsultSecond;
            //busyType = BusyType.Consult;
        }
        private void pbConsultByUser_Click(object sender, EventArgs e)
        {
            SelectUnit();
        }
        #endregion
        #region 评价
        private void pbEvaluate_Click(object sender, EventArgs e)
        {
            busyType = BusyType.Evaluate;
            GotoReadCard(1);
            //pageLocation = PageLocation.EvaluateReadCard;
            //pncard.BringToFront();
            //pbReturnMain.BringToFront();
            //pbLastPage.BringToFront();
            //txtCard.Text = "";
            //idCard = "";
            //txtCard.Focus();
            //person = new Person();
            //Start(true);
        }
        #endregion

        #region 页面跳转

        /// <summary>
        /// 刷身份证type0：办事读卡 1：评价读卡 2：领卡读卡
        /// </summary>
        /// <param name="type"></param>
        private void GotoReadCard(int type)
        {
            pnReadCard.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (type == 0)
                pageLocation = PageLocation.WorkReadCard;//办事读卡
            else if (type == 1)
                pageLocation = PageLocation.EvaluateReadCard;//评价读卡
            else if (type == 2)
                pageLocation = PageLocation.GetCardReadCard;//领卡读卡
            txtCard.Text = "";
            idCard = "";
            txtCard.Focus();
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
            pncard.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            txtCard.Text = "";
            idCard = "";
            txtCard.Focus();
            if (type == 0)
                pageLocation = PageLocation.WorkInputIdCard;//办事
            else if (type == 1)
                pageLocation = PageLocation.EvaluateInputCard;//评价读卡
            else if (type == 2)
                pageLocation = PageLocation.GetCardInputCard;//领卡读卡

        }

        /// <summary>
        /// 身份证读卡或者输入卡 后续处理
        /// </summary>
        /// <param name="idNo"></param>
        private void ProcessIdCard(string idNo)
        {
            if (person == null || person.idcard == null || person.idcard.ToString() == "")
            {
                person = new Person();
                person.idcard = idNo;
            }
            if (busyType == BusyType.Work)
            {
                #region 办事 读身份证

                var requestStr = GetAppointmentByID.Replace("@paperCode", idNo);
                var jsonString = http.HttpGet(requestStr, "");
                var Appointment = script.DeserializeObject(jsonString) as Dictionary<string, object>;
                if (Appointment != null)
                {
                    var status = Appointment["status"].ToString();
                    var dataQuery = Appointment["data"] as Dictionary<string, object>;
                    var dataArr = dataQuery["dataList"] as object[];
                    if (dataArr == null || dataArr.Count() == 0)
                    {
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
                        SelectUnit();
                    }
                    else
                    {
                        #region 有预约
                        #region
                        appList = new List<TAppointmentModel>();
                        foreach (Dictionary<string, object> data in dataArr)
                        {
                            var busiCode = data["busiCode"] == null ? "" : data["busiCode"].ToString();
                            var reserveSeq = data["reserveSeq"] == null ? "" : data["reserveSeq"].ToString();
                            var busiName = data["busiName"] == null ? "" : data["busiName"].ToString();
                            var userName = data["userName"] == null ? "" : data["userName"].ToString();
                            var paperType = data["paperType"] == null ? "" : data["paperType"].ToString();// 10 为身份证
                            var paperCode = data["paperCode"] == null ? "" : data["paperCode"].ToString();
                            var mobilePhone = data["mobilePhone"] == null ? "" : data["mobilePhone"].ToString();
                            var comName = data["comName"] == null ? "" : data["comName"].ToString();
                            var reserveDate = data["reserveDate"] == null ? "" : data["reserveDate"].ToString();
                            var reserveStartTime = data["reserveStartTime"] == null ? "" : data["reserveStartTime"].ToString();
                            var reserveEndTime = data["reserveEndTime"] == null ? "" : data["reserveEndTime"].ToString();
                            var approveSeq = data["approveSeq"] == null ? "" : data["approveSeq"].ToString();
                            var approveName = data["approveName"] == null ? "" : data["approveName"].ToString();
                            var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                            var unitCode = data["unitCode"] == null ? "" : data["unitCode"].ToString();
                            TAppointmentModel app = new TAppointmentModel();
                            app.appType = 1;
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
                            app.userName = userName;
                            app.isCheck = false;
                            appList.Add(app);
                        }
                        #endregion

                        ShowAppointment();
                        #region
                        //if (appList.Count == 1)
                        //{
                        //    #region
                        //    selectAppoomt = appList[0]; //目前默认取第一条直接出号，暂时不做让选择预约号功能
                        //    selectUnit = uList.Where(u => u.unitName == selectAppoomt.unitName).FirstOrDefault();
                        //    selectBusy = bList.Where(b => b.busiName == selectAppoomt.busiName).FirstOrDefault();
                        //    if (selectUnit == null || selectBusy == null)
                        //    {
                        //        frmMsg frm = new frmMsg();//提示
                        //        frm.msgInfo = "预约的部门/业务不存在！";
                        //        frm.ShowDialog();
                        //        return;
                        //    }
                        //    //出号
                        //    OutQueueNo(selectAppoomt);
                        //    #endregion
                        //}
                        //else
                        //{
                        //    #region
                        //    #endregion
                        //}
                        #endregion
                        #endregion
                    }
                }
                else
                {
                    frmMsg frm = new frmMsg();//提示
                    frm.msgInfo = "获取用户预约接口错误！";
                    frm.ShowDialog();
                    return;
                }

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
                        var busiCode = data["approveSeq"] == null ? "" : data["approveSeq"].ToString();
                        var busiName = data["approveName"] == null ? "" : data["approveName"].ToString();
                        var unitSeq = data["unitSeq"] == null ? "" : data["unitSeq"].ToString();
                        var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                        var controlSeq = data["controlSeq"] == null ? "" : data["controlSeq"].ToString();
                        var beginDate = data["beginDate"] == null ? "" : data["beginDate"].ToString();
                        selectUnit = uList.Where(u => u.unitSeq == unitSeq).FirstOrDefault();
                        if (selectUnit != null)
                        {
                            selectBusy = bList.Where(b => b.busiSeq == busiCode).FirstOrDefault();
                            if (selectBusy != null)
                            {
                                TGetCardModel model = new TGetCardModel();
                                model.controlSeq = controlSeq;
                                model.unitName = unitName;
                                model.unitSeq = unitSeq;
                                model.busTypeSeq = busiCode;
                                model.busTypeName = busiName;
                                model.custCardId = idNo;
                                model.outCardTime = DateTime.Now;
                                gBll.Insert(model);
                                OutQueueNo(null);
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
            Start(false);
            pnMain.BringToFront();
            cureentPage = 0;
            idCard = "";
            txtCard.Text = "";
         
            mobilePhone = "";
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
            if (pageLocation == PageLocation.WorkReadCard || pageLocation == PageLocation.EvaluateReadCard || pageLocation == PageLocation.GetCardReadCard || pageLocation == PageLocation.ConsultSelectUnit)
            {
                pbReturn_Click(null, null);
            }
            else if (pageLocation == PageLocation.WorkInputIdCard)
            {
                pbPersonal_Click(null, null);
            }
            else if (pageLocation == PageLocation.GetCardInputCard)
            {
                pbGetCard_Click(null, null);
            }
            else if (pageLocation == PageLocation.EvaluateInputCard)
            {
                pbEvaluate_Click(null, null);
            }
            //else if (pageLocation == PageLocation.WorkInputPhone)
            //{
            //    pbPersonal_Click(null, null);
            //}
            else if (pageLocation == PageLocation.WorkSelectUnit)
            {
                pbPersonal_Click(null, null); //InsertPhone();
            }
            else if (pageLocation == PageLocation.WorkSelectBusy)
            {
                SelectUnit();
            }
            else if (pageLocation == PageLocation.ConsultSelectBusy)
            {
                SelectUnit();
            }
            else if (pageLocation == PageLocation.Evaluate)
            {
                pbEvaluate_Click(null, null);
            }
            else if (pageLocation == PageLocation.WorkAppointment)
            {
                pbPersonal_Click(null, null);
            }
        }
        #endregion

        #region 退出密码框

        private void pnexit_Click(object sender, EventArgs e)
        {
            inputPwd = "";
            pnPwd.BringToFront();
            exit = new Thread(new ThreadStart(ExitInputPwd));
            exit.IsBackground = true;
            exit.Start();
        }

        private void ExitInputPwd()
        {
            int max = Convert.ToInt32(Sencod);
            int index = 0;
            while (true)
            {
                if (index < max)
                {
                    Thread.Sleep(1000);
                }
                else
                {
                    inputPwd = "";
                    this.Invoke(new Action(() => { pnPwd.SendToBack(); }));
                    break;
                }
                index++;
            }
        }

        private void pd_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            string No = pb.Name.Substring(2, 1);
            if (No == "f")
            {
                if (inputPwd != ExitPwd)
                {
                    frmMsg frm = new frmMsg();
                    frm.msgInfo = "退出密码不正确！";
                    frm.ShowDialog();
                }
                else
                {
                    ExitThread();
                    Application.ExitThread();
                }
            }
            else
            {
                inputPwd += No;
            }
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
                this.Invoke(new Action(() =>
                {
                    CreateUnit();
                }));
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
            uList.Clear();
            bList.Clear();
            var unitList = uBll.GetModelList();
            var businessList = bBll.GetModelList();
            var areaStr = GetUnit.Replace("@areaSeq", areaSeq);
            var unitString = http.HttpGet(areaStr, "");
            var units = script.DeserializeObject(unitString) as Dictionary<string, object>;
            if (units != null)
            {
                var dataArr = units["data"] == null ? null : units["data"] as object[];
                foreach (var item in dataArr)
                {
                    var data = item as Dictionary<string, object>;
                    if (data != null)
                    {
                        var unitSeq = data["unitSeq"] == null ? "" : data["unitSeq"].ToString();
                        var unitName = data["unitName"] == null ? "" : data["unitName"].ToString();
                        TUnitModel unit = new TUnitModel { unitSeq = unitSeq, unitName = unitName };
                        if (unitList.Where(u => u.unitSeq == unitSeq && u.unitName == unitName).Count() == 0)
                        {
                            uBll.Insert(unit);
                        }
                        uList.Add(unit);
                    }
                }
            }
            foreach (var uSeq in uList)
            {
                var busyStr = GetUnitBusiness.Replace("@unitSeq", uSeq.unitSeq);
                var businessString = http.HttpGet(busyStr, "");
                var business = script.DeserializeObject(businessString) as Dictionary<string, object>;
                if (business != null)
                {
                    var data = business["data"] == null ? null : business["data"] as Dictionary<string, object>;
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
                            TBusinessModel buss = new TBusinessModel
                            {
                                acceptBusi = Convert.ToBoolean(Convert.ToInt32(acceptBusi)),
                                busiCode = busiCode,
                                busiSeq = busiSeq,
                                askBusi = Convert.ToBoolean(Convert.ToInt32(askBusi)),
                                busiName = busiName,
                                busiType = busiType,
                                getBusi = Convert.ToBoolean(Convert.ToInt32(getBusi)),
                                unitSeq = uSeq.unitSeq,
                                unitName = uSeq.unitName,
                            };
                            if (businessList.Where(b => b.unitSeq == uSeq.unitSeq && b.busiCode == busiCode && b.busiName == busiName).Count() == 0)
                            {
                                bBll.Insert(buss);
                            }
                            bList.Add(buss);
                        }
                    }
                }
            }
        }
        #endregion

        #region 动态创建：部门、业务、 预约列表、可评价列表

        //动态创建部门
        private void CreateUnit()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            Console.WriteLine(sw.ElapsedTicks);
            this.pnUnit.Controls.Clear();
            int count = 0;
            int sX = 25;//起始坐标
            int sY = 25;//起始坐标
            int height = 50;//一行高度
            int width = 490;
            int currY = 0;
            Console.WriteLine(sw.ElapsedTicks);
            foreach (var u in uList)
            {
                PictureBox pb = new PictureBox();
                pb.Name = "pb_u_" + count;
                pb.Tag = u;
                pb.Cursor = Cursors.Hand;
                pb.Image = Properties.Resources.button_1;
                pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                pb.Size = new Size(width, height);
                pb.Click += new EventHandler(pb_Click);
                pb.Paint += new PaintEventHandler(pb_Paint);
                pb.MouseDown += pb_MouseDown;
                pb.MouseUp += pb_MouseUp;
                if (count % 3 == 0)
                {
                    pb.Location = new Point(sX, sY + currY);
                }
                else if (count % 3 == 1)
                {
                    int x = sX + width + 5;
                    pb.Location = new Point(x, sY + currY);
                }
                else if (count % 3 == 2)
                {
                    int x = sX + width + 5 + width + 5;
                    pb.Location = new Point(x, sY + currY);
                    currY += (sY + height);
                }
                pnUnit.Controls.Add(pb);
                count++;
            }
            Console.WriteLine(sw.ElapsedTicks);
            pnUnit.ResumeLayout();
            Console.WriteLine(sw.ElapsedTicks);

        }
        //动态创建业务
        private void CreateBusiness()
        {
            this.pnBusiness.Controls.Clear();
            int count = 0;
            int sX = 25;//起始坐标
            int sY = 25;//起始坐标
            int height = 50;//一行高度
            int width = 490;
            int currY = 0;
            var busList = bList.Where(b => b.unitSeq == selectUnit.unitSeq).ToList();
            foreach (var u in busList)
            {
                PictureBox pb = new PictureBox();
                pb.Name = "pb_b_" + count;
                pb.Tag = u;
                pb.Cursor = Cursors.Hand;
                pb.Image = Properties.Resources.button_1;
                pb.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
                pb.Size = new Size(width, height);
                pb.Click += new EventHandler(pbu_Click);
                pb.Paint += new PaintEventHandler(pbu_Paint);
                pb.MouseDown += pb_MouseDown;
                pb.MouseUp += pb_MouseUp;
                if (count % 3 == 0)
                {
                    pb.Location = new Point(sX, sY + currY);
                }
                else if (count % 3 == 1)
                {
                    int x = sX + width + 5;
                    pb.Location = new Point(x, sY + currY);
                }
                else if (count % 3 == 2)
                {
                    int x = sX + width + 5 + width + 5;
                    pb.Location = new Point(x, sY + currY);
                    currY += (sY + height);
                }
                pnBusiness.Controls.Add(pb);
                count++;
            }
            pnBusiness.ResumeLayout();
        }
        //动态创建预约列表
        private void CreateAppointment()
        {
            this.pnAppointmentMain.Controls.Clear();
            int count = 0;
            int sX = 30;//起始坐标
            int sY = 50;//起始坐标
            int jj = 4;//间距
            int currY = 0;
            var list = appList.Skip(pageCountA * cureentPageA).Take(pageCountA);
            foreach (var e in list)
            {
                AppointmentCard card = new AppointmentCard();
                card.model = e;
                card.action += UpdateAppointment;
                card.Location = new Point(sX, sY + currY);
                currY += (jj + card.Height);
                pnAppointmentMain.Controls.Add(card);
                count++;
            }
            pnAppointmentMain.ResumeLayout();
        }
        private void UpdateAppointment(TAppointmentModel model)
        {
            var ev = appList.Where(l => l.reserveSeq == model.reserveSeq).ToList().FirstOrDefault();
            if (ev != null)
                ev.isCheck = model.isCheck;
        }
        //动态创建可评价列表
        private void CreateEvaluate()
        {
            this.pnEvaluateMain.Controls.Clear();
            int count = 0;
            int sX = 30;//起始坐标
            int sY = 50;//起始坐标
            int jj = 4;//间距
            int currY = 0;
            var list = eList.Skip(pageCount * cureentPage).Take(pageCount);
            foreach (var e in list)
            {
                EvaluateCard card = new EvaluateCard();
                card.model = e;
                card.action += UpdateEvaluate;
                card.Location = new Point(sX, sY + currY);
                currY += (jj + card.Height);
                pnEvaluateMain.Controls.Add(card);
                count++;
            }
            pnEvaluateMain.ResumeLayout();
        }
        //委托更新评价标识
        private void UpdateEvaluate(TEvaluateModel model)
        {
            var ev = eList.Where(l => l.controlSeq == model.controlSeq).ToList().FirstOrDefault();
            if (ev != null)
                ev.evaluateResult = model.evaluateResult;
        }
        //默认图标显示
        void pb_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.button_1;
        }
        //按下图标显示
        void pb_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.button_2;
        }
        //部门绘制 
        void pb_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            TUnitModel unit = pb.Tag as TUnitModel;
            Font font = new Font("黑体", 22, FontStyle.Bold);
            e.Graphics.DrawString(unit.unitName, font, new SolidBrush(Color.White), 10, 7);
        }
        //业务绘制
        void pbu_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            TBusinessModel busy = pb.Tag as TBusinessModel;
            Font font = new Font("黑体", 22, FontStyle.Bold);
            e.Graphics.DrawString(busy.busiName, font, new SolidBrush(Color.White), 10, 7);
        }
        //选择部门
        void pb_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            TUnitModel unit = pb.Tag as TUnitModel;
            selectUnit = unit;
            CreateBusiness();
            SelectBusy();
        }
        //选择业务
        void pbu_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            TBusinessModel busy = pb.Tag as TBusinessModel;
            selectBusy = busy;
            if (busyType == BusyType.Work)
            {
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
                            var userList = data["userInfo"] as object[];
                            var uinfo = userList.FirstOrDefault() as Dictionary<string, object>;
                            if (uinfo != null)
                            {
                                zName = uinfo["name"].ToString();
                                zPhone = uinfo["mobilePhone"].ToString();
                            }
                        }
                    }
                }
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
                }
            }
            OutQueueNo(null); //出号
            pbReturn_Click(null, null);
        }
        //输入手机号码
        void pbInputPhone_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("输入手机号码", font, new SolidBrush(Color.White), 275, 15);
        }
        //刷身份证或输入身份证号码
        void pbInputCard_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("请输入您本人的真实身份证号码", font, new SolidBrush(Color.White), 60, 15); //刷身份证或输入身份证号码
        }

        #endregion

        #region 输入身份证和手机号
        //数字键盘绘制
        private void pb1_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            string No = pb.Name.Substring(2, 1);
            Font font = new Font("黑体", 65, FontStyle.Bold);
            if (No == "b")
            {
                e.Graphics.DrawString("退格", font, new SolidBrush(Color.White), 25, 17);
            }
            else if (No == "f")
            {
                if (pb.Name.Substring(1, 1) == "t")
                    e.Graphics.DrawString("确定", font, new SolidBrush(Color.White), 10, 17);
                else if (pb.Name.Substring(1, 1) == "d")
                    e.Graphics.DrawString("确定", font, new SolidBrush(Color.White), 180, 17);
                else
                    e.Graphics.DrawString("确定", font, new SolidBrush(Color.White), 350, 17);
            }
            else
            {
                e.Graphics.DrawString(No, font, new SolidBrush(Color.White), 80, 17);
            }
        }
        //身份证输入
        private void pb1_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            string No = pb.Name.Substring(2, 1);
            if (No == "b")
            {
                if (txtCard.Text.Length > 0 && txtCard.Text.Length <= 18)
                {
                    txtCard.Text = txtCard.Text.Substring(0, txtCard.Text.Length - 1);
                    txtCard.SelectionStart = txtCard.Text.Length;
                }
            }
            else if (No == "f")
            {
                //完成
                if (txtCard.Text.Length != 18)
                {
                    frmMsg frm = new frmMsg();//提示
                    frm.msgInfo = "身份证号码格式不正确！";
                    frm.ShowDialog();
                    return;
                }
                else
                {
                    idCard = txtCard.Text;
                    ProcessIdCard(idCard);
                }
            }
            else
            {
                if (txtCard.Text.Length < 18)
                {
                    txtCard.Text = txtCard.Text + No;
                    txtCard.SelectionStart = txtCard.Text.Length;
                }
            }

        }
       
        #endregion

        #region  显示预约列表、显示评价列表、选择部门、选择业务
        private void ShowAppointment()
        {
            CreateAppointment();
            this.pnAppointment.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            pageLocation = PageLocation.WorkAppointment;
        }
        private void ShowEvaluate()
        {
            CreateEvaluate();
            this.pnEvaluate.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            pageLocation = PageLocation.Evaluate;
        }
        private void SelectUnit()
        {
            Start(false);
            this.pnSelectUnit.BringToFront(); //选择 部门，选择业务， 取号
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
        }
        
        private void SelectBusy()
        {
            pnSelectBusy.BringToFront();
            pbReturnMain.BringToFront();
            pbLastPage.BringToFront();
            if (busyType == BusyType.Work)
                pageLocation = PageLocation.WorkSelectBusy;
            else
                pageLocation = PageLocation.ConsultSelectBusy;
        }
        #endregion

        #region  出号
        private void OutQueueNo(TAppointmentModel app)
        {
            try
            {
                if (app != null)
                {
                    selectAppoomt = app; //目前默认取第一条直接出号，暂时不做让选择预约号功能
                    selectUnit = uList.Where(u => u.unitName == selectAppoomt.unitName).FirstOrDefault();
                    selectBusy = bList.Where(b => b.unitName == selectUnit.unitName && b.busiName == selectAppoomt.busiName).FirstOrDefault();
                }
                //验证业务扩展属性
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
                var lineList = lineBll.GetModelList();
                var line = lineList.Where(l => l.unitSeq == selectUnit.unitSeq && l.busiSeq == selectBusy.busiSeq).FirstOrDefault();
                var queue = InsertQueue(line, app, ticketStart);
                var area = "";
                var windowBusy = wbList.Where(w => w.unitSeq == selectUnit.unitSeq && w.busiSeq == selectBusy.busiSeq).FirstOrDefault();
                if (windowBusy != null)
                {
                    var window = wList.Where(w => w.ID == windowBusy.WindowID).FirstOrDefault();
                    if (window != null)
                    {
                        var windoware = waList.Where(w => w.id == window.AreaName).FirstOrDefault();
                        if (windoware != null)
                        {
                            area = windoware.areaName;
                        }
                    }
                }
                //通过业务找区域
                LogHelper.WriterQueueLog(string.Format("已出票：部门[{0}]，业务[{1}]，票号[{2}]，预约号[{3}]，身份证号[{4}]，姓名[{5}]，时间[{6}]。",
                    queue.unitName, queue.busTypeName, queue.ticketNumber, app == null ? "" : app.reserveSeq, person.idcard, person.name, DateTime.Now));
                Print(queue, area, waitNo, app == null ? "" : app.reserveSeq);
            }
            catch (Exception ex)
            {
                LogHelper.WriterQueueLog(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":出票过程中发生错误," + ex.Message);
                frmMsg frm = new frmMsg();//提示
                frm.msgInfo = "出票过程中发生错误，请联系管理员！";
                frm.ShowDialog();
            }
        }

        //出票
        private void Print(TQueueModel model, string area, int wait, string reserveSeq)
        {
            try
            {
                DataTable table = GetQueue(model, area, wait, reserveSeq);
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
        private DataTable GetQueue(TQueueModel model, string area, int wait, string reserveSeq)
        {
            DataTable table = new DataTable("table");
            table.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn ("area",typeof(string)),
                new DataColumn ("waitCount",typeof(string)),
                new DataColumn ("unitName",typeof(string)),
                new DataColumn ("busyName",typeof(string)),
                new DataColumn ("ticketNumber",typeof(string)),
                new DataColumn ("reserveSeq",typeof(string)),
            });
            DataRow row = table.NewRow();
            row["area"] = area;
            row["waitCount"] = wait.ToString();
            row["unitName"] = model.unitName;
            row["busyName"] = model.busTypeName;
            row["ticketNumber"] = model.ticketNumber;
            row["reserveSeq"] = reserveSeq;
            table.Rows.Add(row);
            return table;
        }

        //插入排队数据
        private TQueueModel InsertQueue(TLineUpMaxNoModel maxNo, TAppointmentModel app, string ticketStart)
        {
            int No = maxNo == null ? 1 : maxNo.lineDate.Date != DateTime.Now.Date ? 1 : maxNo.maxNo + 1;
            TQueueModel line = new TQueueModel();
            line.busTypeName = selectBusy.busiName;
            line.busTypeSeq = selectBusy.busiSeq;
            line.qNumber = No.ToString();
            line.state = 0;
            line.ticketNumber = ticketStart + No.ToString("000");//需要扩展业务属性
            line.ticketTime = DateTime.Now;
            line.unitName = selectUnit.unitName;
            line.unitSeq = selectUnit.unitSeq;
            line.vipLever = "";
            line.windowName = "";
            line.windowNumber = "";
            line.idCard = person.idcard;
            line.qNmae = person.name;
            qBll.Insert(line);
            if (app != null)
            {
                aBll.Insert(app);
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
            if (maxNo == null)
            {
                maxNo = new TLineUpMaxNoModel();
                maxNo.areaSeq = "";
                maxNo.busiSeq = selectBusy.busiSeq;
                maxNo.lineDate = DateTime.Now;
                maxNo.maxNo = 1;
                maxNo.unitSeq = selectUnit.unitSeq;
                lineBll.Insert(maxNo);
            }
            else
            {
                if (maxNo.lineDate.Date != DateTime.Now.Date)
                    maxNo.maxNo = 1;
                else
                    maxNo.maxNo = maxNo.maxNo + 1;
                maxNo.lineDate = DateTime.Now;
                lineBll.Update(maxNo);
            }
            return line;
        }

        #endregion

        #region 保存评、评价相关

        private void pbSubmit_Click(object sender, EventArgs e)
        {
            SaveEvaluateInfo(eList);
        }

        private void SaveEvaluateInfo(List<TEvaluateModel> evList)
        {
            try
            {
                DateTime time = DateTime.Now;
                var list = evList.Where(e => e.evaluateResult > 0).ToList();
                if (list.Count == 0)
                {
                    frmMsg frm = new frmMsg();
                    frm.msgInfo = "请先评价然后进行保存！";
                    frm.ShowDialog();
                    return;
                }
                foreach (var ev in list)
                {
                    ev.handleTime = time;
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
                            return;
                        }
                    }
                    else
                    {

                        frmMsg frm = new frmMsg();//无法评价
                        frm.msgInfo = "评价信息保存失败，当前业务[" + ev.approveName + "]无法评价！";
                        frm.ShowDialog();
                        return;
                    }
                    eBll.Insert(ev);
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriterEvaluateLog("评价信息保存错误：" + ex.Message);
                frmMsg frm = new frmMsg();//提示
                frm.msgInfo = "评价信息保存异常，请联系管理员！";
                frm.ShowDialog();
            }

        }

        private void pnPrevious_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            Font font = new Font("黑体", 20, FontStyle.Bold);
            if (pb.Name == "pnPrevious" || pb.Name == "pnPrevious1")
            {
                e.Graphics.DrawString("上一页", font, new SolidBrush(Color.Black), 17, 4);
            }
            else if (pb.Name == "pbNext" || pb.Name == "pbNext1")
            {
                e.Graphics.DrawString("下一页", font, new SolidBrush(Color.Black), 17, 4);
            }
            else if (pb.Name == "pbSubmit")
            {
                font = new Font("黑体", 40, FontStyle.Bold);
                e.Graphics.DrawString("提 交", font, new SolidBrush(Color.Black), 55, 10);
            }
            else
            {
                font = new Font("黑体", 40, FontStyle.Bold);
                e.Graphics.DrawString("确 认", font, new SolidBrush(Color.Black), 55, 10);
            }
        }

        private void pnPrevious_Click(object sender, EventArgs e)
        {
            int max = eList.Count / pageCount;
            if ((eList.Count % pageCount) > 0)
                max++;
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pnPrevious")
            {
                if (cureentPage == 1)
                    return;
                else
                {
                    cureentPage--;
                }
            }
            else
            {
                if (max == cureentPage + 1)
                    return;
                else
                    cureentPage++;
            }
            CreateEvaluate();
        }

        #endregion

        #region 关闭、MouseDown MouseUp Paint

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            ExitThread();
        }

        private void pbButton_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            Font fontMain = new Font("黑体", 60, FontStyle.Bold);
            Font fontSec = new Font("黑体", 50, FontStyle.Bold);
            Font fontThi = new Font("黑体", 36, FontStyle.Bold);

            if (pic.Name == "pbWork")
            {
                e.Graphics.DrawString("办事", fontMain, new SolidBrush(Color.White), 90, 25);
            }
            else if (pic.Name == "pbGetCard")
            {
                e.Graphics.DrawString("领证", fontMain, new SolidBrush(Color.White), 90, 25);
            }
            else if (pic.Name == "pbConsult")
            {
                e.Graphics.DrawString("咨询", fontMain, new SolidBrush(Color.White), 90, 25);
            }
            else if (pic.Name == "pbEvaluate")
            {
                e.Graphics.DrawString("评价", fontMain, new SolidBrush(Color.White), 90, 25);
            }
            else if (pic.Name == "pbConsultByUser")
            {
                e.Graphics.DrawString("窗口人工咨询", fontSec, new SolidBrush(Color.White), 10, 15);
            }
            else if (pic.Name == "pbUnit")
            {
                e.Graphics.DrawString("选择部门", fontThi, new SolidBrush(Color.White), 75, 12);
            }
            else if (pic.Name == "pbBusy")
            {
                e.Graphics.DrawString("选择业务", fontThi, new SolidBrush(Color.White), 75, 12);
            }
            else if (pic.Name == "pbPersonal")
            {
                e.Graphics.DrawString("个人办事", fontSec, new SolidBrush(Color.White), 100, 15);
            }
            else if (pic.Name == "pbCompany")
            {
                e.Graphics.DrawString("企业(法人)办事", fontSec, new SolidBrush(Color.White), 10, 15);
            }

        }

        private void pbButton_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.buttonmain2;
        }

        private void pbButton_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.buttonmain1;
        }

        private void pbButtonN_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.nubmer_button_2;
        }

        private void pbButtonN_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.nubmer_button_1;
        }

        private void pbButtonB_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pbReturnMain")
                pb.Image = Properties.Resources.home_button_2;
            else if (pb.Name == "pbLastPage")
                pb.Image = Properties.Resources.back_button2;
            else if (pb.Name == "pbfinish" || pb.Name == "pdf")
                pb.Image = Properties.Resources.buttonok2;
        }

        private void pbButtonB_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pbReturnMain")
                pb.Image = Properties.Resources.home_button_1;
            else if (pb.Name == "pbLastPage")
                pb.Image = Properties.Resources.back_button1;
            else if (pb.Name == "pbfinish" || pb.Name == "pdf")
                pb.Image = Properties.Resources.buttonok1;
        }

        private void pbE_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.提交_button1;
        }

        private void pbE_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.提交_button2;
        }

        #endregion

        #region  刷身份证相关

        private void pbReadCard_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("请将您的身份证靠", font, new SolidBrush(Color.Black), 185, 145);
            e.Graphics.DrawString("近指定的感应区域", font, new SolidBrush(Color.Black), 185, 245);
        }

        private void pbGotoInput_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("如未带身份证请点击此按钮", font, new SolidBrush(Color.Black), 75, 15);
        }

        private void pnWriterCard_Paint(object sender, PaintEventArgs e)
        {
            if (idCard != "")
            {
                Font font = new Font("黑体", 40, FontStyle.Bold);
                e.Graphics.DrawString(idCard, font, new SolidBrush(Color.Black), 5, 3);
            }
        }

        private void pbGotoInput_Click(object sender, EventArgs e)
        {
            int type = 0;
            if (busyType == BusyType.Work)
                type = 0;
            else if (busyType == BusyType.Evaluate)
                type = 1;
            else if (busyType == BusyType.GetCard)
                type = 2;
            GotoInputCard(type);
        }

        #endregion

        #region 预约列表 相关
        private void pnPrevious1_Click(object sender, EventArgs e)
        {
            int max = appList.Count / pageCountA;
            if ((appList.Count % pageCountA) > 0)
                max++;
            PictureBox pb = sender as PictureBox;
            if (pb.Name == "pnPrevious1")
            {
                if (cureentPageA == 0)
                    return;
                else
                {
                    cureentPageA--;
                }
            }
            else
            {
                if (max == cureentPageA + 1)
                    return;
                else
                    cureentPageA++;
            }
            CreateAppointment();
        }
        private void pbOk_Click(object sender, EventArgs e)
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
                    selectAppoomt = app; //目前默认取第一条直接出号，暂时不做让选择预约号功能
                    selectUnit = uList.Where(u => u.unitName == selectAppoomt.unitName).FirstOrDefault();
                    if (selectUnit == null)
                    {
                        isError = true;
                        continue;
                    }
                    selectBusy = bList.Where(b => b.unitName == selectUnit.unitName && b.busiName == selectAppoomt.busiName).FirstOrDefault();
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
                apList.ForEach(ap => { OutQueueNo(ap); });
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
        #endregion

    }
}
