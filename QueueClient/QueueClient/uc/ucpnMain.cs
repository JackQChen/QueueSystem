using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public partial class ucpnMain : UserControl
    {
        public ucpnMain()
        {
            InitializeComponent();
        }

        public event Action Work;

        private void pbWork_Click(object sender, EventArgs e)
        {
            if (Work != null)
                Work();
        }

        private void pbWorking2_Click(object sender, EventArgs e)
        {
            if (Work != null)
                Work();
        }

        private void pbWork_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            Font fontMain = new Font("黑体", 60, FontStyle.Bold);
            if (pic.Name == "pbWorking")
            {
                e.Graphics.DrawString("办事", fontMain, new SolidBrush(Color.White), 140, 60);
            }
            else if (pic.Name == "pbWorking2")
            {
                e.Graphics.DrawString("咨询", fontMain, new SolidBrush(Color.White), 140, 60);
            }
        }

        
    }
}
