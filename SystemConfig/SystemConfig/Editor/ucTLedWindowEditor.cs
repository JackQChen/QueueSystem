using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Model;
using SystemConfig.Editor.Search;

namespace SystemConfig.Editor
{
    public partial class ucTLedWindowEditor : UserControl, ICheckData
    {
        BLL.TWindowBLL windowBll = new BLL.TWindowBLL();
        BLL.TLedControllerBLL controllerBll = new BLL.TLedControllerBLL();
        BLL.TLedWindowBLL ledWinBll = new BLL.TLedWindowBLL();
        List<TWindowModel> windowList;

        public ucTLedWindowEditor()
        {
            InitializeComponent();
        }

        private void ucTLedWindowEditor_Load(object sender, EventArgs e)
        {
            this.Column7.DataSource = new BLL.TDictionaryBLL().GetModelList(DictionaryString.WorkState);
            this.Column7.DisplayMember = "Name";
            this.Column7.ValueMember = "Value";
            this.txtController.ValueInit += val =>
            {
                this.txtController.Text = this.controllerBll.GetModel(Convert.ToInt32(val)).Name;
            };
            windowList = this.windowBll.GetModelList();
            #region windowSearch
            SearchPanel srhWindow = new SearchPanel();
            srhWindow.Init(this.txtWindow, this.sdgWindowGrid);
            this.txtWindow.ValueInit += val =>
            {
                var win = this.windowList.Where(p => p.Number == val.ToString()).FirstOrDefault();
                if (win != null)
                    this.txtWindow.Text = win.Name;
            };
            this.txtWindow.TextChanged += (s, a) =>
            {
                var list = windowList.Where(p => p.Name.Contains(this.txtWindow.Text.Trim())).ToList();
                this.sdgWindowGrid.DataSource = list;
            };
            this.sdgWindowGrid.SearchDone += () =>
            {
                if (this.sdgWindowGrid.CurrentRow == null)
                    return;
                dynamic data = this.sdgWindowGrid.CurrentRow.DataBoundItem;
                this.txtWindow.Text = data.Name;
                this.txtWindow.Value = data.Number;
            };
            #endregion
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.txtWindow.Text == "")
            {
                MessageBox.Show("对应窗口不能为空！");
                this.txtWindow.Focus();
                return false;
            }
            if (this.txtDisplay.Text == "")
            {
                MessageBox.Show("显示文本不能为空！");
                this.txtDisplay.Focus();
                return false;
            }
            if (this.txtPosition.Text == "")
            {
                MessageBox.Show("屏幕位置不能为空！");
                this.txtPosition.Focus();
                return false;
            }
            var model = this.ledWinBll.GetModelList().Where(p => p.ID != this.numericTextBox1.Value).Where(p =>
                p.WindowNumber == this.txtWindow.Value.ToString()
                ).FirstOrDefault();
            if (model != null)
            {
                MessageBox.Show("窗口信息对应不能重复！");
                this.txtWindow.Focus();
                return false;
            }
            return true;
        }

        #endregion
    }
}
