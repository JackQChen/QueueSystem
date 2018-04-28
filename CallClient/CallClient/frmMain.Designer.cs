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
            this.messageIndicator1 = new MessageClient.MessageIndicator();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.panel5 = new CustomSkin.Windows.Forms.Panel();
            this.panel3 = new CustomSkin.Windows.Forms.Panel();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel4 = new CustomSkin.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.lblWindow = new System.Windows.Forms.Label();
            this.txtAlready = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnRefresh = new CustomSkin.Windows.Forms.Button();
            this.btnGiveUpAll = new CustomSkin.Windows.Forms.Button();
            this.txtTicket = new CustomSkin.Windows.Forms.TextBox();
            this.txtHangCount = new CustomSkin.Windows.Forms.TextBox();
            this.txtWait = new CustomSkin.Windows.Forms.TextBox();
            this.txtQueueCount = new CustomSkin.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWindow = new CustomSkin.Windows.Forms.TextBox();
            this.panel1 = new CustomSkin.Windows.Forms.Panel();
            this.btnBackCall = new CustomSkin.Windows.Forms.Button();
            this.btnHang = new CustomSkin.Windows.Forms.Button();
            this.btnMove = new CustomSkin.Windows.Forms.Button();
            this.btnPause = new CustomSkin.Windows.Forms.Button();
            this.btnCall = new CustomSkin.Windows.Forms.Button();
            this.btnReCall = new CustomSkin.Windows.Forms.Button();
            this.btnEv = new CustomSkin.Windows.Forms.Button();
            this.btnCance = new CustomSkin.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.panel5.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel1.SuspendLayout();
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
            // messageIndicator1
            // 
            this.messageIndicator1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.messageIndicator1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.messageIndicator1.Location = new System.Drawing.Point(4, 466);
            this.messageIndicator1.Margin = new System.Windows.Forms.Padding(4);
            this.messageIndicator1.Name = "messageIndicator1";
            this.messageIndicator1.Size = new System.Drawing.Size(614, 40);
            this.messageIndicator1.TabIndex = 7;
            // 
            // timer1
            // 
            this.timer1.Interval = 2500;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Transparent;
            this.panel5.Controls.Add(this.panel3);
            this.panel5.Controls.Add(this.panel1);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(4, 41);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(614, 425);
            this.panel5.TabIndex = 14;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.Controls.Add(this.listView1);
            this.panel3.Controls.Add(this.panel4);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(614, 360);
            this.panel3.TabIndex = 13;
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.Location = new System.Drawing.Point(265, 0);
            this.listView1.Margin = new System.Windows.Forms.Padding(4);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(349, 360);
            this.listView1.TabIndex = 8;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 800;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.Transparent;
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.label1);
            this.panel4.Controls.Add(this.lblWindow);
            this.panel4.Controls.Add(this.txtAlready);
            this.panel4.Controls.Add(this.label6);
            this.panel4.Controls.Add(this.btnRefresh);
            this.panel4.Controls.Add(this.btnGiveUpAll);
            this.panel4.Controls.Add(this.txtTicket);
            this.panel4.Controls.Add(this.txtHangCount);
            this.panel4.Controls.Add(this.txtWait);
            this.panel4.Controls.Add(this.txtQueueCount);
            this.panel4.Controls.Add(this.label5);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.label3);
            this.panel4.Controls.Add(this.label2);
            this.panel4.Controls.Add(this.txtWindow);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(265, 360);
            this.panel4.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(38, 43);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 16);
            this.label1.TabIndex = 33;
            this.label1.Text = "窗口号：";
            // 
            // lblWindow
            // 
            this.lblWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWindow.Font = new System.Drawing.Font("黑体", 50F, System.Drawing.FontStyle.Bold);
            this.lblWindow.Location = new System.Drawing.Point(117, 15);
            this.lblWindow.Name = "lblWindow";
            this.lblWindow.Size = new System.Drawing.Size(140, 68);
            this.lblWindow.TabIndex = 32;
            this.lblWindow.Text = "101";
            this.lblWindow.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtAlready
            // 
            this.txtAlready.Font = new System.Drawing.Font("宋体", 15F);
            this.txtAlready.Location = new System.Drawing.Point(118, 206);
            this.txtAlready.Margin = new System.Windows.Forms.Padding(4);
            this.txtAlready.Name = "txtAlready";
            this.txtAlready.ReadOnly = true;
            this.txtAlready.Size = new System.Drawing.Size(131, 30);
            this.txtAlready.TabIndex = 31;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label6.Location = new System.Drawing.Point(1, 211);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(110, 16);
            this.label6.TabIndex = 30;
            this.label6.Text = "已办理人数：";
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.Transparent;
            this.btnRefresh.FocusBorder = true;
            this.btnRefresh.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Bold);
            this.btnRefresh.Location = new System.Drawing.Point(237, 393);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(84, 58);
            this.btnRefresh.TabIndex = 28;
            this.btnRefresh.Text = "刷新";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Visible = false;
            // 
            // btnGiveUpAll
            // 
            this.btnGiveUpAll.BackColor = System.Drawing.Color.Transparent;
            this.btnGiveUpAll.FocusBorder = true;
            this.btnGiveUpAll.Font = new System.Drawing.Font("微软雅黑", 13F, System.Drawing.FontStyle.Bold);
            this.btnGiveUpAll.Location = new System.Drawing.Point(121, 395);
            this.btnGiveUpAll.Margin = new System.Windows.Forms.Padding(4);
            this.btnGiveUpAll.Name = "btnGiveUpAll";
            this.btnGiveUpAll.Size = new System.Drawing.Size(84, 58);
            this.btnGiveUpAll.TabIndex = 29;
            this.btnGiveUpAll.Text = "当前窗\r\n口弃号";
            this.btnGiveUpAll.UseVisualStyleBackColor = true;
            this.btnGiveUpAll.Visible = false;
            // 
            // txtTicket
            // 
            this.txtTicket.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTicket.Font = new System.Drawing.Font("宋体", 15F);
            this.txtTicket.Location = new System.Drawing.Point(118, 315);
            this.txtTicket.Margin = new System.Windows.Forms.Padding(4);
            this.txtTicket.Name = "txtTicket";
            this.txtTicket.ReadOnly = true;
            this.txtTicket.Size = new System.Drawing.Size(131, 30);
            this.txtTicket.TabIndex = 27;
            this.txtTicket.WatermarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtTicket.WatermarkText = null;
            // 
            // txtHangCount
            // 
            this.txtHangCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHangCount.Font = new System.Drawing.Font("宋体", 15F);
            this.txtHangCount.Location = new System.Drawing.Point(118, 263);
            this.txtHangCount.Margin = new System.Windows.Forms.Padding(4);
            this.txtHangCount.Name = "txtHangCount";
            this.txtHangCount.ReadOnly = true;
            this.txtHangCount.Size = new System.Drawing.Size(131, 30);
            this.txtHangCount.TabIndex = 26;
            this.txtHangCount.WatermarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtHangCount.WatermarkText = null;
            // 
            // txtWait
            // 
            this.txtWait.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWait.Font = new System.Drawing.Font("宋体", 15F);
            this.txtWait.Location = new System.Drawing.Point(118, 151);
            this.txtWait.Margin = new System.Windows.Forms.Padding(4);
            this.txtWait.Name = "txtWait";
            this.txtWait.ReadOnly = true;
            this.txtWait.Size = new System.Drawing.Size(131, 30);
            this.txtWait.TabIndex = 25;
            this.txtWait.WatermarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtWait.WatermarkText = null;
            // 
            // txtQueueCount
            // 
            this.txtQueueCount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtQueueCount.Font = new System.Drawing.Font("宋体", 15F);
            this.txtQueueCount.Location = new System.Drawing.Point(118, 103);
            this.txtQueueCount.Margin = new System.Windows.Forms.Padding(4);
            this.txtQueueCount.Name = "txtQueueCount";
            this.txtQueueCount.ReadOnly = true;
            this.txtQueueCount.Size = new System.Drawing.Size(131, 30);
            this.txtQueueCount.TabIndex = 24;
            this.txtQueueCount.WatermarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtQueueCount.WatermarkText = null;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(21, 318);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(93, 16);
            this.label5.TabIndex = 23;
            this.label5.Text = "当前票号：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label4.Location = new System.Drawing.Point(21, 266);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 16);
            this.label4.TabIndex = 22;
            this.label4.Text = "挂起人数：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label3.Location = new System.Drawing.Point(21, 156);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 16);
            this.label3.TabIndex = 21;
            this.label3.Text = "等候人数：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label2.Location = new System.Drawing.Point(21, 108);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 16);
            this.label2.TabIndex = 20;
            this.label2.Text = "取号人数：";
            // 
            // txtWindow
            // 
            this.txtWindow.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWindow.Font = new System.Drawing.Font("宋体", 15F);
            this.txtWindow.Location = new System.Drawing.Point(118, 28);
            this.txtWindow.Margin = new System.Windows.Forms.Padding(4);
            this.txtWindow.Name = "txtWindow";
            this.txtWindow.ReadOnly = true;
            this.txtWindow.Size = new System.Drawing.Size(131, 30);
            this.txtWindow.TabIndex = 19;
            this.txtWindow.Visible = false;
            this.txtWindow.WatermarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(204)))), ((int)(((byte)(204)))), ((int)(((byte)(204)))));
            this.txtWindow.WatermarkText = null;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Transparent;
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.btnBackCall);
            this.panel1.Controls.Add(this.btnHang);
            this.panel1.Controls.Add(this.btnMove);
            this.panel1.Controls.Add(this.btnPause);
            this.panel1.Controls.Add(this.btnCall);
            this.panel1.Controls.Add(this.btnReCall);
            this.panel1.Controls.Add(this.btnEv);
            this.panel1.Controls.Add(this.btnCance);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 360);
            this.panel1.Margin = new System.Windows.Forms.Padding(4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(614, 65);
            this.panel1.TabIndex = 12;
            // 
            // btnBackCall
            // 
            this.btnBackCall.BackColor = System.Drawing.Color.Transparent;
            this.btnBackCall.FocusBorder = true;
            this.btnBackCall.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnBackCall.Location = new System.Drawing.Point(540, 1);
            this.btnBackCall.Margin = new System.Windows.Forms.Padding(4);
            this.btnBackCall.Name = "btnBackCall";
            this.btnBackCall.Size = new System.Drawing.Size(70, 58);
            this.btnBackCall.TabIndex = 14;
            this.btnBackCall.Text = "回呼";
            this.btnBackCall.UseVisualStyleBackColor = true;
            this.btnBackCall.Click += new System.EventHandler(this.btnBackCall_Click);
            // 
            // btnHang
            // 
            this.btnHang.BackColor = System.Drawing.Color.Transparent;
            this.btnHang.FocusBorder = true;
            this.btnHang.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnHang.Location = new System.Drawing.Point(463, 1);
            this.btnHang.Margin = new System.Windows.Forms.Padding(4);
            this.btnHang.Name = "btnHang";
            this.btnHang.Size = new System.Drawing.Size(70, 58);
            this.btnHang.TabIndex = 13;
            this.btnHang.Text = "挂起";
            this.btnHang.UseVisualStyleBackColor = true;
            this.btnHang.Click += new System.EventHandler(this.btnHang_Click);
            // 
            // btnMove
            // 
            this.btnMove.BackColor = System.Drawing.Color.Transparent;
            this.btnMove.FocusBorder = true;
            this.btnMove.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnMove.Location = new System.Drawing.Point(386, 1);
            this.btnMove.Margin = new System.Windows.Forms.Padding(4);
            this.btnMove.Name = "btnMove";
            this.btnMove.Size = new System.Drawing.Size(70, 58);
            this.btnMove.TabIndex = 12;
            this.btnMove.Text = "转移";
            this.btnMove.UseVisualStyleBackColor = true;
            this.btnMove.Click += new System.EventHandler(this.btnMove_Click);
            // 
            // btnPause
            // 
            this.btnPause.BackColor = System.Drawing.Color.Transparent;
            this.btnPause.FocusBorder = true;
            this.btnPause.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnPause.Location = new System.Drawing.Point(309, 1);
            this.btnPause.Margin = new System.Windows.Forms.Padding(4);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(70, 58);
            this.btnPause.TabIndex = 11;
            this.btnPause.Text = "暂停";
            this.btnPause.UseVisualStyleBackColor = true;
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnCall
            // 
            this.btnCall.BackColor = System.Drawing.Color.Transparent;
            this.btnCall.FocusBorder = true;
            this.btnCall.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCall.Location = new System.Drawing.Point(1, 1);
            this.btnCall.Margin = new System.Windows.Forms.Padding(4);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(70, 58);
            this.btnCall.TabIndex = 1;
            this.btnCall.Text = "呼叫(F12)";
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // btnReCall
            // 
            this.btnReCall.BackColor = System.Drawing.Color.Transparent;
            this.btnReCall.FocusBorder = true;
            this.btnReCall.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnReCall.Location = new System.Drawing.Point(78, 1);
            this.btnReCall.Margin = new System.Windows.Forms.Padding(4);
            this.btnReCall.Name = "btnReCall";
            this.btnReCall.Size = new System.Drawing.Size(70, 58);
            this.btnReCall.TabIndex = 2;
            this.btnReCall.Text = "重呼";
            this.btnReCall.UseVisualStyleBackColor = true;
            this.btnReCall.Click += new System.EventHandler(this.btnReCall_Click);
            // 
            // btnEv
            // 
            this.btnEv.BackColor = System.Drawing.Color.Transparent;
            this.btnEv.FocusBorder = true;
            this.btnEv.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnEv.Location = new System.Drawing.Point(155, 1);
            this.btnEv.Margin = new System.Windows.Forms.Padding(4);
            this.btnEv.Name = "btnEv";
            this.btnEv.Size = new System.Drawing.Size(70, 58);
            this.btnEv.TabIndex = 10;
            this.btnEv.Text = "评价";
            this.btnEv.UseVisualStyleBackColor = true;
            this.btnEv.Click += new System.EventHandler(this.btnEv_Click);
            // 
            // btnCance
            // 
            this.btnCance.BackColor = System.Drawing.Color.Transparent;
            this.btnCance.FocusBorder = true;
            this.btnCance.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnCance.Location = new System.Drawing.Point(232, 1);
            this.btnCance.Margin = new System.Windows.Forms.Padding(4);
            this.btnCance.Name = "btnCance";
            this.btnCance.Size = new System.Drawing.Size(70, 58);
            this.btnCance.TabIndex = 4;
            this.btnCance.Text = "弃号";
            this.btnCance.UseVisualStyleBackColor = true;
            this.btnCance.Click += new System.EventHandler(this.btnCance_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(116)))), ((int)(((byte)(193)))));
            this.ClientSize = new System.Drawing.Size(622, 510);
            this.Controls.Add(this.panel5);
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
            this.TitleColor = System.Drawing.Color.White;
            this.TitleStyle = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem btnExit;
        private System.Windows.Forms.ToolStripMenuItem btnConfig;
        private MessageClient.MessageIndicator messageIndicator1;
        private System.Windows.Forms.ToolStripMenuItem btnSet;
        private System.Windows.Forms.Timer timer1;
        private CustomSkin.Windows.Forms.Panel panel5;
        private CustomSkin.Windows.Forms.Panel panel3;
        private CustomSkin.Windows.Forms.Panel panel4;
        private CustomSkin.Windows.Forms.Panel panel1;
        private CustomSkin.Windows.Forms.Button btnBackCall;
        private CustomSkin.Windows.Forms.Button btnHang;
        private CustomSkin.Windows.Forms.Button btnMove;
        private CustomSkin.Windows.Forms.Button btnPause;
        private CustomSkin.Windows.Forms.Button btnCall;
        private CustomSkin.Windows.Forms.Button btnReCall;
        private CustomSkin.Windows.Forms.Button btnEv;
        private CustomSkin.Windows.Forms.Button btnCance;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.TextBox txtAlready;
        private System.Windows.Forms.Label label6;
        private CustomSkin.Windows.Forms.Button btnRefresh;
        private CustomSkin.Windows.Forms.Button btnGiveUpAll;
        private CustomSkin.Windows.Forms.TextBox txtTicket;
        private CustomSkin.Windows.Forms.TextBox txtHangCount;
        private CustomSkin.Windows.Forms.TextBox txtWait;
        private CustomSkin.Windows.Forms.TextBox txtQueueCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private CustomSkin.Windows.Forms.TextBox txtWindow;
        private System.Windows.Forms.Label lblWindow;
        private System.Windows.Forms.Label label1;
    }
}

