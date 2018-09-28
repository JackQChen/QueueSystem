using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using QueueClient.Properties;
using Model;

namespace QueueClient
{
    public partial class ucBusy : VirtualControl
    {
        public ucBusy()
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
