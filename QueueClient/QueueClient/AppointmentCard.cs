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
            Font fName = new Font("黑体", 24, FontStyle.Bold);
            Font fTitle = new Font("黑体", 18, FontStyle.Bold);
            e.Graphics.DrawString(model.busiName, fName, new SolidBrush(Color.Black), 100, 30);//业务名称
            e.Graphics.DrawString(string.Format("办理人：{0}", model.userName), fTitle, new SolidBrush(Color.Black), 850, 40);//办理人
            e.Graphics.DrawString(model.type == 0 ? "网上预约业务" : "网上申办业务", fTitle, new SolidBrush(Color.Black), 1100, 40);//类型
            e.Graphics.DrawString(model.reserveDate.ToString("yyyy-MM-dd"), fTitle, new SolidBrush(Color.Black), 1300, 40);//时间
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
