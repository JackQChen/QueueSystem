using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemConfig.DataGrids
{
    public interface IExtension : IDataGrid
    {
        DataGridView ClickDataGridView { get; }
        object Model { get; }
        object GetCreateModel();
    }
}
