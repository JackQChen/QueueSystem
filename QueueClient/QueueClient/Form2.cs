using System;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using Model;
using ReportManager;

namespace QueueClient
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            PrintHelper he = new PrintHelper();
            TQueueModel model = new TQueueModel();
            model.unitName = "123";
            model.ticketNumber = "vs";
            he.Print(model, "阳江", 10);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DataTable table = GetQueue("第一大厅", 15);
            //print.Print(model, area, wait);
            PrintManager print = new PrintManager();
            PrintManager.CanDesign = true;
            print.InitReport("排队小票");
            print.AddData(table, "QueueBill");
            print.PreView();
            print.Dispose();
        }
        private DataTable GetQueue(string area, int wait)
        {
            DataTable table = new DataTable("table");
            table.Columns.AddRange(new DataColumn[] 
            {
                new DataColumn ("area",typeof(string)),
                new DataColumn ("waitCount",typeof(string)),
                new DataColumn ("unitName",typeof(string)),
                new DataColumn ("busyName",typeof(string)),
                new DataColumn ("ticketNumber",typeof(string)),
            });
            DataRow row = table.NewRow();
            row["area"] = area;
            row["waitCount"] = wait.ToString();
            row["unitName"] = "公安厅";
            row["busyName"] = "办理身份证";
            row["ticketNumber"] = "CA005";
            table.Rows.Add(row);
            return table;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DataTable table = GetQueue("第一大厅", 15);
            //print.Print(model, area, wait);
            PrintManager print = new PrintManager();
            PrintManager.CanDesign = true;
            print.InitReport("排队小票");
            print.AddData(table, "QueueBill");
            print.Print();
            print.Dispose();
        }
        int iRetUSB = 0;
        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                int iPort;
                for (iPort = 1001; iPort <= 1016; iPort++)
                {
                    iRetUSB = CVRSDK.CVR_InitComm(iPort);
                    if (iRetUSB == 1)
                    {
                        break;
                    }
                }
                if ((iRetUSB == 1))
                {
                    MessageBox.Show("初始化成功！");
                    int authenticate = CVRSDK.CVR_Authenticate();
                    if (authenticate == 1)
                    {
                        int readContent = CVRSDK.CVR_Read_Content(4);
                        if (readContent == 1)
                        {
                            MessageBox.Show("读卡操作成功！");
                            byte[] number = new byte[30];
                            var length = 36;
                            CVRSDK.GetPeopleIDCode(ref number[0], ref length);
                            var id = System.Text.Encoding.GetEncoding("GB2312").GetString(number).Replace("\0", "").Trim();
                            MessageBox.Show(id);
                        }
                        else
                        {
                            MessageBox.Show("读卡操作失败！");
                        }
                    }
                    else
                    {
                        MessageBox.Show("未放卡或卡片放置不正确");
                    }
                }
                else
                {
                    MessageBox.Show("初始化失败！");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            frmMsg msg = new frmMsg();
            msg.msgInfo = "身份证号码输入错误，电话号码也错误，你他妈的全都是错误，你还有脸了！";
            msg.ShowDialog();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            txtCard.Text = "";
            are.Set();
        }
        bool suppend = true;
        AutoResetEvent are = new AutoResetEvent(false);
        private void ReadCard()
        {
            while (true)
            {
                if (suppend)
                {
                    are.WaitOne(-1, false);
                    suppend = false;
                }
                int isHaveCard = CVRSDK.CVR_Authenticate();
                if (isHaveCard == 1)
                {
                    int readOk = CVRSDK.CVR_Read_Content(4);
                    if (readOk == 1)
                    {
                        byte[] number = new byte[30];
                        var length = 36;
                        CVRSDK.GetPeopleIDCode(ref number[0], ref length);
                        var idCard = System.Text.Encoding.GetEncoding("GB2312").GetString(number).Replace("\0", "").Trim();
                        this.Invoke(new Action(() => { txtCard.Text = idCard; }));
                    }
                    else
                    {
                        Thread.Sleep(100);//读卡失败
                    }
                }
                else
                {
                    Thread.Sleep(100);//无卡，继续循环
                }
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            suppend = true;
        }
        Thread thead;
        private void button8_Click(object sender, EventArgs e)
        {
            int iPort;
            for (iPort = 1001; iPort <= 1016; iPort++)
            {
                iRetUSB = CVRSDK.CVR_InitComm(iPort);
                if (iRetUSB == 1)
                {
                    break;
                }
            }
            if (iRetUSB != 1)
            {
                MessageBox.Show("初始化未成功");
            }
            txtCard.Focus();
            thead = new Thread(new ThreadStart(ReadCard));
            thead.Start();
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (thead != null)
                    thead.Abort();
                CVRSDK.CVR_CloseComm();
                Application.ExitThread();
            }
            catch
            {
            }
        }

        private void txtCard_TextChanged(object sender, EventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            txtCard.Text = "";
        }
    }
}
