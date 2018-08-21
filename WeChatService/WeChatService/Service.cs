using System;
using System.Collections.Generic;
using System.Configuration;
using MessageLib;
using QueueMessage;
using System.IO;

namespace WeChatService
{
    public class Service : TcpServer<ExtraData>
    {
        ServiceClient client;
        WeChatProcess wechatProcess;
        Business busi;
        string strKey;

        public Service()
        {
            strKey = ConfigurationManager.AppSettings["AccessKey"];
            client = new ServiceClient();
            wechatProcess = new WeChatProcess(this);
            busi = new Business();
            client.ReceiveMessage += new Action<IntPtr, Message>(client_ReceiveMessage);
            wechatProcess.ReceiveMessage += new Action<IntPtr, object>(wechatProcess_ReceiveMessage);
            this.OnPrepareListen += new TcpServerEvent.OnPrepareListenEventHandler(Service_OnPrepareListen);
            this.OnAccept += new TcpServerEvent.OnAcceptEventHandler(Service_OnAccept);
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnReceive += new TcpServerEvent.OnReceiveEventHandler(Service_OnReceive);
        }

        HandleResult Service_OnPrepareListen(TcpServer sender, IntPtr soListen)
        {
            dynamic section = ConfigurationManager.GetSection("ServiceConfig");
            var port = section.Configs["排队消息服务"].Port;
            this.client.Connect("127.0.0.1", ushort.Parse(port));
            return HandleResult.Ignore;
        }

        void client_ReceiveMessage(IntPtr connId, Message message)
        {
            try
            {
                switch (message.GetType().Name)
                {
                    case MessageName.WeChatMessage:
                        {
                            var msg = message as WeChatMessage;
                            var obj = busi.PushNotify(msg.ID);
                            foreach (var cId in this.GetAllConnectionIDs())
                                this.SendMessage(cId, obj);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                Log("ClientEx=" + ex.Message);
            }
        }

        HandleResult Service_OnAccept(TcpServer server, IntPtr connId, IntPtr pClient)
        {
            Log("Conn:" + connId);
            var guid = Guid.NewGuid().ToString();
            this.SendMessage(connId,
                new
                {
                    code = 1000,
                    message = "请进行身份验证",
                    key = Encrypt.AESEncrypt(guid, strKey)
                });
            var data = new ExtraData() { GUID = guid };
            this.SetExtra(connId, data);
            return HandleResult.Ok;
        }

        HandleResult Service_OnClose(TcpServer server, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            Log("Close:" + connId);
            this.RemoveExtra(connId);
            return HandleResult.Ok;
        }

        HandleResult Service_OnReceive(IntPtr connId, byte[] bytes)
        {
            Log("Recv:" + BytesInfo(bytes));
            ExtraData msg = null;
            try
            {
                msg = this.GetExtra(connId);
                this.wechatProcess.RecvData(connId, msg, bytes);
            }
            catch (Exception ex)
            {
                this.SDK_OnError(this, connId, ex);
                msg.Data = null;
                msg.Position = 0;
            }
            return HandleResult.Ok;
        }

        public void SendMessage(IntPtr connId, object obj)
        {
            var bytes = this.wechatProcess.FormatterMessageBytes(obj);
            Log("Send:" + BytesInfo(bytes));
            this.Send(connId, bytes, bytes.Length);
        }

        //授权通过后才会触发
        void wechatProcess_ReceiveMessage(IntPtr connId, object message)
        {
            try
            {
                var dic = message as Dictionary<string, object>;
                if (dic != null)
                {
                    if (dic.ContainsKey("method"))
                    {
                        var method = dic["method"].ToString();
                        switch (method)
                        {
                            case "HeartBeat":
                                this.SendMessage(connId, new
                                {
                                    method = "HeartBeat",
                                    param = new
                                    {
                                        Message = "心跳消息接收成功",
                                        DateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    }
                                });
                                break;
                            case "QueueCheck":
                                this.SendMessage(connId, busi.QueueCheck(dic["param"] as Dictionary<string, object>));
                                break;
                            case "GetQueueInfo":
                                this.SendMessage(connId, busi.GetQueueInfo(dic["param"] as Dictionary<string, object>));
                                break;
                            case "ProcessQueue":
                                this.SendMessage(connId, busi.ProcessQueue(dic["param"] as Dictionary<string, object>));
                                break;
                            case "GetWaitInfo":
                                this.SendMessage(connId, busi.GetWaitInfo(dic["param"] as Dictionary<string, object>));
                                break;
                            case "GetWaitInfoByUnit":
                                this.SendMessage(connId, busi.GetWaitInfoByUnit(dic["param"] as Dictionary<string, object>));
                                break;
                            case "GetWaitInfoAll":
                                this.SendMessage(connId, busi.GetWaitInfoAll());
                                break;
                            default:
                                this.SendMessage(connId, StateList.State[StateInfo.Invalid]);
                                break;
                        }
                    }
                    else
                        this.SendMessage(connId, StateList.State[StateInfo.Invalid]);
                }
                else
                {
                    this.SendMessage(connId, StateList.State[StateInfo.Invalid]);
                }
            }
            catch (Exception ex)
            {
                Log("ServiceEx=" + ex.Message);
                this.SendMessage(connId, StateList.State[StateInfo.Error]);
            }
        }

        string BytesInfo(byte[] btArr)
        {
            var str = "";
            for (int i = 0; i < btArr.Length; i++)
                str += btArr[i].ToString("x").ToUpper();
            return str;
        }

        object asyncObj = new object();

        public override void Log(string strLog)
        {
            lock (asyncObj)
            {
                var dir = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                File.AppendAllText(dir + DateTime.Now.ToString("yyyyMMdd") + ".log",
                         string.Format("{0}\r\n{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), strLog));
            }
        }

    }
}
