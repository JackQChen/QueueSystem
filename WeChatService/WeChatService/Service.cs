using System;
using MessageLib;
using System.Text;
using System.Configuration;
using System.Collections.Generic;

namespace WeChatService
{
    public class Service : TcpServer<ExtraData>
    {
        Process process;
        string strKey;

        public Service()
        {
            strKey = ConfigurationManager.AppSettings["AccessKey"];
            process = new Process(this);
            process.ReceiveMessage += new Action<IntPtr, object>(process_ReceiveMessage);
            this.OnAccept += new TcpServerEvent.OnAcceptEventHandler(Service_OnAccept);
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnReceive += new TcpServerEvent.OnReceiveEventHandler(Service_OnReceive);
        }

        HandleResult Service_OnAccept(TcpServer server, IntPtr connId, IntPtr pClient)
        {
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
            this.RemoveExtra(connId);
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

        public void SendMessage(IntPtr connId, object obj)
        {
            var bytes = this.process.FormatterMessageBytes(obj);
            this.Send(connId, bytes, bytes.Length);
        }

        void process_ReceiveMessage(IntPtr connId, object message)
        {
            var dic = message as Dictionary<string, object>;
            if (dic != null)
            {
                if (dic.ContainsKey("request"))
                {
                    var request = dic["request"].ToString();
                    switch (request)
                    {
                        case "GetBusiInfo":
                            this.SendMessage(connId, GetBusiInfo());
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

        object GetBusiInfo()
        {
            return new { name = "test1", code = 111 };
        }

    }
}
