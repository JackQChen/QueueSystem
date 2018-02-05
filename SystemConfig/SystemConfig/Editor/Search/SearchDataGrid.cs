using System.Windows.Forms;
using System;

namespace SystemConfig.Editor.Search
{
    public class SearchDataGrid : DataGridView
    {
        public event Action SearchDone;

        public SearchDataGrid()
        {
            this.ReadOnly = true;
            this.AllowUserToAddRows = false;
            this.AutoGenerateColumns = false;
            this.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.MultiSelect = false;
            this.KeyDown += new KeyEventHandler(OnKeyDown);
            this.CellDoubleClick += new DataGridViewCellEventHandler(SearchDataGrid_CellDoubleClick);
        }

        void SearchDataGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (this.SearchDone != null)
                this.SearchDone();
            ((SearchPanel)this.Parent).ClosePanel(true);
        }

        internal void OnKeyDown(object sender, KeyEventArgs e)
        {
            var index = -1;
            if (this.CurrentCell != null)
                index = this.CurrentCell.RowIndex;
            if (e.KeyCode == Keys.Up)
                index--;
            else if (e.KeyCode == Keys.Down)
                index++;
            else if (e.KeyCode == Keys.Enter)
            {
                if (this.SearchDone != null)
                    this.SearchDone();
                ((SearchPanel)this.Parent).ClosePanel(true);
            }
            else if (e.KeyCode == Keys.Escape)
                ((SearchPanel)this.Parent).ClosePanel(false);
            if (index < 0 || index > this.Rows.Count - 1)
                return;
            this.CurrentCell = this.Rows[index].Cells[0];
        }
    }
}
