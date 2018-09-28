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
    public class ucUnitContainer : VirtualControlContainer
    {
        private System.Windows.Forms.PictureBox pbPrevious;
        private System.Windows.Forms.PictureBox pbNext;
        public event Action<object> previous;
        public event Action<object> next;
        public List<TUnitModel> uList;
        public event Action SelectBusy;
        VirtualControlContainer pnUnits;
        public TUnitModel selectUnit { get; set; }
        public int pageCount = 30;//
        public int cureentPage = 0;//以配置为准
        string[] units = null;
        string[] pos = null;
        string[] size = null;
        Image[] imgList = null;
        int xpos = 128;
        int ypos = 90;
        public ucUnitContainer()
        {
            this.Height = 1080;
            this.Width = 1920;
            pnUnits = new VirtualControlContainer();
            //pnUnits.BackColor = Color.Transparent;
            pnUnits.Size = new Size(1652, 740);
            pnUnits.Location = new Point(xpos, 90);
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
            this.Controls.Add(pnUnits);
            this.Controls.Add(pbPrevious);
            this.Controls.Add(pbNext);
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
            this.Draw();
        }

        public void CreateUnit()
        {
            this.pnUnits.ClearControl();
            this.pnUnits.BackgroundImage = imgList[cureentPage];
            this.pnUnits.Draw();
            units = System.Configuration.ConfigurationManager.AppSettings["Units"].ToString().Split('|');
            pos = System.Configuration.ConfigurationManager.AppSettings["Position"].ToString().Split('#');
            size = System.Configuration.ConfigurationManager.AppSettings["Size"].ToString().Split('|');
            var uns = units[cureentPage].Split(',');
            var siz = size[cureentPage].Split(',');
            var posi = pos[cureentPage].Split('|');
            this.pnUnits.MouseUp += this.pb_MouseUp;
            for (int i = 0; i < uns.Count(); i++)
            {
                var dep = uList.Where(q => q.unitSeq == uns[i]).FirstOrDefault();
                var width = Convert.ToInt32(siz[i]);
                var posx = Convert.ToInt32(posi[i].Split(',')[0]) + xpos;
                var posy = Convert.ToInt32(posi[i].Split(',')[1]) + ypos;
                UnitCard pb = SetUnit("pb_u_" + (i + 1).ToString(), dep, new Size(width, 75), posx, posy);
                this.pnUnits.AddControl(pb);
            }
            this.pnUnits.Draw();
        }
        UnitCard SetUnit(string name, object tag, Size size, int x, int y)
        {
            UnitCard pb = new UnitCard();
            pb.Name = name;
            pb.Tag = tag;
            pb.Image = Properties.Resources.蓝色_点击前1;
            pb.Rectangle.Size = size;
            pb.Rectangle.Location = new Point(x, y);
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
