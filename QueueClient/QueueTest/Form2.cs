using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace QueueTest
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }
        int iRetUSB = 0;
        AutoResetEvent are = new AutoResetEvent(false);
        private void button1_Click(object sender, EventArgs e)
        {
            jg = (int)(Convert.ToDecimal(textBox1.Text.Trim()) * 1000);
            int iPort;
            for (iPort = 1001; iPort <= 1016; iPort++)
            {
                iRetUSB = CVRSDK.CVR_InitComm(iPort);
                if (iRetUSB == 1)
                    break;
            }
            if (iRetUSB != 1)
            {
                this.Invoke(new Action(() => { this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + "身份证读卡器初始化失败，请重试！\r\n"); }));
            }
            else
            {
                isread = true;
                this.Invoke(new Action(() =>
                {
                    this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + "初始化成功，循环读卡开始(0.2秒执行一次)...\r\n");
                }));
                new Thread(new ThreadStart(ReadIDCard)) { IsBackground = true }.Start();
            }
        }
        bool isread = true;
        int jg = 100;
        private void ReadIDCard()
        {
            int time = 0;
            while (isread)
            {
                time++;
                int isHaveCard = CVRSDK.CVR_Authenticate();
                if (isHaveCard == 1)
                {
                    int readOk = CVRSDK.CVR_Read_Content(4);
                    if (readOk == 1)
                    {
                        #region
                        byte[] name = new byte[30];
                        int length = 30;
                        CVRSDK.GetPeopleName(ref name[0], ref length);
                        byte[] number = new byte[30];
                        length = 36;
                        CVRSDK.GetPeopleIDCode(ref number[0], ref length);
                        byte[] address = new byte[30];
                        length = 70;
                        CVRSDK.GetPeopleAddress(ref address[0], ref length);
                        var iCard = System.Text.Encoding.GetEncoding("GB2312").GetString(number).Replace("\0", "").Trim();
                        var iName = System.Text.Encoding.GetEncoding("GB2312").GetString(name).Replace("\0", "").Trim();
                        var iAdress = System.Text.Encoding.GetEncoding("GB2312").GetString(address).Replace("\0", "").Trim();
                        if (iCard != "")
                        {
                            int t = time;
                            new Thread(() =>
                            {
                                BeginInvoke(new Action(() =>
                                {
                                    this.BeginInvoke(new Action(() => { this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + string.Format("身份证读卡成功：本次读卡循环读取了{3}次，证件号码{0} 姓名{1} 地址{2}\r\n", iCard, iName, iAdress, t)); }));
                                }));
                            }) { IsBackground = true }.Start();
                            time = 0;
                        }
                        else
                        {
                            BeginInvoke(new Action(() =>
                                     {
                                         this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + " 未读取到身份证号码\r\n");
                                     }));
                        }

                        #endregion
                    }
                    else
                    {
                        BeginInvoke(new Action(() =>
                                  {
                                      this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + " 读卡操作失败！请重新放上卡片。\r\n");
                                  }));
                    }
                }
                else
                {
                    BeginInvoke(new Action(() =>
                                {
                                    this.richTextBox1.AppendText(DateTime.Now.ToString("HH:mm:ss:fff") + " 未放卡或卡片放置不正确，请重新放上卡片。\r\n");
                                }));
                }
                Thread.Sleep(jg);
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            isread = false;
            try { CVRSDK.CVR_CloseComm(); }
            catch { }
        }
        ShareMemory2 share = new ShareMemory2();
        private void button3_Click(object sender, EventArgs e)
        {
            share.Init();
            var p1 = new Person() { idcard = "6227231987026545645160719", name = "测123123123试", address = "123123123地址", sex = "男" };
            var bytes1 = FormatterMessageBytes(p1);

            var p2 = new Person() { idcard = "622723198702160743434343419", name = "测试", address = "地址" };
            var bytes2 = FormatterMessageBytes(p2);


            var p3 = new Person() { idcard = "622723198702160719", name = "测试", address = "地址" };
            var bytes3 = FormatterMessageBytes(p3);


            var p4 = new Person() { idcard = "622723198702160719234234234234", name = "234234234234234234测试", address = "地址" };
            var bytes4 = FormatterMessageBytes(p4);


            var p5 = new Person() { idcard = "6227231987021607太热太热19", name = "测试", address = "地址" };
            var bytes5 = FormatterMessageBytes(p5);

            var p6 = new Person() { idcard = "622723198702160719", name = "测的试", address = "地址" };
            var bytes6 = FormatterMessageBytes(p6);

            share.Write(bytes6, 0, bytes6.Length);
        }

        byte[] nbyte = new byte[1024 * 10];
        private void button4_Click(object sender, EventArgs e)
        {
            var bytes = new byte[1024 * 10];
            share.Read(ref bytes, 0, bytes.Length);
            if (BytesCompare(bytes, nbyte))
                return;
            var p = FormatterBytes(bytes);
            if (p != null)
            {
                MessageBox.Show(p.idcard);
            }
        }
        byte[] FormatterObjectBytes(object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj is null");
            byte[] buff;
            using (var ms = new MemoryStream())
            {
                IFormatter iFormatter = new BinaryFormatter();
                iFormatter.Serialize(ms, obj);
                buff = ms.GetBuffer();
            }
            return buff;
        }

        private bool BytesCompare(byte[] b1, byte[] b2)
        {
            if (b1 == null || b2 == null) return false;
            if (b1.Length != b2.Length) return false;
            return string.Compare(Convert.ToBase64String(b1), Convert.ToBase64String(b2), false) == 0 ? true : false;
        }

        Person FormatterBytes(byte[] bytes)
        {
            byte[] buff;
            using (var ms = new MemoryStream(bytes))
            {
                IFormatter iFormatter = new BinaryFormatter();
                var obj = iFormatter.Deserialize(ms) as Person;
                return obj;
            }
        }
        public byte[] FormatterMessageBytes(Person message)
        {
            return FormatterObjectBytes(message);
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }




    }
}
