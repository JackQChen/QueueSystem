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
    public partial class ucInput : VirtualControl
    {
        public ucInput()
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
            if (No == "b")
            {
                g.DrawString("退格", font, Brush, this.Rectangle.X + 25, this.Rectangle.Y + 17);
            }
            else if (No == "f")
            {
                g.DrawString("确定", font, Brush, this.Rectangle.X + 350, this.Rectangle.Y + 17);
            }
            else if (No == "I")
            {
                Font nfont = new Font("黑体", 40, FontStyle.Bold);
                g.DrawString("请输入本人的身份证号码", nfont, new SolidBrush(Color.White), this.Rectangle.X + 130, this.Rectangle.Y + 15); //刷身份证或输入身份证号码
            }
            else
            {
                g.DrawString(No, font, Brush, this.Rectangle.X + 80, this.Rectangle.Y + 17);
            }
        }
    }
}
