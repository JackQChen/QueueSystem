namespace QueueClient
{
    partial class ucpnCard
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
            this.txtCard = new System.Windows.Forms.TextBox();
            this.pnNumber = new QueueClient.VirtualControlContainer();
            this.SuspendLayout();
            // 
            // txtCard
            // 
            this.txtCard.BackColor = System.Drawing.SystemColors.Window;
            this.txtCard.Font = new System.Drawing.Font("微软雅黑", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.txtCard.ForeColor = System.Drawing.Color.SeaGreen;
            this.txtCard.Location = new System.Drawing.Point(508, 126);
            this.txtCard.MaxLength = 18;
            this.txtCard.Name = "txtCard";
            this.txtCard.Size = new System.Drawing.Size(903, 113);
            this.txtCard.TabIndex = 34;
            // 
            // pnNumber
            // 
            this.pnNumber.BackColor = System.Drawing.Color.Transparent;
            this.pnNumber.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pnNumber.Image = null;
            this.pnNumber.Location = new System.Drawing.Point(507, 24);
            this.pnNumber.Name = "pnNumber";
            this.pnNumber.Size = new System.Drawing.Size(931, 886);
            this.pnNumber.TabIndex = 48;
            // 
            // ucpnCard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.txtCard);
            this.Controls.Add(this.pnNumber);
            this.Name = "ucpnCard";
            this.Size = new System.Drawing.Size(1920, 910);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtCard;
        private VirtualControlContainer pnNumber;
    }
}
