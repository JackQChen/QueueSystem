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
    public partial class ucpnSelectBusyPhoto : UserControl
    {
        public ucpnSelectBusyPhoto()
        {
            InitializeComponent();
        }
        public event Action<object> previous;
        public event Action<object> next;
        public TBusinessModel selectBusy { get; set; }
        public event Action SelectedBusy;
        public List<TBusinessModel> bList;
        public int pageCount = 4;//一页显示4个
        public int cureentPage = 0;//页码。从1开始
        //动态创建业务
        public void CreateBusiness()
        {
            this.pnBusiness.ClearControl();
            int count = 0;
            int sY = 0;//起始坐标
            int sX = 0;
            var list = bList.Skip(pageCount * cureentPage).Take(pageCount);
            foreach (var u in list)
            {
                ucBusyCard pb = new ucBusyCard();
                pb.Name = "pb_b_" + count;
                pb.Tag = u;
                pb.unitSeq = u.unitSeq;
                pb.busiSeq = u.busiSeq;
                pb.busiName = u.busiName;
                pb.action += new Action<object>(pbu_click);
                pb.Rectangle.Location = new Point(sX, sY);
                pb.Rectangle.Size = new Size(429, 737);
                pb.ButtonRectangle.Location = new Point(sX + 60, sY + 5);
                pb.ButtonRectangle.Size = new Size(280, 50);
                pnBusiness.AddControl(pb);
                count++;
                sX += pb.Rectangle.Width;
            }
            pnBusiness.MouseMove += (s, e) =>
            {
                var ctl = pnBusiness.controls.Find(m => ((ucBusyCard)m).ButtonRectangle.Contains(e.Location)) as ucBusyCard;
                if (ctl != null)
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Default;
            };
            pnBusiness.MouseClick += (s, e) =>
            {
                var ctl = pnBusiness.controls.Find(m => ((ucBusyCard)m).ButtonRectangle.Contains(e.Location)) as ucBusyCard;
                if (ctl != null)
                    ctl.OnButtonClick();
            };
            pnBusiness.Draw();
        }

        void pbu_Click(object sender, EventArgs e)
        {
            BusyCard pb = sender as BusyCard;
            TBusinessModel busy = pb.Tag as TBusinessModel;
            selectBusy = busy;
            if (SelectedBusy != null)
                SelectedBusy();
        }

        void pbu_click(object sender)
        {
            ucBusyCard pb = sender as ucBusyCard;
            TBusinessModel busy = pb.Tag as TBusinessModel;
            selectBusy = busy;
            if (SelectedBusy != null)
                SelectedBusy();
        }

        private void pbPrevious_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.部门下一页_蓝色;
        }

        private void pbPrevious_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.部门下一页;
        }

        private void pbPrevious_Click(object sender, EventArgs e)
        {
            if (previous != null)
                previous(sender);
        }

        private void pbNext_Click(object sender, EventArgs e)
        {
            if (next != null)
                next(sender);
        }

        private void pbPrevious_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            Font font = new Font("黑体", 20, FontStyle.Bold);
            if (pb.Name == "pbPrevious")
            {
                e.Graphics.DrawString("上一页", font, new SolidBrush(Color.Black), 17, 10);
            }
            else if (pb.Name == "pbNext")
            {
                e.Graphics.DrawString("下一页", font, new SolidBrush(Color.Black), 17, 10);
            }
        }
    }
}
