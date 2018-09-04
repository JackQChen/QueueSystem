using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QueueClient.Properties;

namespace QueueClient
{
    public partial class ucBusyCard : VirtualControl
    {
        public Action<object> action;
        public string unitSeq { get; set; }
        public string busiSeq { get; set; }
        public string busiName { get; set; }
        public Image img;

        public Rectangle ButtonRectangle;
        public event Action ButtonClick;

        public ucBusyCard()
        {
            this.ButtonRectangle = new Rectangle();
            this.ButtonClick += new Action(ucBusyCard_ButtonClick);
        }

        public void OnButtonClick()
        {
            if (ButtonClick != null)
                ButtonClick();
        }

        void ucBusyCard_ButtonClick()
        {
            if (action != null)
                action(this);
        }

        public override void Draw(Graphics g)
        {
            if (img == null)
            {
                try
                {
                    img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "img\\ItemPhoto\\" + unitSeq + "\\" + busiSeq + ".png");
                }
                catch
                {

                }
            }
            g.DrawImage(Resources.长按钮, this.ButtonRectangle.X + 20, this.ButtonRectangle.Y + 20, this.ButtonRectangle.Width, this.ButtonRectangle.Height);
            Font fontMain = new Font("黑体", 24, FontStyle.Bold);
            g.DrawString("取    号", fontMain, new SolidBrush(Color.White), this.ButtonRectangle.X + 70, this.ButtonRectangle.Y + 25);
            if (img != null)
            {
                g.DrawImage(img, this.Rectangle.X, 90, this.Rectangle.Width, this.Rectangle.Height - 90);
            }
        }

        private void pbWorking_Click(object sender, EventArgs e)
        {
            if (action != null)
                action(this);
        }

        private void pbWorking_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void pbWorking_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void pbWorking_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pic = sender as PictureBox;
            Font fontMain = new Font("黑体", 24, FontStyle.Bold);
            e.Graphics.DrawString("取    号", fontMain, new SolidBrush(Color.White), 70, 10);
        }
    }
}
