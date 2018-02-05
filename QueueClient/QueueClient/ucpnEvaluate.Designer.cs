namespace QueueClient
{
    partial class ucpnEvaluate
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
            this.pnEvaluateList = new System.Windows.Forms.Panel();
            this.pnEvaluateMain = new System.Windows.Forms.Panel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pbNext = new System.Windows.Forms.PictureBox();
            this.pnPrevious = new System.Windows.Forms.PictureBox();
            this.pbSubmit = new System.Windows.Forms.PictureBox();
            this.pnEvaluateList.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnPrevious)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSubmit)).BeginInit();
            this.SuspendLayout();
            // 
            // pnEvaluateList
            // 
            this.pnEvaluateList.BackColor = System.Drawing.Color.Transparent;
            this.pnEvaluateList.BackgroundImage = global::QueueClient.Properties.Resources.评价背景;
            this.pnEvaluateList.Controls.Add(this.pnEvaluateMain);
            this.pnEvaluateList.Controls.Add(this.panel1);
            this.pnEvaluateList.Location = new System.Drawing.Point(197, 110);
            this.pnEvaluateList.Name = "pnEvaluateList";
            this.pnEvaluateList.Size = new System.Drawing.Size(1566, 773);
            this.pnEvaluateList.TabIndex = 22;
            // 
            // pnEvaluateMain
            // 
            this.pnEvaluateMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnEvaluateMain.Location = new System.Drawing.Point(0, 0);
            this.pnEvaluateMain.Name = "pnEvaluateMain";
            this.pnEvaluateMain.Size = new System.Drawing.Size(1566, 696);
            this.pnEvaluateMain.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pbNext);
            this.panel1.Controls.Add(this.pnPrevious);
            this.panel1.Controls.Add(this.pbSubmit);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 696);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1566, 77);
            this.panel1.TabIndex = 1;
            // 
            // pbNext
            // 
            this.pbNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbNext.Image = global::QueueClient.Properties.Resources.下一页;
            this.pbNext.Location = new System.Drawing.Point(1384, 18);
            this.pbNext.Name = "pbNext";
            this.pbNext.Size = new System.Drawing.Size(118, 32);
            this.pbNext.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbNext.TabIndex = 2;
            this.pbNext.TabStop = false;
            this.pbNext.Click += new System.EventHandler(this.pbNext_Click);
            this.pbNext.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious_Paint);
            this.pbNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnPrevious_MouseDown);
            this.pbNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnPrevious_MouseUp);
            // 
            // pnPrevious
            // 
            this.pnPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pnPrevious.Image = global::QueueClient.Properties.Resources.下一页;
            this.pnPrevious.Location = new System.Drawing.Point(44, 18);
            this.pnPrevious.Name = "pnPrevious";
            this.pnPrevious.Size = new System.Drawing.Size(118, 32);
            this.pnPrevious.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pnPrevious.TabIndex = 1;
            this.pnPrevious.TabStop = false;
            this.pnPrevious.Click += new System.EventHandler(this.pnPrevious_Click);
            this.pnPrevious.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious_Paint);
            this.pnPrevious.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pnPrevious_MouseDown);
            this.pnPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pnPrevious_MouseUp);
            // 
            // pbSubmit
            // 
            this.pbSubmit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSubmit.Image = global::QueueClient.Properties.Resources.提交_button1;
            this.pbSubmit.Location = new System.Drawing.Point(680, 2);
            this.pbSubmit.Name = "pbSubmit";
            this.pbSubmit.Size = new System.Drawing.Size(252, 72);
            this.pbSubmit.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbSubmit.TabIndex = 0;
            this.pbSubmit.TabStop = false;
            this.pbSubmit.Visible = false;
            this.pbSubmit.Click += new System.EventHandler(this.pbSubmit_Click);
            this.pbSubmit.Paint += new System.Windows.Forms.PaintEventHandler(this.pnPrevious_Paint);
            // 
            // ucpnEvaluate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnEvaluateList);
            this.Name = "ucpnEvaluate";
            this.Size = new System.Drawing.Size(1920, 944);
            this.pnEvaluateList.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pnPrevious)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSubmit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnEvaluateList;
        private System.Windows.Forms.Panel pnEvaluateMain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pbNext;
        private System.Windows.Forms.PictureBox pnPrevious;
        private System.Windows.Forms.PictureBox pbSubmit;
    }
}
