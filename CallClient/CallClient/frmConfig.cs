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
namespace CallClient
{
    public partial class frmConfig : CustomSkin.Windows.Forms.FormBase
    {
        OperateIni ini;
        string windowNo;//窗口号
        string windowName;//窗口名称
        List<TWindowModel> wList;
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
            if (cmbSelect.SelectedValue == null || cmbSelect.SelectedValue.ToString() == "")
            {
                MessageBox.Show("请选择窗口", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            string fun = "";
            if (f1.Text == "")
            {
                fun += "呼叫,";
                //MessageBox.Show("请配置呼叫快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f2.Text == "")
            {
                fun += "重呼,";
                //MessageBox.Show("请配置重呼快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f3.Text == "")
            {
                fun += "评价,";
                //MessageBox.Show("请配置评价快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f4.Text == "")
            {
                fun += "弃号,";
                //MessageBox.Show("请配置弃号快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f5.Text == "")
            {
                fun += "暂停,";
                //MessageBox.Show("请配置暂停快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f6.Text == "")
            {
                fun += "转移,";
                //MessageBox.Show("请配置转移快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f7.Text == "")
            {
                fun += "挂起,";
                //MessageBox.Show("请配置挂起快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            if (f8.Text == "")
            {
                fun += "回呼,";
                //MessageBox.Show("请配置回呼快捷键！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                //return;
            }
            
            List<string> list = new List<string>() { f1.Text, f2.Text, f3.Text, f4.Text, f5.Text, f6.Text, f7.Text, f8.Text };
            list = list.Where(l => l != "").ToList();
            if (list.GroupBy(l => l).Count() < list.Count)
            {
                MessageBox.Show("存在快捷键重复，请修改后保存！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            if (fun != "")
            {
                if (DialogResult.Cancel == MessageBox.Show("部分功能未设置快捷键【" + fun.Substring(0, fun.Length - 1) + "】，确认不设置快捷键？", "提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
                    return;
            }
            ini.WriteString("WindowSet", "WindwoNo", cmbSelect.SelectedValue.ToString());
            ini.WriteString("WindowSet", "WindowName", cmbSelect.Text);
            ini.WriteString("Shortcutkey", "Fuction1", f1.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction2", f2.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction3", f3.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction4", f4.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction5", f5.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction6", f6.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction7", f7.Text.ToString());
            ini.WriteString("Shortcutkey", "Fuction8", f8.Text.ToString());
            //ini.WriteString("CallSet", "SerialPort", cmbPort.Text);
            MessageBox.Show("保存成功，重启系统生效！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            ini = new OperateIni(System.Windows.Forms.Application.StartupPath + @"\WindowConfig.ini");
            windowNo = ini.ReadString("WindowSet", "WindwoNo");
            windowName = ini.ReadString("WindowSet", "WindowName");
            f1.Text = ini.ReadString("Shortcutkey", "Fuction1");
            f2.Text = ini.ReadString("Shortcutkey", "Fuction2");
            f3.Text = ini.ReadString("Shortcutkey", "Fuction3");
            f4.Text = ini.ReadString("Shortcutkey", "Fuction4");
            f5.Text = ini.ReadString("Shortcutkey", "Fuction5");
            f6.Text = ini.ReadString("Shortcutkey", "Fuction6");
            f7.Text = ini.ReadString("Shortcutkey", "Fuction7");
            f8.Text = ini.ReadString("Shortcutkey", "Fuction8");
            //portName = ini.ReadString("CallSet", "SerialPort");
            //cmbPort.Text = portName;
            txtWindowName.Text = windowName;
            wList = wBll.GetModelList().Where(w => w.State == "1").ToList();
            var window = wList.Where(w => w.Number == windowNo).FirstOrDefault();
            cmbSelect.DisplayMember = "Name";
            cmbSelect.ValueMember = "Number";
            cmbSelect.DataSource = wList;
            if (window != null)
                cmbSelect.SelectedValue = window.Number;
        }
    }
}
