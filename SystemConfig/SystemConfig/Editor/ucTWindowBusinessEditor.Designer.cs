namespace SystemConfig.Editor
{
    partial class ucTWindowBusinessEditor
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
            this.label1 = new System.Windows.Forms.Label();
            this.numericTextBox1 = new SystemConfig.Editor.Controls.NumericTextBox();
            this.txtWindow = new SystemConfig.Editor.Controls.ValueTextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtUnit = new SystemConfig.Editor.Controls.ValueTextBox();
            this.txtBusi = new SystemConfig.Editor.Controls.ValueTextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sdgUnitGrid = new SystemConfig.Editor.Search.SearchDataGrid();
            this.Column4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sdgBusiGrid = new SystemConfig.Editor.Search.SearchDataGrid();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.sdgUnitGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.sdgBusiGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(30, 105);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 58;
            this.label1.Text = "对应单位";
            // 
            // numericTextBox1
            // 
            this.numericTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.numericTextBox1.Location = new System.Drawing.Point(89, 48);
            this.numericTextBox1.Name = "numericTextBox1";
            this.numericTextBox1.ReadOnly = true;
            this.numericTextBox1.Size = new System.Drawing.Size(238, 21);
            this.numericTextBox1.TabIndex = 57;
            this.numericTextBox1.Tag = "ID";
            this.numericTextBox1.Text = "0";
            // 
            // txtWindow
            // 
            this.txtWindow.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindow.Location = new System.Drawing.Point(89, 75);
            this.txtWindow.Name = "txtWindow";
            this.txtWindow.ReadOnly = true;
            this.txtWindow.Size = new System.Drawing.Size(238, 21);
            this.txtWindow.TabIndex = 56;
            this.txtWindow.Tag = "WindowID";
            this.txtWindow.Value = null;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(30, 78);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 55;
            this.label6.Text = "所属窗口";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(66, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 12);
            this.label5.TabIndex = 54;
            this.label5.Text = "ID";
            // 
            // txtUnit
            // 
            this.txtUnit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUnit.Location = new System.Drawing.Point(89, 102);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(238, 21);
            this.txtUnit.TabIndex = 0;
            this.txtUnit.Tag = "unitSeq";
            this.txtUnit.Value = null;
            // 
            // txtBusi
            // 
            this.txtBusi.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBusi.Location = new System.Drawing.Point(89, 129);
            this.txtBusi.Name = "txtBusi";
            this.txtBusi.Size = new System.Drawing.Size(238, 21);
            this.txtBusi.TabIndex = 1;
            this.txtBusi.Tag = "busiSeq";
            this.txtBusi.Value = null;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 60;
            this.label2.Text = "对应业务";
            // 
            // sdgUnitGrid
            // 
            this.sdgUnitGrid.AllowUserToAddRows = false;
            this.sdgUnitGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sdgUnitGrid.BackgroundColor = System.Drawing.Color.White;
            this.sdgUnitGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sdgUnitGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column4,
            this.Column5,
            this.Column6});
            this.sdgUnitGrid.Location = new System.Drawing.Point(278, 178);
            this.sdgUnitGrid.MultiSelect = false;
            this.sdgUnitGrid.Name = "sdgUnitGrid";
            this.sdgUnitGrid.ReadOnly = true;
            this.sdgUnitGrid.RowTemplate.Height = 23;
            this.sdgUnitGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sdgUnitGrid.Size = new System.Drawing.Size(240, 150);
            this.sdgUnitGrid.TabIndex = 62;
            // 
            // Column4
            // 
            this.Column4.DataPropertyName = "id";
            this.Column4.FillWeight = 80.70423F;
            this.Column4.HeaderText = "单位ID";
            this.Column4.Name = "Column4";
            this.Column4.ReadOnly = true;
            // 
            // Column5
            // 
            this.Column5.DataPropertyName = "unitSeq";
            this.Column5.FillWeight = 60.9137F;
            this.Column5.HeaderText = "单位编码";
            this.Column5.Name = "Column5";
            this.Column5.ReadOnly = true;
            // 
            // Column6
            // 
            this.Column6.DataPropertyName = "unitName";
            this.Column6.FillWeight = 158.3821F;
            this.Column6.HeaderText = "单位名称";
            this.Column6.Name = "Column6";
            this.Column6.ReadOnly = true;
            // 
            // sdgBusiGrid
            // 
            this.sdgBusiGrid.AllowUserToAddRows = false;
            this.sdgBusiGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.sdgBusiGrid.BackgroundColor = System.Drawing.Color.White;
            this.sdgBusiGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.sdgBusiGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3});
            this.sdgBusiGrid.Location = new System.Drawing.Point(32, 211);
            this.sdgBusiGrid.MultiSelect = false;
            this.sdgBusiGrid.Name = "sdgBusiGrid";
            this.sdgBusiGrid.ReadOnly = true;
            this.sdgBusiGrid.RowTemplate.Height = 23;
            this.sdgBusiGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.sdgBusiGrid.Size = new System.Drawing.Size(240, 150);
            this.sdgBusiGrid.TabIndex = 63;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.DataPropertyName = "busiSeq";
            this.dataGridViewTextBoxColumn1.FillWeight = 72F;
            this.dataGridViewTextBoxColumn1.HeaderText = "业务流水号";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.DataPropertyName = "busiCode";
            this.dataGridViewTextBoxColumn2.FillWeight = 62F;
            this.dataGridViewTextBoxColumn2.HeaderText = "业务编码";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.DataPropertyName = "busiName";
            this.dataGridViewTextBoxColumn3.FillWeight = 187.095F;
            this.dataGridViewTextBoxColumn3.HeaderText = "业务名称";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // ucTWindowBusinessEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.sdgBusiGrid);
            this.Controls.Add(this.sdgUnitGrid);
            this.Controls.Add(this.txtBusi);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtUnit);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericTextBox1);
            this.Controls.Add(this.txtWindow);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Name = "ucTWindowBusinessEditor";
            this.Size = new System.Drawing.Size(354, 296);
            this.Tag = "窗口业务对应";
            this.Load += new System.EventHandler(this.ucTWindowBusinessEditor_Load);
            ((System.ComponentModel.ISupportInitialize)(this.sdgUnitGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.sdgBusiGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private Controls.NumericTextBox numericTextBox1;
        private Controls.ValueTextBox txtWindow;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private Controls.ValueTextBox txtUnit;
        private Controls.ValueTextBox txtBusi;
        private System.Windows.Forms.Label label2;
        private Search.SearchDataGrid sdgUnitGrid;
        private Search.SearchDataGrid sdgBusiGrid;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column4;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column6;
    }
}
