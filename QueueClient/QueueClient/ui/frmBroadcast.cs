using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QueueClient
{
    public partial class frmBroadcast : Form
    {
        public frmBroadcast()
        {
            InitializeComponent();
        }
        int interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["BroadcastInterval"]);//轮播间隔
        int startImg = 1;
        private void frmBroadcast_Load(object sender, EventArgs e)
        {
            Image img = null;
            try
            {
                img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "img\\Broadcast\\" + startImg.ToString() + ".jpeg");
            }
            catch
            {

            }
            if (img != null)
            {
                pbMain.Image = img;
                startImg++;
            }
            else
            {
                if (startImg == 1)
                {
                    this.Close();
                }
                else
                {
                    startImg = 1;
                }
            }

            tm.Interval = interval * 1000;
            tm.Enabled = true;
            tm.Start();
        }

        private void tm_Tick(object sender, EventArgs e)
        {
            Image img = null;
            try
            {
                img = Image.FromFile(AppDomain.CurrentDomain.BaseDirectory + "img\\Broadcast\\" + startImg.ToString() + ".jpeg");
                if (img != null)
                {
                    pbMain.Image = img;
                    startImg++;
                }
                else
                {
                    startImg = 1;
                    tm_Tick(sender, e);
                }
            }
            catch
            {
                startImg = 1;
                tm_Tick(sender, e);
            }
        }

        private void pbMain_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
