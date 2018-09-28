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
    public partial class ucReadCard : VirtualControl
    {
        public string IdCard;
        public Action Input;
        public Rectangle Image1Rectangle;
        public Rectangle Image2Rectangle;
        public Rectangle Image3Rectangle;
        public event Action ButtonClick;

        public ucReadCard()
        {
            this.Image1Rectangle = new Rectangle();
            this.Image2Rectangle = new Rectangle();
            this.Image3Rectangle = new Rectangle();
            this.ButtonClick += new Action(Input_ButtonClick);
        }

        public void OnButtonClick()
        {
            if (ButtonClick != null)
                ButtonClick();
        }

        void Input_ButtonClick()
        {
            if (Input != null)
                Input();
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Resources.刷卡区域, this.Image1Rectangle.X, this.Image1Rectangle.Y, this.Image1Rectangle.Width, this.Image1Rectangle.Height);
            g.DrawImage(Resources.矩形, this.Image2Rectangle.X, this.Image2Rectangle.Y, this.Image2Rectangle.Width, this.Image2Rectangle.Height);
            g.DrawRectangle(new Pen(Color.FromArgb(152, 202, 249), 1), this.Image3Rectangle.X, this.Image3Rectangle.Y, this.Image3Rectangle.Width + 6, this.Image3Rectangle.Height);
            Font font = new Font("黑体", 40, FontStyle.Bold);
            g.DrawString("如未带身份证请点击此按钮", font, new SolidBrush(Color.FromArgb(35, 34, 34)), this.Image2Rectangle.X + 75, this.Image2Rectangle.Y + 15);
            g.DrawString("请将您的身份证靠", font, new SolidBrush(Color.FromArgb(35, 34, 34)), this.Image1Rectangle.X + 185, this.Image1Rectangle.Y + 145);
            g.DrawString("近指定的感应区域", font, new SolidBrush(Color.FromArgb(35, 34, 34)), this.Image1Rectangle.X + 185, this.Image1Rectangle.Y + 245);
            if (!string.IsNullOrEmpty(IdCard))
            {
                Font fc = new Font("黑体", 40, FontStyle.Bold);
                var id = "";
                for (int i = 0; i < IdCard.Length; i++)
                {
                    if (i > 9 && i < 14)
                        id += "*";
                    else
                        id = id + IdCard.Substring(i, 1);
                }
                g.DrawString(id, font, new SolidBrush(Color.FromArgb(35, 34, 34)), this.Image3Rectangle.X + 5, this.Image3Rectangle.Y + 6);
            }
        }
    }
}
