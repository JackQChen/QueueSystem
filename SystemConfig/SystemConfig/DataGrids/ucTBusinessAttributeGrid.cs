using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using BLL;
using Model;

namespace SystemConfig.DataGrids
{
    public partial class ucTBusinessAttributeGrid : UserControl, IExtension
    {
        TBusinessAttributeBLL bll = new TBusinessAttributeBLL();
        public ucTBusinessAttributeGrid()
        {
            InitializeComponent();
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView2.AutoGenerateColumns = false;
            this.dataGridView3.AutoGenerateColumns = false;
        }

        public DataGridView DataGridView
        {
            get
            {
                return this.dataGridView1;
            }
        }

        #region IExtension 成员

        public object Model
        {
            get
            {
                return this.model;
            }
        }

        public DataGridView ClickDataGridView
        {
            get { return this.dataGridView3; }
        }

        public object GetCreateModel()
        {
            if (this.model != null)
                return model;
            var m = new TBusinessAttributeModel();
            m.unitSeq = unitSeq;
            m.busiSeq = busiSeq;
            m.timeInterval = "08:00:00,12:00:00|14:00:00,18:00:00";
            m.ticketRestriction = "999,999";
            return m;
        }

        #endregion

        object model;
        string unitSeq, busiSeq;

        private void dataGridView1_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow == null)
                return;
            var unit = this.dataGridView1.CurrentRow.Cells[0].Value.ToString();
            this.dataGridView2.DataSource = this.bll.GetGridDataByUnitSeq(unit);
        }

        private void dataGridView2_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dataGridView2.CurrentRow == null)
                return;
            object busi = this.dataGridView2.CurrentRow.DataBoundItem;
            unitSeq = busi.GetType().GetProperty("unitSeq").GetValue(busi, null).ToString();
            busiSeq = busi.GetType().GetProperty("busiSeq").GetValue(busi, null).ToString();
            var dataList = this.bll.GetGridDetailData(unitSeq, busiSeq) as IList;
            this.dataGridView3.DataSource = dataList;
            if (dataList.Count == 0)
                this.model = null;
            else
                this.model = dataList[0].GetType().GetProperty("Model").GetValue(dataList[0], null);
        }
    }
}
