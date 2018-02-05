using System.Windows.Forms;
using System;

namespace SystemConfig.Editor
{
    public partial class ucTLedControllerEditor : UserControl, ICheckData
    {
        public ucTLedControllerEditor()
        {
            InitializeComponent();
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.textBox4.Text == "")
            {
                MessageBox.Show("IP不能为空！");
                this.textBox4.Focus();
                return false;
            }
            if (this.TextBox2.Text == "")
            {
                MessageBox.Show("端口不能为空！");
                this.TextBox2.Focus();
                return false;
            }
            if (this.textBox1.Text == "")
            {
                MessageBox.Show("名称不能为空！");
                this.textBox1.Focus();
                return false;
            }
            return true;
        }

        #endregion

        private void TextBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsNumber(e.KeyChar)) && e.KeyChar != (char)13 && e.KeyChar != (char)8)
            {
                e.Handled = true;
            }
        }
    }
}
