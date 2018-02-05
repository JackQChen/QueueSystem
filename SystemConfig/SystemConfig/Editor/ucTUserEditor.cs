using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;

namespace SystemConfig.Editor
{
    public partial class ucTUserEditor : UserControl, ICheckData
    {
        public ucTUserEditor()
        {
            InitializeComponent();
        }

        BLL.TDictionaryBLL dicBll = new BLL.TDictionaryBLL();

        private void ucTUserEditor_Load(object sender, System.EventArgs e)
        {
            this.comboBox1.DataSource = dicBll.GetModelList(DictionaryString.UserSex);
            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            if (this.comboBox1.Items.Count > 0)
                this.comboBox1.SelectedIndex = this.comboBox1.Items.Count - 1;
            this.comboBox2.DataSource = dicBll.GetModelList(DictionaryString.WorkState);
            this.comboBox2.DisplayMember = "Name";
            this.comboBox2.ValueMember = "Value";
            this.comboBox2.DropDownStyle = ComboBoxStyle.DropDownList;
            if (this.comboBox2.Items.Count > 0)
                this.comboBox2.SelectedIndex = 0;
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.textBox4.Text == "")
            {
                MessageBox.Show("用户姓名不能为空！");
                this.textBox4.Focus();
                return false;
            }
            return true;
        }

        #endregion

        private void pictureBox1_DoubleClick(object sender, EventArgs e)
        {
            this.button1.PerformClick();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    this.pictureBox1.ImageLocation = dialog.FileName;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.pictureBox1.Image = null;
        }
    }
}
