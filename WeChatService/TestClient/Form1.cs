using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using MessageLib;
using WeChatService;

namespace TestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        TcpClient client = new TcpClient();
        Process process = new Process();
        ExtraData recvInfo = new ExtraData();
        JavaScriptSerializer convert = new JavaScriptSerializer();

        private void Form1_Load(object sender, EventArgs e)
        {
            client.Connect("127.0.0.1", ushort.Parse("3349"), async: false);
            this.client.OnReceive += new TcpClientEvent.OnReceiveEventHandler(client_OnReceive);
            process.ReceiveMessage += new Action<object>(process_ReceiveMessage);
        }

        void SendMessage(object obj)
        {
            var data = process.FormatterMessageBytes(obj);
            this.client.Send(data, data.Length);
        }

        HandleResult client_OnReceive(TcpClient sender, byte[] bytes)
        {
            this.process.RecvData(recvInfo, bytes);
            return HandleResult.Ignore;
        }

        bool access = false;

        void process_ReceiveMessage(object obj)
        {
            var dic = obj as Dictionary<string, object>;
            if (dic.ContainsKey("key"))
            {
                var guid = Encrypt.AESDecrypt(dic["key"].ToString(), ConfigurationManager.AppSettings["AccessKey"]);
                this.SendMessage(new { key = guid });
            }
            else
            {
                if (dic["code"].ToString() == "1001")
                    access = true;
            }
            this.textBox1.Invoke(new Action(() =>
            {
                this.textBox1.AppendText(convert.Serialize(obj) + "\r\n");
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var data = process.FormatterMessageBytes(
                new
                {
                    method = "GetQueueInfo",
                    param = new
                    {
                        id = "1"
                    }
                });
            this.client.Send(data, data.Length);
        }

    }
}
