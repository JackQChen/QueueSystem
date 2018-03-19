using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using BLL;
using MessageClient;
using Model;
using QueueMessage;

namespace LEDDisplay
{
    public partial class FrmMain : Form
    {
        string ip = System.Configuration.ConfigurationManager.AppSettings["IP"];
        string port = System.Configuration.ConfigurationManager.AppSettings["Port"];
        string clientName = ConfigurationManager.AppSettings["ClientName"];
        TLedControllerBLL ledControlBll = new TLedControllerBLL();
        TLedWindowBLL ledWinBll = new TLedWindowBLL();
        List<TLedControllerModel> listLedControl;
        List<TLedWindowModel> listLedWin;
        AutoResetEvent areConn = new AutoResetEvent(false);

        private const int WM_LED_NOTIFY = 1025;
        CLEDSender LEDSender;
        UInt32 handle = 0;

        public FrmMain()
        {
            InitializeComponent();
        }

        Client client = new Client();
        int fontColor = 0, fontSize = 0, fontStyle = 0;
        string fontName = "";
        Rectangle rectText = Rectangle.Empty;

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.handle = (UInt32)this.Handle;
            fontName = ConfigurationManager.AppSettings["FontName"];
            fontColor = Convert.ToInt32(ConfigurationManager.AppSettings["FontColor"], 16);
            fontSize = Convert.ToInt32(ConfigurationManager.AppSettings["FontSize"]);
            fontStyle = Convert.ToInt32(ConfigurationManager.AppSettings["FontStyle"]);
            var position = ConfigurationManager.AppSettings["Position"].Split(',');
            rectText = new Rectangle(
                Convert.ToInt32(position[0]),
                Convert.ToInt32(position[1]),
                Convert.ToInt32(position[2]),
                Convert.ToInt32(position[3]));
            this.client.ServerIP = ip;
            this.client.ServerPort = ushort.Parse(port);
            this.client.ClientType = ClientType.LEDDisplay;
            this.client.ClientName = clientName;
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
            this.client.OnMessage += new Action<QueueMessage.Message>(client_OnMessage);
            LEDSender = new CLEDSender();
            LEDSender.Do_LED_Startup();
            this.listLedControl = this.ledControlBll.GetModelList();
            this.listLedWin = this.ledWinBll.GetModelList();
            this.Close();
            this.notifyIcon1.ShowBalloonTip(3000);
        }

        class ledSendInfo
        {
            public string ip, port, deviceAddr, position, text;
        }

        ConcurrentDictionary<string, DateTime> callDic = new ConcurrentDictionary<string, DateTime>();
        TimeSpan callSpan = new TimeSpan(0, 0, 5);

        void client_OnMessage(QueueMessage.Message message)
        {
            switch (message.Type)
            {
                case MessageType.Call:
                    {
                        var msg = message as CallMessage;
                        if (msg.IsLEDMessage)
                        {
                            var win = this.listLedWin.Find(m => m.WindowNumber == msg.WindowNo);
                            if (win != null)
                            {
                                var ctl = this.listLedControl.Find(m => m.ID == win.ControllerID);
                                if (ctl != null)
                                {
                                    if (!callDic.ContainsKey(win.WindowNumber))
                                        callDic.TryAdd(win.WindowNumber, DateTime.Now);
                                    //频繁呼叫直接取消
                                    else if (DateTime.Now - callDic[win.WindowNumber] < callSpan)
                                        return;
                                    new Thread(arg =>
                                    {
                                        var sendInfo = arg as ledSendInfo;
                                        //LED闪烁
                                        this.SendLEDMessage(sendInfo.ip, ushort.Parse(sendInfo.port), sendInfo.deviceAddr, sendInfo.position, sendInfo.text, true);
                                        Thread.Sleep(3000);
                                        this.SendLEDMessage(sendInfo.ip, ushort.Parse(sendInfo.port), sendInfo.deviceAddr, sendInfo.position, sendInfo.text);
                                    }).Start(
                                    new ledSendInfo()
                                    {
                                        ip = ctl.IP,
                                        port = ctl.Port,
                                        deviceAddr = ctl.DeviceAddress,
                                        position = win.Position,
                                        text = win.DisplayText.Replace("{Number}", msg.TicketNo)
                                    });
                                    callDic.AddOrUpdate(win.WindowNumber, DateTime.Now, (k, v) =>
                                    {
                                        return DateTime.Now;
                                    });
                                }
                            }
                        }
                    }
                    break;
                case MessageType.Rate:
                    {
                        var msg = message as RateMessage;
                        var win = this.listLedWin.Find(m => m.WindowNumber == msg.WindowNo);
                        if (win != null)
                        {
                            var ctl = this.listLedControl.Find(m => m.ID == win.ControllerID);
                            if (ctl != null)
                                this.SendLEDMessage(ctl.IP, ushort.Parse(ctl.Port), ctl.DeviceAddress, win.Position, "热情为您服务");
                        }
                    }
                    break;
                case MessageType.Operate:
                    {
                        var msg = message as OperateMessage;
                        var win = this.listLedWin.Find(m => m.WindowNumber == msg.WindowNo);
                        if (win != null)
                        {
                            var ctl = this.listLedControl.Find(m => m.ID == win.ControllerID);
                            if (ctl != null)
                            {
                                if (msg.Operate == Operate.Pause)
                                    this.SendLEDMessage(ctl.IP, ushort.Parse(ctl.Port), ctl.DeviceAddress, win.Position, "  暂 停 服 务");
                                else if (msg.Operate == Operate.Reset)
                                    this.SendLEDMessage(ctl.IP, ushort.Parse(ctl.Port), ctl.DeviceAddress, win.Position, "热情为您服务");
                            }
                        }
                    }
                    break;
            }
            this.txtSoundMesInfo.Invoke(new Action(() =>
            {
                this.messageIndicator1.SetState(StateType.Success, "收到消息:" + message.ToString());
                this.txtSoundMesInfo.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n" + message.ToString() + "。\r\n");
                LogService.Info(message.ToString());
            }));
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }

