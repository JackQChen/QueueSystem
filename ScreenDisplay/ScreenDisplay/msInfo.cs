using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenDisplay
{
    public partial class msInfo : UserControl
    {
        public event Action msClick;
        public msInfo()
        {
            InitializeComponent();
        }

        public int Index { get; set; }

        public object Data { get; set; }

        public string QueueNumber
        {
            get
            {
                return lbQNumber.Text;
            }
            set
            {
                lbQNumber.Text = value;
            }
        }

        public string WindowNumber
        {
            get
            {
                return lbWNumber.Text;
            }
            set
            {
                lbWNumber.Text = value;
            }
        }

        public Color TicketColor
        {
            get
            {
                return lbQNumber.ForeColor;
            }
            set
            {
                lbQNumber.ForeColor = value;
            }
        }

        public Color WindowColor
        {
            get
            {
                return lbWNumber.ForeColor;
            }
            set
            {
                lbWNumber.ForeColor = value;
            }
        }

        public Color OtherColor
        {
            get
            {
                return this.label1.ForeColor;
            }
            set
            {
                label1.ForeColor = value;
                label3.ForeColor = value;
                label5.ForeColor = value;
            }
        }

        public string VIPText
        {
            get { return lblVip.Text; }
            set { lblVip.Text = value; }
        }

        public bool VIPVisible
        {
            get
            {
                return lblVip.Visible;
            }
            set
            {
                lblVip.Visible = value;
            }
        }

        public Color VIPColor
        {
            get
            {
                return lblVip.ForeColor;
            }
            set
            {
                lblVip.ForeColor = value;
            }
        }

        public Font VIPFont
        {
            get
            {
                return this.lblVip.Font;
            }
            set
            {
                lblVip.Font = value;
            }
        }

        public Color BackColorPage
        {
            get
            {
                return this.BackColor;
            }
            set
            {
                this.BackColor = value;
            }
        }

        public int RowHeight
        {
            get
            {
                return this.Height;
            }
            set
            {
                this.Height = value;
            }
        }

        public Font TextFont
        {
            get
            {
                return this.lbQNumber.Font;
            }
            set
            {
                label1.Font = value;
                lbQNumber.Font = value;
                label3.Font = value;
                lbWNumber.Font = value;
                label5.Font = value;
            }
        }

        private void msInfo_Click(object sender, EventArgs e)
        {
            if (msClick != null)
                msClick();
        }

    }
}
