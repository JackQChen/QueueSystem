using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;

namespace QueueClient
{
    public partial class AppointmentCard : UserControl
    {
        public AppointmentCard()
        {
            InitializeComponent();
        }
        public TAppointmentModel model;
        public Action<TAppointmentModel> action;

        bool isCheck = true;
        private void pbIsCheck_Click(object sender, EventArgs e)
        {
            if (isCheck)
            {
                model.isCheck = false;
                pbIsCheck.Image = Properties.Resources.预约勾选框;
                pbMain.Image = Properties.Resources.预约未选中;
            }
            else
            {
                model.isCheck = true;
                pbIsCheck.Image = Properties.Resources.预约勾选框1;
                pbMain.Image = Properties.Resources.预约选中;
            }
            isCheck = !isCheck;
            if (action != null)
                action(model);
        }

        private void pbMain_Paint(object sender, PaintEventArgs e)
        {
            Font fName = new Font("黑体", 22, FontStyle.Bold);
            Font fTitle = new Font("黑体", 18, FontStyle.Bold);
            string info = string.Format("{0} {1}      {2}:{3} {4}     办理人：{5}", model.unitName, model.busiName, model.type == 0 ? "网上预约业务" : "网上申办业务", model.reserveDate.ToString("yyyy-MM-dd"),
                model.type == 0 ? model.reserveStartTime.ToString("HH:mm") + "-" + model.reserveEndTime.ToString("HH:mm") : "           ", model.userName);
            e.Graphics.DrawString(info, fName, new SolidBrush(Color.Black), 100, 30);//业务名称
            //e.Graphics.DrawString(model.unitName, fName, new SolidBrush(Color.Black), 100, 30);//业务名称
            //e.Graphics.DrawString(model.busiName, fName, new SolidBrush(Color.Black), 100, 30);//业务名称
            //e.Graphics.DrawString(model.type == 0 ? "网上预约业务" : "网上申办业务", fTitle, new SolidBrush(Color.Black), 1100, 40);//类型
            //e.Graphics.DrawString(model.reserveDate.ToString("yyyy-MM-dd"), fTitle, new SolidBrush(Color.Black), 1300, 40);//时间
            //if (model.type == 0)
            //{
            //    //时间段
            //    e.Graphics.DrawString(model.reserveStartTime.ToString("HH:mm") + "-" + model.reserveEndTime.ToString("HH:mm"), fTitle, new SolidBrush(Color.Black), 1300, 40);//时间
            //}
            //e.Graphics.DrawString(string.Format("办理人：{0}", model.userName), fTitle, new SolidBrush(Color.Black), 850, 40);//办理人
        }

        private void AppointmentCard_Load(object sender, EventArgs e)
        {
            if (model.isCheck)
            {
                isCheck = true;
                pbIsCheck.Image = Properties.Resources.预约勾选框1;
                pbMain.Image = Properties.Resources.预约选中;
            }
            else
            {
                isCheck = false;
                pbIsCheck.Image = Properties.Resources.预约勾选框;
                pbMain.Image = Properties.Resources.预约未选中;
            }
        }
    }
}
