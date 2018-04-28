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
            if (Consult != null)
                Consult();
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
            //PictureBox pic = sender as PictureBox;
            //Font fontMain = new Font("黑体", 60, FontStyle.Bold);
            //Font fontS = new Font("黑体", 16, FontStyle.Bold);
            //if (pic.Name == "pbWork")
            //{
            //    e.Graphics.DrawString("办事", fontMain, new SolidBrush(Color.White), 210, 60);
            //}
            //else if (pic.Name == "pbGetCard")
            //{
            //    e.Graphics.DrawString("领证", fontMain, new SolidBrush(Color.White), 210, 60);
            //}
            //else if (pic.Name == "pbConsult")
            //{
            //    e.Graphics.DrawString("咨询", fontMain, new SolidBrush(Color.White), 210, 60);
            //}
            //else if (pic.Name == "pbEvaluate")
            //{
            //    e.Graphics.DrawString("评价", fontS, new SolidBrush(Color.White), 20, 67);
            //}
            //else if (pic.Name == "pbWorkGuide")
            //{
            //    e.Graphics.DrawString("办事指南", fontS, new SolidBrush(Color.White), 2, 67);
            //}
            //else if (pic.Name == "pbInvestment")
            //{
            //    fontS = new Font("黑体", 22, FontStyle.Bold);
            //    e.Graphics.DrawString("投资项目服务专窗", fontS, new SolidBrush(Color.White), 70, 25);
            //}
        }
    }
}
