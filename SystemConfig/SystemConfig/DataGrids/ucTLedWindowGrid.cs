using System;
using System.Collections;
using System.Windows.Forms;
using BLL;
using Model;

namespace SystemConfig.DataGrids
{
    public partial class ucTLedWindowGrid : UserControl, IExtension
    {
        object model;
        int controllerId;
        TLedWindowBLL bll = new TLedWindowBLL();

        public ucTLedWindowGrid()
        {
            InitializeComponent();
            this.dataGridView2.AutoGenerateColumns = false;
        }

        #region IDataGrid 成员

        public DataGridView DataGridView
        {
            get { return this.dataGridView1; }
        }

        #endregion

        #region IExtension 成员

        public DataGridView ClickDataGridView
        {
            get { return this.dataGridView2; }
        }

        public object Model
        {
            get { return this.model; }
        }

        public object GetCreateModel()
        {
            var m = new TLedWindowModel();
            m.ControllerID = controllerId;
            return m;
        }

        #endregion

        private void dataGridView1_CurrentCellChanged(object sender, System.EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            controllerId = Convert.ToInt32(this.dataGridView1.CurrentRow.Cells[0].Value);
            var dataList = this.bll.GetGridDataByControllerId(controllerId) as IList;
            this.dataGridView2.DataSource = dataList;
            if (dataList.Count == 0)
                this.model = null;
        }

        private void dataGridView2_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
                return;
            var data = this.dataGridView2.CurrentRow.DataBoundItem;
            this.model = data.GetType().GetProperty("Model").GetValue(data, null);
        }
    }
}
