using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;

namespace QueueClient
{
    public partial class ucpnSelectUnit : UserControl
    {

        public ucpnSelectUnit()
        {
            InitializeComponent();

        }

        public event Action<object> previous;
        public event Action<object> next;
        public List<TUnitModel> uList;
        public event Action SelectBusy;
        public TUnitModel selectUnit { get; set; }
        public int pageCount = 30;//一页显示32个
        public int cureentPage = 0;//页码。从1开始
        //动态创建部门
        public void CreateUnit()
        {
            this.pnUnit.ClearControl();
            int rowCount = 6;//一行6个
            int count = 0;
            int sY = 25;//起始坐标
            int height = 105;//一行高度
            int width = 206;
            int currY = 0;
            int currX = 25;
            int yGAP = 19;//行间距 
            int xGAP = 81;//列间距
            this.pnUnit.MouseUp += this.pb_MouseUp;
            var list = uList.Skip(pageCount * cureentPage).Take(pageCount);
            foreach (var u in list)
            {
                UnitCard pb = new UnitCard();
                pb.Name = "pb_u_" + count;
                pb.Tag = u;
                pb.Image = Properties.Resources.蓝色_点击前1;
                pb.Rectangle.Size = new Size(width, height);
                pb.MouseClick += pb_Click;
                pb.MouseDown += pb_MouseDown;
                pb.MouseEnter += (s, e) =>
                {
                    this.Cursor = Cursors.Hand;
                };
                pb.MouseLeave += (s, e) =>
                {
                    this.Cursor = Cursors.Default;
                };
                pb.Rectangle.Location = new Point(currX, currY + sY);
                currX = currX + width + xGAP;
                if (count % rowCount == rowCount - 1)
                {
                    currY += (sY + height + yGAP);
                    currX = 25;
                }
                this.pnUnit.AddControl(pb);
                count++;
            }
            this.pnUnit.Draw();
        }

        //选择部门
        void pb_Click(object sender, EventArgs e)
        {
            UnitCard pb = sender as UnitCard;
            TUnitModel unit = pb.Tag as TUnitModel;
            selectUnit = unit;
            //CreateBusiness();
            if (SelectBusy != null)
                SelectBusy();
        }
        UnitCard lastCard;
        //按下图标显示
        void pb_MouseDown(object sender, EventArgs e)
        {
            UnitCard pb = sender as UnitCard;
            pb.Image = Properties.Resources.银色_点击后1;
            pb.Brush =  new SolidBrush(Color.Black);
            pb.Refresh();
            this.lastCard = pb;
        }
        //默认图标显示
        void pb_MouseUp(object sender, EventArgs e)
        {
            if (this.lastCard != null)
            {
                lastCard.Image = Properties.Resources.蓝色_点击前1;
                lastCard.Brush = new SolidBrush(Color.Black);
                lastCard.Refresh();
            }
        }

        private void ucpnSelectUnit_Load(object sender, EventArgs e)
        {

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
