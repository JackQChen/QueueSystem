using System;
using System.Threading;
using MessageLib;
using QueueMessage;

namespace MessageClient
{
    public class Client : TcpClient
    {
        Process process = new Process();
        ExtraData data = new ExtraData();

        public string ServerIP { get; set; }
        public ushort ServerPort { get; set; }
        public ClientType ClientType { get; set; }
        public string ClientName { get; set; }

        public event Action<string, string> OnResult;
        public event Action<Message> OnMessage;
        public event Action OnDisconnect;
        private AutoResetEvent areConn = new AutoResetEvent(false);

        public Client()
        {
            this.OnReceive += new TcpClientEvent.OnReceiveEventHandler(Client_OnReceive);
            this.OnClose += new TcpClientEvent.OnCloseEventHandler(Client_OnClose);
            this.process.ReceiveMessage += new Action<IntPtr, Message>(process_ReceiveMessage);
            //自动重连处理
            new Thread(() =>
            {
                while (true)
                {
                    areConn.WaitOne();
                    while (true)
                    {
                        if (this.Login())
                            break;
                        else
                        {
                            if (this.OnDisconnect != null)
                                this.OnDisconnect();
                        }
                        Thread.Sleep(10000);
                    }
                }
            }) { IsBackground = true }.Start();
        }

        HandleResult Client_OnClose(TcpClient sender, SocketOperation enOperation, int errorCode)
        {
            areConn.Set();
            return HandleResult.Ignore;
        }

        HandleResult Client_OnReceive(TcpClient sender, byte[] bytes)
        {
            process.RecvData(this.ConnectionId, data, bytes);
            return HandleResult.Ok;
        }

        void process_ReceiveMessage(IntPtr connId, Message message)
        {
            switch (message.GetType().Name)
            {
                case MessageName.RestartMessage:
                    {
                        //以无参方式重启
                        System.Windows.Forms.Application.Exit();
                        System.Diagnostics.Process.Start(System.Windows.Forms.Application.ExecutablePath);
                    }
                    break;
                case MessageName.ResultMessage:
                    {
                        var msg = message as ResultMessage;
                        if (this.OnResult != null)
                            this.OnResult(msg.Operate, msg.Result);
                        if (msg.Operate == OperateName.Logout)
                            this.Stop();
                    }
                    break;
                default:
                    if (this.OnMessage != null)
                        this.OnMessage(message);
                    break;
            }
        }

        public void SendMessage(Message message)
        {
            if (!this.IsStarted)
                this.Connect(this.ServerIP, this.ServerPort, false);
            var bytes = this.process.FormatterMessageBytes(message);
            this.Send(bytes, bytes.Length);
        }

        public bool Login()
        {
            if (!this.IsStarted)
            {
                if (!this.Connect(this.ServerIP, this.ServerPort, false))
                    return false;
            }
            var bytes = this.process.FormatterMessageBytes(new LoginMessage()
            {
                ClientType = this.ClientType,
                ClientName = this.ClientName
            });
            this.Send(bytes, bytes.Length);
            return true;
        }

        public void Logout()
        {
            var message = new LogoutMessage();
            this.SendMessage(message);
        }

    }
}
