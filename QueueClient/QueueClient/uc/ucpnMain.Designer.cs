namespace QueueClient
{
    partial class ucpnMain
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
            this.pbWorking2 = new System.Windows.Forms.PictureBox();
            this.pbSelect = new System.Windows.Forms.PictureBox();
            this.pbWorking = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbWorking2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelect)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbWorking)).BeginInit();
            this.SuspendLayout();
            // 
            // pbWorking2
            // 
            this.pbWorking2.BackColor = System.Drawing.Color.Transparent;
            this.pbWorking2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbWorking2.Image = global::QueueClient.Properties.Resources.矩形_16_拷贝_2;
            this.pbWorking2.Location = new System.Drawing.Point(1216, 348);
            this.pbWorking2.Name = "pbWorking2";
            this.pbWorking2.Size = new System.Drawing.Size(490, 190);
            this.pbWorking2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbWorking2.TabIndex = 19;
            this.pbWorking2.TabStop = false;
            this.pbWorking2.Click += new System.EventHandler(this.pbWorking2_Click);
            this.pbWorking2.Paint += new System.Windows.Forms.PaintEventHandler(this.pbWork_Paint);
            // 
            // pbSelect
            // 
            this.pbSelect.BackColor = System.Drawing.Color.Transparent;
            this.pbSelect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbSelect.Image = global::QueueClient.Properties.Resources.请选择您要办理的类型;
            this.pbSelect.Location = new System.Drawing.Point(589, 91);
            this.pbSelect.Name = "pbSelect";
            this.pbSelect.Size = new System.Drawing.Size(725, 71);
            this.pbSelect.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbSelect.TabIndex = 18;
            this.pbSelect.TabStop = false;
            // 
            // pbWorking
            // 
            this.pbWorking.BackColor = System.Drawing.Color.Transparent;
            this.pbWorking.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbWorking.Image = global::QueueClient.Properties.Resources.矩形_16_拷贝_2;
            this.pbWorking.Location = new System.Drawing.Point(208, 348);
            this.pbWorking.Name = "pbWorking";
            this.pbWorking.Size = new System.Drawing.Size(490, 190);
            this.pbWorking.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbWorking.TabIndex = 15;
            this.pbWorking.TabStop = false;
            this.pbWorking.Paint += new System.Windows.Forms.PaintEventHandler(this.pbWork_Paint);
            // 
            // ucpnMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.pbWorking2);
            this.Controls.Add(this.pbSelect);
            this.Controls.Add(this.pbWorking);
            this.Name = "ucpnMain";
            this.Size = new System.Drawing.Size(1920, 910);
            ((System.ComponentModel.ISupportInitialize)(this.pbWorking2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSelect)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbWorking)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pbWorking;
        private System.Windows.Forms.PictureBox pbSelect;
        private System.Windows.Forms.PictureBox pbWorking2;
    }
}
