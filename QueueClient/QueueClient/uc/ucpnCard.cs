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
        }

        public void CreateIdCard()
        {
            this.pnNumber.ClearControl();
            //pnNumber.MouseUp += this.pb_MouseUp;
            var nwidth = 254;
            var nheight = 115;//数字按钮的高度和宽度
            Size size = new Size(nwidth, nheight);
            var xpos = 330;
            var ypos = 135;
            var xp = 1;
            var yp = 226;
            IdCard pb = SetId("pbInputCard", 3, new Size(903, 90), xp, 1);
            IdCard pb1 = SetId("pb11", 1, size, xp, yp);
            IdCard pb2 = SetId("pb21", 1, size, xp + xpos, yp);
            IdCard pb3 = SetId("pb31", 1, size, xp + xpos + xpos, yp);
            IdCard pb4 = SetId("pb41", 1, size, xp, yp + ypos);
            IdCard pb5 = SetId("pb5", 1, size, xp + xpos, yp + ypos);
            IdCard pb6 = SetId("pb61", 1, size, xp + xpos + xpos, yp + ypos);
            IdCard pb7 = SetId("pb71", 1, size, xp, yp + ypos + ypos);
            IdCard pb8 = SetId("pb81", 1, size, xp + xpos, yp + ypos + ypos);
            IdCard pb9 = SetId("pb91", 1, size, xp + xpos + xpos, yp + ypos + ypos);
            IdCard pbX = SetId("pbX1", 1, size, xp, yp + ypos + ypos + ypos);
            IdCard pb0 = SetId("pb01", 1, size, xp + xpos, yp + ypos + ypos + ypos);
            IdCard pbback = SetId("pbback1", 1, size, xp + xpos + xpos, yp + ypos + ypos + ypos);
            IdCard pbfinish = SetId("pbfinish1", 2, new Size(903, 115), xp, yp + ypos + ypos + ypos + ypos);
            this.pnNumber.AddControl(pb);
            this.pnNumber.AddControl(pb1);
            this.pnNumber.AddControl(pb2);
            this.pnNumber.AddControl(pb3);
            this.pnNumber.AddControl(pb4);
            this.pnNumber.AddControl(pb5);
            this.pnNumber.AddControl(pb6);
            this.pnNumber.AddControl(pb7);
            this.pnNumber.AddControl(pb8);
            this.pnNumber.AddControl(pb9);
            this.pnNumber.AddControl(pbX);
            this.pnNumber.AddControl(pb0);
            this.pnNumber.AddControl(pbback);
            this.pnNumber.AddControl(pbfinish);
            this.pnNumber.Draw();
        }

        IdCard SetId(string name, int type, Size size, int x, int y)
        {
            IdCard pb = new IdCard();
            pb.Name = name;
            if (type == 1)
                pb.Image = Properties.Resources.数字按钮;
            else if (type == 2)
                pb.Image = Properties.Resources.确认按钮;
            else if (type == 3)
                pb.Image = Properties.Resources.请输入身份证号码;
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
            if (clickAction != null)
                clickAction();
            IdCard pb = sender as IdCard;
            string No = pb.Name.Substring(2, 1);
            if (No == "b")
            {
                if (txtCard.Text.Length > 0 && txtCard.Text.Length <= 18)
                {
                    int cle = txtCard.SelectionStart;
                    if (cle < 1)
                        return;
                    if (cle == txtCard.Text.Length)
                        txtCard.Text = txtCard.Text.Substring(0, txtCard.Text.Length - 1);
                    else
                        txtCard.Text = txtCard.Text.Substring(0, cle - 1) + txtCard.Text.Substring(cle, txtCard.Text.Length - cle);
                    txtCard.SelectionStart = cle - 1;
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
                if (No == "I")
                {
                    return;
                }
                if (txtCard.Text.Length < 18)
                {
                    var selection = txtCard.SelectionStart;
                    var up = txtCard.Text.Substring(0, txtCard.SelectionStart);
                    var dowm = txtCard.Text.Substring(txtCard.SelectionStart, txtCard.Text.Length - txtCard.SelectionStart);
                    txtCard.Text = up + No + dowm;
                    txtCard.SelectionStart = selection + 1;
                }
            }
        }
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
    }

    class IdCard : VirtualControl
    {
        public IdCard()
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
