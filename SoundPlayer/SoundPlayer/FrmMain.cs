using System;
using System.Configuration;
using System.Threading;
using System.Windows.Forms;
using MessageClient;
using MessageLib;
using QueueMessage;
using System.Collections;
using System.Collections.Generic;

namespace SoundPlayer
{
    public partial class FrmMain : Form
    {
        private AutoResetEvent arePlay = new AutoResetEvent(false);
        Queue<string> queuePlay = new Queue<string>();

        private Voice vc;
        string ip = System.Configuration.ConfigurationManager.AppSettings["IP"];
        string port = System.Configuration.ConfigurationManager.AppSettings["Port"];
        string clientName = ConfigurationManager.AppSettings["ClientName"];
        int type = Convert.ToInt32(ConfigurationManager.AppSettings["VoiceType"]);
        string areaNo = "";
        bool isOk = false;
        public FrmMain()
        {
            InitializeComponent();
        }

        Client client = new Client();
        AutoResetEvent areConn = new AutoResetEvent(false);
        public static void SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] == null)
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
            }
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            SetConfigValue("AreaNo", "1");
            areaNo = System.Configuration.ConfigurationManager.AppSettings["AreaNo"];
            this.client.ServerIP = ip;
            this.client.ServerPort = ushort.Parse(port);
            this.client.ClientType = ClientType.SoundPlayer;
            this.client.ClientName = areaNo;
            if (!this.client.Login())
                this.messageIndicator1.SetState(StateType.Error, "未连接");
            this.client.OnResult += (msgType, msgText) =>
            {
                this.messageIndicator1.SetState(StateType.Success, msgText);
            };
            this.client.OnMessage += new Action<QueueMessage.Message>(client_OnMessage);
            this.client.OnDisconnect += () =>
            {
                this.messageIndicator1.SetState(StateType.Error, "未连接");
            };
            //语音消息
            new Thread(() =>
            {
                try
                {
                    vc = new Voice();
                    vc.synth.SpeakCompleted += (s, a) =>
                    {
                        arePlay.Set();
                    };
                    isOk = true;
                    PlaySound("语音服务器初始化成功。");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("语音包初始化失败,原因:" + ex.Message, "提示", MessageBoxButtons.OK);
                    this.txtSoundMesInfo.Invoke(new Action(() =>
                    {
                        this.txtSoundMesInfo.AppendText("语音包初始化失败,原因:" + ex.Message + "\r\n");
                    }));
                }
                while (true)
                {
                    while (queuePlay.Count > 0)
                    {
                        var voiceText = queuePlay.Dequeue();
                        var play = type == 1 ? ConvertTo(voiceText) : voiceText;
                        vc.PlayText(play);
                        arePlay.WaitOne(-1);
                        LogService.Debug("播报完成:" + play);
                    }
                    Thread.Sleep(1000);
                }
            }) { IsBackground = true }.Start();
            this.Close();
            this.notifyIcon1.ShowBalloonTip(3000);
        }

        void client_OnMessage(QueueMessage.Message obj)
        {
            this.txtSoundMesInfo.Invoke(new Action(() =>
            {
                if (isOk)
                    this.txtSoundMesInfo.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n" + obj.ToString() + "。\r\n");
                else
                    this.txtSoundMesInfo.AppendText("因语音包初始化失败，该条语音播放未成功：" + obj.ToString() + "。\r\n");
                LogService.Info(obj.ToString());
            }));
            if (isOk)
                PlaySound(obj.ToString());
        }

        object asyncObj = new object();

        private void PlaySound(string voiceText)
        {
            lock (asyncObj)
            {
                queuePlay.Enqueue(voiceText);
            }
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.ShowInTaskbar = false;
            this.notifyIcon1.Visible = true;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.client.Logout();
            Application.ExitThread();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            //托盘区图标隐藏
            notifyIcon1.Visible = false;
            this.Show();
        }

        public string ConvertTo(string playText)
        {
            return Microsoft.VisualBasic.Strings.StrConv(playText, Microsoft.VisualBasic.VbStrConv.TraditionalChinese, 0);
        }
    }
}
