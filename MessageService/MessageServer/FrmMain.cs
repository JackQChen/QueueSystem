using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using MessageLib;
using Register;

namespace MessageServer
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        Extra<string, TcpServer> serverList = new Extra<string, TcpServer>();
        Extra<string, Extra<string, string>> clientList = new Extra<string, Extra<string, string>>();
        int lastSelectedIndex = -1;
        Action<string, string> actServiceState, actLog;

        private void FrmMain_Shown(object sender, EventArgs e)
        {
            RegInfo regInfo = null;
            if (Register.Validate.Check(true, out regInfo))
            {
                if (regInfo.RegBase.OrganizationCode != ConfigurationManager.AppSettings["AreaNo"])
                {
                    MessageBox.Show("非授权的机构编码，请核对！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.FormClosing -= this.FrmMain_FormClosing;
                    Application.Exit();
                }
                else
                {
                    var strReg = string.Format("授权给：{0} 机构编码：{1} 到期时间：{2}",
                        regInfo.RegBase.OrganizationName,
                         regInfo.RegBase.OrganizationCode,
                         Convert.ToDateTime(regInfo.RegBase.ExpiryDate).ToString("yyyy-MM-dd"));
                    this.labRegInfo.Text = strReg;
                }
            }
            else
            {
                this.FormClosing -= this.FrmMain_FormClosing;
                Application.Exit();
            }
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000 * 60 * 60 * 24);
                    if (!Register.Validate.Check(false))
                    {
                        this.FormClosing -= this.FrmMain_FormClosing;
                        Application.Exit();
                    }
                }
            }) { IsBackground = true }.Start();
            actServiceState = (name, state) =>
            {
                this.lvService.Items[name].SubItems[4].Text = state;
            };
            actLog = (name, log) =>
            {
                this.txtLog.AppendText(string.Format("{0} - {1}\r\n{2}\r\n",
                            DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                            name, log));
            };
            //handleCreate 
            this.tabServer.SelectedTab = this.tabLog;
            this.tabServer.SelectedTab = this.tabPerformance;
            this.tabServer.SelectedTab = this.tabMain;
            this.lvClient.ListViewItemSorter = new ListViewItemComparer<int>(0);
            this.InitService();
            this.tsStart.PerformClick();
            this.Close();
        }

        private void tsStart_Click(object sender, System.EventArgs e)
        {
            this.labState.Text = "正在启动...";
            this.statusStrip1.Update();
            StartService();
            if (this.lvService.Items.Count > 0)
                this.lvService.Items[0].Selected = true;
            this.labState.Text = "服务正在运行...";
            this.statusStrip1.Update();
            this.tsStart.Enabled = false;
            this.tsStop.Enabled = true;
        }

        private void tsStop_Click(object sender, System.EventArgs e)
        {
            if (MessageBox.Show("确定停止服务吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                StopService();
                this.labState.Text = "服务已停止";
                this.statusStrip1.Update();
                this.tsStart.Enabled = true;
                this.tsStop.Enabled = false;
            }
        }

        private void InitService()
        {
            var config = (ServiceConfig)ConfigurationManager.GetSection("ServiceConfig");
            foreach (Config cfg in config.Configs)
            {
                TcpServer service = null;
                try
                {
                    service = Activator.CreateInstance(Type.GetType(cfg.Type)) as TcpServer;
                    service.Name = cfg.Name;
                    service.Port = ushort.Parse(cfg.Port);
                    this.serverList.Set(cfg.Name, service);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message
                    + "\r\n" + (ex.InnerException == null ? "" : ex.InnerException.Message), "错误 - "
                    + cfg.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
                ListViewItem serviceItem = this.lvService.Items.Add(cfg.Name, "", -1);
                serviceItem.SubItems.Add("TCP");
                serviceItem.SubItems.Add(cfg.Port);
                serviceItem.SubItems.Add(cfg.Name);
                serviceItem.SubItems.Add("准备就绪");
                serviceItem.Tag = new ServiceInfo(service);
                this.clientList.Set(cfg.Name, new Extra<string, string>());
                service.OnLog += (srv, log) =>
                {
                    if (this.txtLog.IsHandleCreated)
                        this.txtLog.Invoke(actLog, srv.Name, log);
                    return HandleResult.Ok;
                };
                service.OnError += (srv, connId, ex) =>
                {
                    srv.Log(string.Format("{0}:{1}",
                            this.clientList.Get(srv.Name).Get(connId.ToString()),
                            ex.Message));
                    return HandleResult.Ok;
                };
                service.OnPrepareListen += (srv, so) =>
                {
                    if (this.lvService.IsHandleCreated)
                        this.lvService.Invoke(actServiceState, srv.Name, "正在运行");
                    return HandleResult.Ok;
                };
                service.OnAccept += (srv, connId, pClient) =>
                {
                    string ip = "";
                    ushort port = 0;
                    srv.GetRemoteAddress(connId, ref ip, ref port);
                    var cList = this.clientList.Get(srv.Name);
                    cList.Set(connId.ToString(), ip + ":" + port);
                    return HandleResult.Ok;
                };
                service.OnClose += (srv, connId, enOperation, errorCode) =>
                {
                    var cList = this.clientList.Get(srv.Name);
                    cList.Remove(connId.ToString());
                    return HandleResult.Ok;
                };
                service.OnShutdown += srv =>
                {
                    if (this.lvService.IsHandleCreated)
                        this.lvService.Invoke(actServiceState, srv.Name, "已停止");
                    return HandleResult.Ok;
                };
            }
        }

        private void StartService()
        {
            foreach (var key in this.serverList.Dictionary.Keys)
            {
                var server = this.serverList.Get(key);
                if (!server.Start())
                {
                    MessageBox.Show(string.Format("服务启动失败:{0}({1})", server.ErrorMessage, server.ErrorCode), "错误 - " + key, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    continue;
                }
            }
        }

        private void StopService()
        {
            foreach (var service in this.serverList.Dictionary.Values)
                service.Stop();
        }

        private void tsAddProtocol_Click(object sender, EventArgs e)
        {

        }

        private void tsRemoveProtocol_Click(object sender, EventArgs e)
        {

        }

        private void tsSetting_Click(object sender, System.EventArgs e)
        {
            using (var frm = new FrmSetting())
            {
                frm.ShowDialog(this);
            }
        }

        void Exit()
        {
            if (MessageBox.Show("确定停止服务并退出吗？", "提示",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.FormClosing -= this.FrmMain_FormClosing;
                this.StopService();
                Application.Exit();
            }
        }

        private void tsExit_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void lvService_DoubleClick(object sender, EventArgs e)
        {
            this.btnServicePanel.PerformClick();
        }

        private void lvService_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvService.SelectedItems.Count != 1)
                return;
            var curIndex = this.lvService.SelectedItems[0].Index;
            if (lastSelectedIndex != curIndex)
            {
                this.lvClient.Items.Clear();
                var srvName = this.lvService.Items[curIndex].Name;
                var cList = this.clientList.Get(srvName);
                cList.Changed = true;
                this.lvService.Items[curIndex].SubItems[0].Text = "●";
                if (this.lastSelectedIndex != -1)
                    this.lvService.Items[lastSelectedIndex].SubItems[0].Text = "";
                lastSelectedIndex = curIndex;
                UpdateClientList();
                this.btnServicePanel.Enabled = this.serverList.Get(srvName) as IServiceUI != null;
            }
        }

        void UpdateClientList()
        {
            var cList = this.clientList.Get(this.lvService.Items[this.lastSelectedIndex].Name);
            if (!cList.Changed)
                return;
            cList.Changed = false;
            var keys = this.lvClient.Items.Cast<ListViewItem>().Select(s => s.Name);
            var ids = cList.Dictionary.Keys;
            foreach (var key in keys.Except(ids))
                this.lvClient.Items.RemoveByKey(key);
            this.lvClient.Items.AddRange(ids.Except(keys).Select(
                key => new ListViewItem(new string[] { key, cList.Get(key), "连接" })
                {
                    Name = key
                }).ToArray());
        }

        private void btnServicePanel_Click(object sender, EventArgs e)
        {
            if (this.lastSelectedIndex == -1)
                return;
            var srvName = this.lvService.Items[lastSelectedIndex].Name;
            var service = this.serverList.Get(srvName);
            var ui = service as IServiceUI;
            if (ui == null)
                return;
            using (var frm = Activator.CreateInstance(ui.UIType, new object[] { service }) as Form)
            {
                if (frm != null)
                    frm.ShowDialog(this);
            }
        }

        private void btnDisConn_Click(object sender, EventArgs e)
        {
            if (this.lastSelectedIndex == -1)
                return;
            var srvName = this.lvService.Items[lastSelectedIndex].Name;
            foreach (ListViewItem item in this.lvClient.SelectedItems)
                this.serverList.Get(srvName).Disconnect(new IntPtr(Convert.ToInt32(item.Name)));
        }

        internal class ListViewItemComparer<T> : IComparer
        {
            private int col;

            public ListViewItemComparer(int col)
            {
                this.col = col;
            }

            public int Compare(object x, object y)
            {
                int returnVal = -1;
                string str1 = ((ListViewItem)x).SubItems[col].Text, str2 = ((ListViewItem)y).SubItems[col].Text;
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Int32: returnVal = Convert.ToInt32(str1).CompareTo(Convert.ToInt32(str2)); break;
                    case TypeCode.String:
                    default: returnVal = string.Compare(str1, str2); break;
                }
                return returnVal;
            }
        }

        private void timerServerState_Tick(object sender, EventArgs e)
        {
            if (this.lastSelectedIndex == -1)
                return;
            UpdateClientList();
            var si = this.lvService.Items[lastSelectedIndex].Tag as ServiceInfo;
            si.接收速率 = FormatFileSize(si.totalRecv - si.lastRecv) + "/s";
            si.发送速率 = FormatFileSize(si.totalSend - si.lastSend) + "/s";
            si.lastRecv = si.totalRecv;
            si.lastSend = si.totalSend;
            this.pgService.SelectedObject = si;
        }

        internal class ServiceInfo
        {
            private TcpServer server;
            internal long lastRecv, lastSend;
            internal long totalRecv, totalSend;

            public ServiceInfo(TcpServer srv)
            {
                this.server = srv;
            }

            [Category("服务状态")]
            public string 当前连接数 { get { return this.server.ConnectionCount.ToString(); } set { } }
            [Category("服务状态")]
            public string 累计发送
            {
                get
                {
                    this.totalSend = this.server.TotalSendCount;
                    return FormatFileSize(this.server.TotalSendCount);
                }
                set { }
            }
            [Category("服务状态")]
            public string 累计接收
            {
                get
                {
                    this.totalRecv = this.server.TotalRecvCount;
                    return FormatFileSize(this.server.TotalRecvCount);
                }
                set { }
            }
            [Category("服务状态")]
            public string 发送速率 { get; set; }
            [Category("服务状态")]
            public string 接收速率 { get; set; }

            [Category("缓存池状态")]
            public string 最大连接数 { get { return this.server.MaxConnectionCount.ToString(); } set { } }
            [Category("缓存池状态")]
            public string 最大并发量 { get { return this.server.AcceptSocketCount.ToString(); } set { } }
            [Category("缓存池状态")]
            public string 工作线程数 { get { return this.server.WorkerThreadCount.ToString(); } set { } }

        }

        /// <summary>
        /// 转换文件大小格式
        /// </summary>
        /// <param name="fileSize">文件大小</param>
        /// <returns></returns>
        internal static string FormatFileSize(long fileSize)
        {
            if (fileSize < 0)
                return "ErrorSize";
            else if (fileSize >= 1024 * 1024 * 1024)
                return string.Format("{0:########0.00} GB", ((Double)fileSize) / (1024 * 1024 * 1024));
            else if (fileSize >= 1024 * 1024)
                return string.Format("{0:####0.00} MB", ((Double)fileSize) / (1024 * 1024));
            else if (fileSize >= 1024)
                return string.Format("{0:####0.00} KB", ((Double)fileSize) / 1024);
            else
                return string.Format("{0} Bytes", fileSize);
        }

        private void txtLog_DoubleClick(object sender, EventArgs e)
        {
            this.txtLog.Clear();
        }

        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
            this.notifyIcon.ShowBalloonTip(100);
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Exit();
        }

        private void 显示主界面ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

    }
}
