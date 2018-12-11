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
using System.Web.Script.Serialization;

namespace ServiceTest
{
    public partial class MainForm : Form
    {
        public MainForm()
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

        private void button4_Click(object sender, EventArgs e)
        {
            BEvaluateBLL eBll = new BEvaluateBLL();
            var obj = eBll.GetSatisfied(2, DateTime.Parse("2017-01-01"), DateTime.Now);
            var objx = eBll.GetSatisfied(1, DateTime.Parse("2017-01-01"), DateTime.Now);

            BCallBLL cBll = new BCallBLL();
            var obj2 = cBll.GetWaitFor(DateTime.Parse("2017-01-01"), DateTime.Now);

            var obj3 = eBll.GetEvaluate(DateTime.Parse("2017-01-01"), DateTime.Now);
            var obj4 = eBll.GetFavorableComment(DateTime.Parse("2017-01-01"), DateTime.Now);

            var obj5 = eBll.GetWorkPercent(1, DateTime.Parse("2018-11-01"), DateTime.Parse("2018-11-11"), null, null);
            this.richTextBox1.Text = new JavaScriptSerializer().Serialize(obj5);
        }
    }
}
