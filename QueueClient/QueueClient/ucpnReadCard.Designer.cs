namespace QueueClient
{
    partial class ucpnReadCard
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
            this.pnWriterCard = new System.Windows.Forms.Panel();
            this.pbGotoInput = new System.Windows.Forms.PictureBox();
            this.pbReadCard = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbGotoInput)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbReadCard)).BeginInit();
            this.SuspendLayout();
            // 
            // pnWriterCard
            // 
            this.pnWriterCard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(152)))), ((int)(((byte)(202)))), ((int)(((byte)(249)))));
            this.pnWriterCard.Location = new System.Drawing.Point(676, 513);
            this.pnWriterCard.Name = "pnWriterCard";
            this.pnWriterCard.Size = new System.Drawing.Size(608, 68);
            this.pnWriterCard.TabIndex = 5;
            this.pnWriterCard.Paint += new System.Windows.Forms.PaintEventHandler(this.pnWriterCard_Paint);
            // 
            // pbGotoInput
            // 
            this.pbGotoInput.BackColor = System.Drawing.Color.Transparent;
            this.pbGotoInput.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbGotoInput.Image = global::QueueClient.Properties.Resources.矩形;
            this.pbGotoInput.Location = new System.Drawing.Point(563, 703);
            this.pbGotoInput.Name = "pbGotoInput";
            this.pbGotoInput.Size = new System.Drawing.Size(830, 90);
            this.pbGotoInput.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbGotoInput.TabIndex = 4;
            this.pbGotoInput.TabStop = false;
            this.pbGotoInput.Click += new System.EventHandler(this.pbGotoInput_Click);
            this.pbGotoInput.Paint += new System.Windows.Forms.PaintEventHandler(this.pbGotoInput_Paint);
            // 
            // pbReadCard
            // 
            this.pbReadCard.BackColor = System.Drawing.Color.Transparent;
            this.pbReadCard.Image = global::QueueClient.Properties.Resources.刷卡区域;
            this.pbReadCard.Location = new System.Drawing.Point(563, 166);
            this.pbReadCard.Name = "pbReadCard";
            this.pbReadCard.Size = new System.Drawing.Size(830, 477);
            this.pbReadCard.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbReadCard.TabIndex = 3;
            this.pbReadCard.TabStop = false;
            this.pbReadCard.Paint += new System.Windows.Forms.PaintEventHandler(this.pbReadCard_Paint);
            // 
            // ucpnReadCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pnWriterCard);
            this.Controls.Add(this.pbGotoInput);
            this.Controls.Add(this.pbReadCard);
            this.Name = "ucpnReadCard";
            this.Size = new System.Drawing.Size(1920, 910);
            ((System.ComponentModel.ISupportInitialize)(this.pbGotoInput)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbReadCard)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbGotoInput;
        private System.Windows.Forms.PictureBox pbReadCard;
        private System.Windows.Forms.Panel pnWriterCard;
    }
}
