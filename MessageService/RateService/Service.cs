using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using MessageLib;

namespace RateService
{
    public class Service : WebSocketServer, IServiceUI
    {
        JavaScriptSerializer convert = new JavaScriptSerializer();
        Process process = new Process();
        List<string> loginOperation = new List<string>();
        byte[] btDisConn = new byte[] { 0x3, 0xe9 };
        internal Extra<DeviceInfo> deviceList = new Extra<DeviceInfo>();
        internal bool deviceListChanged = false;
        OperateIni ini;

        public Service()
        {
            ini = new OperateIni(AppDomain.CurrentDomain.BaseDirectory + "RateUpdate.ini");
            this.OnClose += new TcpServerEvent.OnCloseEventHandler(Service_OnClose);
            this.OnWSMessageBody += new WebSocketEvent.OnWSMessageBodyEventHandler(Service_OnWSMessageBody);
            loginOperation.AddRange("raterequest,rateoperate,ratesubmit".Split(','));
        }

        HandleResult Service_OnClose(TcpServer sender, IntPtr connId, SocketOperation enOperation, int errorCode)
        {
            this.deviceList.Remove(connId);
            deviceListChanged = true;
            return HandleResult.Ok;
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
                var device = this.deviceList.Get(connId);
                if (loginOperation.Contains(method) && string.IsNullOrEmpty(device == null ? "" : device.UserCode))
                {
                    var rData = new ResponseData { code = "1000", request = requestData.method, result = "当前用户尚未登录" };
                    this.SendWSMessage(connId, rData.ToResultData());
                    return HandleResult.Ignore;
                }
                switch (method)
                {
                    case "raterequest":
                        if (device.UserCode == "QueueService")
                        {
                            var param = requestData.param as Dictionary<string, object>;
                            var targetList = this.deviceList.Dictionary.Values.Where(p => p.WindowNumber == param["winNum"].ToString()).ToList();
                            foreach (var target in targetList)
                            {
                                var rateData = new RequestData
                                {
                                    method = "RateQuest",
                                    param = new
                                    {
                                        rateId = param["rateId"].ToString(),
                                        transactor = param["transactor"].ToString(),
                                        item = param["item"].ToString(),
                                        date = param["date"].ToString(),
                                        reserveSeq = param["reserveSeq"].ToString()
                                    }
                                };
                                this.SendWSMessage(target.ID, rateData.ToResultData());
                            }
                            var rData = new ResponseData { code = "0", request = requestData.method, result = targetList.Count == 0 ? "无此用户信息" : "已发送评价请求" };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        else
                        {
                            var rData = new ResponseData { code = "1001", request = requestData.method, result = "该用户无此操作权限" };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    case "rateoperate":
                        if (device.UserCode == "QueueService")
                        {
                            var param = requestData.param as Dictionary<string, object>;
                            var targetList = this.deviceList.Dictionary.Values.Where(p => p.WindowNumber == param["winNum"].ToString()).ToList();
                            foreach (var target in targetList)
                            {
                                var rateData = new RequestData
                                {
                                    method = "RateOperate",
                                    param = new
                                    {
                                        operate = param["operate"].ToString()
                                    }
                                };
                                this.SendWSMessage(target.ID, rateData.ToResultData());
                            }
                            var rData = new ResponseData { code = "0", request = requestData.method, result = targetList.Count == 0 ? "无此用户信息" : "已发送操作请求" };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        else
                        {
                            var rData = new ResponseData { code = "1001", request = requestData.method, result = "该用户无此操作权限" };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    case "getwindowlist":
                        {
                            var rData = new ResponseData { code = "0", request = requestData.method, result = process.GetWindowList() };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    case "getupdateinfo":
                        {
                            var rData = new ResponseData
                            {
                                code = "0",
                                request = requestData.method,
                                result = new
                                {
                                    version = ini.ReadString("Config", "Version"),
                                    url = ini.ReadString("Config", "Url")
                                }
                            };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    case "getuserphoto":
                        {
                            var param = requestData.param as Dictionary<string, object>;
                            var rData = new ResponseData { code = "0", request = requestData.method, result = this.process.GetUserPhoto(param["userCode"].ToString()) };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    case "login":
                        {
                            ResponseData rData = null;
                            if (device == null)
                            {
                                var param = requestData.param as Dictionary<string, object>;
                                string winNum = param["winNum"].ToString(), userCode = param["userCode"].ToString();
                                if (process.Login(winNum, userCode))
                                {
                                    string ip = "";
                                    ushort port = 0;
                                    this.GetRemoteAddress(connId, ref ip, ref port);
                                    this.deviceList.Set(connId, new DeviceInfo()
                                    {
                                        ID = connId,
                                        IP = ip + ":" + port,
                                        WindowNumber = winNum,
                                        UserCode = userCode,
                                        ConnTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                                    });
                                    rData = new ResponseData { code = "0", request = requestData.method, result = "登录成功!" };
                                    deviceListChanged = true;
                                }
                                else
                                    rData = new ResponseData { code = "1002", request = requestData.method, result = "窗口或用户信息不正确!" };
                            }
                            else
                                rData = new ResponseData { code = "0", request = requestData.method, result = "当前用户已登录!" };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    case "ratesubmit":
                        {
                            var param = requestData.param as Dictionary<string, object>;
                            ResponseData rData = null;
                            if (process.RateSubmit(device.UserCode, device.WindowNumber, param["rateId"].ToString(), param["attitude"].ToString(), param["quality"].ToString(), param["efficiency"].ToString(), param["honest"].ToString()))
                                rData = new ResponseData { code = "0", request = requestData.method, result = "评价成功!" };
                            else
                                rData = new ResponseData { code = "1003", request = requestData.method, result = "评价失败!" };
                            this.SendWSMessage(connId, rData.ToResultData());
                        }
                        break;
                    default:
                        {
                            var rData = new ResponseData
                            {
                                code = "999",
                                request = requestData.method,
                                result = "未知指令"
                            };
                            this.SendWSMessage(connId, rData.ToResultData());
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
