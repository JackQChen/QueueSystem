using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using MessageLib;
using BLL;
using System.Linq;
using System.Collections.Generic;
using Model;

namespace ScreenDisplayService
{
    public class Service : WebSocketServer, IServiceUI
    {
        JavaScriptSerializer convert = new JavaScriptSerializer();
        byte[] btDisConn = new byte[] { 0x3, 0xe9 };
        internal Extra<IntPtr, DeviceInfo> deviceList = new Extra<IntPtr, DeviceInfo>();
        BCallBLL callBLL = new BCallBLL();
        TWindowBLL wBll = new TWindowBLL();
        TScreenConfigBLL sBll = new TScreenConfigBLL();
        List<BCallModel> aList = new List<BCallModel>();
        List<TWindowModel> wList = new List<TWindowModel>();
        List<TScreenConfigModel> screeList = new List<TScreenConfigModel>();
        public Service()
        {
            this.OnAccept += new TcpServerEvent.OnAcceptEventHandler(Service_OnAccept);
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnWSMessageBody += new WebSocketEvent.OnWSMessageBodyEventHandler(Service_OnWSMessageBody);
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    wList = wBll.GetModelList();
                    screeList = sBll.GetModelList();
                    Thread.Sleep(2 * 60 * 1000);
                }
            });
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    aList = callBLL.ScreenAllList();
                    Thread.Sleep(1500);
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
                    case "getconfig":
                        {
                            var rData = new ResponseData { code = "0", result = "" };
                            var ip = this.deviceList.Get(connId).IP;
                            var config = screeList.Where(s => s.IP == ip).FirstOrDefault().Config;
                            var param = convert.DeserializeObject(config);
                            rData.result = param;
                            var btQueueInfo = rData.ToBytes();
                            this.SendWSMessage(connId, btQueueInfo);
                        }
                        break;
                    case "getqueuelist":
                        {
                            var rData = new ResponseData { code = "0", result = "" };
                            var ip = this.deviceList.Get(connId).IP;
                            var config = screeList.Where(s => s.IP == ip).FirstOrDefault().Config;
                            var param = convert.DeserializeObject(config) as Dictionary<string, object>;
                            var winAreaNo = param["winArea"].ToString();
                            var arr = GetCallByArea(winAreaNo);
                            rData.result = arr;
                            var btQueueInfo = rData.ToBytes();
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

        Array GetCallByArea(string areas)
        {
            string[] areaList = areas.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var winList = wList.Where(q => areaList.Contains(q.AreaName.ToString())).Select(s => s.Number).ToList();
            var callList = aList.Where(q => winList.Contains(q.windowNumber)).ToList();
            var arr = callList.Select(s => new { ticketNo = s.ticketNumber, winNo = s.windowNumber }).ToArray();
            return arr;
        }

        #region IServiceUI 成员

        public Type UIType
        {
            get { return typeof(FrmService); }
        }

        #endregion
    }
}
