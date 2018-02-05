namespace QueueClient
{
    partial class ucpnAppointment
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
            this.panel3 = new System.Windows.Forms.Panel();
            this.pnAppointmentMain = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pbNext1 = new System.Windows.Forms.PictureBox();
            this.pnPrevious1 = new System.Windows.Forms.PictureBox();
            this.pbOk = new System.Windows.Forms.PictureBox();
            this.pbOther = new System.Windows.Forms.PictureBox();
            this.panel3.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnPrevious1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOther)).BeginInit();
            this.SuspendLayout();
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.Transparent;
            this.panel3.BackgroundImage = global::QueueClient.Properties.Resources.预约背景;
            this.panel3.Controls.Add(this.pnAppointmentMain);
            this.panel3.Controls.Add(this.panel5);
            this.panel3.Location = new System.Drawing.Point(189, 103);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1565, 775);
            this.panel3.TabIndex = 22;
            // 
            // pnAppointmentMain
            // 
            this.pnAppointmentMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnAppointmentMain.Location = new System.Drawing.Point(0, 0);
            this.pnAppointmentMain.Name = "pnAppointmentMain";
            this.pnAppointmentMain.Size = new System.Drawing.Size(1565, 634);
            this.pnAppointmentMain.TabIndex = 2;
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.pbOther);
            this.panel5.Controls.Add(this.pbNext1);
            this.panel5.Controls.Add(this.pnPrevious1);
            this.panel5.Controls.Add(this.pbOk);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(0, 634);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(1565, 141);
            this.panel5.TabIndex = 1;
            // 
            // pbNext1
            // 
            this.pbNext1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbNext1.Image = global::QueueClient.Properties.Resources.预约下一页;
            this.pbNext1.Location = new System.Drawing.Point(1382, 35);
            this.pbNext1.Name = "pbNext1";
            this.pbNext1.Size = new System.Drawing.Size(118, 32);
            this.pbNext1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbNext1.TabIndex = 2;
            this.pbNext1.TabStop = false;
            this.pbNext1.Click += new System.EventHandler(this.pbNext1_Click);
            this.pbNext1.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious1_Paint);
            // 
            // pnPrevious1
            // 
            this.pnPrevious1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnPrevious1.Image = global::QueueClient.Properties.Resources.预约下一页;
            this.pnPrevious1.Location = new System.Drawing.Point(81, 35);
            this.pnPrevious1.Name = "pnPrevious1";
            this.pnPrevious1.Size = new System.Drawing.Size(118, 32);
            this.pnPrevious1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pnPrevious1.TabIndex = 1;
            this.pnPrevious1.TabStop = false;
            this.pnPrevious1.Click += new System.EventHandler(this.pnPrevious1_Click);
            this.pnPrevious1.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious1_Paint);
            this.pnPrevious1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnPrevious1_MouseDown);
            this.pnPrevious1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnPrevious1_MouseUp);
            // 
            // pbOk
            // 
            this.pbOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOk.Image = global::QueueClient.Properties.Resources.预约提交;
            this.pbOk.Location = new System.Drawing.Point(349, 35);
            this.pbOk.Name = "pbOk";
            this.pbOk.Size = new System.Drawing.Size(238, 66);
            this.pbOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOk.TabIndex = 0;
            this.pbOk.TabStop = false;
            this.pbOk.Click += new System.EventHandler(this.pbOk_Click);
            this.pbOk.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious1_Paint);
            this.pbOk.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbOk_MouseDown);
            this.pbOk.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbOk_MouseUp);
            // 
            // pbOther
            // 
            this.pbOther.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOther.Image = global::QueueClient.Properties.Resources.预约提交;
            this.pbOther.Location = new System.Drawing.Point(1003, 35);
            this.pbOther.Name = "pbOther";
            this.pbOther.Size = new System.Drawing.Size(238, 66);
            this.pbOther.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOther.TabIndex = 3;
            this.pbOther.TabStop = false;
            this.pbOther.Click += new System.EventHandler(this.pbOther_Click);
            this.pbOther.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious1_Paint);
            // 
            // ucpnAppointment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.panel3);
            this.Name = "ucpnAppointment";
            this.Size = new System.Drawing.Size(1920, 910);
            this.panel3.ResumeLayout(false);
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnPrevious1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOther)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnAppointmentMain;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.PictureBox pbNext1;
        private System.Windows.Forms.PictureBox pnPrevious1;
        private System.Windows.Forms.PictureBox pbOk;
        private System.Windows.Forms.PictureBox pbOther;
    }
}
