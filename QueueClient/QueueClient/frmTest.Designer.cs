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
            ((System.ComponentModel.ISupportInitialize)(this.pbOther)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).BeginInit();
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
            this.pbOk.Paint += new System.Windows.Forms.PaintEventHandler(this.pbOk_Paint);
            // 
            // frmTest
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 443);
            this.Controls.Add(this.pbOther);
            this.Controls.Add(this.pbOk);
            this.Name = "frmTest";
            this.Text = "frmTest";
            ((System.ComponentModel.ISupportInitialize)(this.pbOther)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOk)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pbOther;
        private System.Windows.Forms.PictureBox pbOk;
    }
}