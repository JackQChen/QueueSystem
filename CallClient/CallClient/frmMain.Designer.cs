namespace CallClient
{
    partial class frmMain
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnSet = new System.Windows.Forms.ToolStripMenuItem();
            this.btnConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.btnExit = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCall = new CustomSkin.Windows.Forms.Button();
            this.btnReCall = new CustomSkin.Windows.Forms.Button();
            this.btnCance = new CustomSkin.Windows.Forms.Button();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.messageIndicator1 = new MessageClient.MessageIndicator();
            this.btnEv = new CustomSkin.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnGiveUpAll = new CustomSkin.Windows.Forms.Button();
            this.btnRefresh = new CustomSkin.Windows.Forms.Button();
            this.btnBackCall = new CustomSkin.Windows.Forms.Button();
            this.btnHang = new CustomSkin.Windows.Forms.Button();
            this.btnMove = new CustomSkin.Windows.Forms.Button();
            this.btnPause = new CustomSkin.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel3 = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTicket = new System.Windows.Forms.TextBox();
            this.txtHangCount = new System.Windows.Forms.TextBox();
            this.txtWait = new System.Windows.Forms.TextBox();
            this.txtQueueCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWindow = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.SuspendLayout();
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "叫号端";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnSet,
            this.btnConfig,
            this.btnExit});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 70);
            // 
            // btnSet
            // 
            this.btnSet.Name = "btnSet";
            this.btnSet.Size = new System.Drawing.Size(124, 22);
            this.btnSet.Text = "高级选项";
            this.btnSet.Visible = false;
            this.btnSet.Click += new System.EventHandler(this.btnSet_Click);
            // 
            // btnConfig
            // 
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(124, 22);
            this.btnConfig.Text = "系统配置";
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // btnExit
            // 
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(124, 22);
            this.btnExit.Text = "退出系统";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnCall
            // 
            this.btnCall.BackColor = System.Drawing.Color.Transparent;
            this.btnCall.FocusBorder = true;
            this.btnCall.Font = new System.Drawing.Font("宋体", 12F);
            this.btnCall.Location = new System.Drawing.Point(24, 13);
            this.btnCall.Margin = new System.Windows.Forms.Padding(4);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(84, 58);
            this.btnCall.TabIndex = 1;
            this.btnCall.Text = "呼叫(F12)";
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // btnReCall
            // 
            this.btnReCall.BackColor = System.Drawing.Color.Transparent;
            this.btnReCall.FocusBorder = true;
            this.btnReCall.Font = new System.Drawing.Font("宋体", 12F);
            this.btnReCall.Location = new System.Drawing.Point(121, 14);
            this.btnReCall.Margin = new System.Windows.Forms.Padding(4);
            this.btnReCall.Name = "btnReCall";
            this.btnReCall.Size = new System.Drawing.Size(84, 58);
            this.btnReCall.TabIndex = 2;
            this.btnReCall.Text = "重呼";
            this.btnReCall.UseVisualStyleBackColor = true;
            this.btnReCall.Click += new System.EventHandler(this.btnReCall_Click);
            // 
            // btnCance
            // 
            this.btnCance.BackColor = System.Drawing.Color.Transparent;
            this.btnCance.FocusBorder = true;
            this.btnCance.Font = new System.Drawing.Font("宋体", 12F);
            this.btnCance.Location = new System.Drawing.Point(315, 13);
            this.btnCance.Margin = new System.Windows.Forms.Padding(4);
            this.btnCance.Name = "btnCance";
            this.btnCance.Size = new System.Drawing.Size(84, 58);
            this.btnCance.TabIndex = 4;
            this.btnCance.Text = "弃号";
            this.btnCance.UseVisualStyleBackColor = true;
            this.btnCance.Click += new System.EventHandler(this.btnCance_Click);
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(0, 0);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(987, 590);
            this.listView1.TabIndex = 5;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 800;
            // 
            // messageIndicator1
            // 
            this.messageIndicator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.messageIndicator1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.messageIndicator1.Location = new System.Drawing.Point(4, 669);
            this.messageIndicator1.Margin = new System.Windows.Forms.Padding(4);
            this.messageIndicator1.Name = "messageIndicator1";
            this.messageIndicator1.Size = new System.Drawing.Size(1003, 40);
            this.messageIndicator1.TabIndex = 7;
            // 
            // btnEv
            // 
            this.btnEv.BackColor = System.Drawing.Color.Transparent;
            this.btnEv.FocusBorder = true;
            this.btnEv.Font = new System.Drawing.Font("宋体", 12F);
            this.btnEv.Location = new System.Drawing.Point(218, 14);
            this.btnEv.Margin = new System.Windows.Forms.Padding(4);
            this.btnEv.Name = "btnEv";
            this.btnEv.Size = new System.Drawing.Size(84, 58);
            this.btnEv.TabIndex = 10;
            this.btnEv.Text = "评价";
            this.btnEv.UseVisualStyleBackColor = true;
            this.btnEv.Click += new System.EventHandler(this.btnEv_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnGiveUpAll);
            this.panel1.Controls.Add(this.btnRefresh);
            this.panel1.Controls.Add(this.btnBackCall);
            this.panel1.Controls.Add(this.btnHang);
            this.panel1.Controls.Add(this.btnMove);
            this.panel1.Controls.Add(this.btnPause);
            this.panel1.Controls.Add(this.btnCall);
            this.panel1.Controls.Add(this.btnReCall);
            this.panel1.Controls.Add(this.btnEv);
            this.panel1.Controls.Add(this.btnCance);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(4, 512);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(987, 82);
            this.panel1.TabIndex = 11;
            // 
            // btnGiveUpAll
            // 
            this.btnGiveUpAll.BackColor = System.Drawing.Color.Transparent;
            this.btnGiveUpAll.FocusBorder = true;
            this.btnGiveUpAll.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.btnGiveUpAll.Location = new System.Drawing.Point(795, 13);
            this.btnGiveUpAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnGiveUpAll.Name = "btnGiveUpAll";
            this.btnGiveUpAll.Size = new System.Drawing.Size(84, 58);
            this.btnGiveUpAll.TabIndex = 15;
            this.btnGiveUpAll.Text = "当前窗\r\n口弃号";
            this.btnGiveUpAll.UseVisualStyleBackColor = true;
            this.btnGiveUpAll.Click += new System.EventHandler(this.btnGiveUpAll_Click_1);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.FocusBorder = true;
            this.btnRefresh.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(887, 12);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(84, 58);
            this.btnRefresh.TabIndex = 15;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnBackCall
            // 
            this.btnBackCall.BackColor = System.Drawing.Color.Transparent;
            this.btnBackCall.FocusBorder = true;
            this.btnBackCall.Font = new System.Drawing.Font("宋体", 12F);
            this.btnBackCall.Location = new System.Drawing.Point(703, 14);
            this.btnBackCall.Margin = new System.Windows.Forms.Padding(4);
            this.btnBackCall.Name = "btnBackCall";
            this.btnBackCall.Size = new System.Drawing.Size(84, 58);
            this.btnBackCall.TabIndex = 14;
            this.btnBackCall.Text = "回呼";
            this.btnBackCall.UseVisualStyleBackColor = true;
            this.btnBackCall.Click += new System.EventHandler(this.btnBackCall_Click);
            // 
            // btnHang
            // 
            this.btnHang.BackColor = System.Drawing.Color.Transparent;
            this.btnHang.FocusBorder = true;
            this.btnHang.Font = new System.Drawing.Font("宋体", 12F);
            this.btnHang.Location = new System.Drawing.Point(606, 14);
            this.btnHang.Margin = new System.Windows.Forms.Padding(4);
            this.btnHang.Name = "btnHang";
            this.btnHang.Size = new System.Drawing.Size(84, 58);
            this.btnHang.TabIndex = 13;
            this.btnHang.Text = "挂起";
            this.btnHang.UseVisualStyleBackColor = true;
            this.btnHang.Click += new System.EventHandler(this.btnHang_Click);
            // 
            // btnMove
            // 
            this.btnMove.BackColor = System.Drawing.Color.Transparent;
            this.btnMove.FocusBorder = true;
            this.btnMove.Font = new System.Drawing.Font("宋体", 12F);
            this.btnMove.Location = new System.Drawing.Point(509, 14);
            this.btnMove.Margin = new System.Windows.Forms.Padding(4);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(84, 58);
            this.btnMove.TabIndex = 12;
            this.btnMove.Text = "转移";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Transparent;
            this.btnPause.FocusBorder = true;
            this.btnPause.Font = new System.Drawing.Font("宋体", 12F);
            this.btnPause.Location = new System.Drawing.Point(412, 13);
            this.btnPause.Margin = new System.Windows.Forms.Padding(4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(84, 58);
            this.btnPause.TabIndex = 11;
            this.btnPause.Text = "暂停";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.listView1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(4, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(987, 590);
            this.panel2.TabIndex = 12;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 2500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Font = new System.Drawing.Font("宋体", 12F);
            this.tabControl1.Location = new System.Drawing.Point(4, 41);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1003, 628);
            this.tabControl1.TabIndex = 13;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(183)))), ((int)(((byte)(214)))));
            this.tabPage1.Controls.Add(this.panel4);
            this.tabPage1.Controls.Add(this.panel3);
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage1.Size = new System.Drawing.Size(995, 598);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "功能操作";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.listView2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(380, 4);
            this.panel4.Margin = new System.Windows.Forms.Padding(4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(611, 508);
            this.panel4.TabIndex = 13;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5});
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.Font = new System.Drawing.Font("宋体", 12F);
            this.listView2.FullRowSelect = true;
            this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listView2.Location = new System.Drawing.Point(0, 0);
            this.listView2.Margin = new System.Windows.Forms.Padding(4);
            this.listView2.MultiSelect = false;
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(611, 508);
            this.listView2.TabIndex = 1;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "票号";
            this.columnHeader3.Width = 135;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "排队时间";
            this.columnHeader4.Width = 190;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "姓名";
            this.columnHeader5.Width = 100;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.groupBox1);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel3.Location = new System.Drawing.Point(4, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(376, 508);
            this.panel3.TabIndex = 12;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtTicket);
            this.groupBox1.Controls.Add(this.txtHangCount);
            this.groupBox1.Controls.Add(this.txtWait);
            this.groupBox1.Controls.Add(this.txtQueueCount);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtWindow);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(376, 508);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "当前信息";
            // 
            // txtTicket
            // 
            this.txtTicket.Font = new System.Drawing.Font("宋体", 15F);
            this.txtTicket.Location = new System.Drawing.Point(149, 252);
            this.txtTicket.Margin = new System.Windows.Forms.Padding(4);
            this.txtTicket.Name = "txtTicket";
            this.txtTicket.ReadOnly = true;
            this.txtTicket.Size = new System.Drawing.Size(203, 30);
            this.txtTicket.TabIndex = 9;
            // 
            // txtHangCount
            // 
            this.txtHangCount.Font = new System.Drawing.Font("宋体", 15F);
            this.txtHangCount.Location = new System.Drawing.Point(149, 200);
            this.txtHangCount.Margin = new System.Windows.Forms.Padding(4);
            this.txtHangCount.Name = "txtHangCount";
            this.txtHangCount.ReadOnly = true;
            this.txtHangCount.Size = new System.Drawing.Size(203, 30);
            this.txtHangCount.TabIndex = 8;
            // 
            // txtWait
            // 
            this.txtWait.Font = new System.Drawing.Font("宋体", 15F);
            this.txtWait.Location = new System.Drawing.Point(149, 148);
            this.txtWait.Margin = new System.Windows.Forms.Padding(4);
            this.txtWait.Name = "txtWait";
            this.txtWait.ReadOnly = true;
            this.txtWait.Size = new System.Drawing.Size(203, 30);
            this.txtWait.TabIndex = 7;
            // 
            // txtQueueCount
            // 
            this.txtQueueCount.Font = new System.Drawing.Font("宋体", 15F);
            this.txtQueueCount.Location = new System.Drawing.Point(149, 96);
            this.txtQueueCount.Margin = new System.Windows.Forms.Padding(4);
            this.txtQueueCount.Name = "txtQueueCount";
            this.txtQueueCount.ReadOnly = true;
            this.txtQueueCount.Size = new System.Drawing.Size(203, 30);
            this.txtQueueCount.TabIndex = 6;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 15F);
            this.label5.Location = new System.Drawing.Point(12, 256);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(109, 20);
            this.label5.TabIndex = 5;
            this.label5.Text = "当前票号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 15F);
            this.label4.Location = new System.Drawing.Point(12, 204);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 20);
            this.label4.TabIndex = 4;
            this.label4.Text = "挂起人数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 15F);
            this.label3.Location = new System.Drawing.Point(12, 152);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(109, 20);
            this.label3.TabIndex = 3;
            this.label3.Text = "等候人数：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 15F);
            this.label2.Location = new System.Drawing.Point(12, 100);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 20);
            this.label2.TabIndex = 2;
            this.label2.Text = "排队人数：";
            // 
            // txtWindow
            // 
            this.txtWindow.Font = new System.Drawing.Font("宋体", 15F);
            this.txtWindow.Location = new System.Drawing.Point(149, 44);
            this.txtWindow.Margin = new System.Windows.Forms.Padding(4);
            this.txtWindow.Name = "txtWindow";
            this.txtWindow.ReadOnly = true;
            this.txtWindow.Size = new System.Drawing.Size(203, 30);
            this.txtWindow.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 15F);
            this.label1.Location = new System.Drawing.Point(12, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(109, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "当前窗口：";
            // 
            // tabPage2
            // 
            this.tabPage2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(183)))), ((int)(((byte)(214)))));
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4);
            this.tabPage2.Size = new System.Drawing.Size(995, 598);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "操作日志";
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(183)))), ((int)(((byte)(214)))));
            this.ClientSize = new System.Drawing.Size(1011, 713);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.messageIndicator1);
            this.Font = new System.Drawing.Font("宋体", 12F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Padding = new System.Windows.Forms.Padding(4, 41, 4, 4);
            this.SkinButtonVisible = true;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "叫号系统";
            this.TitleStyle = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btnExit;
        private System.Windows.Forms.ToolStripMenuItem btnConfig;
        private  CustomSkin.Windows.Forms.Button  btnCall;
        private CustomSkin.Windows.Forms.Button btnReCall;
        private CustomSkin.Windows.Forms.Button btnCance;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private MessageClient.MessageIndicator messageIndicator1;
        private CustomSkin.Windows.Forms.Button btnEv;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem btnSet;
        private System.Windows.Forms.Timer timer1;
        private CustomSkin.Windows.Forms.Button btnPause;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private CustomSkin.Windows.Forms.Button btnBackCall;
        private CustomSkin.Windows.Forms.Button btnHang;
        private CustomSkin.Windows.Forms.Button btnMove;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtWindow;
        private System.Windows.Forms.Label label1;
        private CustomSkin.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtTicket;
        private System.Windows.Forms.TextBox txtHangCount;
        private System.Windows.Forms.TextBox txtWait;
        private System.Windows.Forms.TextBox txtQueueCount;
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private CustomSkin.Windows.Forms.Button btnGiveUpAll;
    }
}

