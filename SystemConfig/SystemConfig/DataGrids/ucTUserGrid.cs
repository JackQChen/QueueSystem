using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemConfig.DataGrids
{
    public partial class ucTUserGrid : UserControl, IDataGrid
    {
        public ucTUserGrid()
        {
            InitializeComponent();
        }

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
