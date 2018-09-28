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
    public class ucBusyContainer : VirtualControlContainer
    {
        private System.Windows.Forms.PictureBox pbPrevious;
        private System.Windows.Forms.PictureBox pbNext;
        public event Action<object> previous;
        public event Action<object> next;
        public List<TBusinessModel> bList;
        public event Action SelectedBusy;
        VirtualControlContainer pnBusiness;
        public TBusinessModel selectBusy { get; set; }
        public int pageCount = 30;//
        public int cureentPage = 0;//以配置为准
        int xpos = 128;
        int ypos = 90;
        public ucBusyContainer()
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
            int rowCount = 6;//一行
            int count = 0;
            //int sX = 25;//起始坐标
            int sY = 25;//起始坐标
            int height = 105;//一行高度
            int width = 206;
            int currY = 0;
            int currX = 6;
            int yGAP = 19;//行间距 
            int xGAP = 81;//列间距
            var list = bList.Skip(pageCount * cureentPage).Take(pageCount);
            this.pnBusiness.MouseUp += this.pb_MouseUp;
            foreach (var u in list)
            {
                ucBusy pb = new ucBusy();
                pb.Name = "pb_b_" + count;
                pb.Tag = u;
                pb.MouseEnter += (s, e) =>
                {
                    this.Cursor = Cursors.Hand;
                };
                pb.MouseLeave += (s, e) =>
                {
                    this.Cursor = Cursors.Default;
                };
                pb.Image = Properties.Resources.蓝色_点击前1;
                pb.Rectangle.Size = new Size(width, height);
                pb.MouseClick += pbu_Click;
                pb.MouseDown += pb_MouseDown;
                pb.Rectangle.Location = new Point(currX, currY + sY);
                currX = currX + width + xGAP;
                if (count % rowCount == rowCount - 1)
                {
                    currY += (sY + height + yGAP);
                    currX = 6;
                }
                pnBusiness.AddControl(pb);
                count++;
            }
            pnBusiness.Draw();
        }

        void pbu_Click(object sender, EventArgs e)
        {
            ucBusy pb = sender as ucBusy;
            TBusinessModel busy = pb.Tag as TBusinessModel;
            selectBusy = busy;
            if (SelectedBusy != null)
                SelectedBusy();
        }
        ucBusy lastCard;
        //按下图标显示
        void pb_MouseDown(object sender, MouseEventArgs e)
        {
            ucBusy pb = sender as ucBusy;
            pb.Image = Properties.Resources.银色_点击后1;
            pb.Brush = new SolidBrush(Color.Black);
            pb.Refresh();
            this.lastCard = pb;
        }
        //默认图标显示
        void pb_MouseUp(object sender, MouseEventArgs e)
        {
            if (this.lastCard != null)
            {
                lastCard.Image = Properties.Resources.蓝色_点击前1;
                lastCard.Brush = new SolidBrush(Color.Black);
                lastCard.Refresh();
            }
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
