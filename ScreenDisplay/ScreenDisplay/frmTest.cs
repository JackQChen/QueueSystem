using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ScreenDisplay
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(lblVip.Text);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            lblVip.Text = "网上\r\n预约";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            lblVip.Text = "网上\r预约";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            lblVip.Text = "网上\n预约";
        }
    }
}
