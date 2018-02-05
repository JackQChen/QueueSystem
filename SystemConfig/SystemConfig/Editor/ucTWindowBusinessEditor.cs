using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Model;
using SystemConfig.Editor.Search;

namespace SystemConfig.Editor
{
    public partial class ucTWindowBusinessEditor : UserControl, ICheckData
    {
        public ucTWindowBusinessEditor()
        {
            InitializeComponent();
        }
        BLL.TWindowBLL windowBll = new BLL.TWindowBLL();
        List<TUnitModel> unitList;
        List<TBusinessModel> busiList;

        private void ucTWindowBusinessEditor_Load(object sender, EventArgs e)
        {
            this.txtWindow.ValueInit += val =>
            {
                var win = windowBll.GetModel(Convert.ToInt32(val));
                if (win != null)
                    this.txtWindow.Text = win.Name;
            };
            unitList = new BLL.TUnitBLL().GetModelList();
            busiList = new BLL.TBusinessBLL().GetModelList();
            #region unitSearch
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
            #endregion
            #region businessSearch
            SearchPanel srhBusi = new SearchPanel();
            srhBusi.Init(this.txtBusi, this.sdgBusiGrid);
            this.txtBusi.TextChanged += (s, a) =>
            {
                if (this.txtUnit.Value == null)
                    return;
                var list = this.busiList.Where(p => p.unitSeq == this.txtUnit.Value.ToString() && p.busiName.Contains(this.txtBusi.Text.Trim())).ToList();
                this.sdgBusiGrid.DataSource = list;
            };
            this.txtBusi.ValueInit += val =>
            {
                var busi = busiList.Where(p => p.busiSeq == val.ToString()).FirstOrDefault();
                if (busi != null)
                    this.txtBusi.Text = busi.busiName;
            };
            this.sdgBusiGrid.SearchDone += () =>
            {
                if (this.sdgBusiGrid.CurrentRow == null)
                    return;
                dynamic data = this.sdgBusiGrid.CurrentRow.DataBoundItem;
                this.txtBusi.Text = data.busiName;
                this.txtBusi.Value = data.busiSeq;
            };
            #endregion
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.txtUnit.Text.Trim() == "")
            {
                MessageBox.Show("单位不能为空！");
                this.txtUnit.Focus();
                return false;
            }
            if (this.txtBusi.Text.Trim() == "")
            {
                MessageBox.Show("业务不能为空！");
                this.txtBusi.Focus();
                return false;
            }
            return true;
        }

        #endregion
    }
}
