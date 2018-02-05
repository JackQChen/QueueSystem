namespace QueueClient
{
    partial class AppointmentCard
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

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.pbIsCheck = new System.Windows.Forms.PictureBox();
            this.pbMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbIsCheck)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.SuspendLayout();
            // 
            // pbIsCheck
            // 
            this.pbIsCheck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(237)))), ((int)(((byte)(237)))));
            this.pbIsCheck.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbIsCheck.Image = global::QueueClient.Properties.Resources.预约勾选框1;
            this.pbIsCheck.Location = new System.Drawing.Point(24, 21);
            this.pbIsCheck.Name = "pbIsCheck";
            this.pbIsCheck.Size = new System.Drawing.Size(60, 53);
            this.pbIsCheck.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbIsCheck.TabIndex = 1;
            this.pbIsCheck.TabStop = false;
            this.pbIsCheck.Click += new System.EventHandler(this.pbIsCheck_Click);
            // 
            // pbMain
            // 
            this.pbMain.BackColor = System.Drawing.Color.Transparent;
            this.pbMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbMain.Image = global::QueueClient.Properties.Resources.预约未选中;
            this.pbMain.Location = new System.Drawing.Point(0, 0);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(1507, 94);
            this.pbMain.TabIndex = 0;
            this.pbMain.TabStop = false;
            this.pbMain.Paint += new System.Windows.Forms.PaintEventHandler(this.pbMain_Paint);
            // 
            // AppointmentCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pbIsCheck);
            this.Controls.Add(this.pbMain);
            this.Name = "AppointmentCard";
            this.Size = new System.Drawing.Size(1507, 94);
            this.Load += new System.EventHandler(this.AppointmentCard_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbIsCheck)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbMain;
        private System.Windows.Forms.PictureBox pbIsCheck;
    }
}
