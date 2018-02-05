using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemConfig.DataGrids
{
    public interface IDataGrid
    {
        DataGridView DataGridView { get; }
    }
}
