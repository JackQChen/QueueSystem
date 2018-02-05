using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using QueueClient.Properties;

namespace QueueClient
{
    public partial class frmThankMsg : Form
    {
        int exitTime = 1;
        public string msgInfo;
        public frmThankMsg()
        {
            InitializeComponent();
            //从指定的位图中获取透明度大于 10 的区域；
            Bitmap img = Resources.感谢您的评价;//570, 346   767/472
            GraphicsPath grapth = GetNoneTransparentRegion(img, 10);
            this.Region = new Region(grapth);

            //要显示的图片设置为窗体背景；
            this.BackgroundImage = img;
            this.BackgroundImageLayout = ImageLayout.Stretch;

            //在修改窗体尺寸之前设置窗体为无边框样式；
            this.FormBorderStyle = FormBorderStyle.None;
            this.Width = img.Width;
            this.Height = img.Height;
        }

        #region 无边框移动

        private Point offSet = new Point(), currLocation;
        private bool isMouseDown = false;

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = true;
                offSet = e.Location;
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseDown)
            {
                currLocation = MousePosition;
                currLocation.Offset(-offSet.X, -offSet.Y);
                this.Location = currLocation;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            isMouseDown = false;
        }

        #endregion


        /// <summary>
        /// 返回指定图片中的非透明区域；
        /// </summary>
        /// <param name="img">位图</param>
        /// <param name="alpha">alpha 小于等于该值的为透明</param>
        /// <returns></returns>
        public static GraphicsPath GetNoneTransparentRegion(Bitmap img, byte alpha)
        {
            int height = img.Height;
            int width = img.Width;

            int xStart, xEnd;
            GraphicsPath grpPath = new GraphicsPath();
            for (int y = 0; y < height; y++)
            {
                //逐行扫描；
                for (int x = 0; x < width; x++)
                {
                    //略过连续透明的部分；
                    while (x < width && img.GetPixel(x, y).A <= alpha)
                    {
                        x++;
                    }
                    //不透明部分；
                    xStart = x;
                    while (x < width && img.GetPixel(x, y).A > alpha)
                    {
                        x++;
                    }
                    xEnd = x;
                    if (img.GetPixel(x - 1, y).A > alpha)
                    {
                        grpPath.AddRectangle(new Rectangle(xStart, y, xEnd - xStart, 1));
                    }
                }
            }
            return grpPath;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (exitTime == 0)
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
                exitTime--;
        }
    }
}
