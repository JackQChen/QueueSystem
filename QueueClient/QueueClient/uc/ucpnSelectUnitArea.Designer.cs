namespace QueueClient
{
    partial class ucpnSelectUnitArea
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucpnSelectUnitArea));
            this.pbPrevious = new System.Windows.Forms.PictureBox();
            this.pbNext = new System.Windows.Forms.PictureBox();
            this.pnUnit = new QueueClient.VirtualControlContainer();
            ((System.ComponentModel.ISupportInitialize)(this.pbPrevious)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).BeginInit();
            this.SuspendLayout();
            // 
            // pbPrevious
            // 
            this.pbPrevious.Image = global::QueueClient.Properties.Resources.部门下一页;
            this.pbPrevious.Location = new System.Drawing.Point(131, 831);
            this.pbPrevious.Name = "pbPrevious";
            this.pbPrevious.Size = new System.Drawing.Size(144, 52);
            this.pbPrevious.TabIndex = 24;
            this.pbPrevious.TabStop = false;
            this.pbPrevious.Click += new System.EventHandler(this.pbPrevious_Click);
            this.pbPrevious.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPrevious_Paint);
            this.pbPrevious.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseDown);
            this.pbPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseUp);
            // 
            // pbNext
            // 
            this.pbNext.Image = global::QueueClient.Properties.Resources.部门下一页;
            this.pbNext.Location = new System.Drawing.Point(1643, 831);
            this.pbNext.Name = "pbNext";
            this.pbNext.Size = new System.Drawing.Size(144, 52);
            this.pbNext.TabIndex = 25;
            this.pbNext.TabStop = false;
            this.pbNext.Click += new System.EventHandler(this.pbNext_Click);
            this.pbNext.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPrevious_Paint);
            this.pbNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseDown);
            this.pbNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseUp);
            // 
            // pnUnit
            // 
            this.pnUnit.BackColor = System.Drawing.Color.Transparent;
            this.pnUnit.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pnUnit.BackgroundImage")));
            this.pnUnit.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnUnit.Image = null;
            this.pnUnit.Location = new System.Drawing.Point(106, 97);
            this.pnUnit.Name = "pnUnit";
            this.pnUnit.Size = new System.Drawing.Size(1725, 717);
            this.pnUnit.TabIndex = 23;
            // 
            // ucpnSelectUnitArea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pbNext);
            this.Controls.Add(this.pbPrevious);
            this.Controls.Add(this.pnUnit);
            this.Name = "ucpnSelectUnitArea";
            this.Size = new System.Drawing.Size(1920, 910);
            this.Load += new System.EventHandler(this.ucpnSelectUnit_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbPrevious)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbNext)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private VirtualControlContainer pnUnit;
        private System.Windows.Forms.PictureBox pbPrevious;
        private System.Windows.Forms.PictureBox pbNext;
    }
}
