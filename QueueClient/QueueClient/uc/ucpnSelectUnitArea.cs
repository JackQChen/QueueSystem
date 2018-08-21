using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;
using System.IO;

namespace QueueClient
{
    public partial class ucpnSelectUnitArea : UserControl
    {

        public ucpnSelectUnitArea()
        {
            InitializeComponent();

        }

        public event Action<object> previous;
        public event Action<object> next;
        public List<TUnitModel> uList;
        public event Action SelectBusy;
        public TUnitModel selectUnit { get; set; }
        public int pageCount = 30;//
        public int cureentPage = 0;//目前按照固定2个页面 。如果页码为0 加载图1 ，页面为2加载图2
        string[] units1 = new string[] { "1", "2", "3", "4" };//页面1固定部门编号 
        string[] units2 = new string[] { "5", "6", "", "" };//页面2固定部门编号 
        Image page1Img = null;//页面1图
        Image page2Img = null;//页面2图
        Image[] imgList = null;
        //动态创建部门
        public void CreateUnit()
        {
            this.pnUnit.ClearControl();
            pnUnit.BackgroundImage = imgList[cureentPage];
            if (cureentPage == 0)
            {
                pnUnit.MouseUp += this.pb_MouseUp;
                var u1 = uList.Where(u => u.unitSeq == units1[0]).FirstOrDefault();
                var u2 = uList.Where(u => u.unitSeq == units1[1]).FirstOrDefault();
                var u3 = uList.Where(u => u.unitSeq == units1[2]).FirstOrDefault();
                var u4 = uList.Where(u => u.unitSeq == units1[3]).FirstOrDefault();
                Size size1 = new Size(300, 75);
                Size size2 = new Size(237, 75);
                Size size3 = new Size(324, 75);
                Size size4 = new Size(440, 75);
                UnitCard pb1 = SetUnit("pb_u_1", u1, size1);
                UnitCard pb2 = SetUnit("pb_u_2", u2, size2);
                UnitCard pb3 = SetUnit("pb_u_3", u3, size3);
                UnitCard pb4 = SetUnit("pb_u_4", u4, size4);
                pb1.Rectangle.Location = new Point(390, 31);
                pb2.Rectangle.Location = new Point(208, 119);
                pb3.Rectangle.Location = new Point(646, 137);
                pb4.Rectangle.Location = new Point(764, 326);
                this.pnUnit.AddControl(pb1);
                this.pnUnit.AddControl(pb2);
                this.pnUnit.AddControl(pb3);
                this.pnUnit.AddControl(pb4);

            }
            else if (cureentPage == 1)
            {

            }
            else if (cureentPage == 2)
            {

            }
            this.pnUnit.Draw();
        }

        UnitCard SetUnit(string name, object tag, Size size)
        {
            UnitCard pb = new UnitCard();
            pb.Name = name;
            pb.Tag = tag;
            pb.Image = Properties.Resources.蓝色_点击前1;
            pb.Rectangle.Size = size;
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
            return pb;
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
            pb.Brush = new SolidBrush(Color.Black);
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
            try
            {
                string path = AppDomain.CurrentDomain.BaseDirectory + "img\\FloorDistribution";
                DirectoryInfo root = new DirectoryInfo(path);
                FileInfo[] files = root.GetFiles();
                imgList = new Image[files.Count()];
                int index = 0;
                foreach (var f in files)
                {
                    Image img = Image.FromFile(f.FullName);
                    if (img != null)
                    {
                        imgList[index] = img;
                        index++;
                    }
                }
            }
            catch
            {

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
    class UnitCard : VirtualControl
    {
        public UnitCard()
        {
        }

        public Image Image;
        public SolidBrush Brush;
        public override void Draw(Graphics g)
        {
            g.DrawImage(Image, Rectangle, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
            TUnitModel unit = this.Tag as TUnitModel;
            Font font = new Font("黑体", 22, FontStyle.Bold);
            if (Brush == null)
                Brush = new SolidBrush(Color.Black);
            string unitName = unit.unitName;
            if (unit.unitName.Trim().Length > 8)
                unitName = unitName.Substring(0, 8);
            int rowLength = 4;
            int cX = 23;
            if (unitName.Length <= rowLength)
                g.DrawString(unitName, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 20);//只有三个字，那就居中
            else
            {
                var firstLine = unitName.Substring(0, rowLength);
                var remain = unitName.Substring(rowLength, unitName.Length - rowLength);
                if (remain.Length <= rowLength)
                {
                    g.DrawString(firstLine, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 20);
                    g.DrawString(remain, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 55);
                }
                //else
                //{
                //    g.DrawString(firstLine, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 13);
                //    var secondLine = remain.Substring(0, rowLength);
                //    g.DrawString(secondLine, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 58);
                //    var last = remain.Substring(rowLength, remain.Length - rowLength);
                //    g.DrawString(last, font, Brush, this.Rectangle.X + cX, this.Rectangle.Y + 103);
                //}

            }
        }
    }
}
