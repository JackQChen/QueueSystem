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

namespace ReadIdCard
{
    public partial class frmMain : Form
    {
        ShareMemory2 share = new ShareMemory2();
        public frmMain()
        {
            InitializeComponent();
        }
        int iRetUSB = 0;
        private void frmMain_Load(object sender, EventArgs e)
        {
            share.Init();
            this.notifyIcon1.ShowBalloonTip(3000);
            int iPort;
            for (iPort = 1001; iPort <= 1016; iPort++)
            {
                iRetUSB = CVRSDK.CVR_InitComm(iPort);
                if (iRetUSB == 1)
                    break;
            }
            if (iRetUSB != 1)
            {
                this.Invoke(new Action(() =>
                {
                    ShowMsg("身份证读卡器初始化失败，请重试！", true);
                }));
            }
            else
            {
                this.Invoke(new Action(() =>
                {
                    ShowMsg("初始化读卡成功，开始读卡...", true);
                }));
                new Thread(new ThreadStart(ReadIDCard)) { IsBackground = true }.Start();
            }
            this.Hide();
            new Thread(() =>
            {
                int index = 0;
                while (true)
                {
                    if (index > 10)
                    {
                        BeginInvoke(new Action(() => { this.Hide(); }));
                        break;
                    }
                    Thread.Sleep(200);
                    index++;
                }
            }) { IsBackground = true }.Start();

            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(60 * 2 * 1000);//每2分钟清空一次
                    BeginInvoke(new Action(() => { txtSoundMesInfo.Text = ""; }));
                }
            }) { IsBackground = true }.Start();
        }

        void ShowMsg(string msg, bool isWriter)
        {
            txtSoundMesInfo.AppendText(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss:fff") + ": " + msg + "\r\n");
            if (isWriter)
                WriterReadIdCardLog(msg);
        }
        private void ReadIDCard()
        {
            int time = 0;
            while (true)
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
                                //写入共享内存
                                var p = new Person() { idcard = iCard, name = iName, address = iAdress };
                                var bytes = FormatterMessageBytes(p);
                                share.Write(bytes, 0, bytes.Length);
                                BeginInvoke(new Action(() =>
                                {
                                    ShowMsg(string.Format("身份证读卡成功：本次读卡循环读取了{3}次，证件号码{0} 姓名{1} 地址{2}\r\n", iCard, iName, iAdress, t), false);
                                }));
                            }) { IsBackground = true }.Start();
                            time = 0;
                        }
                        else
                        {
                            BeginInvoke(new Action(() =>
                            {
                                ShowMsg("未读取到身份证号码", false);
                            }));
                        }

                        #endregion
                    }
                    else
                    {
                        BeginInvoke(new Action(() =>
                        {
                            ShowMsg("读卡操作失败！请重新放上卡片。", false);
                        }));
                    }
                }
                else
                {
                    BeginInvoke(new Action(() =>
                    {
                        ShowMsg("未放卡或卡片放置不正确，请重新放上卡片。", false);
                    }));
                }
                Thread.Sleep(50);
            }
        }
        public byte[] FormatterMessageBytes(Person message)
        {
            return FormatterObjectBytes(message);
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
        private void btnExit_Click(object sender, EventArgs e)
        {
            try { CVRSDK.CVR_CloseComm(); }
            catch { }
            Application.ExitThread();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            //托盘区图标隐藏
            notifyIcon1.Visible = false;
            this.Show();
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }
        public void WriterReadIdCardLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\ReadIdCardLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + ": " + logString);
            }
        }
    }
}
