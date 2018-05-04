using System;
using System.Collections.Generic;
using System.Configuration;
using MessageLib;
using QueueMessage;

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
            this.client.Connect("127.0.0.1", ushort.Parse(ConfigurationManager.AppSettings["QueueServicePort"]));
            return HandleResult.Ignore;
        }

        void client_ReceiveMessage(IntPtr connId, Message message)
        {
            switch (message.GetType().Name)
            {
                case MessageName.WeChatMessage:
                    {
                        var msg = message as WeChatMessage;
                        //msg.ID
                    }
                    break;
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
            var dic = message as Dictionary<string, object>;
            if (dic != null)
            {
                if (dic.ContainsKey("method"))
                {
                    var method = dic["method"].ToString();
                    switch (method)
                    {
                        case "GetQueueInfo":
                            this.SendMessage(connId, busi.GetQueueInfo(dic["param"] as Dictionary<string, object>));
                            break;
                        case "ProcessQueue":
                            this.SendMessage(connId, busi.ProcessQueue(dic["param"] as Dictionary<string, object>));
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

        string BytesInfo(byte[] btArr)
        {
            var str = "";
            for (int i = 0; i < btArr.Length; i++)
                str += btArr[i].ToString("x").ToUpper();
            return str;
        }

        object asyncObj = new object();

        void Log(string strLog)
        {
            //lock (asyncObj)
            //{
            //    var dir = AppDomain.CurrentDomain.BaseDirectory + "Log\\";
            //    if (!Directory.Exists(dir))
            //        Directory.CreateDirectory(dir);
            //    File.AppendAllText(dir + DateTime.Now.ToString("yyyyMMdd") + ".log",
            //             string.Format("{0}\r\n{1}\r\n", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), strLog));
            //}
        }

    }
}
