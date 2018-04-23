using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using BLL;
using MessageClient;
using Model;
using QueueMessage;
using System.Collections;

namespace CallClient
{
    public partial class frmMain : CustomSkin.Windows.Forms.FormBase
    {
        OperateIni ini;
        string ip = "";
        string port = "";
        TOprateLogBLL oBll = new TOprateLogBLL();
        TQueueBLL qBll = new TQueueBLL();
        TWindowBLL wBll = new TWindowBLL();
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        TBusinessAttributeBLL baBll = new TBusinessAttributeBLL();
        List<TBusinessAttributeModel> baList = new List<TBusinessAttributeModel>();
        TCallBLL cBll = new TCallBLL();
        List<TWindowBusinessModel> wbList;
        Client client = new Client();
        Hotkey hotkey;
        int hkCall, hkRecall, hkEv, hkGiveUp, hkpause, hkMove, hkHang, hkCallBack;
        object objLock = new object();
        string windowNo;//窗口号
        string windowName;//窗口名称
        string f1, f2, f3, f4, f5, f6, f7, f8;
        List<TWindowBusinessModel> windowBusys;//窗口业务
        List<TWindowBusinessModel> windowBusyGreens;//绿色通道
        TWindowModel windowModel;
        TCallStateBLL csBll = new TCallStateBLL();
        TCallStateModel stateModel;
        public frmMain()
        {
            InitializeComponent();
        }

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

