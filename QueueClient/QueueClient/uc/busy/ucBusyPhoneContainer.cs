using Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public class ucBusyPhoneContainer : VirtualControlContainer
    {
        private System.Windows.Forms.PictureBox pbPrevious;
        private System.Windows.Forms.PictureBox pbNext;
        public event Action<object> previous;
        public event Action<object> next;
        public List<TBusinessModel> bList;
        public event Action SelectedBusy;
        VirtualControlContainer pnBusiness;
        public TBusinessModel selectBusy { get; set; }
        public int pageCount = 4;//
        public int cureentPage = 0;//以配置为准
        int xpos = 128;
        int ypos = 90;
        public ucBusyPhoneContainer()
        {
            this.Height = 1080;
            this.Width = 1920;
            pnBusiness = new VirtualControlContainer();
            pnBusiness.BackColor = Color.Transparent;
            pnBusiness.Size = new Size(1652, 740);
            pnBusiness.Location = new Point(xpos, 90);
            pnBusiness.BackgroundImageLayout = ImageLayout.Stretch;
            // 
            // pbPrevious
            // 
            this.pbPrevious = new PictureBox();
            this.pbPrevious.BackColor = Color.Transparent;
            this.pbPrevious.Image = global::QueueClient.Properties.Resources.部门下一页;
            this.pbPrevious.Location = new System.Drawing.Point(131, 831);
            this.pbPrevious.Name = "pbPrevious";
            this.pbPrevious.Size = new System.Drawing.Size(144, 52);
            this.pbPrevious.TabIndex = 24;
            this.pbPrevious.TabStop = false;
            this.pbPrevious.Click += new System.EventHandler(this.pbPrevious_Click);
            this.pbPrevious.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPrevious_Paint);
            this.pbPrevious.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseDown);
            this.pbPrevious.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseUp);
            // 
            // pbNext
            // 
            this.pbNext = new PictureBox();
            this.pbNext.BackColor = Color.Transparent;
            this.pbNext.Image = global::QueueClient.Properties.Resources.部门下一页;
            this.pbNext.Location = new System.Drawing.Point(1630, 831);
            this.pbNext.Name = "pbNext";
            this.pbNext.Size = new System.Drawing.Size(144, 52);
            this.pbNext.TabIndex = 25;
            this.pbNext.TabStop = false;
            this.pbNext.Click += new System.EventHandler(this.pbNext_Click);
            this.pbNext.Paint += new System.Windows.Forms.PaintEventHandler(this.pbPrevious_Paint);
            this.pbNext.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseDown);
            this.pbNext.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pbPrevious_MouseUp);
            this.Controls.Add(pnBusiness);
            this.Controls.Add(pbPrevious);
            this.Controls.Add(pbNext);
            this.Draw();
        }

        public void CreateBusiness()
        {
            this.pnBusiness.ClearControl();
            int count = 0;
            int sY = 0;//起始坐标
            int sX = 0;
            var list = bList.Skip(pageCount * cureentPage).Take(pageCount);
            foreach (var u in list)
            {
                ucBusyPhone pb = new ucBusyPhone();
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
                var ctl = pnBusiness.controls.Find(m => ((ucBusyPhone)m).ButtonRectangle.Contains(e.Location)) as ucBusyPhone;
                if (ctl != null)
                    this.Cursor = Cursors.Hand;
                else
                    this.Cursor = Cursors.Default;
            };
            pnBusiness.MouseClick += (s, e) =>
            {
                var ctl = pnBusiness.controls.Find(m => ((ucBusyPhone)m).ButtonRectangle.Contains(e.Location)) as ucBusyPhone;
                if (ctl != null)
                    ctl.OnButtonClick();
            };
            pnBusiness.Draw();
        }
        void pbu_click(object sender)
        {
            ucBusyPhone pb = sender as ucBusyPhone;
            TBusinessModel busy = pb.Tag as TBusinessModel;
            selectBusy = busy;
            if (SelectedBusy != null)
                SelectedBusy();
        }
        
        #region  下一页 上一页
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

        #endregion
    }

}
