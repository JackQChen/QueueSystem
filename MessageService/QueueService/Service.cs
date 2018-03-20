using System;
using System.Collections.Generic;
using System.Configuration;
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
        internal bool clientListChanged = false, needConn = false;
        WebSocketSharp.WebSocket rateClient;
        JavaScriptSerializer convert = new JavaScriptSerializer();

        public Service()
        {
            process = new Process();
            process.ReceiveMessage += new Action<IntPtr, Message>(process_ReceiveMessage);
            dynamic section = ConfigurationManager.GetSection("ServiceConfig");
            var port = section.Configs["评价系统服务"].Port;
            rateClient = new WebSocketSharp.WebSocket("ws://127.0.0.1:" + port);
            rateClient.OnOpen += new EventHandler(rateClient_OnOpen);
            rateClient.OnClose += new EventHandler<WebSocketSharp.CloseEventArgs>(rateClient_OnClose);
            this.OnPrepareListen += new TcpServerEvent.OnPrepareListenEventHandler(Service_OnPrepareListen);
            this.OnShutdown += new TcpServerEvent.OnShutdownEventHandler(Service_OnShutdown);
            this.OnAccept += new TcpServerEvent.OnAcceptEventHandler(Service_OnAccept);
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnReceive += new TcpServerEvent.OnReceiveEventHandler(Service_OnReceive);
        }

        void rateClient_OnOpen(object sender, EventArgs e)
        {
            rateClient.SendAsync(convert.Serialize(new
            {
                method = "Login",
                param = new
                {
                    winNum = "D840F2A3-C421-4B3A-B385-12B25727F70F",
                    userCode = "QueueService"
                }
            }), null);
        }

        void rateClient_OnClose(object sender, WebSocketSharp.CloseEventArgs e)
        {
            if (needConn)
            {
                rateClient.ConnectAsync();
                needConn = false;
            }
        }

        HandleResult Service_OnPrepareListen(TcpServer sender, IntPtr soListen)
        {
            if (rateClient.ReadyState == WebSocketSharp.WebSocketState.Closing)
                needConn = true;
            else
                rateClient.ConnectAsync();
            return HandleResult.Ok;
        }

        HandleResult Service_OnShutdown(TcpServer sender)
        {
            rateClient.CloseAsync();
            return HandleResult.Ok;
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
            if (message.Type != MessageType.Login && !this.clientList.Dictionary.ContainsKey(connId))
            {
                var resultMsg = new ResultMessage();
                resultMsg.Operate = MessageType.Result;
                resultMsg.Result = "当前用户尚未登录";
                this.SendMessage(connId, resultMsg);
                return;
            }
            switch (message.Type)
            {
                case MessageType.Login:
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
                        resultMsg.Operate = MessageType.Login;
                        resultMsg.Result = islogin ? "您已登录过系统" : "登录成功";
                        this.SendMessage(connId, resultMsg);
                    }
                    #endregion
                    break;
                case MessageType.Call:
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
                        resultMsg.Operate = MessageType.Call;
                        resultMsg.Result = "呼叫成功";
                        this.SendMessage(connId, resultMsg);
                    }
                    #endregion
                    break;
                case MessageType.Rate:
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
                        if (!rateClient.IsConnected)
                            rateClient.Connect();
                        rateClient.SendAsync(convert.Serialize(new
                        {
                            method = "RateRequest",
                            param = new
                            {
                                winNum = msg.WindowNo,
                                rateId = msg.RateId,
                                transactor = msg.Transactor,
                                date = msg.WorkDate,
                                item = msg.ItemName,
                                reserveSeq = msg.reserveSeq
                            }
                        }), null);
                    }
                    #endregion
                    break;
                case MessageType.Operate:
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
                        if (!rateClient.IsConnected)
                            rateClient.Connect();
                        rateClient.SendAsync(convert.Serialize(new
                        {
                            method = "RateOperate",
                            param = new
                            {
                                winNum = msg.WindowNo,
                                operate = msg.Operate.ToString()
                            }
                        }), null);
                    }
                    #endregion
                    break;
                case MessageType.Logout:
                    #region logout
                    {
                        var resultMsg = new ResultMessage();
                        resultMsg.Operate = MessageType.Logout;
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
