namespace QueueClient
{
    partial class frmTest
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
            this.pbOther = new System.Windows.Forms.PictureBox();
            this.pbOk = new System.Windows.Forms.PictureBox();
            this.button1 = new System.Windows.Forms.Button();
            this.pbMain = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbOther)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).BeginInit();
            this.SuspendLayout();
            // 
            // pbOther
            // 
            this.pbOther.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOther.Image = global::QueueClient.Properties.Resources.预约提交;
            this.pbOther.Location = new System.Drawing.Point(42, 289);
            this.pbOther.Name = "pbOther";
            this.pbOther.Size = new System.Drawing.Size(238, 66);
            this.pbOther.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOther.TabIndex = 5;
            this.pbOther.TabStop = false;
            this.pbOther.Visible = false;
            this.pbOther.Paint += new System.Windows.Forms.PaintEventHandler(this.pbOk_Paint);
            // 
            // pbOk
            // 
            this.pbOk.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbOk.Image = global::QueueClient.Properties.Resources.预约提交;
            this.pbOk.Location = new System.Drawing.Point(42, 190);
            this.pbOk.Name = "pbOk";
            this.pbOk.Size = new System.Drawing.Size(238, 66);
            this.pbOk.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pbOk.TabIndex = 4;
            this.pbOk.TabStop = false;
            this.pbOk.Visible = false;
            this.pbOk.Paint += new System.Windows.Forms.PaintEventHandler(this.pbOk_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(446, 170);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 6;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // pbMain
            // 
            this.pbMain.BackColor = System.Drawing.Color.Red;
            this.pbMain.Image = global::QueueClient.Properties.Resources.预约未选中;
            this.pbMain.Location = new System.Drawing.Point(8, 8);
            this.pbMain.Name = "pbMain";
            this.pbMain.Size = new System.Drawing.Size(1507, 94);
            this.pbMain.TabIndex = 7;
            this.pbMain.TabStop = false;
            this.pbMain.Paint += new System.Windows.Forms.PaintEventHandler(this.pbMain_Paint);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1524, 443);
            this.Controls.Add(this.pbMain);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.pbOther);
            this.Controls.Add(this.pbOk);
            this.Name = "frmTest";
            this.Text = "frmTest";
            this.Load += new System.EventHandler(this.frmTest_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbOther)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMain)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbOther;
        private System.Windows.Forms.PictureBox pbOk;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.PictureBox pbMain;
    }
}