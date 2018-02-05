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
    public partial class ucpnCard : UserControl
    {
        public Action clickAction;
        public ucpnCard()
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
            sBrush.Add("b", new SolidBrush(Color.White));
            sBrush.Add("X", new SolidBrush(Color.White));
            sBrush.Add("f", new SolidBrush(Color.White));
        }
        Dictionary<string, SolidBrush> sBrush = new Dictionary<string, SolidBrush>();
        string _cardId;
        public string CardId
        {
            get { return _cardId; }
            set
            {
                txtCard.Text = value;
                _cardId = value;
            }
        }
        public event Action<string> ProcessIdCard;
        private void pbInputCard_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("请输入本人的身份证号码", font, new SolidBrush(Color.White), 130, 15); //刷身份证或输入身份证号码
        }

        private void pb1_Click(object sender, EventArgs e)
        {
            if (clickAction != null)
                clickAction();
            PictureBox pb = sender as PictureBox;
            string No = pb.Name.Substring(2, 1);
            if (No == "b")
            {
                if (txtCard.Text.Length > 0 && txtCard.Text.Length <= 18)
                {
                    txtCard.Text = txtCard.Text.Substring(0, txtCard.Text.Length - 1);
                    txtCard.SelectionStart = txtCard.Text.Length;
                }
            }
            else if (No == "f")
            {
                //完成
                if (txtCard.Text.Length != 18)
                {
                    frmMsg frm = new frmMsg();//提示
                    frm.msgInfo = "身份证号码格式不正确！";
                    frm.ShowDialog();
                    return;
                }
                else
                {
                    try
                    {
                        var CardIdentity = new CardIdentity(txtCard.Text);
                    }
                    catch
                    {
                        frmMsg frm = new frmMsg();//提示
                        frm.msgInfo = "请输入有效的身份证号码!！";
                        frm.ShowDialog();
                        return;
                    }
                    CardId = txtCard.Text;
                    if (ProcessIdCard != null)
                        ProcessIdCard(CardId);
                }
            }
            else
            {
                if (txtCard.Text.Length < 18)
                {
                    txtCard.Text = txtCard.Text + No;
                    txtCard.SelectionStart = txtCard.Text.Length;
                }
            }
        }

        private void pb1_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            string No = pb.Name.Substring(2, 1);
            Font font = new Font("黑体", 65, FontStyle.Bold);
            if (No == "b")
            {
                e.Graphics.DrawString("退格", font, sBrush[No], 25, 17);
            }
            else if (No == "f")
            {
                e.Graphics.DrawString("确定", font, sBrush[No], 350, 17);
            }
            else
            {
                e.Graphics.DrawString(No, font, sBrush[No], 80, 17);
            }
        }

        private void pbX_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮2;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.Black);
        }

        private void pbX_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.数字按钮;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.White);
        }

        private void pbfinish_MouseDown(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.确认按钮2;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.Black);
        }

        private void pbfinish_MouseUp(object sender, MouseEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            pb.Image = Properties.Resources.确认按钮;
            string No = pb.Name.Substring(2, 1);
            sBrush[No] = new SolidBrush(Color.White);
        }
    }
}
