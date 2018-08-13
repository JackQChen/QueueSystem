namespace QueueClient
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
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.pntitle = new System.Windows.Forms.Panel();
            this.pnexit = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.lblMes = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.pbLastPage = new System.Windows.Forms.PictureBox();
            this.pbReturnMain = new System.Windows.Forms.PictureBox();
            this.pnMain = new System.Windows.Forms.Panel();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.timer3 = new System.Windows.Forms.Timer(this.components);
            this.pntitle.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLastPage)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbReturnMain)).BeginInit();
            this.pnMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 36000000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // pntitle
            // 
            this.pntitle.BackColor = System.Drawing.Color.Transparent;
            this.pntitle.BackgroundImage = global::QueueClient.Properties.Resources.title;
            this.pntitle.Controls.Add(this.pnexit);
            this.pntitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.pntitle.Location = new System.Drawing.Point(0, 0);
            this.pntitle.Name = "pntitle";
            this.pntitle.Size = new System.Drawing.Size(1264, 110);
            this.pntitle.TabIndex = 0;
            // 
            // pnexit
            // 
            this.pnexit.BackColor = System.Drawing.Color.Transparent;
            this.pnexit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnexit.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnexit.Location = new System.Drawing.Point(0, 0);
            this.pnexit.Name = "pnexit";
            this.pnexit.Size = new System.Drawing.Size(129, 110);
            this.pnexit.TabIndex = 14;
            this.pnexit.Click += new System.EventHandler(this.pnexit_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.Transparent;
            this.panel2.BackgroundImage = global::QueueClient.Properties.Resources.底部黑色;
            this.panel2.Controls.Add(this.lblMes);
            this.panel2.Controls.Add(this.lblTime);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 730);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1264, 60);
            this.panel2.TabIndex = 1;
            // 
            // lblMes
            // 
            this.lblMes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMes.AutoSize = true;
            this.lblMes.Font = new System.Drawing.Font("黑体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMes.ForeColor = System.Drawing.Color.White;
            this.lblMes.Location = new System.Drawing.Point(906, 13);
            this.lblMes.Name = "lblMes";
            this.lblMes.Size = new System.Drawing.Size(349, 35);
            this.lblMes.TabIndex = 1;
            this.lblMes.Text = "剩余操作时间：10秒";
            this.lblMes.Visible = false;
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("黑体", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(5, 12);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(449, 35);
            this.lblTime.TabIndex = 0;
            this.lblTime.Text = "2018-12-27 17:30 星期三";
            // 
            // pbLastPage
            // 
            this.pbLastPage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbLastPage.BackColor = System.Drawing.Color.Transparent;
            this.pbLastPage.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbLastPage.Image = global::QueueClient.Properties.Resources.L返回按钮;
            this.pbLastPage.Location = new System.Drawing.Point(951, 22);
            this.pbLastPage.Name = "pbLastPage";
            this.pbLastPage.Size = new System.Drawing.Size(170, 60);
            this.pbLastPage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbLastPage.TabIndex = 37;
            this.pbLastPage.TabStop = false;
            this.pbLastPage.Click += new System.EventHandler(this.pbLastPage_Click);
            // 
            // pbReturnMain
            // 
            this.pbReturnMain.BackColor = System.Drawing.Color.Transparent;
            this.pbReturnMain.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbReturnMain.Image = global::QueueClient.Properties.Resources.L首页按钮;
            this.pbReturnMain.Location = new System.Drawing.Point(131, 22);
            this.pbReturnMain.Name = "pbReturnMain";
            this.pbReturnMain.Size = new System.Drawing.Size(170, 60);
            this.pbReturnMain.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbReturnMain.TabIndex = 36;
            this.pbReturnMain.TabStop = false;
            this.pbReturnMain.Click += new System.EventHandler(this.pbReturn_Click);
            // 
            // pnMain
            // 
            this.pnMain.BackColor = System.Drawing.Color.Transparent;
            this.pnMain.Controls.Add(this.pbLastPage);
            this.pnMain.Controls.Add(this.pbReturnMain);
            this.pnMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnMain.Location = new System.Drawing.Point(0, 110);
            this.pnMain.Name = "pnMain";
            this.pnMain.Size = new System.Drawing.Size(1264, 620);
            this.pnMain.TabIndex = 2;
            // 
            // timer2
            // 
            this.timer2.Enabled = true;
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // timer3
            // 
            this.timer3.Enabled = true;
            this.timer3.Interval = 1000;
            this.timer3.Tick += new System.EventHandler(this.timer3_Tick);
            // 
            // frmMain3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::QueueClient.Properties.Resources.背景;
            this.ClientSize = new System.Drawing.Size(1264, 790);
            this.Controls.Add(this.pnMain);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pntitle);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "frmMain3";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "排队取票系统";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.pntitle.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbLastPage)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbReturnMain)).EndInit();
            this.pnMain.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Panel pntitle;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel pnMain;
        private System.Windows.Forms.PictureBox pbLastPage;
        private System.Windows.Forms.PictureBox pbReturnMain;
        private System.Windows.Forms.Panel pnexit;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblMes;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.Timer timer3;
    }
}

