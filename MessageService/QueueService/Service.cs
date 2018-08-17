using System;
using System.Linq;
using System.Web.Script.Serialization;
using MessageLib;
using QueueMessage;

namespace QueueService
{
    public class Service : TcpServer<ExtraData>, IServiceUI
    {
        Process process;
        internal Extra<IntPtr, ClientInfo> clientList = new Extra<IntPtr, ClientInfo>();
        JavaScriptSerializer convert = new JavaScriptSerializer();

        public Service()
        {
            process = new Process();
            process.ReceiveMessage += new Action<IntPtr, Message>(process_ReceiveMessage);
            this.OnAccept += new TcpServerEvent.OnAcceptEventHandler(Service_OnAccept);
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnReceive += new TcpServerEvent.OnReceiveEventHandler(Service_OnReceive);
        }

        HandleResult Service_OnAccept(TcpServer server, IntPtr connId, IntPtr pClient)
        {
            this.SetExtra(connId, new ExtraData());
            return HandleResult.Ok;
        }

        HandleResult Service_OnClose(TcpServer server, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            this.RemoveExtra(connId);
            this.clientList.Remove(connId);
            this.clientList.Changed = true;
            return HandleResult.Ok;
        }

        HandleResult Service_OnReceive(IntPtr connId, byte[] bytes)
        {
            ExtraData msg = null;
            try
            {
                msg = this.GetExtra(connId);
                this.process.RecvData(connId, msg, bytes);
            }
            catch (Exception ex)
            {
                this.SDK_OnError(this, connId, ex);
                msg.Data = null;
                msg.Position = 0;
            }
            return HandleResult.Ok;
        }

        public void SendMessage(IntPtr connId, Message message)
        {
            var bytes = this.process.FormatterMessageBytes(message);
            this.Send(connId, bytes, bytes.Length);
        }

        void process_ReceiveMessage(IntPtr connId, Message message)
        {
            var msgName = message.GetType().Name;
            if (msgName != MessageName.LoginMessage && !this.clientList.Dictionary.ContainsKey(connId))
            {
                var resultMsg = new ResultMessage();
                resultMsg.Operate = msgName;
                resultMsg.Result = "当前用户尚未登录";
                this.SendMessage(connId, resultMsg);
                return;
            }
            switch (msgName)
            {
                case MessageName.LoginMessage:
                    #region login
                    {
                        var islogin = false;
                        if (this.clientList.Dictionary.ContainsKey(connId))
                            islogin = true;
                        else
                        {
                            var msg = message as LoginMessage;
                            string ip = "";
                            ushort port = 0;
                            this.GetRemoteAddress(connId, ref ip, ref port);
                            this.clientList.Set(connId, new ClientInfo()
                            {
                                ID = connId,
                                IP = ip + ":" + port,
                                Name = msg.ClientName,
                                Type = msg.ClientType.ToString(),
                                ConnTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                            });
                            this.clientList.Changed = true;
                        }
                        var resultMsg = new ResultMessage();
                        resultMsg.Operate = OperateName.Login;
                        resultMsg.Result = islogin ? "您已登录过系统" : "登录成功";
                        this.SendMessage(connId, resultMsg);
                    }
                    #endregion
                    break;
                case MessageName.ClientQueryMessage:
                    #region clientQuery
                    {
                        var msg = message as ClientQueryMessage;
                        var allClient = this.clientList.Dictionary.Values.ToArray();
                        if (msg.QueryType == ClientQueryType.Request)
                        {
                            msg.QueryClientID = connId;
                            var bytes = this.process.FormatterMessageBytes(message);
                            var listRate = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.RateService);
                            foreach (var client in listRate)
                                this.Send(client.ID, bytes, bytes.Length);
                        }
                        else
                        {
                            var bytes = this.process.FormatterMessageBytes(message);
                            var listWin = allClient.Where(p => p.Type == ClientType.Window.ToString() && p.ID == msg.QueryClientID);
                            foreach (var client in listWin)
                                this.Send(client.ID, bytes, bytes.Length);
                        }
                    }
                    #endregion
                    break;
                case MessageName.ClientChangedMessage:
                    #region clientChanged
                    {
                        var msg = message as ClientChangedMessage;
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = this.clientList.Dictionary.Values.ToArray();
                        var listWin = allClient.Where(p => p.Type == ClientType.Window.ToString());
                        foreach (var client in listWin)
                            this.Send(client.ID, bytes, bytes.Length);
                    }
                    #endregion
                    break;
                case MessageName.CallMessage:
                    #region call
                    {
                        var msg = message as CallMessage;
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = this.clientList.Dictionary.Values.ToArray();
                        if (msg.IsSoundMessage)
                        {
                            var listSound = allClient.Where(p => p.Type == ClientType.SoundPlayer.ToString() && p.Name.Split(',').Contains(msg.AreaNo));
                            foreach (var client in listSound)
                                this.Send(client.ID, bytes, bytes.Length);
                        }
                        if (msg.IsLEDMessage)
                        {
                            var listLED = allClient.Where(p => p.Type == ClientType.LEDDisplay.ToString());
                            foreach (var client in listLED)
                                this.Send(client.ID, bytes, bytes.Length);
                        }
                        var resultMsg = new ResultMessage();
                        resultMsg.Operate = OperateName.Call;
                        resultMsg.Result = "呼叫成功";
                        this.SendMessage(connId, resultMsg);
                    }
                    #endregion
                    break;
                case MessageName.RateMessage:
                    #region rate
                    {
                        var msg = message as RateMessage;
                        //cq 2018-01-08 发出评价请求时更新LED屏显示内容
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = this.clientList.Dictionary.Values.ToArray();
                        var listLED = allClient.Where(p => p.Type == ClientType.LEDDisplay.ToString());
                        foreach (var client in listLED)
                            this.Send(client.ID, bytes, bytes.Length);
                        //向评价服务转发
                        var listRate = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.RateService);
                        foreach (var client in listRate)
                            this.SendMessage(client.ID, message);
                    }
                    #endregion
                    break;
                case MessageName.OperateMessage:
                    #region operate
                    {
                        var msg = message as OperateMessage;
                        //cq 2018-01-08 发出评价请求时更新LED屏显示内容
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = this.clientList.Dictionary.Values.ToArray();
                        var listLED = allClient.Where(p => p.Type == ClientType.LEDDisplay.ToString());
                        foreach (var client in listLED)
                            this.Send(client.ID, bytes, bytes.Length);
                        //向评价服务转发
                        var listRate = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.RateService);
                        foreach (var client in listRate)
                            this.SendMessage(client.ID, message);
                    }
                    #endregion
                    break;
                case MessageName.WeChatMessage:
                    #region wechat
                    {
                        var msg = message as WeChatMessage;
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = this.clientList.Dictionary.Values.ToArray();
                        var listWeChat = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.WeChatService);
                        foreach (var client in listWeChat)
                            this.Send(client.ID, bytes, bytes.Length);
                    }
                    #endregion
                    break;
                case MessageName.LogoutMessage:
                    #region logout
                    {
                        var resultMsg = new ResultMessage();
                        resultMsg.Operate = OperateName.Logout;
                        resultMsg.Result = "注销成功";
                        this.SendMessage(connId, resultMsg);
                    }
                    #endregion
                    break;
            }
        }

        #region IServiceUI 成员

        public Type UIType
        {
            get { return typeof(FrmService); }
        }

        #endregion
    }
}
