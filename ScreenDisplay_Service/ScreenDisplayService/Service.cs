using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;
using MessageLib;
using System.Threading.Tasks;
using System.Threading;

namespace ScreenDisplayService
{
    public class Service : WebSocketServer, IServiceUI
    {
        JavaScriptSerializer convert = new JavaScriptSerializer();
        byte[] btDisConn = new byte[] { 0x3, 0xe9 };
        internal Extra<IntPtr, DeviceInfo> deviceList = new Extra<IntPtr, DeviceInfo>();

        byte[] btQueueInfo;

        public Service()
        {
            this.OnAccept += new TcpServerEvent.OnAcceptEventHandler(Service_OnAccept);
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnWSMessageBody += new WebSocketEvent.OnWSMessageBodyEventHandler(Service_OnWSMessageBody);
            Task.Factory.StartNew(() =>
            {
                var data = new ResponseData { code = "0", result = "" };
                while (true)
                {
                    data.result = Guid.NewGuid().ToString();
                    btQueueInfo = data.ToBytes();
                    Thread.Sleep(1000);
                }
            });
        }

        HandleResult Service_OnAccept(TcpServer sender, IntPtr connId, IntPtr pClient)
        {
            string ip = "";
            ushort port = 0;
            this.GetRemoteAddress(connId, ref ip, ref port);
            deviceList.Set(connId, new DeviceInfo()
            {
                ID = connId,
                IP = ip,
                DeviceName = "",
                ConnTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });
            return HandleResult.Ignore;
        }

        HandleResult Service_OnClose(TcpServer sender, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            if (!this.deviceList.Dictionary.ContainsKey(connId))
                return HandleResult.Ignore;
            this.deviceList.Remove(connId);
            return HandleResult.Ignore;
        }

        HandleResult Service_OnWSMessageBody(IntPtr connId, byte[] data)
        {
            try
            {
                if (BitConverter.ToString(data) == BitConverter.ToString(btDisConn))
                    return HandleResult.Ignore;
                var strData = Encoding.UTF8.GetString(data);
                RequestData requestData = null;
                try
                {
                    requestData = convert.Deserialize<RequestData>(strData);
                }
                catch
                {
                    return HandleResult.Ignore;
                }
                var method = requestData.method.Trim().ToLower();
                switch (method)
                {
                    case "getqueuelist":
                        {
                            //var param = requestData.param as Dictionary<string, object>; 
                            this.SendWSMessage(connId, btQueueInfo);
                        }
                        break;
                    default:
                        {
                            var rData = new ResponseData
                            {
                                code = "999",
                                result = "未知指令"
                            };
                            this.SendWSMessage(connId, rData.ToBytes());
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                this.SDK_OnError(this, connId, ex);
            }
            return HandleResult.Ok;
        }

        #region IServiceUI 成员

        public Type UIType
        {
            get { return typeof(FrmService); }
        }

        #endregion
    }
}
