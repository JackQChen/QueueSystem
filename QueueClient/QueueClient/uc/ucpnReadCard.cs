using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace QueueClient
{
    public partial class ucpnReadCard : UserControl
    {
        public ucpnReadCard()
        {
            InitializeComponent();
        }
        Color col = Color.FromArgb(35, 34, 34);
        public Panel pnCard { get { return pnWriterCard; } set { pnWriterCard = value; } }
        public string IdCard { get; set; }
        public event Action GotoInput;
        private void pbReadCard_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("请将您的身份证靠", font, new SolidBrush(col), 185, 145);
            e.Graphics.DrawString("近指定的感应区域", font, new SolidBrush(col), 185, 245);
        }

        private void pnWriterCard_Paint(object sender, PaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(IdCard))
            {
                Font font = new Font("黑体", 40, FontStyle.Bold);
                var id = "";
                for (int i = 0; i < IdCard.Length; i++)
                {
                    if (i > 9 && i < 14)
                        id += "*";
                    else
                        id = id + IdCard.Substring(i, 1);
                }
                e.Graphics.DrawString(id, font, new SolidBrush(col), 5, 6);
            }
        }

        private void pbGotoInput_Paint(object sender, PaintEventArgs e)
        {
            Font font = new Font("黑体", 40, FontStyle.Bold);
            e.Graphics.DrawString("如未带身份证请点击此按钮", font, new SolidBrush(col), 75, 15);
        }

        private void pbGotoInput_Click(object sender, EventArgs e)
        {
            this.pbGotoInput.Enabled = false;
            if (GotoInput != null)
                GotoInput();
            new Thread(() =>
            {
                int i = 0;
                while (true)
                {
                    if (i > 4)
                    {
                        BeginInvoke(new Action(() => { this.pbGotoInput.Enabled = true; }));
                        break;
                    }
                    i++;
                    Thread.Sleep(200);
                }
            }) { IsBackground = true }.Start();
        }
    }
}
