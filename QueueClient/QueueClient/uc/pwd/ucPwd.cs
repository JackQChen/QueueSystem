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
    public partial class ucPwd : VirtualControl
    {
        public ucPwd()
        {
        }
        public Image Image;
        public SolidBrush Brush;
        public override void Draw(Graphics g)
        {
            g.DrawImage(Image, Rectangle, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
            Font font = new Font("黑体", 65, FontStyle.Bold);
            if (Brush == null)
                Brush = new SolidBrush(Color.White);
            string No = this.Name.Substring(2, 1);
            if (No == "f")
            {
                g.DrawString("确定", font, Brush, this.Rectangle.X + 180, this.Rectangle.Y + 17);
            }
            else
            {
                g.DrawString(No, font, Brush, this.Rectangle.X + 80, this.Rectangle.Y + 17);
            }
        }
    }
}
