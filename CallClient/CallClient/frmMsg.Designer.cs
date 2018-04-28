namespace CallClient
{
    partial class frmMsg
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTimes = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lblTicket = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Font = new System.Drawing.Font("宋体", 16F);
            this.label1.Location = new System.Drawing.Point(17, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(252, 22);
            this.label1.TabIndex = 0;
            this.label1.Text = "有新的排队数据，票号：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.BackColor = System.Drawing.Color.Transparent;
            this.label2.Font = new System.Drawing.Font("宋体", 16F);
            this.label2.Location = new System.Drawing.Point(136, 145);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 22);
            this.label2.TabIndex = 1;
            this.label2.Text = "显示剩余：";
            // 
            // lblTimes
            // 
            this.lblTimes.AutoSize = true;
            this.lblTimes.BackColor = System.Drawing.Color.Transparent;
            this.lblTimes.Font = new System.Drawing.Font("宋体", 16F);
            this.lblTimes.ForeColor = System.Drawing.Color.Red;
            this.lblTimes.Location = new System.Drawing.Point(256, 145);
            this.lblTimes.Name = "lblTimes";
            this.lblTimes.Size = new System.Drawing.Size(21, 22);
            this.lblTimes.TabIndex = 2;
            this.lblTimes.Text = "5";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("宋体", 16F);
            this.label4.Location = new System.Drawing.Point(293, 145);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 22);
            this.label4.TabIndex = 3;
            this.label4.Text = "秒";
            // 
            // lblTicket
            // 
            this.lblTicket.BackColor = System.Drawing.Color.Transparent;
            this.lblTicket.Font = new System.Drawing.Font("宋体", 16F);
            this.lblTicket.Location = new System.Drawing.Point(265, 76);
            this.lblTicket.Name = "lblTicket";
            this.lblTicket.Size = new System.Drawing.Size(65, 22);
            this.lblTicket.TabIndex = 4;
            // 
            // frmMsg
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(183)))), ((int)(((byte)(214)))));
            this.ClientSize = new System.Drawing.Size(358, 192);
            this.Controls.Add(this.lblTicket);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblTimes);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMsg";
            this.ShowInTaskbar = false;
            this.Text = "温馨提示";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMsg_FormClosing);
            this.Load += new System.EventHandler(this.frmMsg_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTimes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblTicket;
    }
}