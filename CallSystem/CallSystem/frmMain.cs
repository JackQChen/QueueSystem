using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Windows.Forms;
using BLL;
using MessageClient;
using MessageLib;
using Model;
using QueueMessage;
using System.Threading;
using System.Configuration;

namespace CallSystem
{
    public partial class frmMain : Form
    {
        public byte[] Head = new byte[] { 0xFF, 0x68 }; //帧头
        public byte Adress;      //通讯地址
        public byte Order;          //命令
        public byte[] Data;         //数据区
        public byte End = 0x16;
        SerialPort serialPort;
        OperateIni ini;
        string portName = "";//端口号
        string clearTime = "";// System.Configuration.ConfigurationManager.AppSettings["ClearTime"];
        string ip = "";// System.Configuration.ConfigurationManager.AppSettings["IP"];
        string port = "";// System.Configuration.ConfigurationManager.AppSettings["Port"];
        string clientName;
        string areaNo = "";
        //string test = "";
        TOprateLogBLL oBll = new TOprateLogBLL();
        TQueueBLL qBll = new TQueueBLL();
        TWindowBLL wBll = new TWindowBLL();
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        TCallBLL cBll = new TCallBLL();
        List<TWindowModel> wList;
        List<TWindowBusinessModel> wbList;
        Dictionary<string, string> wArea;
        Dictionary<int, WorkState> wState;
        Dictionary<int, WorkState> wpState;
        Dictionary<int, TCallModel> wModel;//呼叫器-呼叫状态
        Dictionary<int, string> wNum;//呼叫器-窗口号
        Dictionary<int, TWindowBusinessModel> wBusy;//呼叫器-窗口业务
        Dictionary<int, Dictionary<string, int>> wReCall;//重呼限制
        Dictionary<int, List<TWindowBusinessModel>> wlBusy;
        Dictionary<int, TCallModel> wHang;//挂起
        Dictionary<string, int> wCall;
        Client client = new Client();
        object objLock = new object();
        AutoResetEvent areConn = new AutoResetEvent(false);
        //Thread thread;
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
            SetConfigValue("AreaNo", "1");
            areaNo = System.Configuration.ConfigurationManager.AppSettings["AreaNo"];
            clientName = System.Configuration.ConfigurationManager.AppSettings["ClientName"];
            clearTime = System.Configuration.ConfigurationManager.AppSettings["ClearTime"];
            ip = System.Configuration.ConfigurationManager.AppSettings["IP"];
            port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            cmbAdress.Text = "1";
            ini = new OperateIni(System.Windows.Forms.Application.StartupPath + @"\WindowConfig.ini");
            portName = ini.ReadString("CallSet", "SerialPort");
            wList = wBll.GetModelList().Where(w => w.State == "1").ToList();
            wbList = wbBll.GetModelList();
            wArea = new Dictionary<string, string>();
            wState = new Dictionary<int, WorkState>();
            wpState = new Dictionary<int, WorkState>();
            wModel = new Dictionary<int, TCallModel>();
            wNum = new Dictionary<int, string>();
            wCall = new Dictionary<string, int>();
            wBusy = new Dictionary<int, TWindowBusinessModel>();
            wlBusy = new Dictionary<int, List<TWindowBusinessModel>>();
            wReCall = new Dictionary<int, Dictionary<string, int>>();
            wHang = new Dictionary<int, TCallModel>();
            //根据配置分区窗口
            var areaList = areaNo.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            wList = wList.Where(w => areaList.Contains(w.AreaName.ToString())).ToList();
            foreach (var w in wList)
            {
                wArea.Add(w.Number, w.AreaName.ToString());
                wCall.Add(w.Number, w.CallNumber);
                wNum.Add(w.CallNumber, w.Number);
                wState.Add(w.CallNumber, WorkState.Defalt);
                var busyList = wbList.Where(b => b.WindowID == w.ID).ToList().OrderBy(o => o.priorityLevel).ToList();
                var busy = busyList.FirstOrDefault();
                if (busy != null)
                {
                    wlBusy.Add(w.CallNumber, busyList);
                    wBusy.Add(w.CallNumber, busy);
                }
            }
            serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
            //serialPort.DataReceived += new SerialDataReceivedEventHandler(serialPort_DataReceived);
            serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            serialPort.ReadBufferSize = 9600;
            serialPort.ReceivedBytesThreshold = 1;
            try
            {
                serialPort.Open();
                //thread = new Thread(new ThreadStart(sp_DataReceived));
                //thread.IsBackground = true;
                //thread.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("呼叫器端口打开失败，请重新配置：" + ex.Message);
            }
            client.ServerIP = ip;
            this.client.OnClose += (s, enOperation, errorCode) =>
            {
                areConn.Set();
                return HandleResult.Ignore;
            };
            client.ServerPort = ushort.Parse(port);
            client.ClientType = ClientType.Window;
            client.ClientName = clientName;
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
            this.Hide();
            this.ShowInTaskbar = false;
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            notifyIcon1.Visible = true;
            this.Show();
        }
        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }
        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte temp = (byte)serialPort.ReadByte();
            if (temp == Head[0])
            {
                byte temp2 = (byte)serialPort.ReadByte();
                if (temp2 == Head[1])
                {
                    #region 数据接收
                    var length = (byte)serialPort.ReadByte();//数据长度
                    var length2 = (byte)serialPort.ReadByte();//数据长度
                    var fixedvalue = (byte)serialPort.ReadByte();//固定值 0x68
                    var adress = (byte)serialPort.ReadByte();//通讯地址
                    var funccode = (byte)serialPort.ReadByte();//功能码
                    int index = 0;
                    if (length <= 2)
                    {
                        WriterLog("有呼叫器【" + adress + "】协议出错，发送过来的数据长度小于等于2【" + length + "】！本次操作取消！");
                        return;
                    }
                    var data = new byte[length - 2];//发送的数据内容
                    while (index < length - 2)
                    {
                        data[index] = (byte)serialPort.ReadByte();
                        index++;
                    }
                    var check = (byte)serialPort.ReadByte();//校验和
                    var end = (byte)serialPort.ReadByte();//结束码 0x16

                    #endregion

                    #region 发送响应指令
                    var send = new byte[9 + data.Length];
                    send[0] = temp;
                    send[1] = temp2;
                    send[2] = length;
                    send[3] = length2;
                    send[4] = fixedvalue;
                    send[5] = adress;
                    send[6] = funccode;
                    int startIndex = 7;
                    foreach (byte b in data)
                    {
                        send[startIndex] = b;
                        startIndex++;
                    }
                    send[7 + data.Length] = check;
                    send[8 + data.Length] = end;
                    SendOrder(send);

                    #endregion

                    #region  功能操作
                    //判断操作类型，进行对应的操作
                    if (data[data.Length - 1] == 0x0C)
                    {
                        CallNo(adress);//呼叫
                    }
                    else if (data[data.Length - 1] == 0x0D)
                    {
                        ReCallNo(adress); //重呼
                    }
                    else if (data[data.Length - 1] == 0x0B)
                    {
                        EvaluateService(adress); //评价
                    }
                    else if (data[data.Length - 1] == 0x0E)
                    {
                        //取消 * 暂定为弃号键 ***该功能已不用。
                    }
                    else if (data[data.Length - 1] == 0x0F)
                    {
                        if (data.Length == 2)
                        {
                            if (data[data.Length - 2] == 0x06)
                            {
                                GiveUpNo(adress);//6+确认为弃号
                            }
                            else if (data[data.Length - 2] == 0x00)
                            {
                                //0+确认为挂起
                                Hang(adress);
                            }
                            else if (data[data.Length - 2] == 0x03)
                            {
                                //3+确认为回呼
                                CallBack(adress);
                            }
                        }
                        else
                        {
                            WriterLog("发送确定键，但是指令未用，本次操作自动忽略！");
                            return ;
                        }
                    }
                    else if (data[data.Length - 1] == 0x11)
                    {
                        //一米 ******** 暂无处理
                    }
                    else if (data[data.Length - 1] == 0x12)
                    {
                        //等候  暂停功能
                        Pause(adress);
                    }
                    else if (data[data.Length - 1] == 0x13)
                    {
                        //转移
                        Transfer(adress);
                    }

                    #endregion
                }
            }
        }
        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            WriterLog("SerialPort Received Error!");
        }
        public void SendOrder(byte[] sendData)
        {
            try
            {
                serialPort.Write(sendData, 0, sendData.Length);
            }
            catch (Exception ex)
            {
                WriterLog("给叫号器写数据错误：" + ex.Message);
                return;
            }
        }
        private void btnExit_Click(object sender, EventArgs e)
        {
            try
            {
                if (serialPort.IsOpen)
                    serialPort.Close();
            }
            catch (Exception ex)
            {
                WriterLog("串口关闭错误：" + ex.Message);
            }
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
        /// <summary>
        /// 判断是否可操作  
        /// </summary>
        /// <param name="adress"></param>
        /// <param name="type">0：呼叫 1：重呼 2：评价 3：暂停 4 :弃号 5：转移 6：挂起 7：回呼</param>
        /// <returns></returns>
        private bool GetWindowByAdress(int adress, int type)
        {
            string otype = type == 0 ? "叫号" : type == 1 ? "重呼" : type == 2 ? "评价" : type == 3 ? "暂停" : type == 4 ? "弃号" : type == 5 ? "转移" : type == 6 ? "挂起" : "回呼";
            if (!wNum.Keys.Contains(adress) || !wState.Keys.Contains(adress))
            {
                WriterLog("呼叫器地址【" + String.Format("{0:X}", adress) + "[" + adress + "]】对应的窗口未找到（请核查是否属于该区域），本次操作【" + otype + "】失败。");
                return false;
            }
            if (!wBusy.Keys.Contains(adress))
            {
                WriterLog("窗口【" + wNum[adress] + "】没有绑定业务，本次操作【" + otype + "】失败。");
                return false;
            }
            return true;
        }

        #region 功能方法

        #region
        //呼叫*顺呼
        private void CallNo(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 0))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Defalt || wState[adress] == WorkState.Evaluate)
                    {

                        try
                        {
                            var model = cBll.CallNo(wlBusy[adress], wNum[adress], "");//用户暂时为空
                            //var model = cBll.CallNo(wBusy[adress].unitSeq, wBusy[adress].busiSeq, wNum[adress], "");//用户暂时为空
                            if (model != null)
                            {
                                if (wModel.ContainsKey(adress))
                                    wModel[adress] = model;
                                else
                                    wModel.Add(adress, model);
                                Dictionary<string, int> rd = new Dictionary<string, int>();
                                rd.Add(model.ticketNumber, 1);
                                if (wReCall.ContainsKey(adress))
                                    wReCall[adress] = rd;
                                else
                                    wReCall.Add(adress, rd);

                                var callString = "请" + model.ticketNumber + "号到 " + wNum[adress] + "号窗口办理 ";
                                client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = wNum[adress], AreaNo = wArea[wNum[adress]], IsLEDMessage = true, IsSoundMessage = true });
                                wState[adress] = WorkState.Call;
                                this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                                SendTicket(adress, model.ticketNumber.Substring(model.ticketNumber.Length - 3, 3));
                                WriterCallLog(0, callString);

                            }
                        }
                        catch (Exception ex)
                        {
                            WriterLog("叫号异常：" + ex.Message);
                        }
                    }
                }
            }
        }
        //重呼
        private void ReCallNo(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 1))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Call)
                    {
                        //判断重呼限制
                        Dictionary<string, int> dic = wReCall[adress];
                        if (dic.ContainsKey(wModel[adress].ticketNumber))
                        {
                            int count = dic[wModel[adress].ticketNumber];
                            if (count > 5)
                                return;
                            else
                                dic[wModel[adress].ticketNumber] = count + 1;
                        }
                        var callString = "请" + wModel[adress].ticketNumber + "号到 " + wNum[adress] + "号窗口办理 ";
                        client.SendMessage(new CallMessage() { TicketNo = wModel[adress].ticketNumber, WindowNo = wNum[adress], AreaNo = wArea[wNum[adress]], IsLEDMessage = true, IsSoundMessage = true });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendTicket(adress, wModel[adress].ticketNumber.Substring(wModel[adress].ticketNumber.Length - 3, 3));
                        WriterCallLog(1, callString);
                    }
                }
            }
        }
        //评价
        private void EvaluateService(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 2))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Call)
                    {
                        try
                        {
                            wModel[adress].state = 1;
                            wModel[adress].sysFlag = 1;
                            cBll.Update(wModel[adress]);
                            wState[adress] = WorkState.Evaluate;
                            client.SendMessage(new RateMessage() //发送评价请求
                            {
                                WindowNo = wNum[adress],
                                RateId = wModel[adress].handleId,
                                ItemName = "暂时测试",
                                WorkDate = DateTime.Now.ToShortDateString(),
                                Transactor = "办理人测试"
                            }
                            );
                            SendWait(adress);
                            string mess = " [" + wModel[adress].ticketNumber + "]号已评价。";
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : [" + wModel[adress].ticketNumber + "]号已评价。"); }));
                            WriterCallLog(2, mess);
                        }
                        catch (Exception ex)
                        {
                            WriterLog("评价异常：" + ex.Message);
                        }
                    }
                    else
                    {
                        if (wState[adress] == WorkState.Defalt || wState[adress] == WorkState.Evaluate)
                        {
                            SendWait(adress);
                        }
                    }
                }
            }
        }
        //弃号
        private void GiveUpNo(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 4))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Call)
                    {
                        try
                        {
                            string mess = wModel[adress].ticketNumber + "号已弃号。";
                            wModel[adress].state = -1;
                            wModel[adress].sysFlag = 1;
                            cBll.Update(wModel[adress]);
                            wState[adress] = WorkState.Evaluate;
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                            this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                            SendWait(adress);
                            WriterCallLog(4, mess);
                        }
                        catch (Exception ex)
                        {
                            WriterLog("弃号异常：" + ex.Message);
                        }
                    }
                }
            }
        }
        //暂停
        private void Pause(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 3))
                {
                    if (wState[adress] != WorkState.PauseService)
                    {
                        string mess = wNum[adress] + "号窗口暂停服务";
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Pause });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                        if (wpState.ContainsKey(adress))
                            wpState[adress] = wState[adress];
                        else
                            wpState.Add(adress, wState[adress]);
                        wState[adress] = WorkState.PauseService;
                        WriterCallLog(0, mess);
                    }
                }
            }

        }

        //转移-丢回去
        private void Transfer(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 5))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Call)
                    {
                        //转移号码
                        cBll.Transfer(wModel[adress]);
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        var callString = wModel[adress].ticketNumber + "号已转移(重置) ";
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendWait(adress);
                        wState[adress] = WorkState.Defalt;
                        WriterCallLog(5, callString);
                    }
                }
            }
        }

        //挂起
        private void Hang(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 6))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Call)
                    {
                        if (wHang.ContainsKey(adress))
                        {
                            wHang[adress] = wModel[adress];
                        }
                        else
                        {
                            wHang.Add(adress, wModel[adress]);
                        }
                        wHang[adress].state = 3;
                        wHang[adress].sysFlag = 1;
                        cBll.Update(wHang[adress]);
                        var callString = wModel[adress].ticketNumber + "号已挂起";
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendWait(adress);
                        wState[adress] = WorkState.Defalt;
                        WriterCallLog(6, callString);
                    }
                }
            }
        }

        //回呼
        private void CallBack(int adress)
        {
            lock (objLock)
            {
                if (GetWindowByAdress(adress, 7))
                {
                    if (wState[adress] == WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        wState[adress] = wpState[adress];
                    }
                    if (wState[adress] == WorkState.Defalt || wState[adress] == WorkState.Evaluate)
                    {
                        if (wHang.ContainsKey(adress))
                        {
                            var model = wHang[adress];
                            if (wModel.ContainsKey(adress))
                                wModel[adress] = model;
                            else
                                wModel.Add(adress, model);
                            model.state = 0;
                            model.sysFlag = 1;
                            cBll.Update(model);
                            var callString = wModel[adress].ticketNumber + "号回呼";
                            client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = wNum[adress], AreaNo = wArea[wNum[adress]], IsLEDMessage = true, IsSoundMessage = true });
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                            SendTicket(adress, wHang[adress].ticketNumber.Substring(wHang[adress].ticketNumber.Length - 3, 3));
                            wState[adress] = WorkState.Call;
                            WriterCallLog(7, callString);
                            wHang.Clear();
                        }
                    }
                }
            }

        }

        #endregion

        #region
        //发送三位票号
        private void SendTicket(int adress, string ticket)
        {
            var tfir = (byte)Convert.ToInt32(ticket.Substring(0, 1));
            var tsec = (byte)Convert.ToInt32(ticket.Substring(1, 1));
            var tthr = (byte)Convert.ToInt32(ticket.Substring(2, 1));
            var send = new byte[15];
            send[0] = 0xFF;
            send[1] = 0x68;
            send[2] = 0x08;
            send[3] = 0x08;
            send[4] = 0x68;
            send[5] = (byte)adress;
            send[6] = 0x02;
            send[7] = 0x0A;
            send[8] = 0x0A;
            send[9] = 0x0A;
            send[10] = tfir;
            send[11] = tsec;
            send[12] = tthr;
            send[13] = (byte)((adress + 2 + tfir + tsec + tthr + 0x0A + 0x0A + 0x0A) % 256);
            send[14] = 0x16;
            SendOrder(send);
        }

        private void SendWait(int adress)
        {
            var list = qBll.GetModelList(wlBusy[adress], 0);
            var lcount = list.ToList().Count;
            string count = lcount > 999 ? "999" : lcount.ToString("000");
            var wfir = (byte)Convert.ToInt32(count.Substring(0, 1));
            var wsec = (byte)Convert.ToInt32(count.Substring(1, 1));
            var wthr = (byte)Convert.ToInt32(count.Substring(2, 1));
            var send = new byte[15];
            send[0] = 0xFF;
            send[1] = 0x68;
            send[2] = 0x08;
            send[3] = 0x08;
            send[4] = 0x68;
            send[5] = (byte)adress;
            send[6] = 0x02;
            send[7] = wfir;
            send[8] = wsec;
            send[9] = wthr;
            send[10] = 0x0A;
            send[11] = 0x0A;
            send[12] = 0x0A;
            send[13] = (byte)((adress + 2 + wfir + wsec + wthr + 0x0A + 0x0A + 0x0A) % 256);
            send[14] = 0x16;
            SendOrder(send);
        }

        //叫号器蜂鸣
        private void SendBeep(int adress)
        {
            var send = new byte[9];
            send[0] = 0xFF;
            send[1] = 0x68;
            send[2] = 0x02;
            send[3] = 0x02;
            send[4] = 0x68;
            send[5] = (byte)adress;
            send[6] = 0x03;
            send[7] = (byte)((adress + 3) % 256);// 0x03;//计算校验和
            send[8] = 0x16;
            SendOrder(send);
        }
        #endregion

        #endregion

        #region 测试
        private void btnCall_Click(object sender, EventArgs e)
        {
            CallNo(int.Parse(cmbAdress.Text));
        }

        private void btnReCall_Click(object sender, EventArgs e)
        {
            ReCallNo(int.Parse(cmbAdress.Text));
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            //FinishWork(int.Parse(cmbAdress.Text));
            //CallNo(1);
        }
        private void btnEv_Click(object sender, EventArgs e)
        {
            EvaluateService(int.Parse(cmbAdress.Text));
        }

        private void btnCance_Click(object sender, EventArgs e)
        {
            GiveUpNo(int.Parse(cmbAdress.Text));
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Pause(int.Parse(cmbAdress.Text));
        }


        #endregion

        private void btnSet_Click(object sender, EventArgs e)
        {
            frmQueueSet frm = new frmQueueSet();
            frm.ShowDialog();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] cle = clearTime.Split(',');
            foreach (var c in cle)
            {
                if (DateTime.Now.ToString("HH:mm:ss") == c)
                {
                    var tList = cBll.GiveUpAll();
                    foreach (var t in tList)
                    {
                        string mess = "自动批量弃号：窗口[" + t.windowNumber + "]票号[" + t.ticketNumber + "]已完成弃号";
                        this.client.SendMessage(new OperateMessage() { WindowNo = t.windowNumber, Operate = Operate.Reset });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                        if (wCall.Keys.Contains(t.windowNumber))
                            wState[wCall[t.windowNumber]] = WorkState.Defalt;
                        WriterCallLog(4, mess);
                    }
                }
            }
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
                oprateFlag = clientName,
                oprateType = "叫号端",
                oprateClassifyType = otype,
                oprateTime = DateTime.Now,
                oprateLog = text,
                sysFlag =0
            });
        }


    }

    public enum WorkState
    {
        /// <summary>
        /// 默认初始
        /// </summary>
        Defalt,
        /// <summary>
        /// 呼叫
        /// </summary>
        Call,
        /// <summary>
        /// 转移
        /// </summary>
        Transfer,
        /// <summary>
        /// 已评价
        /// </summary>
        Evaluate,
        /// <summary>
        /// 暂停服务
        /// </summary>
        PauseService

    }
}
