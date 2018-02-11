using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using BLL;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;
using System.Text;

namespace DataUpload
{
    public partial class FrmMain : Form
    {
        int BasicInterval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BasicInterval"]);
        int BusyInterval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BusyInterval"]);
        public FrmMain()
        {
            InitializeComponent();
        }
        Thread threadBasic;
        Thread threadBusy;
        bool isBasic = true;
        bool isBusy = true;
        List<IUploadData> basicListBll;
        List<IUploadData> busyListBll;
        Dictionary<int, string> dicClient;
        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.notifyIcon1.ShowBalloonTip(3000);
            var tps = typeof(TUserBLL).Assembly.GetTypes().Where(p => { return p.GetInterface("IUploadData") != null; });
            basicListBll = new List<IUploadData>();
            busyListBll = new List<IUploadData>();
            foreach (var tp in tps)
            {
                var instance = Activator.CreateInstance(tp) as IUploadData;
                if (instance.IsBasic)
                    basicListBll.Add(instance);
                else
                    busyListBll.Add(instance);
            }
            dicClient = new Dictionary<int, string>();
            foreach (ConnectionStringSettings x in ConfigurationManager.ConnectionStrings)
            {
                if (x.ElementInformation.LineNumber == 0)
                    continue;
                if (x.Name != "Server")
                {
                    dicClient.Add(Convert.ToInt32(x.Name), x.ConnectionString);
                }
            }
            threadBasic = new Thread(new ThreadStart(StartUploadBasic)) { IsBackground = true };
            threadBasic.Start();
            threadBusy = new Thread(new ThreadStart(StartUploadBusy)) { IsBackground = true };
            threadBusy.Start();
        }

        void StartUploadBasic()
        {
            while (isBasic)
            {
                DateTime start = DateTime.Now;
                StringBuilder logText = new StringBuilder();
                foreach (var key in dicClient)
                {
                    foreach (var instance in basicListBll)
                    {
                        var type = instance.GetType().Name;
                        type = "T_" + type.Substring(1, type.Length - 4);
                        var insert = instance.ProcessInsertData(key.Key, "Server");
                        var update = instance.ProcessUpdateData(key.Key, "Server");
                        string area = "                      区域编码【" + key.Key + "】表【" + type + "】同步：";
                        area += (insert < 0 ? "新增出错；" : insert == 0 ? "本次无新增；" : "新增【" + insert.ToString() + "】条；");
                        area += (update < 0 ? "更新出错；" : update == 0 ? "本次无更新；" : "更新【" + update.ToString() + "】条；");
                        logText.Append(area + "\r\n");
                    }
                }
                TimeSpan ts = DateTime.Now - start;
                WriterLog("基础数据同步\r\n" + logText.ToString());
                WriterLog("本次基础数据同步完成：共同步【" + dicClient.Count + "】个客户端，用时【" + ts.Minutes + "】分【" + ts.Seconds + "】秒。\r\n*****************************************************************************");
                Thread.Sleep(BasicInterval * 1000);
            }
        }

        void StartUploadBusy()
        {
            while (isBusy)
            {
                StringBuilder logText = new StringBuilder();
                DateTime start = DateTime.Now;
                foreach (var key in dicClient)
                {
                    foreach (var instance in busyListBll)
                    {
                        var type = instance.GetType().Name;
                        type = "T_" + type.Substring(1, type.Length - 4);
                        var insert = instance.ProcessInsertData(key.Key, "Server");
                        var update = instance.ProcessUpdateData(key.Key, "Server");
                        string area = "                      区域编码【" + key.Key + "】表【" + type + "】同步：";
                        area += (insert < 0 ? "新增出错；" : insert == 0 ? "本次无新增；" : "新增【" + insert.ToString() + "】条；");
                        area += (update < 0 ? "更新出错；" : update == 0 ? "本次无更新；" : "更新【" + update.ToString() + "】条；");
                        logText.Append(area + "\r\n");
                    }
                }
                TimeSpan ts = DateTime.Now - start;
                WriterLog("业务数据同步明细\r\n" + logText.ToString());
                WriterLog("本次业务数据同步完成：共同步【" + dicClient.Count + "】个客户端，用时【" + ts.Minutes + "】分【" + ts.Seconds + "】秒。\r\n*****************************************************************************");
                Thread.Sleep(BusyInterval * 1000);

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
            isBusy = false;
            isBasic = false;
            if (threadBasic != null)
                threadBasic.Abort();
            if (threadBusy != null)
                threadBusy.Abort();
            Application.ExitThread();
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            //托盘区图标隐藏
            notifyIcon1.Visible = false;
            this.Show();
        }

        private void WriterLog(string text)
        {
            this.Invoke(new Action(() => { this.txtSoundMesInfo.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + text + "\r\n"); }));
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Upload_Exception.txt", DateTime.Now + " : " + text + "\r\n");
        }
    }
}
