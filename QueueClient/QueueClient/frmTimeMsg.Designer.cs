namespace QueueClient
{
    partial class frmTimeMsg
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
            this.pbOk = new System.Windows.Forms.PictureBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).BeginInit();
            this.SuspendLayout();
            // 
            // pbOk
            // 
            this.pbOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.pbOk.BackColor = System.Drawing.Color.Transparent;
            this.pbOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOk.Image = global::QueueClient.Properties.Resources.弹窗确认按钮;
            this.pbOk.Location = new System.Drawing.Point(213, 296);
            this.pbOk.Name = "pbOk";
            this.pbOk.Size = new System.Drawing.Size(134, 46);
            this.pbOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbOk.TabIndex = 0;
            this.pbOk.TabStop = false;
            this.pbOk.Visible = false;
            this.pbOk.Click += new System.EventHandler(this.pbOk_Click);
            this.pbOk.Paint += new System.Windows.Forms.PaintEventHandler(this.pbOk_Paint);
            this.pbOk.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbOk_MouseDown);
            this.pbOk.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbOk_MouseUp);
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // frmTimeMsg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(588, 366);
            this.Controls.Add(this.pbOk);
            this.Name = "frmTimeMsg";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "消息提示框";
            this.Load += new System.EventHandler(this.frmMsg_Load);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.frmMsg_Paint);
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbOk;
        private System.Windows.Forms.Timer timer1;


    }
}

