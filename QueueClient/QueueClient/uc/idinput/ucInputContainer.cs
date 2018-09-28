using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public class ucInputContainer : VirtualControlContainer
    {
        public Action clickAction;
        TextBox txtCard;
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
        public ucInputContainer()
        {
            this.ClearControl();
            this.Height = 1080;
            this.Width = 1920;
            txtCard = new TextBox();
            txtCard.BackColor = System.Drawing.SystemColors.Window;
            txtCard.Font = new System.Drawing.Font("微软雅黑", 60F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            txtCard.ForeColor = System.Drawing.Color.SeaGreen;
            txtCard.Location = new System.Drawing.Point(508, 126);
            txtCard.MaxLength = 18;
            txtCard.Name = "txtCard";
            txtCard.Size = new System.Drawing.Size(903, 113);
            txtCard.TabIndex = 34;
            var nwidth = 254;
            var nheight = 115;//数字按钮的高度和宽度
            Size size = new Size(nwidth, nheight);
            var xpos = 330;
            var ypos = 135;
            var xp = 1 + 507;//其中507为x偏移量
            var yp = 226;
            var ydif = 22;//高度差 调整
            ucInput pb = SetId("pbInputCard", 3, new Size(903, 90), xp, 1 + ydif);
            ucInput pb1 = SetId("pb11", 1, size, xp, yp + ydif);
            ucInput pb2 = SetId("pb21", 1, size, xp + xpos, yp + ydif);
            ucInput pb3 = SetId("pb31", 1, size, xp + xpos + xpos, yp + ydif);
            ucInput pb4 = SetId("pb41", 1, size, xp, yp + ypos + ydif);
            ucInput pb5 = SetId("pb5", 1, size, xp + xpos, yp + ypos + ydif);
            ucInput pb6 = SetId("pb61", 1, size, xp + xpos + xpos, yp + ypos + ydif);
            ucInput pb7 = SetId("pb71", 1, size, xp, yp + ypos + ypos + ydif);
            ucInput pb8 = SetId("pb81", 1, size, xp + xpos, yp + ypos + ypos + ydif);
            ucInput pb9 = SetId("pb91", 1, size, xp + xpos + xpos, yp + ypos + ypos + ydif);
            ucInput pbX = SetId("pbX1", 1, size, xp, yp + ypos + ypos + ypos + ydif);
            ucInput pb0 = SetId("pb01", 1, size, xp + xpos, yp + ypos + ypos + ypos + ydif);
            ucInput pbback = SetId("pbback1", 1, size, xp + xpos + xpos, yp + ypos + ypos + ypos + ydif);
            ucInput pbfinish = SetId("pbfinish1", 2, new Size(903, 115), xp, yp + ypos + ypos + ypos + ypos + ydif);
            this.Controls.Add(txtCard);
            this.AddControl(pb);
            this.AddControl(pb1);
            this.AddControl(pb2);
            this.AddControl(pb3);
            this.AddControl(pb4);
            this.AddControl(pb5);
            this.AddControl(pb6);
            this.AddControl(pb7);
            this.AddControl(pb8);
            this.AddControl(pb9);
            this.AddControl(pbX);
            this.AddControl(pb0);
            this.AddControl(pbback);
            this.AddControl(pbfinish);
        }

        ucInput SetId(string name, int type, Size size, int x, int y)
        {
            ucInput pb = new ucInput();
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
            ucInput pb = sender as ucInput;
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

    }

}
