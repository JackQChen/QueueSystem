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
    public partial class ucpnEvaluate : UserControl
    {
        public event Action enter;
        public event Action<BEvaluateModel> enterEvaluate;
        public event Action<object> previous;
        public event Action<object> next;
        public List<BEvaluateModel> eList;
        public int pageCount = 3;//评价数据一页显示3个
        public int cureentPage = 0;//评价数据页码。从1开始

        public ucpnEvaluate()
        {
            InitializeComponent();
        }
        //动态创建可评价列表
        public void CreateEvaluate()
        {
            this.pnEvaluateMain.Controls.Clear();
            int count = 0;
            int sX = 30;//起始坐标
            int sY = 30;//起始坐标
            int jj = 4;//间距
            int currY = 0;
            var list = eList.Skip(pageCount * cureentPage).Take(pageCount);
            foreach (var e in list)
            {
                ucEvaluateCard card = new ucEvaluateCard();
                card.model = e;
                //card.action += UpdateEvaluate;
                card.Location = new Point(sX, sY + currY);
                currY += (jj + card.Height);
                pnEvaluateMain.Controls.Add(card);
                count++;
            }
            pnEvaluateMain.ResumeLayout();
        }
        //委托更新评价标识
        private void UpdateEvaluate(BEvaluateModel model)
        {
            var ev = eList.Where(l => l.controlSeq == model.controlSeq).ToList().FirstOrDefault();
            if (ev != null)
            {
                ev.evaluateResult = model.evaluateResult;
                if (enterEvaluate != null)
                    enterEvaluate(ev);
            }
        }

        private void pnPrevious_Click(object sender, EventArgs e)
        {
            if (previous != null)
                previous(sender);
        }

        private void pbNext_Click(object sender, EventArgs e)
        {
            if (next != null)
                next(sender);
        }

        private void pbSubmit_Click(object sender, EventArgs e)
        {
            if (enter != null)
                enter();
        }

        private void pnPrevious_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            Font font = new Font("黑体", 18, FontStyle.Bold);
            if (pb.Name == "pnPrevious" || pb.Name == "pnPrevious1")
            {
                e.Graphics.DrawString("上一页", font, new SolidBrush(Color.Black), 17, 4);
            }
            else if (pb.Name == "pbNext" || pb.Name == "pbNext1")
            {
                e.Graphics.DrawString("下一页", font, new SolidBrush(Color.Black), 17, 4);
            }
            else if (pb.Name == "pbSubmit")
            {
                font = new Font("黑体", 40, FontStyle.Bold);
                e.Graphics.DrawString("提 交", font, new SolidBrush(Color.Black), 55, 10);
            }
            else
            {
                font = new Font("黑体", 40, FontStyle.Bold);
                e.Graphics.DrawString("确 认", font, new SolidBrush(Color.Black), 55, 10);
            }
        }

        private void pnPrevious_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.下一页点击后;

        }

        private void pnPrevious_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.下一页;

        }
    }
}
