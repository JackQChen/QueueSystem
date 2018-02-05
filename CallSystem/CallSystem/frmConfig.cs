using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using Model;
namespace CallSystem
{
    public partial class frmConfig : Form
    {
        OperateIni ini;
        //string windowNo;//窗口号
        //string windowName;//窗口名称
        string portName = "";//端口号
        //List<TWindowModel> wList;
        TWindowBLL wBll = new TWindowBLL();
        public frmConfig()
        {
            InitializeComponent();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //if (cmbSelect.SelectedValue == null || cmbSelect.SelectedValue.ToString() == "")
            //{
            //    MessageBox.Show("请选择窗口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;

            //}
            if (cmbPort.Text == "")
            {
                MessageBox.Show("端口号不能为空", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            //ini.WriteString("WindowSet", "WindwoNo", cmbSelect.SelectedValue.ToString());
            //ini.WriteString("WindowSet", "WindowName", cmbSelect.Text);
            ini.WriteString("CallSet", "SerialPort", cmbPort.Text);
            MessageBox.Show("保存成功，重启系统生效！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            ini = new OperateIni(System.Windows.Forms.Application.StartupPath + @"\WindowConfig.ini");
            //windowNo = ini.ReadString("WindowSet", "WindwoNo");
            //windowName = ini.ReadString("WindowSet", "WindowName");
            portName = ini.ReadString("CallSet", "SerialPort");
            cmbPort.Text = portName;
            //txtWindowName.Text = windowName;
            //wList = wBll.GetModelList().Where(w => w.State == "1").ToList();
            //var window = wList.Where(w => w.Number == windowNo).FirstOrDefault();
            //cmbSelect.DisplayMember = "Name";
            //cmbSelect.ValueMember = "Number";
            //cmbSelect.DataSource = wList;
            //if (window != null)
            //    cmbSelect.SelectedValue = window.Number;
        }
    }
}
