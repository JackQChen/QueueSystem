using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Model;
using SystemConfig.Editor.Search;

namespace SystemConfig.Editor
{
    public partial class ucTBusinessEditor : UserControl, ICheckData
    {
        BLL.TDictionaryBLL dicBll = new BLL.TDictionaryBLL();
        List<TUnitModel> unitList;

        public ucTBusinessEditor()
        {
            InitializeComponent();
        }

        private void ucTBusinessEditor_Load(object sender, EventArgs e)
        {
            this.comboBox1.DataSource = dicBll.GetModelList(DictionaryString.AppointmentType);
            this.comboBox1.DisplayMember = "Name";
            this.comboBox1.ValueMember = "Value";
            this.comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
            if (this.comboBox1.Items.Count > 0)
                this.comboBox1.SelectedIndex = 0;
            unitList = new BLL.TUnitBLL().GetModelList();
            SearchPanel srhUnit = new SearchPanel();
            srhUnit.Init(this.txtUnit, this.sdgUnitGrid);
            this.txtUnit.TextChanged += (s, a) =>
            {
                var list = unitList.Where(p => p.unitName.Contains(this.txtUnit.Text.Trim())).ToList();
                this.sdgUnitGrid.DataSource = list;
            };
            this.txtUnit.ValueInit += val =>
            {
                var unit = unitList.Where(p => p.unitSeq == val.ToString()).FirstOrDefault();
                if (unit != null)
                    this.txtUnit.Text = unit.unitName;
            };
            this.sdgUnitGrid.SearchDone += () =>
            {
                if (this.sdgUnitGrid.CurrentRow == null)
                    return;
                dynamic data = this.sdgUnitGrid.CurrentRow.DataBoundItem;
                this.txtUnit.Text = data.unitName;
                this.txtUnit.Value = data.unitSeq;
            };
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.txtUnit.Text == "")
            {
                MessageBox.Show("用户姓名不能为空！");
                this.txtUnit.Focus();
                return false;
            }
            return true;
        }

        #endregion
    }
}
