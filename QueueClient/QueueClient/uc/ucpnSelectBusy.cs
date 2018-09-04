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
    public partial class ucpnSelectBusy : UserControl
    {
        public ucpnSelectBusy()
        {
            InitializeComponent();
        }
        public event Action<object> previous;
        public event Action<object> next;
        public TBusinessModel selectBusy { get; set; }
        public event Action SelectedBusy;
        public List<TBusinessModel> bList;
        public int pageCount = 30;//一页显示32个
        public int cureentPage = 0;//页码。从1开始
        BusyCard lastCard;
        //动态创建业务
        public void CreateBusiness( )
        {
            this.pnBusiness.ClearControl();
            int rowCount = 6;//一行
            int count = 0;
            //int sX = 25;//起始坐标
            int sY = 25;//起始坐标
            int height = 105;//一行高度
            int width = 206;
            int currY = 0;
            int currX = 25;
            int yGAP = 19;//行间距 
            int xGAP = 81;//列间距
            var list = bList.Skip(pageCount * cureentPage).Take(pageCount);
            this.pnBusiness.MouseUp += this.pb_MouseUp;
            foreach (var u in list)
            {
                BusyCard pb = new BusyCard();
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
                    currX = 25;
                }
                pnBusiness.AddControl(pb);
                count++;
            }
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
        ////业务绘制
        //void pbu_Paint(object sender, PaintEventArgs e)
        //{
        //    PictureBox pb = sender as PictureBox;
        //    TBusinessModel busy = pb.Tag as TBusinessModel;
        //    Font font = new Font("黑体", 22, FontStyle.Bold);
        //    e.Graphics.DrawString(busy.busiName, font, new SolidBrush(Color.White), 10, 7);
        //}

        //按下图标显示
        void pb_MouseDown(object sender, MouseEventArgs e)
        {
            BusyCard pb = sender as BusyCard;
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

    class BusyCard : VirtualControl
    {
        public BusyCard()
        {
        }

        public Image Image;
        public SolidBrush Brush;
        public override void Draw(Graphics g)
        {
            g.DrawImage(Image, Rectangle, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
            TBusinessModel busy = this.Tag as TBusinessModel;
            Font font = new Font("黑体", 18, FontStyle.Bold);
            if (Brush == null)
                Brush = new SolidBrush(Color.Black);
            string busiName = busy.busiName;
            if (busy.busiName.Trim().Length > 21)
                busiName = busiName.Substring(0, 21);
            int rowLength = 7;
            int cX = 10;
            if (busiName.Length <= rowLength)
                g.DrawString(busiName, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 40);//只有一行字，那就居中
            else
            {
                var firstLine = busiName.Substring(0, rowLength);
                var remain = busiName.Substring(rowLength, busiName.Length - rowLength);
                if (remain.Length <= rowLength)
                {
                    g.DrawString(firstLine, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 25);
                    g.DrawString(remain, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 55);
                }
                else
                {
                    g.DrawString(firstLine, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 10);
                    var secondLine = remain.Substring(0, rowLength);
                    g.DrawString(secondLine, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 40);
                    var last = remain.Substring(rowLength, remain.Length - rowLength);
                    g.DrawString(last, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 70);
                }
            }
        }
    }

}
