using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;
using BLL;

namespace QueueClient
{
    public partial class frmTest : Form
    {
        public frmTest()
        {
            InitializeComponent();
        }

        private void pbOk_Paint(object sender, PaintEventArgs e)
        {
            PictureBox pb = sender as PictureBox;
            Font font = new Font("黑体", 18, FontStyle.Bold);
            if (pb.Name == "pbOk")
            {
                font = new Font("黑体", 26, FontStyle.Bold);
                e.Graphics.DrawString("取    票", font, new SolidBrush(Color.Black), 35, 15);
            }
            else if (pb.Name == "pbOther")
            {
                font = new Font("黑体", 26, FontStyle.Bold);
                e.Graphics.DrawString("办理其他业务", font, new SolidBrush(Color.Black), 7, 15);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            selectUnit = new TUnitModel()
            {
                unitSeq = "2931",
                unitName = "江城区社保局"
            };
            selectBusy = new TBusinessModel()
            {
                busiSeq = "3610",
                busiName = "社保制卡"
            };
            TAppointmentModel app = new TAppointmentModel()
            {
                approveSeq = "111111",
                appType = 0,
                reserveSeq = "22222",
                reserveDate = DateTime.Now,
                reserveStartTime = DateTime.Now,
                reserveEndTime = DateTime.Now,
            };

            InsertQueue(app, "AA");
        }
        TQueueBLL qBll = new TQueueBLL();
        TUnitModel selectUnit;
        TBusinessModel selectBusy;
        TAppointmentBLL aBll = new TAppointmentBLL();
        private TQueueModel InsertQueue(TAppointmentModel app, string ticketStart)
        {
            string idCard = "";
            string qNmae = "";
            string reserveSeq = app == null ? "" : app.reserveSeq;
            var line = qBll.QueueLine(selectBusy, selectUnit, ticketStart, idCard, qNmae, app);
            if (app != null)
            {
                app.sysFlag = 0;
                aBll.Insert(app);
            }
            return line;
        }
    }
}
