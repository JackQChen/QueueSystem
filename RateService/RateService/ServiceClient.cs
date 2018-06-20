using System;
using MessageLib;
using QueueMessage;

namespace RateService
{
    public class ServiceClient : TcpClient
    {
        QueueMessage.ExtraData extraData = new QueueMessage.ExtraData();
        QueueMessage.Process process = new QueueMessage.Process();
        public event Action<IntPtr, Message> ReceiveMessage;

        public ServiceClient()
        {
            this.OnConnect += new TcpClientEvent.OnConnectEventHandler(ServiceClient_OnConnect);
            this.OnReceive += new TcpClientEvent.OnReceiveEventHandler(ServiceClient_OnReceive);
            this.process.ReceiveMessage += new Action<IntPtr, Message>(process_ReceiveMessage);
        }

        HandleResult ServiceClient_OnConnect(TcpClient sender)
        {
            this.SendMessage(new LoginMessage()
            {
                ClientType = ClientType.Service,
                ClientName = ServiceName.RateService
            });
            return HandleResult.Ignore;
        }

        HandleResult ServiceClient_OnReceive(TcpClient sender, byte[] bytes)
        {
            this.process.RecvData(this.ConnectionId, extraData, bytes);
            return HandleResult.Ignore;
        }

        void process_ReceiveMessage(IntPtr connId, Message message)
        {
            if (this.ReceiveMessage != null)
                this.ReceiveMessage(connId, message);
        }

        public void SendMessage(Message message)
        {
            var bytes = this.process.FormatterMessageBytes(message);
            this.Send(bytes, bytes.Length);
        }
    }
}
