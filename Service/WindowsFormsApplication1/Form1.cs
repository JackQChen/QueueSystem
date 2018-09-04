using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DAL;
using BLL;
using System.Runtime.Remoting;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        TUserBLL bll = new TUserBLL();
        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < 50; i++)
            {
                new Thread(() =>
                {
                    var max = bll.GetMaxId();
                    dic.Add(max, max);
                    this.Invoke(new Action(() => { this.richTextBox1.Text += max + "\r\n"; }));
                }) { IsBackground = true }.Start();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            TWindowBLL dal = new TWindowBLL();
            for (int i = 0; i < 100; i++)
            {
                new Thread(() =>
                {
                    var id = dal.GetMaxId();
                    Thread.Sleep(5000);
                    this.Invoke(new Action(() => { this.richTextBox1.Text += id + "\r\n"; }));
                }) { IsBackground = true }.Start();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var remotingConfigPath = AppDomain.CurrentDomain.BaseDirectory + "Service.xml";
            RemotingConfiguration.Configure(remotingConfigPath, false);
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
        }
    }
}
