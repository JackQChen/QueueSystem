using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        private void pbOk_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            Font font = new Font("黑体", 18, FontStyle.Bold);
             if (pb.Name == "pbOk")
            {
                font = new Font("黑体", 26, FontStyle.Bold);
                e.Graphics.DrawString("取    票", font, new SolidBrush(Color.Black), 35, 15);
            }
            else if (pb.Name == "pbOther")
            {
                font = new Font("黑体", 26, FontStyle.Bold);
                e.Graphics.DrawString("办理其他业务", font, new SolidBrush(Color.Black), 7, 15);
            }
        }
    }
}
