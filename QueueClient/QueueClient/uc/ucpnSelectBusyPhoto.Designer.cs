namespace QueueClient
{
    partial class ucpnSelectBusyPhoto
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
            this.pnBusiness = new QueueClient.VirtualControlContainer();
            this.pbNext = new System.Windows.Forms.PictureBox();
            this.pbPrevious = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPrevious)).BeginInit();
            this.SuspendLayout();
            // 
            // pnBusiness
            // 
            this.pnBusiness.BackColor = System.Drawing.Color.Transparent;
            this.pnBusiness.Image = null;
            this.pnBusiness.Location = new System.Drawing.Point(106, 77);
            this.pnBusiness.Name = "pnBusiness";
            this.pnBusiness.Size = new System.Drawing.Size(1725, 737);
            this.pnBusiness.TabIndex = 23;
            // 
            // pbNext
            // 
            this.pbNext.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbNext.Image = global::QueueClient.Properties.Resources.部门下一页;
            this.pbNext.Location = new System.Drawing.Point(1643, 831);
            this.pbNext.Name = "pbNext";
            this.pbNext.Size = new System.Drawing.Size(144, 52);
            this.pbNext.TabIndex = 27;
            this.pbNext.TabStop = false;
            this.pbNext.Click += new System.EventHandler(this.pbNext_Click);
            this.pbNext.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPrevious_Paint);
            this.pbNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseDown);
            this.pbNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseUp);
            // 
            // pbPrevious
            // 
            this.pbPrevious.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbPrevious.Image = global::QueueClient.Properties.Resources.部门下一页;
            this.pbPrevious.Location = new System.Drawing.Point(131, 831);
            this.pbPrevious.Name = "pbPrevious";
            this.pbPrevious.Size = new System.Drawing.Size(144, 52);
            this.pbPrevious.TabIndex = 26;
            this.pbPrevious.TabStop = false;
            this.pbPrevious.Click += new System.EventHandler(this.pbPrevious_Click);
            this.pbPrevious.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPrevious_Paint);
            this.pbPrevious.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseDown);
            this.pbPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseUp);
            // 
            // ucpnSelectBusyPhoto
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pbNext);
            this.Controls.Add(this.pbPrevious);
            this.Controls.Add(this.pnBusiness);
            this.Name = "ucpnSelectBusyPhoto";
            this.Size = new System.Drawing.Size(1920, 910);
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbPrevious)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private VirtualControlContainer pnBusiness;
        private System.Windows.Forms.PictureBox pbNext;
        private System.Windows.Forms.PictureBox pbPrevious;
    }
}