        void SendLEDMessage(string ip, ushort port, string deviceAddr, string position, string text)
        {
            this.SendLEDMessage(ip, port, deviceAddr, position, text, false);
        }

        void SendLEDMessage(string ip, ushort port, string deviceAddr, string position, string text, bool isFlash)
        {
            LogService.Debug(string.Format("SendLEDMessage->Start:Text={0}", text));
            var strArr = position.Split(',');
            int ChapterIndex = Convert.ToInt32(strArr[0]),
                RegionIndex = Convert.ToInt32(strArr[1]),
                LeafIndex = Convert.ToInt32(strArr[2]),
                ObjectIndex = Convert.ToInt32(strArr[3]);
            TSenderParam param = new TSenderParam();
            param.devParam.devType = LEDSender.DEVICE_TYPE_UDP;
            param.devParam.rmtHost = ip;
            param.devParam.locPort = port;
            param.devParam.rmtPort = 6666;
            param.devParam.dstAddr = ushort.Parse(deviceAddr);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = this.handle;
            param.wmMessage = WM_LED_NOTIFY;
            //这个操作中，ChapterIndex=0，RegionIndex=0，LeafIndex=0，ObjectIndex=0 只更新控制卡内第1个节目中的第1个分区中的第1个页面中的第1个对象
            //如果ChapterIndex=1，RegionIndex=2，LeafIndex=1，ObjectIndex=2只更新控制卡内第2个节目中的第3个分区中的第2个页面中的第3个对象
            //以此类推 
            ushort K = (ushort)LEDSender.Do_MakeObject(LEDSender.ROOT_PLAY_OBJECT, LEDSender.ACTMODE_REPLACE,
                ChapterIndex, RegionIndex, LeafIndex, ObjectIndex,
                LEDSender.COLOR_MODE_DOUBLE);
            LEDSender.Do_AddText(K, rectText.Left, rectText.Top, rectText.Width, rectText.Height, LEDSender.V_TRUE, 0, text, this.fontName, this.fontSize, this.fontColor, this.fontStyle, LEDSender.V_FALSE, 0, 1, 5, 1, 5, isFlash ? 1 : 0, 1000, 10000);
            var result = LEDSender.Do_LED_SendToScreen(ref param, K);
            if (result == LEDSender.R_DEVICE_READY)
                this.messageIndicator1.SetState(StateType.Success, "正在执行命令或者发送数据...");
            else if (result == LEDSender.R_DEVICE_INVALID)
                this.messageIndicator1.SetState(StateType.Success, "打开通讯设备失败");
            else if (result == LEDSender.R_DEVICE_BUSY)
                this.messageIndicator1.SetState(StateType.Success, "设备忙，正在通讯中...");
            LogService.Debug("SendLEDMessage->End");
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_LED_NOTIFY:
                    TNotifyParam notifyparam = new TNotifyParam();
                    LEDSender.Do_LED_GetNotifyParam_BufferToFile(ref notifyparam, AppDomain.CurrentDomain.BaseDirectory + "play.dat", (int)m.WParam);
                    if (notifyparam.notify == LEDSender.LM_TIMEOUT)
                    {
                        this.messageIndicator1.SetState(StateType.Success, "命令执行超时");
                    }
                    else if (notifyparam.notify == LEDSender.LM_TX_COMPLETE)
                    {
                        if (notifyparam.result == LEDSender.RESULT_FLASH)
                        {
                            this.messageIndicator1.SetState(StateType.Success, "数据传送完成，正在写入Flash");
                        }
                        else
                        {
                            this.messageIndicator1.SetState(StateType.Success, "数据传送完成");
                        }
                    }
                    break;
                default:
                    base.DefWndProc(ref m);
                    break;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.client.Logout();
            Application.ExitThread();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            //托盘区图标隐藏
            notifyIcon1.Visible = false;
            this.Show();
        }

    }
}
