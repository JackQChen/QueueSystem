using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MessageClient;
using QueueMessage;

namespace MessageClientTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        Client client = new Client();
        Action<string> actLog;

        private void Form1_Load(object sender, EventArgs e)
        {
            actLog = new Action<string>(log =>
            {
                this.textBox1.AppendText(log + "\r\n");
            });
            client.ServerIP = "127.0.0.1";
            client.ServerPort = ushort.Parse("3347");
            client.ClientType = ClientType.Window;
            client.ClientName = "张三";
            client.OnResult += new Action<string, string>(client_OnResult);
            client.OnMessage += new Action<QueueMessage.Message>(client_OnMessage);
        }

        void client_OnMessage(QueueMessage.Message obj)
        {
            var callMsg = obj as CallMessage;
            if (callMsg != null)
                this.Log("Message:" + callMsg.TicketNo);
        }

        void client_OnResult(string operate, string result)
        {
            this.Log(operate + ":" + result);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Log("登录中...");
            client.ClientType = ClientType.Window;
            client.ClientName = "张三";
            client.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Log("注销中...");
            this.client.Stop();
        }

        void Log(string text)
        {
            this.textBox1.Invoke(actLog, new object[] { text + "\r\n" });
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.Log("登录中...");
            client.ClientType = ClientType.SoundPlayer;
            client.ClientName = "张三";
            client.Start();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.client.SendMessage(new CallMessage() { TicketNo = "2", WindowNo = "1", IsSoundMessage = true });
        }

        private void button5_Click(object sender, EventArgs e)
        {
            this.client.SendMessage(new CallMessage() { TicketNo = "2", WindowNo = "1", IsSoundMessage = true });
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.client.SendMessage(new CallMessage() { TicketNo = "3", WindowNo = "2", IsLEDMessage = true });
        }

        private void button7_Click(object sender, EventArgs e)
        {
            this.client.SendMessage(new CallMessage() { TicketNo = "3", WindowNo = "2", IsLEDMessage = true });
        }


        private void button8_Click(object sender, EventArgs e)
        {
            this.client.SendMessage(new RateMessage() { RateId = "123", WindowNo = "2" });
            this.client.SendMessage(new OperateMessage() { WindowNo = "1", Operate = Operate.Resume });
        }
    }
}
