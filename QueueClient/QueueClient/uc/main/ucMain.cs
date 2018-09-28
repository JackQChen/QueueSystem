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
    public partial class ucMain : VirtualControl
    {
        public Action Work;
        public Rectangle ImageRectangle;
        public Rectangle Button1Rectangle;
        public Rectangle Button2Rectangle;
        public event Action Button1Click;
        public event Action Button2Click;

        public ucMain()
        {
            this.ImageRectangle = new Rectangle();
            this.Button1Rectangle = new Rectangle();
            this.Button2Rectangle = new Rectangle();
            this.Button1Click += new Action(Work_ButtonClick);
            this.Button2Click += new Action(Work_ButtonClick);
        }
        
        public void OnButton1Click()
        {
            if (Button1Click != null)
                Button1Click();
        }
        public void OnButton2Click()
        {
            if (Button2Click != null)
                Button2Click();
        }

        void Work_ButtonClick()
        {
            if (Work != null)
                Work();
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(Resources.请选择您要办理的类型, this.ImageRectangle.X, this.ImageRectangle.Y, this.ImageRectangle.Width, this.ImageRectangle.Height);
            g.DrawImage(Resources.矩形_16_拷贝_2, this.Button1Rectangle.X, this.Button1Rectangle.Y, this.Button1Rectangle.Width, this.Button1Rectangle.Height);
            g.DrawImage(Resources.矩形_16_拷贝_2, this.Button2Rectangle.X, this.Button2Rectangle.Y, this.Button2Rectangle.Width, this.Button2Rectangle.Height);
            Font fontMain = new Font("黑体", 60, FontStyle.Bold);
            g.DrawString("办事", fontMain, new SolidBrush(Color.White), this.Button1Rectangle.X + 140, this.Button1Rectangle.Y + 60);
            g.DrawString("咨询", fontMain, new SolidBrush(Color.White), this.Button2Rectangle.X + 140, this.Button2Rectangle.Y + 60);
        }
    }
}
