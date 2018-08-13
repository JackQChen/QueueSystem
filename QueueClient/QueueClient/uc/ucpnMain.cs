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
        public event Action GetCard;
        public event Action Consult;
        public event Action Evaluate;
        public event Action UserGuide;
        public event Action Investment;
        private void pbWork_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pbWork_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void pbInvestment_Click(object sender, EventArgs e)
        {
            if (Investment != null)
                Investment();
        }

        private void pbWork_Click(object sender, EventArgs e)
        {
            if (Work != null)
                Work();
        }

        private void pbGetCard_Click(object sender, EventArgs e)
        {
            if (GetCard != null)
                GetCard();
        }

        private void pbConsult_Click(object sender, EventArgs e)
        {
            if (Work != null)
                Work();
        }

        private void pbEvaluate_Click(object sender, EventArgs e)
        {
            if (Evaluate != null)
                Evaluate();
        }

        private void pbWorkGuide_Click(object sender, EventArgs e)
        {
            if (UserGuide != null)
                UserGuide();
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
                e.Graphics.DrawString("领证", fontMain, new SolidBrush(Color.White), 140, 60);
            }
        }

        private void pbWorking2_Click(object sender, EventArgs e)
        {
            if (GetCard != null)
                GetCard();
        }
    }
}