        private void frmMain_Load(object sender, EventArgs e)
        {
            ip = System.Configuration.ConfigurationManager.AppSettings["IP"];
            port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            ini = new OperateIni(System.Windows.Forms.Application.StartupPath + @"\WindowConfig.ini");
            windowNo = ini.ReadString("WindowSet", "WindwoNo");
            windowName = ini.ReadString("WindowSet", "WindowName");
            f1 = ini.ReadString("Shortcutkey", "Fuction1");
            f2 = ini.ReadString("Shortcutkey", "Fuction2");
            f3 = ini.ReadString("Shortcutkey", "Fuction3");
            f4 = ini.ReadString("Shortcutkey", "Fuction4");
            f5 = ini.ReadString("Shortcutkey", "Fuction5");
            f6 = ini.ReadString("Shortcutkey", "Fuction6");
            f7 = ini.ReadString("Shortcutkey", "Fuction7");
            f8 = ini.ReadString("Shortcutkey", "Fuction8");
            windowModel = wBll.GetModelList().Where(w => w.State == "1" && w.Number == windowNo).FirstOrDefault();
            wbList = wbBll.GetModelList();
            baList = baBll.GetModelList();
            windowBusys = new List<TWindowBusinessModel>();
            windowBusyGreens = new List<TWindowBusinessModel>();
            var busyList = wbList.Where(b => b.WindowID == windowModel.ID).ToList().OrderBy(o => o.priorityLevel).ToList();
            if (busyList != null && busyList.Count > 0)
            {
                windowBusys.AddRange(busyList);
                var gbList = new List<TWindowBusinessModel>();
                foreach (var bs in busyList)
                {
                    var gb = baList.Where(b => b.unitSeq == bs.unitSeq && b.busiSeq == bs.busiSeq && b.isGreenChannel == 1).FirstOrDefault();
                    if (gb != null)
                        gbList.Add(bs);
                }
                windowBusyGreens.AddRange(gbList);
            }
            else
            {
                MessageBox.Show("当前窗口未绑定业务，无法叫号！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information); return;
            }
            client.ServerIP = ip;
            client.ServerPort = ushort.Parse(port);
            client.ClientType = ClientType.CallClient;
            client.ClientName = windowName;
            if (!this.client.Login())
                this.messageIndicator1.SetState(StateType.Error, "未连接");
            this.client.OnResult += (msgType, msgText) =>
            {
                this.messageIndicator1.SetState(StateType.Success, msgText);
            };
            this.client.OnDisconnect += () =>
            {
                this.messageIndicator1.SetState(StateType.Error, "未连接");
            };
            this.ShowInTaskbar = false;
            this.Hide();
            this.ShowInTaskbar = true;
            #region
            var act = new Func<string, Keys>(f =>
            {
                switch (f)
                {
                    case "F1":
                        return Keys.F1;
                    case "F2":
                        return Keys.F2;
                    case "F3":
                        return Keys.F3;
                    case "F4":
                        return Keys.F4;
                    case "F5":
                        return Keys.F5;
                    case "F6":
                        return Keys.F6;
                    case "F7":
                        return Keys.F7;
                    case "F8":
                        return Keys.F8;
                    case "F9":
                        return Keys.F9;
                    case "F10":
                        return Keys.F10;
                    case "F11":
                        return Keys.F11;
                    case "F12":
                        return Keys.F12;
                    default:
                        {
                            return Keys.F;
                        }
                }
            });
            #endregion
            //设置ShowInTaskbar以后Handle会变化，所以热键绑定要放在最后面
            hotkey = new Hotkey(this.Handle);
            if (f1 != "")
            {
                this.btnCall.Text = "呼叫(" + f1 + ")";
                hkCall = hotkey.RegisterHotkey(act(f1), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f2 != "")
            {
                this.btnReCall.Text = "重呼(" + f2 + ")";
                hkRecall = hotkey.RegisterHotkey(act(f2), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f3 != "")
            {
                this.btnEv.Text = "评价(" + f3 + ")";
                hkEv = hotkey.RegisterHotkey(act(f3), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f4 != "")
            {
                this.btnCance.Text = "弃号(" + f4 + ")";
                hkGiveUp = hotkey.RegisterHotkey(act(f4), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f5 != "")
            {
                this.btnPause.Text = "暂停(" + f5 + ")";
                hkpause = hotkey.RegisterHotkey(act(f5), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f6 != "")
            {
                this.btnMove.Text = "转移(" + f6 + ")";
                hkMove = hotkey.RegisterHotkey(act(f6), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f7 != "")
            {
                this.btnHang.Text = "挂起(" + f7 + ")";
                hkHang = hotkey.RegisterHotkey(act(f7), Hotkey.KeyFlags.MOD_NONE);
            }
            if (f8 != "")
            {
                this.btnBackCall.Text = "回呼(" + f8 + ")";
                hkCallBack = hotkey.RegisterHotkey(act(f8), Hotkey.KeyFlags.MOD_NONE);
            }
            hotkey.OnHotkey += new HotkeyEventHandler(OnHotkey);
        }

        private void OnHotkey(int hotkeyID)
        {
            if (hotkeyID == this.hkCall)
            {
                CallNo();
            }
            else if (hotkeyID == this.hkRecall)
            {
                ReCallNo();
            }
            else if (hotkeyID == this.hkEv)
            {
                EvaluateService();
            }
            else if (hotkeyID == this.hkGiveUp)
            {
                GiveUpNo();
            }
            else if (hotkeyID == this.hkpause)
            {
                Pause();
            }
            else if (hotkeyID == this.hkMove)
            {
                Transfer();
            }
            else if (hotkeyID == this.hkHang)
            {
                Hang();
            }
            else if (hotkeyID == this.hkCallBack)
            {
                CallBack();
            }

        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            this.Activate();
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                client.Logout();
            }
            catch
            {

            }
            Application.ExitThread();
        }
        private void btnConfig_Click(object sender, EventArgs e)
        {
            frmConfig frm = new frmConfig();
            frm.ShowDialog();
        }

        #region 功能方法

        #region 8大方法

        //呼叫*顺呼 
        private void CallNo()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        stateModel = new TCallStateModel();
                        stateModel.windowNo = windowNo;
                        stateModel.workState = (int)WorkState.Defalt;
                        csBll.Insert(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Defalt || stateModel.workState == (int)WorkState.Evaluate)
                    {
                        try
                        {
                            var model = cBll.CallNo(windowBusys, windowBusyGreens, windowNo, "");//用户暂时为空
                            if (model != null)
                            {
                                stateModel.callId = model.id;
                                var callString = "请" + model.ticketNumber + "号到 " + windowNo + "号窗口办理 ";
                                client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = windowNo, AreaNo = windowModel.AreaName.ToString(), IsLEDMessage = true, IsSoundMessage = true });
                                stateModel.workState = (int)WorkState.Call;
                                stateModel.ticketNo = model.ticketNumber;
                                stateModel.callId = model.id;
                                stateModel.reCallTimes = 0;
                                csBll.Update(stateModel);
                                this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                                //SendTicket(adress, model.ticketNumber.Substring(model.ticketNumber.Length - 3, 3));
                                WriterCallLog(0, callString);
                            }
                        }
                        catch (Exception ex)
                        {
                            WriterLog("叫号异常：" + ex.Message);
                        }
                    }
                });
            }
        }
        //重呼
        private void ReCallNo()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        return;
                    }

                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Call)
                    {
                        var model = cBll.GetModel(stateModel.callId);
                        if (model == null)
                        {
                            return;
                        }
                        if (stateModel.reCallTimes >= 5)
                        {
                            return;
                        }
                        else
                        {
                            stateModel.reCallTimes = stateModel.reCallTimes + 1;
                            csBll.Update(stateModel);
                        }
                        var callString = "请" + model.ticketNumber + "号到 " + windowNo + "号窗口办理(重呼) ";
                        client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = windowNo, AreaNo = windowModel.AreaName.ToString(), IsLEDMessage = true, IsSoundMessage = true });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        //SendTicket(adress, wModel[adress].ticketNumber.Substring(wModel[adress].ticketNumber.Length - 3, 3));
                        WriterCallLog(1, callString);
                    }
                });
            }

        }
        //评价
        private void EvaluateService()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        stateModel = new TCallStateModel();
                        stateModel.windowNo =windowNo;
                        stateModel.workState = (int)WorkState.Defalt;
                        csBll.Insert(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Call)
                    {
                        try
                        {
                            var model = cBll.GetModel(stateModel.callId);
                            if (model == null)
                            {
                                return;
                            }
                            model.state = 1;
                            model.sysFlag = 1;
                            cBll.Update(model);
                            stateModel.workState = (int)WorkState.Evaluate;
                            stateModel.callId = 0;
                            stateModel.ticketNo = "";
                            csBll.Update(stateModel);
                            client.SendMessage(new RateMessage() //发送评价请求
                            {
                                WindowNo = windowNo,
                                RateId = model.handleId,
                                ItemName = "项目名称",
                                WorkDate = DateTime.Now.ToShortDateString(),
                                Transactor = model.qNmae,
                                reserveSeq = model.reserveSeq
                            }
                            );
                            //SendWait(adress);
                            string mess = " [" + model.ticketNumber + "]号已评价。";
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : [" + model.ticketNumber + "]号已评价。"); }));
                            WriterCallLog(2, mess);
                        }
                        catch (Exception ex)
                        {
                            WriterLog("评价异常：" + ex.Message);
                        }
                    }
                    else
                    {
                        if (stateModel.workState == (int)WorkState.Defalt || stateModel.workState == (int)WorkState.Evaluate)
                        {
                            //SendWait(adress);
                        }
                    }
                });
            }

        }
        //弃号
        private void GiveUpNo()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        return;
                    }
                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Call)
                    {
                        try
                        {
                            var model = cBll.GetModel(stateModel.callId);
                            if (model == null)
                            {
                                return;
                            }
                            string mess = model.ticketNumber + "号已弃号。";
                            model.state = -1;
                            model.sysFlag = 1;
                            cBll.Update(model);
                            stateModel.workState = (int)WorkState.Evaluate;
                            stateModel.callId = 0;
                            stateModel.ticketNo = "";
                            csBll.Update(stateModel);
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                            this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                            //SendWait(adress);
                            WriterCallLog(4, mess);
                        }
                        catch (Exception ex)
                        {
                            WriterLog("弃号异常：" + ex.Message);
                        }
                    }
                });
            }
        }
        //暂停
        private void Pause()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        stateModel = new TCallStateModel();
                        stateModel.windowNo = windowNo;
                        stateModel.workState = (int)WorkState.Defalt;
                        csBll.Insert(stateModel);
                    }
                    if (stateModel.workState != (int)WorkState.PauseService)
                    {
                        string mess = windowNo + "号窗口暂停服务";
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Pause });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                        stateModel.pauseState = stateModel.workState;
                        stateModel.workState = (int)WorkState.PauseService;
                        csBll.Update(stateModel);
                        WriterCallLog(0, mess);
                    }
                });
            }
        }
        //转移-丢回去
        private void Transfer()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        stateModel = new TCallStateModel();
                        stateModel.windowNo = windowNo;
                        stateModel.workState = (int)WorkState.Defalt;
                        csBll.Insert(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Call)
                    {
                        var model = cBll.GetModel(stateModel.callId);
                        if (model == null)
                        {
                            return;
                        }
                        //转移号码
                        cBll.Transfer(model);
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        var callString = model.ticketNumber + "号已转移(重置) ";
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        //SendWait(adress);
                        stateModel.workState = (int)WorkState.Defalt;
                        stateModel.callId = 0;
                        stateModel.ticketNo = "";
                        stateModel.reCallTimes = 0;
                        csBll.Update(stateModel);
                        WriterCallLog(5, callString);
                    }
                });
            }
        }
        //挂起
        private void Hang()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        stateModel = new TCallStateModel();
                        stateModel.windowNo = windowNo;
                        stateModel.workState = (int)WorkState.Defalt;
                        csBll.Insert(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Call)
                    {
                        var model = cBll.GetModel(stateModel.callId);
                        if (model == null)
                        {
                            return;
                        }
                        
                        model.state = 3;
                        model.sysFlag = 1;
                        cBll.Update(model);
                        var callString = model.ticketNumber + "号已挂起";
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        //SendWait(adress);
                        stateModel.workState = (int)WorkState.Defalt;
                        stateModel.hangId = model.id;
                        stateModel.callId = 0;
                        stateModel.ticketNo = "";
                        stateModel.reCallTimes = 0;
                        csBll.Update(stateModel);
                        WriterCallLog(6, callString);
                    }
                });
            }
        }
        //回呼
        private void CallBack()
        {
            lock (objLock)
            {
                LockAction.RunWindowLock(windowNo, () =>
                {
                    stateModel = csBll.GetModel(windowNo);
                    if (stateModel == null)
                    {
                        return;
                    }
                    if (stateModel.workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = windowNo, Operate = Operate.Reset });
                        stateModel.workState = stateModel.pauseState;
                        csBll.Update(stateModel);
                    }
                    if (stateModel.workState == (int)WorkState.Defalt || stateModel.workState == (int)WorkState.Evaluate)
                    {
                        var model = cBll.GetModel(stateModel.hangId);
                        if (model == null)
                        {
                            return;
                        }
                        else
                        {
                            if (model.ticketTime.Date != DateTime.Now.Date)
                            {
                                stateModel.hangId = 0;
                                csBll.Update(stateModel);
                                return;
                            }
                        }
                        model.state = 0;
                        model.sysFlag = 1;
                        cBll.Update(model);
                        var callString = model.ticketNumber + "号回呼";
                        client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = windowNo, AreaNo = windowModel.AreaName.ToString(), IsLEDMessage = true, IsSoundMessage = true });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        //SendTicket(adress, wHang[adress].ticketNumber.Substring(wHang[adress].ticketNumber.Length - 3, 3));
                        stateModel.workState = (int)WorkState.Call;
                        stateModel.ticketNo = model.ticketNumber;
                        stateModel.callId = model.id;
                        stateModel.reCallTimes = 0;
                        stateModel.hangId = 0;
                        csBll.Update(stateModel);
                        WriterCallLog(7, callString);
                    }
                });
            }
        }

        #endregion

        #endregion

        #region 功能按钮
        private void btnCall_Click(object sender, EventArgs e)
        {
            CallNo();
        }

        private void btnReCall_Click(object sender, EventArgs e)
        {
            ReCallNo();
        }

        private void btnEv_Click(object sender, EventArgs e)
        {
            EvaluateService();
        }

        private void btnCance_Click(object sender, EventArgs e)
        {
            GiveUpNo();
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Pause();
        }
        //转移
        private void btnMove_Click(object sender, EventArgs e)
        {
            Transfer();
        }
        //挂起
        private void btnHang_Click(object sender, EventArgs e)
        {
            Hang();
        }
        //回呼
        private void btnBackCall_Click(object sender, EventArgs e)
        {
            CallBack();
        }
        //刷新
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        #endregion

        private void btnSet_Click(object sender, EventArgs e)
        {
            frmQueueSet frm = new frmQueueSet();
            frm.ShowDialog();
        }

        private void WriterLog(string text)
        {
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Exception.txt", DateTime.Now + " : " + text + "\r\n");
        }

        private void WriterCallLog(int type, string text)
        {
            string otype = type == 0 ? "叫号" : type == 1 ? "重呼" : type == 2 ? "评价" : type == 3 ? "暂停" : type == 4 ? "弃号" : type == 5 ? "转移" : type == 6 ? "挂起" : "回呼";
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Record.txt", DateTime.Now + " : 【" + otype + "】" + text + "\r\n");
            oBll.Insert(new TOprateLogModel()
            {
                oprateFlag = windowName,
                oprateType = "叫号PC端",
                oprateClassifyType = otype,
                oprateTime = DateTime.Now,
                oprateLog = text,
                sysFlag = 0
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            RefreshInfo();
        }

        void RefreshInfo()
        {
            this.txtWindow.Text = windowName;
            if (stateModel != null)
            {
                this.txtTicket.Text = stateModel.workState == (int)WorkState.Call ? stateModel.ticketNo : "";
                this.txtHangCount.Text = (stateModel.hangId > 0 ? 1 : 0).ToString();
            }
            else
            {
                this.txtHangCount.Text = "0";
                this.txtTicket.Text = "";
            }
            //查询当前窗口排队等候人数
            var list = qBll.GetModelList(windowBusys, 0).OrderBy(o => o.ticketTime).ToList();//排队中
            var list2 = qBll.GetModelList(windowBusys, 1);//已完成
            this.txtQueueCount.Text = (list.Count + list2.Count).ToString();
            this.txtWait.Text = list.Count.ToString();
            foreach (var item in list)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Tag = item;
                lvItem.SubItems[0].Text = item.ticketNumber;
                lvItem.SubItems.Add(item.ticketTime.ToString("yyyy-MM-dd HH:mm:ss"));
                lvItem.SubItems.Add(item.qNmae);
                listView2.Items.Add(lvItem);
            }
            listView2.Refresh();
        }

        private void btnGiveUpAll_Click_1(object sender, EventArgs e)
        {
            if (DialogResult.OK == MessageBox.Show("确定对当前窗口所有业务的排队票进行批量弃号？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                stateModel = csBll.GetModel(windowNo);
                if (stateModel != null)
                {
                    stateModel.callId = 0;
                    stateModel.workState = 0;
                    stateModel.hangId = 0;
                    stateModel.pauseState = 0;
                    stateModel.ticketNo = "";
                    csBll.Update(stateModel);
                }
                var tList = cBll.GiveUpAll(windowBusys);
                foreach (var t in tList)
                {
                    string mess = "批量弃号：窗口[" + t.windowNumber + "]票号[" + t.ticketNumber + "]已完成弃号";
                    this.client.SendMessage(new OperateMessage() { WindowNo = t.windowNumber, Operate = Operate.Reset });
                    this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                    WriterCallLog(4, mess);
                }
            }
        }

    }

}
