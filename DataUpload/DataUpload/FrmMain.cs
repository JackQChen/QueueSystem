using System;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using BLL;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Configuration;

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
            this.Close();
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
        }

        void StartUploadBasic()
        {
            while (isBasic)
            {
                foreach (var key in dicClient)
                {
                    foreach (var instance in basicListBll)
                    {
                        if (!instance.ProcessInsertData(key.Key, "Server"))
                        {
                            WriterLog("区域【" + key.Key + "】新增同步出错!");
                        }
                        if (!instance.ProcessUpdateData(key.Key, "Server"))
                        {
                            WriterLog("区域【" + key.Key + "】更新同步出错!");
                        }
                        if (!instance.ProcessDeleteData(key.Key, "Server"))
                        {
                            WriterLog("区域【" + key.Key + "】删除同步出错!");
                        }
                    }
                }
                Thread.Sleep(BasicInterval * 1000);
            }
        }

        void StartUploadBusy()
        {

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
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Upload_Exception.txt", DateTime.Now + " : " + text + "\r\n");
        }
    }
}
