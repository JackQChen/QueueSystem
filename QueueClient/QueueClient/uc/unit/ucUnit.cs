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
    public partial class ucUnit : VirtualControl
    {
        public ucUnit()
        {
        }

        public Image Image;
        public SolidBrush Brush;
        public override void Draw(Graphics g)
        {
            g.DrawImage(Image, Rectangle, 0, 0, Image.Width, Image.Height, GraphicsUnit.Pixel);
            TUnitModel unit = this.Tag as TUnitModel;
            if (unit == null)
                return;
            Font font = new Font("黑体", 22, FontStyle.Bold);
            if (Brush == null)
                Brush = new SolidBrush(Color.Black);
            string unitName = unit.unitName;
            if (unit.unitName.Trim().Length > 20)
                unitName = unitName.Substring(0, 20);
            int rowLength = 10;
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
            }
        }
    }
}
