using System.Windows.Forms;
namespace QueueTest
{
    partial class Form1
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

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.pd1 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new MPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pd1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pd1
            // 
            this.pd1.BackColor = System.Drawing.Color.Transparent;
            this.pd1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pd1.Image = ((System.Drawing.Image)(resources.GetObject("pd1.Image")));
            this.pd1.Location = new System.Drawing.Point(12, 74);
            this.pd1.Name = "pd1";
            this.pd1.Size = new System.Drawing.Size(405, 205);
            this.pd1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pd1.TabIndex = 46;
            this.pd1.TabStop = false;
            this.pd1.Click += new System.EventHandler(this.pd1_Click);
            this.pd1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pd1_MouseDown);
            this.pd1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pd1_MouseUp);
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(479, 74);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(405, 205);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 47;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 385);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.pd1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pd1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pd1;
        private MPictureBox pictureBox1;

    }
}

