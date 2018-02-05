using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;
using System.Collections;
using Model;

namespace SystemConfig.DataGrids
{
    public partial class ucTWindowBusinessGrid : UserControl, IExtension
    {
        TWindowBusinessBLL bll = new TWindowBusinessBLL();
        public ucTWindowBusinessGrid()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView2.AutoGenerateColumns = false;
        }

        #region IDataGrid 成员

        public DataGridView DataGridView
        {
            get { return this.dataGridView1; }
        }

        #endregion

        #region IExtension 成员

        public object Model
        {
            get { return this.model; }
        }

        public DataGridView ClickDataGridView
        {
            get { return this.dataGridView2; }
        }

        public object GetCreateModel()
        {
            var m = new TWindowBusinessModel();
            m.WindowID = winId;
            return m;
        }

        #endregion

        object model;
        int winId = 0;

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            winId = Convert.ToInt32(this.dataGridView1.CurrentRow.Cells[0].Value);
            var dataList = this.bll.GetGridDetailData(winId) as IList;
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
