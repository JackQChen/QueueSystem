using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public partial class ucpnPwd : UserControl
    {
        Dictionary<string, SolidBrush> sBrush = new Dictionary<string, SolidBrush>();
        public string InputPwd { get; set; }
        public string ExitPwd { get; set; }
        public event Action Exit;
        public ucpnPwd()
        {
            InitializeComponent();
            sBrush.Add("1", new SolidBrush(Color.White));
            sBrush.Add("2", new SolidBrush(Color.White));
            sBrush.Add("3", new SolidBrush(Color.White));
            sBrush.Add("4", new SolidBrush(Color.White));
            sBrush.Add("5", new SolidBrush(Color.White));
            sBrush.Add("6", new SolidBrush(Color.White));
            sBrush.Add("7", new SolidBrush(Color.White));
            sBrush.Add("8", new SolidBrush(Color.White));
            sBrush.Add("9", new SolidBrush(Color.White));
            sBrush.Add("0", new SolidBrush(Color.White));
            sBrush.Add("f", new SolidBrush(Color.White));
        }

        private void pd1_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮2;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.Black);
        }

        private void pd1_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.White);
        }

        private void pdf_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.确认按钮2;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.Black);
        }

        private void pdf_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.确认按钮;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.White);
        }

        private void pd1_Click(object sender, EventArgs e)
        {
            PictureBox pb = sender as PictureBox;
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

        private void pdf_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            string No = pb.Name.Substring(2, 1);
            Font font = new Font("黑体", 65, FontStyle.Bold);
            if (No == "f")
            {
                e.Graphics.DrawString("确定", font, sBrush[No], 180, 17);
            }
            else
            {
                e.Graphics.DrawString(No, font, sBrush[No], 80, 17);
            }
        }
    }
}
