using System;
using System.Windows.Forms;
using Model;
using SystemConfig.Editor.Search;
using System.Collections.Generic;
using System.Linq;

namespace SystemConfig.Editor
{
    public partial class ucTWindowEditor : UserControl, ICheckData
    {
        public ucTWindowEditor()
        {
            InitializeComponent();
        }

        BLL.TWindowAreaBLL areaBll = new BLL.TWindowAreaBLL();
        List<TWindowAreaModel> areaList;
        BLL.TDictionaryBLL dicBll = new BLL.TDictionaryBLL();
        BLL.TWindowBLL winBll = new BLL.TWindowBLL();

        private void ucTWindowEditor_Load(object sender, System.EventArgs e)
        {
            areaList = areaBll.GetModelList();
            this.comboBox1.DataSource = dicBll.GetModelList(DictionaryString.WorkState);
            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            if (this.comboBox1.Items.Count > 0)
                this.comboBox1.SelectedIndex = 0;
            SearchPanel srhArea = new SearchPanel();
            srhArea.Init(this.textBox7, this.searchDataGrid1);
            this.textBox7.TextChanged += (s, a) =>
            {
                var list = areaList.Where(p => p.areaName.Contains(this.textBox7.Text.Trim())).ToList();
                this.searchDataGrid1.DataSource = list;
            };
            this.textBox7.ValueInit += val =>
            {
                var area = areaBll.GetModel(Convert.ToInt32(val));
                if (area != null)
                    this.textBox7.Text = area.areaName;
            };
            this.searchDataGrid1.SearchDone += () =>
            {
                if (this.searchDataGrid1.CurrentRow == null)
                    return;
                dynamic data = this.searchDataGrid1.CurrentRow.DataBoundItem;
                this.textBox7.Text = data.areaName;
                this.textBox7.Value = data.id;
            };
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.textBox2.Text == "")
            {
                MessageBox.Show("窗口名称不能为空！");
                this.textBox2.Focus();
                return false;
            }
            if (this.textBox3.Text == "")
            {
                MessageBox.Show("窗口号不能为空！");
                this.textBox3.Focus();
                return false;
            }
            var model = this.winBll.GetModelList().Where(p => p.ID != this.textBox1.Value).Where(p =>
                p.Name.Trim().ToLower() == this.textBox2.Text.Trim().ToLower()
                || p.Number.Trim().ToLower() == this.textBox3.Text.Trim().ToLower()
                || p.CallNumber == this.numericTextBox1.Value
                ).FirstOrDefault();
            if (model != null)
            {
                MessageBox.Show("窗口名称、窗口号、呼叫器不能重复！");
                this.textBox2.Focus();
                return false;
            }
            return true;
        }

        #endregion
    }
}
