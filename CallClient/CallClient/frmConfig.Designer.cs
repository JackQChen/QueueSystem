namespace CallClient
{
    partial class frmConfig
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWindowName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbSelect = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbPort = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new CustomSkin.Windows.Forms.Button();
            this.btnClose = new CustomSkin.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.f8 = new System.Windows.Forms.ComboBox();
            this.f7 = new System.Windows.Forms.ComboBox();
            this.f6 = new System.Windows.Forms.ComboBox();
            this.f5 = new System.Windows.Forms.ComboBox();
            this.f4 = new System.Windows.Forms.ComboBox();
            this.f3 = new System.Windows.Forms.ComboBox();
            this.f2 = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.f1 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtWindowName);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.cmbSelect);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(14, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 88);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "窗口配置";
            // 
            // txtWindowName
            // 
            this.txtWindowName.Location = new System.Drawing.Point(114, 23);
            this.txtWindowName.Name = "txtWindowName";
            this.txtWindowName.ReadOnly = true;
            this.txtWindowName.Size = new System.Drawing.Size(203, 21);
            this.txtWindowName.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(31, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 12);
            this.label3.TabIndex = 3;
            this.label3.Text = "当前窗口：";
            // 
            // cmbSelect
            // 
            this.cmbSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSelect.FormattingEnabled = true;
            this.cmbSelect.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10"});
            this.cmbSelect.Location = new System.Drawing.Point(114, 57);
            this.cmbSelect.Name = "cmbSelect";
            this.cmbSelect.Size = new System.Drawing.Size(203, 20);
            this.cmbSelect.TabIndex = 2;
            this.cmbSelect.SelectedIndexChanged += new System.EventHandler(this.cmbSelect_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(31, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "选择窗口：";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbPort);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(400, 53);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 100);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "呼叫器配置";
            // 
            // cmbPort
            // 
            this.cmbPort.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPort.FormattingEnabled = true;
            this.cmbPort.Items.AddRange(new object[] {
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "COM10"});
            this.cmbPort.Location = new System.Drawing.Point(114, 41);
            this.cmbPort.Name = "cmbPort";
            this.cmbPort.Size = new System.Drawing.Size(92, 20);
            this.cmbPort.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 44);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "端口编号：";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.FocusBorder = true;
            this.btnSave.Location = new System.Drawing.Point(127, 414);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(92, 35);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存(&S)";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.Transparent;
            this.btnClose.FocusBorder = true;
            this.btnClose.Location = new System.Drawing.Point(225, 414);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(92, 35);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "关闭(&C)";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.btnClose);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.groupBox3);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Controls.Add(this.groupBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 31);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(397, 483);
            this.panel1.TabIndex = 4;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.f8);
            this.groupBox3.Controls.Add(this.f7);
            this.groupBox3.Controls.Add(this.f6);
            this.groupBox3.Controls.Add(this.f5);
            this.groupBox3.Controls.Add(this.f4);
            this.groupBox3.Controls.Add(this.f3);
            this.groupBox3.Controls.Add(this.f2);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.f1);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Location = new System.Drawing.Point(14, 106);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(335, 295);
            this.groupBox3.TabIndex = 1;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "操作快捷键设置";
            // 
            // f8
            // 
            this.f8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f8.FormattingEnabled = true;
            this.f8.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f8.Location = new System.Drawing.Point(112, 261);
            this.f8.Name = "f8";
            this.f8.Size = new System.Drawing.Size(203, 20);
            this.f8.TabIndex = 19;
            // 
            // f7
            // 
            this.f7.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f7.FormattingEnabled = true;
            this.f7.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f7.Location = new System.Drawing.Point(112, 228);
            this.f7.Name = "f7";
            this.f7.Size = new System.Drawing.Size(203, 20);
            this.f7.TabIndex = 18;
            // 
            // f6
            // 
            this.f6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f6.FormattingEnabled = true;
            this.f6.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f6.Location = new System.Drawing.Point(113, 195);
            this.f6.Name = "f6";
            this.f6.Size = new System.Drawing.Size(203, 20);
            this.f6.TabIndex = 17;
            // 
            // f5
            // 
            this.f5.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f5.FormattingEnabled = true;
            this.f5.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f5.Location = new System.Drawing.Point(113, 162);
            this.f5.Name = "f5";
            this.f5.Size = new System.Drawing.Size(203, 20);
            this.f5.TabIndex = 16;
            // 
            // f4
            // 
            this.f4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f4.FormattingEnabled = true;
            this.f4.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f4.Location = new System.Drawing.Point(113, 129);
            this.f4.Name = "f4";
            this.f4.Size = new System.Drawing.Size(203, 20);
            this.f4.TabIndex = 15;
            // 
            // f3
            // 
            this.f3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f3.FormattingEnabled = true;
            this.f3.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f3.Location = new System.Drawing.Point(112, 96);
            this.f3.Name = "f3";
            this.f3.Size = new System.Drawing.Size(203, 20);
            this.f3.TabIndex = 14;
            // 
            // f2
            // 
            this.f2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f2.FormattingEnabled = true;
            this.f2.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f2.Location = new System.Drawing.Point(113, 63);
            this.f2.Name = "f2";
            this.f2.Size = new System.Drawing.Size(203, 20);
            this.f2.TabIndex = 13;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(43, 266);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(53, 12);
            this.label11.TabIndex = 12;
            this.label11.Text = "回呼键：";
            // 
            // f1
            // 
            this.f1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.f1.FormattingEnabled = true;
            this.f1.Items.AddRange(new object[] {
            "",
            "F1",
            "F2",
            "F3",
            "F4",
            "F5",
            "F6",
            "F7",
            "F8",
            "F9",
            "F10",
            "F11",
            "F12"});
            this.f1.Location = new System.Drawing.Point(113, 30);
            this.f1.Name = "f1";
            this.f1.Size = new System.Drawing.Size(203, 20);
            this.f1.TabIndex = 11;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(43, 233);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(53, 12);
            this.label10.TabIndex = 10;
            this.label10.Text = "挂起键：";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(43, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(53, 12);
            this.label9.TabIndex = 9;
            this.label9.Text = "转移键：";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(43, 167);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 8;
            this.label8.Text = "暂停键：";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(43, 134);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 7;
            this.label7.Text = "弃号键：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 101);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 6;
            this.label6.Text = "评价键：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "重呼键：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 35);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "呼叫键：";
            // 
            // frmConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(183)))), ((int)(((byte)(214)))));
            this.ClientSize = new System.Drawing.Size(403, 517);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfig";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "系统配置";
            this.TitleStyle = true;
            this.Load += new System.EventHandler(this.frmConfig_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ComboBox cmbPort;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtWindowName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbSelect;
        private System.Windows.Forms.Label label2;
        private CustomSkin.Windows.Forms.Button btnSave;
        private CustomSkin.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox f1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.ComboBox f8;
        private System.Windows.Forms.ComboBox f7;
        private System.Windows.Forms.ComboBox f6;
        private System.Windows.Forms.ComboBox f5;
        private System.Windows.Forms.ComboBox f4;
        private System.Windows.Forms.ComboBox f3;
        private System.Windows.Forms.ComboBox f2;
    }
}