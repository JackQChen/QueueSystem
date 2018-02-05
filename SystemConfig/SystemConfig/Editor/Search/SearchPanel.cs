using System.Windows.Forms;
using SystemConfig.Editor.Controls;

namespace SystemConfig.Editor.Search
{
    public partial class SearchPanel : UserControl
    {
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // SearchPanel
            // 
            this.BackColor = System.Drawing.Color.Gainsboro;
            this.Name = "SearchPanel";
            this.ResumeLayout(false);

        }

        public SearchPanel()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.Selectable, false);
        }

        Control _parentForm;
        SearchDataGrid _dataGrid;
        ValueTextBox targetControl;
        string cacheText;
        bool inGrid = false;

        public void Init(ValueTextBox targetCtl, SearchDataGrid dataGrid)
        {
            _dataGrid = dataGrid;
            this.targetControl = targetCtl;
            this._parentForm = this.targetControl.FindForm();
            dataGrid.Parent.Controls.Remove(dataGrid);
            dataGrid.Dock = DockStyle.Fill;
            this.Controls.Add(dataGrid);
            this.Width = 500;
            this.Height = 300;
            this.targetControl.KeyDown += (s, e) =>
            {
                e.Handled = true;
                this._dataGrid.OnKeyDown(s, e);
            };
            this.targetControl.ValueInitDone += (val) =>
            {
                this.ClosePanel(true);
            };
            this.targetControl.TextChanged += (s, e) =>
            {
                var p = this.targetControl.Location;
                p.Offset(0, this.targetControl.Height);
                this.Location = p;
                this._parentForm.Controls.Add(this);
                this.BringToFront();
            };
            this.targetControl.LostFocus += (s, e) =>
            {
                if (!inGrid)
                    this.ClosePanel(false);
            };
            dataGrid.MouseEnter += (s, e) =>
            {
                inGrid = true;
            };
            dataGrid.MouseLeave += (s, e) =>
            {
                inGrid = false;
                this.targetControl.Focus();
            };
        }

        public void ClosePanel(bool isSet)
        {
            if (isSet)
                this.cacheText = this.targetControl.Text;
            else
                this.targetControl.Text = this.cacheText;
            this._parentForm.Controls.Remove(this);
        }
    }
}
