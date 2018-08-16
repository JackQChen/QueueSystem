namespace MessageServer
{
    partial class FrmSetting
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSetting));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.numCacheSize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.numMaxConn = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.numPort = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.cmbProtocol = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCacheSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConn)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.numCacheSize);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.numMaxConn);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.numPort);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cmbProtocol);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(225, 217);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "协议设置";
            // 
            // numCacheSize
            // 
            this.numCacheSize.Location = new System.Drawing.Point(83, 146);
            this.numCacheSize.Name = "numCacheSize";
            this.numCacheSize.Size = new System.Drawing.Size(120, 21);
            this.numCacheSize.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 6;
            this.label4.Text = "缓存大小";
            // 
            // numMaxConn
            // 
            this.numMaxConn.Location = new System.Drawing.Point(83, 108);
            this.numMaxConn.Name = "numMaxConn";
            this.numMaxConn.Size = new System.Drawing.Size(120, 21);
            this.numMaxConn.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 112);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "最大连接数";
            // 
            // numPort
            // 
            this.numPort.Location = new System.Drawing.Point(83, 72);
            this.numPort.Name = "numPort";
            this.numPort.Size = new System.Drawing.Size(120, 21);
            this.numPort.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(35, 76);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "端口号";
            // 
            // cmbProtocol
            // 
            this.cmbProtocol.FormattingEnabled = true;
            this.cmbProtocol.Location = new System.Drawing.Point(82, 33);
            this.cmbProtocol.Name = "cmbProtocol";
            this.cmbProtocol.Size = new System.Drawing.Size(121, 20);
            this.cmbProtocol.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "协议类型";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Location = new System.Drawing.Point(74, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(80, 30);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "确定(&O)";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(160, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(80, 30);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消(&C)";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // FrmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(252, 288);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FrmSetting";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "协议设置";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numCacheSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxConn)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numPort)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbProtocol;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numPort;
        private System.Windows.Forms.NumericUpDown numMaxConn;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numCacheSize;
        private System.Windows.Forms.Label label4;
    }
}