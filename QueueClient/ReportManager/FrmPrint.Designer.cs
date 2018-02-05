using System;
using FastReport;
namespace ReportManager
{
    partial class FrmPreview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPreview));
            this.preview = new PreviewControlEx();
            this.SuspendLayout();
            // 
            // preview
            // 
            this.preview.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.preview.Buttons = ((FastReport.PreviewButtons)((((((FastReport.PreviewButtons.Print | FastReport.PreviewButtons.Save)
                        | FastReport.PreviewButtons.Zoom)
                        | FastReport.PreviewButtons.Outline)
                        | FastReport.PreviewButtons.Navigator)
                        | FastReport.PreviewButtons.Close)));
            this.preview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.preview.Font = new System.Drawing.Font("宋体", 9F);
            this.preview.Location = new System.Drawing.Point(0, 0);
            this.preview.Name = "preview";
            this.preview.PageOffset = new System.Drawing.Point(10, 10); 
            this.preview.Size = new System.Drawing.Size(710, 524);
            this.preview.TabIndex = 1;
            this.preview.UIStyle = FastReport.Utils.UIStyle.VisualStudio2005;
            // 
            // FrmPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(710, 524);
            this.Controls.Add(this.preview);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmPrint";
            this.ShowInTaskbar = false;
            this.Text = "打印预览";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.ResumeLayout(false);

        }

        #endregion

        private PreviewControlEx preview;
    }
}