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
    public partial class ucpnAppointment : UserControl
    {
        public ucpnAppointment()
        {
            InitializeComponent();
        }
        public event Action other;
        public event Action enter;
        public event Action<object> previous;
        public event Action<object> next;
        public List<TAppointmentModel> appList;
        public int pageCountA = 6;
        public int cureentPageA = 0;
        private void pnPrevious1_Paint(object sender, PaintEventArgs e)
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
            else if (pb.Name == "pbOk")
            {
                font = new Font("黑体", 36, FontStyle.Bold);
                e.Graphics.DrawString("取 票", font, new SolidBrush(Color.Black), 50, 10);
            }
            else if (pb.Name == "pbOther")
            {
                font = new Font("黑体", 36, FontStyle.Bold);
                e.Graphics.DrawString("办理其他业务", font, new SolidBrush(Color.Black), 10, 10);
            }
            else
            {
                font = new Font("黑体", 36, FontStyle.Bold);
                e.Graphics.DrawString("确 认", font, new SolidBrush(Color.Black), 50, 10);
            }
        }
        //动态创建预约列表
        public void CreateAppointment()
        {
            this.pnAppointmentMain.Controls.Clear();
            int count = 0;
            int sX = 30;//起始坐标
            int sY = 46;//起始坐标
            int jj = 4;//间距
            int currY = 0;
            var list = appList.Skip(pageCountA * cureentPageA).Take(pageCountA);
            foreach (var e in list)
            {
                AppointmentCard card = new AppointmentCard();
                card.model = e;
                card.action += UpdateAppointment;
                card.Location = new Point(sX, sY + currY);
                currY += (jj + card.Height);
                pnAppointmentMain.Controls.Add(card);
                count++;
            }
            pnAppointmentMain.ResumeLayout();
        }
        private void UpdateAppointment(TAppointmentModel model)
        {
            var ev = appList.Where(l => l.reserveSeq == model.reserveSeq).ToList().FirstOrDefault();
            if (ev != null)
                ev.isCheck = model.isCheck;
        }
        private void pbOk_Click(object sender, EventArgs e)
        {
            if (enter != null)
                enter();
        }

        private void pbOther_Click(object sender, EventArgs e)
        {
            if (other != null)
                other();
        }

        private void pnPrevious1_Click(object sender, EventArgs e)
        {
            if (previous != null)
                previous(sender); 
        }

        private void pbNext1_Click(object sender, EventArgs e)
        {
            if (next != null)
                next(sender);
        }

        private void pnPrevious1_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.预约下一页点击后;
        }

        private void pnPrevious1_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.预约下一页;
        }

        private void pbOk_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.预约提交_蓝色按钮;
        }

        private void pbOk_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.预约提交;
        }

       
    }
}
