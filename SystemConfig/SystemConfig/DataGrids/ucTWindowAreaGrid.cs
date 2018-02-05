using System.Windows.Forms;

namespace SystemConfig.DataGrids
{
    public partial class ucTWindowAreaGrid : UserControl, IDataGrid
    {
        public ucTWindowAreaGrid()
        {
            InitializeComponent();
            //this.dataGridView1.CellValidating += new DataGridViewCellValidatingEventHandler(dataGridView1_CellValidating);
        }

        //private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    var clm = dataGridView1.Columns[e.ColumnIndex];
        //    if (clm.DataPropertyName == "areaName")
        //    {
        //        if (String.IsNullOrEmpty(e.FormattedValue.ToString()))
        //        {
        //            MessageBox.Show(clm.HeaderText + "不能为空！");
        //            e.Cancel = true;
        //        }
        //    }
        //} 

        #region IDataGrid 成员

        public DataGridView DataGridView
        {
            get
            {
                return this.dataGridView1;
            }
        }

        #endregion
    }
}
