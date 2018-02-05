using System.Windows.Forms;
using System.Linq;

namespace SystemConfig.Editor
{
    public partial class ucTWindowAreaEditor : UserControl, ICheckData
    {
        public ucTWindowAreaEditor()
        {
            InitializeComponent();
        }
        BLL.TWindowAreaBLL areaBll = new BLL.TWindowAreaBLL();
        #region ICheckData 成员

        public bool CheckData()
        {
            var model = this.areaBll.GetModelList().Where(p => p.id != this.numericTextBox1.Value).Where(p =>
                p.areaName.Trim().ToLower() == this.textBox4.Text.Trim().ToLower()).FirstOrDefault();
            if (model != null)
            {
                MessageBox.Show("区域名称不能重复！");
                this.textBox4.Focus();
                return false;
            }
            return true;
        }

        #endregion
    }
}
