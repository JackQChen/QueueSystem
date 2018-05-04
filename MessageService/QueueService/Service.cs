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
        internal Extra<ClientInfo> clientList = new Extra<ClientInfo>();
        internal bool clientListChanged = false;
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
            this.clientListChanged = true;
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
                            clientListChanged = true;
                        }
                        var resultMsg = new ResultMessage();
                        resultMsg.Operate = OperateName.Login;
                        resultMsg.Result = islogin ? "您已登录过系统" : "登录成功";
                        this.SendMessage(connId, resultMsg);
                    }
                    #endregion
                    break;
                case MessageName.CallMessage:
                    #region call
                    {
                        var msg = message as CallMessage;
                        if (this.ConnectionCount > 0)
                        {
                            var bytes = this.process.FormatterMessageBytes(message);
                            var allClient = new ClientInfo[this.clientList.Dictionary.Count];
                            this.clientList.Dictionary.Values.CopyTo(allClient, 0);
                            if (msg.IsSoundMessage)
                            {
                                var list = allClient.Where(p => p.Type == ClientType.SoundPlayer.ToString() && p.Name.Split(',').Contains(msg.AreaNo)).ToList();
                                foreach (var client in list)
                                    this.Send(client.ID, bytes, bytes.Length);
                            }
                            if (msg.IsLEDMessage)
                            {
                                var list = allClient.Where(p => p.Type == ClientType.LEDDisplay.ToString()).ToList();
                                foreach (var client in list)
                                    this.Send(client.ID, bytes, bytes.Length);
                            }
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
                        var allClient = new ClientInfo[this.clientList.Dictionary.Count];
                        this.clientList.Dictionary.Values.CopyTo(allClient, 0);
                        var list = allClient.Where(p => p.Type == ClientType.LEDDisplay.ToString()).ToList();
                        foreach (var client in list)
                            this.Send(client.ID, bytes, bytes.Length);
                        //LED屏发送完成 
                        var listRate = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.RateService).ToList();
                        foreach (var service in listRate)
                            this.SendMessage(service.ID, message);
                    }
                    #endregion
                    break;
                case MessageName.OperateMessage:
                    #region operate
                    {
                        var msg = message as OperateMessage;
                        //cq 2018-01-08 发出评价请求时更新LED屏显示内容
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = new ClientInfo[this.clientList.Dictionary.Count];
                        this.clientList.Dictionary.Values.CopyTo(allClient, 0);
                        var list = allClient.Where(p => p.Type == ClientType.LEDDisplay.ToString()).ToList();
                        foreach (var client in list)
                            this.Send(client.ID, bytes, bytes.Length);
                        //LED屏发送完成
                        var listRate = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.RateService).ToList();
                        foreach (var service in listRate)
                            this.SendMessage(service.ID, message);
                    }
                    #endregion
                    break;
                case MessageName.WeChatMessage:
                    #region wechat
                    {
                        var msg = message as WeChatMessage;
                        var bytes = this.process.FormatterMessageBytes(message);
                        var allClient = new ClientInfo[this.clientList.Dictionary.Count];
                        this.clientList.Dictionary.Values.CopyTo(allClient, 0);
                        var list = allClient.Where(p => p.Type == ClientType.Service.ToString() && p.Name == ServiceName.WeChatService).ToList();
                        foreach (var client in list)
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
