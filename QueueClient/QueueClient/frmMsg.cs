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
    public partial class frmMsg : Form
    {
        int exitTime = 10;
        int rowLength = 14;
        public string msgInfo;
        public frmMsg()
        {
            InitializeComponent();
            //从指定的位图中获取透明度大于 10 的区域；
            Bitmap img = Resources.弹窗背景;//570, 346   767/472
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

        private void frmMsg_Paint(object sender, PaintEventArgs e)
        {
            Font fontMain = new Font("黑体", 32, FontStyle.Bold);
            Font font = new Font("黑体", 28, FontStyle.Bold);// new Font("微软雅黑", 28, FontStyle.Bold);
            //e.Graphics.DrawString("温馨提示", fontMain, new SolidBrush(Color.White), 270, 20);
            if (msgInfo.Length <= rowLength)
                e.Graphics.DrawString(msgInfo, font, new SolidBrush(Color.Black), 25, 60);
            else
            {
                var firstLine = msgInfo.Substring(0, rowLength);
                e.Graphics.DrawString(firstLine, font, new SolidBrush(Color.Black), 25, 60);
                var remain = msgInfo.Substring(rowLength, msgInfo.Length - rowLength);
                if (remain.Length <= rowLength)
                {
                    e.Graphics.DrawString(remain, font, new SolidBrush(Color.Black), 25, 110);
                }
                else
                {
                    var secondLine = remain.Substring(0, rowLength);
                    e.Graphics.DrawString(secondLine, font, new SolidBrush(Color.Black), 25, 110);
                    var thirdramin = remain.Substring(rowLength, remain.Length - rowLength);
                    if (thirdramin.Length <= rowLength)
                    {
                        e.Graphics.DrawString(thirdramin, font, new SolidBrush(Color.Black), 25, 160);
                    }
                    else
                    {
                        var third = thirdramin.Substring(0, rowLength);
                        e.Graphics.DrawString(third, font, new SolidBrush(Color.Black), 25, 160);
                        var fourremain = thirdramin.Substring(rowLength, thirdramin.Length - rowLength);
                        if (fourremain.Length <= rowLength)
                        {
                            e.Graphics.DrawString(fourremain, font, new SolidBrush(Color.Black), 25, 210);
                        }
                        else
                        {
                            var four = fourremain.Substring(0, rowLength);
                            e.Graphics.DrawString(four, font, new SolidBrush(Color.Black), 25, 210);
                            var last = fourremain.Substring(rowLength, fourremain.Length - rowLength);
                            e.Graphics.DrawString(last, font, new SolidBrush(Color.Black), 25, 260);

                        }
                        
                    }

                }

            }
        }


        private void frmMsg_Load(object sender, EventArgs e)
        {

        }

        private void pbOk_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        SolidBrush brush = new SolidBrush(Color.Black);
        private void pbOk_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 26, FontStyle.Bold);// new Font("微软雅黑", 38, FontStyle.Bold);
            e.Graphics.DrawString("确 定", font, brush, 15, 4);
        }

        private void pbOk_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.弹窗确认按钮2;
            brush = new SolidBrush(Color.White);
        }

        private void pbOk_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.弹窗确认按钮;
            brush = new SolidBrush(Color.Black);
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
