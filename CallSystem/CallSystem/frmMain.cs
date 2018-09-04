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
        Queue<Oprate> QueueList = new Queue<Oprate>();
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
        string areaId = "";
        int EvaluatorType = 0;
        //string test = "";
        BEvaluateBLL eBll = new BEvaluateBLL();
        TOprateLogBLL oBll = new TOprateLogBLL();
        BQueueBLL qBll = new BQueueBLL();
        TWindowBLL wBll = new TWindowBLL();
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        TBusinessAttributeBLL baBll = new TBusinessAttributeBLL();
        List<TBusinessAttributeModel> baList = new List<TBusinessAttributeModel>();
        BCallBLL cBll = new BCallBLL();
        List<TWindowModel> wList;
        List<TWindowBusinessModel> wbList;
        Dictionary<string, string> wArea;
        Dictionary<int, WorkState> wState;
        Dictionary<int, FCallStateModel> csState;
        Dictionary<int, WorkState> wpState;
        Dictionary<int, BCallModel> wModel;//呼叫器-呼叫状态
        Dictionary<int, string> wNum;//呼叫器-窗口号
        Dictionary<int, TWindowBusinessModel> wBusy;//呼叫器-窗口业务
        Dictionary<int, Dictionary<string, int>> wReCall;//重呼限制
        Dictionary<int, List<TWindowBusinessModel>> wlBusy;
        Dictionary<int, List<TWindowBusinessModel>> wbBusy;//属于绿色通道的窗口业务
        Dictionary<string, string> wUser;//窗口以及对应的当前登录用户
        Dictionary<string, int> wCall;
        Client client = new Client();
        object objLock = new object();
        FCallStateBLL csBll = new FCallStateBLL();
        Thread thread;
        bool isBool = true;
        object Obj = new object();
        LockAction action = new LockAction();
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
            SetConfigValue("AreaId", "1,2");
            SetConfigValue("EvaluatorType", "0");
            areaId = System.Configuration.ConfigurationManager.AppSettings["AreaId"];
            clientName = System.Configuration.ConfigurationManager.AppSettings["ClientName"];
            clearTime = System.Configuration.ConfigurationManager.AppSettings["ClearTime"];
            ip = System.Configuration.ConfigurationManager.AppSettings["IP"];
            port = System.Configuration.ConfigurationManager.AppSettings["Port"];
            EvaluatorType = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["EvaluatorType"]);
            cmbAdress.Text = "1";
            ini = new OperateIni(System.Windows.Forms.Application.StartupPath + @"\WindowConfig.ini");
            portName = ini.ReadString("CallSet", "SerialPort");
            wList = wBll.GetModelList().Where(w => w.State == "1").ToList();
            wbList = wbBll.GetModelList();
            baList = baBll.GetModelList();
            wArea = new Dictionary<string, string>();
            wState = new Dictionary<int, WorkState>();
            csState = new Dictionary<int, FCallStateModel>();
            wpState = new Dictionary<int, WorkState>();
            wModel = new Dictionary<int, BCallModel>();
            wNum = new Dictionary<int, string>();
            wCall = new Dictionary<string, int>();
            wBusy = new Dictionary<int, TWindowBusinessModel>();
            wlBusy = new Dictionary<int, List<TWindowBusinessModel>>();
            wbBusy = new Dictionary<int, List<TWindowBusinessModel>>();
            wReCall = new Dictionary<int, Dictionary<string, int>>();
            wUser = new Dictionary<string, string>();
            //根据配置分区窗口
            var areaList = areaId.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            wList = wList.Where(w => areaList.Contains(w.AreaName.ToString())).ToList();
            foreach (var w in wList)
            {
                wArea[w.Number] = w.AreaName.ToString();
                wCall[w.Number] = w.CallNumber;
                wNum[w.CallNumber] = w.Number;
                wState[w.CallNumber] = WorkState.Defalt;
                csState[w.CallNumber] = new FCallStateModel();
                var busyList = wbList.Where(b => b.WindowID == w.ID).ToList().OrderBy(o => o.priorityLevel).ToList();
                var busy = busyList.FirstOrDefault();
                if (busy != null)
                {
                    wlBusy[w.CallNumber] = busyList;
                    wBusy[w.CallNumber] = busy;
                    var gbList = new List<TWindowBusinessModel>();
                    foreach (var bs in busyList)
                    {
                        var gb = baList.Where(b => b.unitSeq == bs.unitSeq && b.busiSeq == bs.busiSeq && b.isGreenChannel == 1).FirstOrDefault();
                        if (gb != null)
                            gbList.Add(bs);
                    }
                    wbBusy[w.CallNumber] = gbList;
                }
            }
            serialPort = new SerialPort();
            serialPort.PortName = portName;
            serialPort.BaudRate = 9600;
            serialPort.Parity = Parity.None;
            serialPort.DataBits = 8;
            serialPort.StopBits = StopBits.One;
            serialPort.ReadBufferSize = 40960;
            serialPort.ReceivedBytesThreshold = 1;
            serialPort.ErrorReceived += new SerialErrorReceivedEventHandler(serialPort_ErrorReceived);
            try
            {
                serialPort.Open();
            }
            catch (Exception ex)
            {
                MessageBox.Show("呼叫器端口打开失败，请重新配置：" + ex.Message);
            }
            client.ServerIP = ip;
            client.ServerPort = ushort.Parse(port);
            client.ClientType = ClientType.Window;
            client.ClientName = clientName;
            this.client.OnResult += (msgType, msgText) =>
            {
                this.messageIndicator1.SetState(StateType.Success, msgText);
            };
            this.client.OnConnect += () =>
            {
                this.client.SendMessage(new ClientQueryMessage());
            };
            this.client.OnDisconnect += () =>
            {
                this.messageIndicator1.SetState(StateType.Error, "未连接");
            };
            this.client.OnMessage += new Action<QueueMessage.Message>(client_OnMessage);
            client.Start();

            new Thread(() =>
            {
                while (isBool)
                {
                    try
                    {
                        byte temp = (byte)serialPort.ReadByte();
                        if (temp == Head[0])
                        {
                            byte temp2 = (byte)serialPort.ReadByte();
                            if (temp2 == Head[1])
                            {
                                #region 数据接收
                                //WriterSerialPortLog("2.Enter Package...");
                                var length = (byte)serialPort.ReadByte();//数据长度
                                var length2 = (byte)serialPort.ReadByte();//数据长度
                                var fixedvalue = (byte)serialPort.ReadByte();//固定值 0x68
                                var adress = (byte)serialPort.ReadByte();//通讯地址
                                var funccode = (byte)serialPort.ReadByte();//功能码
                                //WriterSerialPortLog(string.Format("3.Received package code [adress:{0}][funccode:{1}].", adress, funccode));
                                var xvalue = 0;//评价器值
                                int index = 0;
                                if (length <= 2)
                                {
                                    WriterLog("有呼叫器【" + adress + "】协议出错，发送过来的数据长度小于等于2【" + length + "】！本次操作取消！");
                                    continue;
                                }
                                else
                                {
                                    if (length != length2 || length > 5)
                                    {
                                        WriterLog("有呼叫器【" + adress + "】发送过来的数据长度【{" + length + "}{" + length2 + "}】超过设定值【5】,疑似协议出错，该条过滤！");
                                        continue;
                                    }
                                }
                                var data = new byte[length - 2];//发送的 数据内容
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
                                if (funccode != 0x11)
                                {
                                    send[6] = funccode;// funccode;
                                    int startIndex = 7;
                                    foreach (byte b in data)
                                    {
                                        send[startIndex] = b;
                                        startIndex++;
                                    }
                                    send[7 + data.Length] = check;
                                    send[8 + data.Length] = end;
                                }
                                else
                                {
                                    send[6] = 0x01;
                                    send[7] = data[0];
                                    xvalue = data[0];
                                    send[8] = (byte)(check - 0x10);
                                    send[9] = end;
                                }
                                SendOrder(send);

                                string sendOrder = "";
                                foreach (var f in send.ToList())
                                {
                                    sendOrder += (" " + f.ToString("X"));
                                }
                                //WriterSerialPortLog(string.Format("4.Send Response Order[{0}]", sendOrder.Length > 0 ? sendOrder.Substring(1) : ""));
                                #endregion

                                #region 功能

                                if (funccode != 0x11)
                                {
                                    #region  功能操作
                                    //判断操作类型，进行对应的操作
                                    if (data[data.Length - 1] == 0x0C)
                                    {
                                        lock (objLock)
                                        {
                                            Oprate opr = new Oprate() { Adress = adress, Type = OType.Call };
                                            QueueList.Enqueue(opr);
                                        }
                                        //CallNo(adress);//呼叫
                                    }
                                    else if (data[data.Length - 1] == 0x0D)
                                    {
                                        lock (objLock)
                                        {
                                            Oprate opr = new Oprate() { Adress = adress, Type = OType.ReCall };
                                            QueueList.Enqueue(opr);
                                        }
                                        //ReCallNo(adress); //重呼
                                    }
                                    else if (data[data.Length - 1] == 0x0B)
                                    {
                                        lock (objLock)
                                        {
                                            Oprate opr = new Oprate() { Adress = adress, Type = OType.Evaluate };
                                            QueueList.Enqueue(opr);
                                        }
                                        //EvaluateService(adress); //评价
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
                                                lock (objLock)
                                                {
                                                    Oprate opr = new Oprate() { Adress = adress, Type = OType.GiveUp };
                                                    QueueList.Enqueue(opr);
                                                }
                                                //GiveUpNo(adress);//6+确认为弃号
                                            }
                                            else if (data[data.Length - 2] == 0x00)
                                            {
                                                //0+确认为挂起
                                                lock (objLock)
                                                {
                                                    Oprate opr = new Oprate() { Adress = adress, Type = OType.Hang };
                                                    QueueList.Enqueue(opr);
                                                }
                                                //Hang(adress);
                                            }
                                            else if (data[data.Length - 2] == 0x03)
                                            {
                                                //3+确认为回呼
                                                lock (objLock)
                                                {
                                                    Oprate opr = new Oprate() { Adress = adress, Type = OType.CallBack };
                                                    QueueList.Enqueue(opr);
                                                }
                                                //CallBack(adress);
                                            }
                                        }
                                        else
                                        {
                                            WriterLog("发送确定键，但是指令未用，本次操作自动忽略！");
                                            continue;
                                        }
                                    }
                                    else if (data[data.Length - 1] == 0x11)
                                    {
                                        //一米 ******** 暂无处理
                                    }
                                    else if (data[data.Length - 1] == 0x12)
                                    {
                                        //等候  暂停功能
                                        lock (objLock)
                                        {
                                            Oprate opr = new Oprate() { Adress = adress, Type = OType.Pause };
                                            QueueList.Enqueue(opr);
                                        }
                                        //Pause(adress);
                                    }
                                    else if (data[data.Length - 1] == 0x13)
                                    {
                                        //转移
                                        lock (objLock)
                                        {
                                            Oprate opr = new Oprate() { Adress = adress, Type = OType.Transfer };
                                            QueueList.Enqueue(opr);
                                        }
                                        //Transfer(adress);
                                    }

                                    #endregion
                                }
                                else
                                {
                                    #region  评价器接收数据
                                    lock (objLock)
                                    {
                                        Oprate opr = new Oprate() { Adress = adress, Type = OType.SaveEvaluate, Value = xvalue };
                                        QueueList.Enqueue(opr);
                                    }
                                    //SaveEvaluate(adress, xvalue);
                                    #endregion
                                }
                                #endregion
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        WriterLog("串口接受数据异常：" + ex.Message);
                    }
                }
            }) { IsBackground = true }.Start();
            thread = new Thread(new ThreadStart(Process));
            thread.IsBackground = true;
            thread.Start();
            this.Hide();
            this.ShowInTaskbar = false;
        }

        void client_OnMessage(QueueMessage.Message obj)
        {
            switch (obj.GetType().Name)
            {
                case MessageName.ClientQueryMessage:
                    {
                        wUser = new Dictionary<string, string>();
                        var msg = obj as ClientQueryMessage;
                        foreach (var li in msg.ClientList)
                        {
                            if (!wUser.ContainsKey(li.Key))
                            {
                                wUser.Add(li.Key, li.Value);
                            }
                        }
                    }
                    break;
                case MessageName.ClientChangedMessage:
                    {
                        var msg = obj as ClientChangedMessage;
                        if (msg.ChangedType == ClientChangedType.Add)
                        {
                            wUser[msg.WindowNumber] = msg.UserID;
                        }
                        else
                            wUser[msg.WindowNumber] = "";
                    }
                    break;
            }
        }

        void Process()
        {
            while (true)
            {
                while (QueueList.Count > 0)
                {
                    var opr = QueueList.Dequeue();
                    DateTime start = DateTime.Now;
                    switch (opr.Type)
                    {
                        case OType.Call:
                            {
                                CallNo(opr.Adress);
                                break;
                            }
                        case OType.CallBack:
                            {
                                CallBack(opr.Adress);
                                break;
                            }
                        case OType.Evaluate:
                            {
                                EvaluateService(opr.Adress);
                                break;
                            }
                        case OType.SaveEvaluate:
                            {
                                SaveEvaluate(opr.Adress, opr.Value);
                                break;
                            }
                        case OType.GiveUp:
                            {
                                GiveUpNo(opr.Adress);
                                break;
                            }
                        case OType.Hang:
                            {
                                Hang(opr.Adress);
                                break;
                            }
                        case OType.Pause:
                            {
                                Pause(opr.Adress);
                                break;
                            }
                        case OType.ReCall:
                            {
                                ReCallNo(opr.Adress);
                                break;
                            }
                        case OType.Transfer:
                            {
                                Transfer(opr.Adress);
                                break;
                            }
                        default: break;
                    }

                }
                Thread.Sleep(1000);
            }
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

        void serialPort_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
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
            isBool = false;
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
                if (thread != null)
                    thread.Abort();
            }
            catch
            { }
            try
            {
                client.Stop();
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

        #region 8 大方法
        //呼叫*顺呼
        private void CallNo(int adress)
        {
            DateTime start = DateTime.Now;
            if (GetWindowByAdress(adress, 0))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        csState[adress] = new FCallStateModel();
                        csState[adress].windowNo = wNum[adress];
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress] = csBll.Insert(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Defalt || csState[adress].workState == (int)WorkState.Evaluate)
                    {

                        try
                        {
                            var userCode = "";
                            if (wUser.ContainsKey(wNum[adress]))
                                userCode = wUser[wNum[adress]];
                            var model = cBll.CallNo(wlBusy[adress], wbBusy[adress], wNum[adress], userCode);//用户暂时为空
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
                                client.SendMessage(new WeChatMessage() { ID = model.qId.ToString() });
                                csState[adress].workState = (int)WorkState.Call;
                                csState[adress].ticketNo = model.ticketNumber;
                                csState[adress].callId = model.ID;
                                csState[adress].reCallTimes = 0;
                                csBll.Update(csState[adress]);
                                this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                                SendTicket(adress, model.ticketNumber.Substring(model.ticketNumber.Length - 3, 3));
                                WriterCallLog(0, callString, adress);

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

        string userCode = "";
        AutoResetEvent are = new AutoResetEvent(false);

        string GetUserCode(string winNo)
        {
            client.Send("");
            are.WaitOne();
            return userCode;
        }

        //重呼
        private void ReCallNo(int adress)
        {
            if (GetWindowByAdress(adress, 1))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        return;
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Call)
                    {
                        var model = cBll.GetModel(csState[adress].callId);
                        if (model == null)
                        {
                            return;
                        }
                        if (csState[adress].reCallTimes >= 5)
                        {
                            return;
                        }
                        else
                        {
                            csState[adress].reCallTimes = csState[adress].reCallTimes + 1;
                            csBll.Update(csState[adress]);
                        }
                        var callString = "请" + model.ticketNumber + "号到 " + wNum[adress] + "号窗口办理(重呼) ";
                        client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = wNum[adress], AreaNo = wArea[wNum[adress]], IsLEDMessage = true, IsSoundMessage = true });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendTicket(adress, model.ticketNumber.Substring(model.ticketNumber.Length - 3, 3));
                        WriterCallLog(1, callString, adress);
                    }
                });
            }
        }
        //评价
        private void EvaluateService(int adress)
        {
            if (GetWindowByAdress(adress, 2))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        csState[adress] = new FCallStateModel();
                        csState[adress].windowNo = wNum[adress];
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress] = csBll.Insert(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Call)
                    {
                        try
                        {
                            var model = cBll.GetModel(csState[adress].callId);
                            if (model == null)
                            {
                                return;
                            }
                            model.finishTime = DateTime.Now;
                            model.state = 1;
                            cBll.Update(model);
                            csState[adress].workState = (int)WorkState.Evaluate;
                            //csState[adress].callId = 0;
                            csState[adress].ticketNo = "";
                            csBll.Update(csState[adress]);
                            client.SendMessage(new RateMessage() //发送评价请求
                            {
                                WindowNo = wNum[adress],
                                RateId = model.handleId,
                                ItemName = "项目名称",
                                WorkDate = DateTime.Now.ToShortDateString(),
                                Transactor = model.qNmae,
                                reserveSeq = model.reserveSeq
                            }
                            );
                            //SendStartE(adress);
                            SendWait(adress);
                            string mess = " [" + model.ticketNumber + "]号已评价。";
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : [" + model.ticketNumber + "]号已评价。"); }));
                            WriterCallLog(2, mess, adress);
                        }
                        catch (Exception ex)
                        {
                            WriterLog("评价异常：" + ex.Message);
                        }
                    }
                    else
                    {
                        if (csState[adress].workState == (int)WorkState.Defalt || csState[adress].workState == (int)WorkState.Evaluate)
                        {
                            SendWait(adress);
                        }
                    }
                });
            }
        }
        //弃号
        private void GiveUpNo(int adress)
        {
            if (GetWindowByAdress(adress, 4))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        return;
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Call)
                    {
                        try
                        {
                            var model = cBll.GetModel(csState[adress].callId);
                            if (model == null)
                            {
                                return;
                            }
                            string mess = model.ticketNumber + "号已弃号。";
                            model.state = -1;
                            model.finishTime = DateTime.Now;
                            cBll.Update(model);
                            csState[adress].workState = (int)WorkState.Evaluate;
                            csState[adress].callId = 0;
                            csState[adress].ticketNo = "";
                            csBll.Update(csState[adress]);
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                            this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                            SendWait(adress);
                            WriterCallLog(4, mess, adress);
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
        private void Pause(int adress)
        {
            if (GetWindowByAdress(adress, 3))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        csState[adress] = new FCallStateModel();
                        csState[adress].windowNo = wNum[adress];
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress] = csBll.Insert(csState[adress]);
                    }
                    if (csState[adress].workState != (int)WorkState.PauseService)
                    {
                        string mess = wNum[adress] + "号窗口暂停服务";
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Pause });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                        csState[adress].pauseState = csState[adress].workState;
                        csState[adress].workState = (int)WorkState.PauseService;
                        csBll.Update(csState[adress]);
                        WriterCallLog(3, mess, adress);
                    }
                });
            }
        }
        //转移-丢回去
        private void Transfer(int adress)
        {
            if (GetWindowByAdress(adress, 5))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        csState[adress] = new FCallStateModel();
                        csState[adress].windowNo = wNum[adress];
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress] = csBll.Insert(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Call)
                    {
                        var model = cBll.GetModel(csState[adress].callId);
                        if (model == null)
                        {
                            return;
                        }
                        //转移号码
                        model.finishTime = DateTime.Now;
                        cBll.Transfer(model);
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        var callString = model.ticketNumber + "号已转移(重置) ";
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendWait(adress);
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress].callId = 0;
                        csState[adress].ticketNo = "";
                        csState[adress].reCallTimes = 0;
                        csBll.Update(csState[adress]);
                        WriterCallLog(5, callString, adress);
                    }
                });
            }
        }
        //挂起
        private void Hang(int adress)
        {
            if (GetWindowByAdress(adress, 6))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        csState[adress] = new FCallStateModel();
                        csState[adress].windowNo = wNum[adress];
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress] = csBll.Insert(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Call)
                    {
                        var model = cBll.GetModel(csState[adress].callId);
                        if (model == null)
                        {
                            return;
                        }
                        model.state = 3;
                        cBll.Update(model);
                        var callString = model.ticketNumber + "号已挂起";
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendWait(adress);
                        csState[adress].workState = (int)WorkState.Defalt;
                        csState[adress].hangId = model.ID;
                        csState[adress].callId = 0;
                        csState[adress].ticketNo = "";
                        csState[adress].reCallTimes = 0;
                        csBll.Update(csState[adress]);
                        WriterCallLog(6, callString, adress);
                    }
                });
            }
        }
        //回呼
        private void CallBack(int adress)
        {
            if (GetWindowByAdress(adress, 7))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        return;
                    }
                    if (csState[adress].workState == (int)WorkState.PauseService)
                    {
                        this.client.SendMessage(new OperateMessage() { WindowNo = wNum[adress], Operate = Operate.Reset });
                        csState[adress].workState = csState[adress].pauseState;
                        csBll.Update(csState[adress]);
                    }
                    if (csState[adress].workState == (int)WorkState.Defalt || csState[adress].workState == (int)WorkState.Evaluate)
                    {

                        var model = cBll.GetModel(csState[adress].hangId);
                        if (model == null)
                        {
                            return;
                        }
                        else
                        {
                            if (model.ticketTime.Date != DateTime.Now.Date)
                            {
                                csState[adress].hangId = 0;
                                csBll.Update(csState[adress]);
                                return;
                            }
                        }
                        model.state = 0;
                        cBll.Update(model);
                        var callString = model.ticketNumber + "号回呼";
                        client.SendMessage(new CallMessage() { TicketNo = model.ticketNumber, WindowNo = wNum[adress], AreaNo = wArea[wNum[adress]], IsLEDMessage = true, IsSoundMessage = true });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + callString); }));
                        SendTicket(adress, model.ticketNumber.Substring(model.ticketNumber.Length - 3, 3));
                        csState[adress].workState = (int)WorkState.Call;
                        csState[adress].ticketNo = model.ticketNumber;
                        csState[adress].callId = model.ID;
                        csState[adress].reCallTimes = 0;
                        csState[adress].hangId = 0;
                        csBll.Update(csState[adress]);
                        WriterCallLog(7, callString, adress);
                    }
                });

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
            //WriterSendLog(string.Format("地址【{0}】发送发票【{1}】已完成。", adress, ticket), adress);
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
            //WriterSendLog(string.Format("地址【{0}】发送等候人数【{1}】已完成。", adress, count.ToString()), adress);
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

        #region 评价器相关

        //发送服务星级、默认都是满星 （经过测试开机后默认就是满星 不需要发送）
        private void SendServiceLever(int adress)
        {
            var send = new byte[10];
            send[0] = 0xFF;
            send[1] = 0x68;
            send[2] = 0x03;
            send[3] = 0x03;
            send[4] = 0x68;
            send[5] = (byte)adress;
            send[6] = 0x02;
            send[7] = 0x05;
            send[8] = (byte)((adress + 2 + 5) % 256);// 0x03;//计算校验和
            send[9] = 0x16;
            SendOrder(send);
        }

        //开启评价 ** 暂时没有用，经过测试点击评价后自动开启评价，无需再次发送命令
        private void SendStartE(int adress)
        {
            var send = new byte[10];
            send[0] = 0xFF;
            send[1] = 0x68;
            send[2] = 0x03;
            send[3] = 0x03;
            send[4] = 0x68;
            send[5] = (byte)adress;
            send[6] = 0x01;
            send[7] = 0x0B;
            send[8] = (byte)((adress + 1 + 0x0B) % 256);// 0x03;//计算校验和
            send[9] = 0x16;
            SendOrder(send);
        }

        //保存评价数据
        private void SaveEvaluate(int adress, int xvalue)
        {
            if (GetWindowByAdress(adress, 2))
            {
                action.lockWin(wNum[adress], () =>
                {
                    csState[adress] = csBll.GetModelByWindowNo(wNum[adress]);
                    if (csState[adress] == null)
                    {
                        return;
                    }
                    if (csState[adress].workState == (int)WorkState.Evaluate)
                    {
                        try
                        {
                            var model = cBll.GetModel(csState[adress].callId);
                            if (model == null)
                            {
                                return;
                            }
                            SaveEvaluate(model, wNum[adress], xvalue);
                            csState[adress].workState = (int)WorkState.Defalt;
                            csBll.Update(csState[adress]);
                            //SendWait(adress);
                            string mess = " [" + model.ticketNumber + "]号已评价(4键评价器)。";
                            this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                            WriterCallLog(2, mess, adress);
                        }
                        catch (Exception ex)
                        {
                            WriterLog("评价保存异常：" + ex.Message);
                        }
                    }
                });
            }
        }

        //保存评价数据(4键评价器)
        private void SaveEvaluate(BCallModel call, string WindowNo, int xvalue)
        {
            BEvaluateModel ev = new BEvaluateModel();
            ev.type = 2;
            ev.handId = call.ID;
            ev.unitSeq = call.unitSeq;
            ev.windowNumber = WindowNo;
            ev.handleTime = DateTime.Now;
            ev.custCardId = call.idCard;
            ev.name = call.qNmae;
            ev.windowUser = "";//4键评价器 没有登录人员
            ev.approveSeq = call.busiSeq;
            ev.evaluateResult = xvalue == 0 ? 1 : xvalue == 1 ? 0 : xvalue == 2 ? 3 : 4;
            eBll.Insert(ev);
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

        private void btnMove_Click(object sender, EventArgs e)
        {
            Transfer(int.Parse(cmbAdress.Text));
        }

        private void btnHank_Click(object sender, EventArgs e)
        {
            Hang(int.Parse(cmbAdress.Text));
        }

        private void btnReCall1_Click(object sender, EventArgs e)
        {
            CallBack(int.Parse(cmbAdress.Text));
        }


        #endregion

        private void btnSet_Click(object sender, EventArgs e)
        {
            frmQueueSet frm = new frmQueueSet();
            frm.ShowDialog();
        }
        LockBLL lbll = new LockBLL();
        private void timer1_Tick(object sender, EventArgs e)
        {
            string[] cle = clearTime.Split(',');
            foreach (var c in cle)
            {
                if (DateTime.Now.ToString("HH:mm:ss") == c)
                {
                    var tList = cBll.GiveUpAll();
                    foreach (var t in tList.Select(s => s.windowNumber).Distinct())
                    {
                        lbll.releaseWin(t);
                        var stateModel = csBll.GetModelByWindowNo(t);
                        if (stateModel != null)
                        {
                            stateModel.callId = 0;
                            stateModel.workState = 0;
                            stateModel.hangId = 0;
                            stateModel.pauseState = 0;
                            stateModel.ticketNo = "";
                            csBll.Update(stateModel);
                        }
                    }
                    foreach (var t in tList)
                    {
                        string mess = "自动批量弃号：窗口[" + t.windowNumber + "]票号[" + t.ticketNumber + "]已完成弃号";
                        this.client.SendMessage(new OperateMessage() { WindowNo = t.windowNumber, Operate = Operate.Reset });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                        if (wCall.Keys.Contains(t.windowNumber))
                            wState[wCall[t.windowNumber]] = WorkState.Defalt;
                        WriterCallLog(4, mess, 0);
                    }
                }
            }
        }

        private void WriterLog(string text)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Exception.txt", DateTime.Now + " : " + text + "\r\n");
        }

        private void WriterCallLog(int type, string text, int adress)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            string otype = type == 0 ? "叫号" : type == 1 ? "重呼" : type == 2 ? "评价" : type == 3 ? "暂停" : type == 4 ? "弃号" : type == 5 ? "转移" : type == 6 ? "挂起" : "回呼";
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Record.txt", DateTime.Now + " : 【" + otype + "】" + text + "\r\n");
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Record_" + adress + ".txt", DateTime.Now + " : 【" + otype + "】" + text + "\r\n");
            oBll.Insert(new TOprateLogModel()
            {
                oprateFlag = clientName,
                oprateType = "叫号端",
                oprateClassifyType = otype,
                oprateTime = DateTime.Now,
                oprateLog = text,
            });
        }

        private void WriterTimeLog(string info, int adress)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Time.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + info + "\r\n");
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_Time_" + adress.ToString() + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + info + "\r\n");
        }

        private void WriterSendLog(string info, int adress)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_SendInfo.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + info + "\r\n");
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_SendInfo_" + adress.ToString() + ".txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + info + "\r\n");
        }

        object sObj = new object();
        private void WriterSerialPortLog(string text)
        {
            lock (sObj)
            {
                string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Call_SerialPort.txt", DateTime.Now + " : " + text + "\r\n");
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            if (DialogResult.Cancel == MessageBox.Show("确认进行清号？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                return;
            try
            {
                var tList = cBll.GiveUpAll();
                if (tList != null && tList.Count != 0)
                {
                    foreach (var t in tList.Select(s => s.windowNumber).Distinct())
                    {
                        lbll.releaseWin(t);
                        var stateModel = csBll.GetModelByWindowNo(t);
                        if (stateModel != null)
                        {
                            stateModel.callId = 0;
                            stateModel.workState = 0;
                            stateModel.hangId = 0;
                            stateModel.pauseState = 0;
                            stateModel.ticketNo = "";
                            csBll.Update(stateModel);
                        }
                    }
                    foreach (var t in tList)
                    {
                        string mess = "自动批量弃号：窗口[" + t.windowNumber + "]票号[" + t.ticketNumber + "]已完成弃号";
                        this.client.SendMessage(new OperateMessage() { WindowNo = t.windowNumber, Operate = Operate.Reset });
                        this.Invoke(new Action(() => { this.listView1.Items.Add(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:fff") + " : " + mess); }));
                        if (wCall.Keys.Contains(t.windowNumber))
                            wState[wCall[t.windowNumber]] = WorkState.Defalt;
                        WriterCallLog(4, mess, 0);
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    public enum WorkState
    {
        /// <summary>
        /// 默认初始
        /// </summary>
        Defalt = 0,
        /// <summary>
        /// 呼叫
        /// </summary>
        Call = 1,
        /// <summary>
        /// 转移
        /// </summary>
        Transfer,
        /// <summary>
        /// 已评价
        /// </summary>
        Evaluate = 2,
        /// <summary>
        /// 暂停服务
        /// </summary>
        PauseService = 3

    }

    public enum OType
    {
        Call,
        ReCall,
        Evaluate,
        GiveUp,
        Pause,
        Transfer,
        Hang,
        CallBack,
        SaveEvaluate
    }
    public class Oprate
    {
        public OType Type { get; set; }
        public int Adress { get; set; }
        public int Value { get; set; }
    }
}
