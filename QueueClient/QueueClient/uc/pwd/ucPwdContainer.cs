using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public class ucPwdContainer : VirtualControlContainer
    {
        public string InputPwd { get; set; }
        public string ExitPwd { get; set; }
        public event Action Exit;
        public ucPwdContainer()
        {
            this.ClearControl();
            this.Height = 1080;
            this.Width = 1920;
            var nwidth = 254;
            var nheight = 115;//数字按钮的高度和宽度
            Size size = new Size(nwidth, nheight);
            var xpos = 330;
            var ypos = 135;
            var xp = 1 + 510;//其中510为x偏移量
            var yp = 145;
            var ydif = 22;//高度差 调整
            ucPwd pb1 = SetId("pb1", 1, size, xp, yp + ydif);
            ucPwd pb2 = SetId("pb2", 1, size, xp + xpos, yp + ydif);
            ucPwd pb3 = SetId("pb3", 1, size, xp + xpos + xpos, yp + ydif);
            ucPwd pb4 = SetId("pb4", 1, size, xp, yp + ypos + ydif);
            ucPwd pb5 = SetId("pb5", 1, size, xp + xpos, yp + ypos + ydif);
            ucPwd pb6 = SetId("pb6", 1, size, xp + xpos + xpos, yp + ypos + ydif);
            ucPwd pb7 = SetId("pb7", 1, size, xp, yp + ypos + ypos + ydif);
            ucPwd pb8 = SetId("pb8", 1, size, xp + xpos, yp + ypos + ypos + ydif);
            ucPwd pb9 = SetId("pb9", 1, size, xp + xpos + xpos, yp + ypos + ypos + ydif);
            ucPwd pb0 = SetId("pb0", 1, size, xp, yp + ypos + ypos + ypos + ydif);
            ucPwd pbfinish = SetId("pbfinish", 2, new Size(575, 118), xp + xpos, yp + ypos + ypos + ypos + ydif);
            this.AddControl(pb1);
            this.AddControl(pb2);
            this.AddControl(pb3);
            this.AddControl(pb4);
            this.AddControl(pb5);
            this.AddControl(pb6);
            this.AddControl(pb7);
            this.AddControl(pb8);
            this.AddControl(pb9);
            this.AddControl(pb0);
            this.AddControl(pbfinish);
            this.Draw();
        }

        ucPwd SetId(string name, int type, Size size, int x, int y)
        {
            ucPwd pb = new ucPwd();
            pb.Name = name;
            if (type == 1)
                pb.Image = Properties.Resources.数字按钮;
            else if (type == 2)
                pb.Image = Properties.Resources.确认按钮;
            pb.Rectangle.Size = size;
            pb.Rectangle.Location = new Point(x, y);
            pb.MouseClick += pb_Click;
            //pb.MouseDown += pb_MouseDown;
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
        private void pb_Click(object sender, EventArgs e)
        {
            ucPwd pb = sender as ucPwd;
            string No = pb.Name.Substring(2, 1);
            if (No == "f")
            {
                if (InputPwd != ExitPwd)
                {
                    InputPwd = "";
                    frmMsg frm = new frmMsg();
                    frm.msgInfo = "退出密码不正确！";
                    frm.ShowDialog();
                }
                else
                {
                    if (Exit != null)
                        Exit();
                }
            }
            else
            {
                InputPwd += No;
            }
        }

    }

}
