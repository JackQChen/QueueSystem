using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;
namespace QueueClient
{
    public partial class EvaluateCard : UserControl
    {
        public EvaluateCard()
        {
            InitializeComponent();
        }
        bool canEdit = true;
        public TEvaluateModel model;
        public Action<TEvaluateModel> action;
        private void pb1_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            if (!canEdit)
            {
                return;
            }
            pb.Image = Properties.Resources.满意蓝色按钮;
            if (pb.Name == "pb1")
            {
                model.evaluateResult = 1;
            }
            else if (pb.Name == "pb2")
            {
                model.evaluateResult = 3;
            }
            else if (pb.Name == "pb3")
            {
                model.evaluateResult = 4;
            }
            canEdit = false;
            if (action != null)
                action(model);
        }

        private void EvaluateCard_Load(object sender, EventArgs e)
        {
            ShowImage(model.evaluateResult);
        }

        private void ShowImage(int result)
        {
            if (result == 0)
            {
                canEdit = true;
                pb1.Image = Properties.Resources.满意灰色按钮;
                pb2.Image = Properties.Resources.满意灰色按钮;
                pb3.Image = Properties.Resources.满意灰色按钮;
            }
            else if (result == 1)//不满意
            {
                canEdit = false;
                pb1.Image = Properties.Resources.满意蓝色按钮;
                pb2.Image = Properties.Resources.满意灰色按钮;
                pb3.Image = Properties.Resources.满意灰色按钮;
            }
            else if (result == 3)//满意
            {
                canEdit = false;
                pb1.Image = Properties.Resources.满意灰色按钮;
                pb2.Image = Properties.Resources.满意蓝色按钮;
                pb3.Image = Properties.Resources.满意灰色按钮;
            }
            else if (result == 4)//非常满意
            {
                canEdit = false;
                pb1.Image = Properties.Resources.满意灰色按钮;
                pb2.Image = Properties.Resources.满意灰色按钮;
                pb3.Image = Properties.Resources.满意蓝色按钮;
            }
        }

        private void pb_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 30, FontStyle.Bold);
            e.Graphics.DrawString(model.approveName, font, new SolidBrush(Color.Black), 100, 30);
        }

        private void pb1_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            Font font = new Font("黑体", 18, FontStyle.Bold);
            SolidBrush sb = new SolidBrush(Color.Black);
            if (pb.Name == "pb1")
            {
                if (model.evaluateResult == 1)
                    sb = new SolidBrush(Color.White);
                e.Graphics.DrawString("不满意", font, sb, 50, 15);
            }
            else if (pb.Name == "pb2")
            {
                if (model.evaluateResult == 3)
                    sb = new SolidBrush(Color.White);
                e.Graphics.DrawString("满  意", font, sb, 55, 15);
            }
            else if (pb.Name == "pb3")
            {
                if (model.evaluateResult == 4)
                    sb = new SolidBrush(Color.White);
                e.Graphics.DrawString("非常满意", font, sb, 35, 15);
            }

        }
    }
}
