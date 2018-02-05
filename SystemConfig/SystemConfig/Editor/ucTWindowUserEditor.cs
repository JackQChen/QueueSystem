using System.Windows.Forms;
using SystemConfig.Editor.Search;
using System;
using Model;
using System.Collections.Generic;
using System.Linq;

namespace SystemConfig.Editor
{
    public partial class ucTWindowUserEditor : UserControl, ICheckData
    {
        public ucTWindowUserEditor()
        {
            InitializeComponent();
        }

        BLL.TWindowBLL windowBll = new BLL.TWindowBLL();
        List<TWindowModel> windowList;
        BLL.TUserBLL userBll = new BLL.TUserBLL();
        List<TUserModel> userList;
        BLL.TWindowUserBLL winUserBll = new BLL.TWindowUserBLL();

        private void ucTWindowUserEditor_Load(object sender, System.EventArgs e)
        {
            this.Column7.DataSource = new BLL.TDictionaryBLL().GetModelList(DictionaryString.WorkState);
            this.Column7.DisplayMember = "Name";
            this.Column7.ValueMember = "Value";
            windowList = this.windowBll.GetModelList();
            userList = this.userBll.GetModelList();
            #region windowSearch
            SearchPanel srhWindow = new SearchPanel();
            srhWindow.Init(this.txtWindow, this.sdgWindowGrid);
            this.txtWindow.TextChanged += (s, a) =>
            {
                var list = windowList.Where(p => p.Name.Contains(this.txtWindow.Text.Trim())).ToList();
                this.sdgWindowGrid.DataSource = list;
            };
            this.txtWindow.ValueInit += val =>
            {
                var win = windowBll.GetModel(Convert.ToInt32(val));
                if (win != null)
                    this.txtWindow.Text = win.Name;
            };
            this.sdgWindowGrid.SearchDone += () =>
            {
                if (this.sdgWindowGrid.CurrentRow == null)
                    return;
                dynamic data = this.sdgWindowGrid.CurrentRow.DataBoundItem;
                this.txtWindow.Text = data.Name;
                this.txtWindow.Value = data.ID;
            };
            #endregion
            #region userSearch
            SearchPanel srhUser = new SearchPanel();
            srhUser.Init(this.txtUser, this.sdgUserGrid);
            this.txtUser.TextChanged += (s, a) =>
            {
                var list = userList.Where(p => p.Name.Contains(this.txtUser.Text.Trim())).ToList();
                this.sdgUserGrid.DataSource = list;
            };
            this.txtUser.ValueInit += val =>
            {
                var user = userBll.GetModel(Convert.ToInt32(val));
                if (user != null)
                    this.txtUser.Text = user.Name;
            };
            this.sdgUserGrid.SearchDone += () =>
            {
                if (this.sdgUserGrid.CurrentRow == null)
                    return;
                dynamic data = this.sdgUserGrid.CurrentRow.DataBoundItem;
                this.txtUser.Text = data.Name;
                this.txtUser.Value = data.ID;
            };
            #endregion
        }

        #region ICheckData 成员

        public bool CheckData()
        {
            if (this.txtWindow.Text == "")
            {
                MessageBox.Show("窗口不能为空！");
                this.txtWindow.Focus();
                return false;
            }
            if (this.txtUser.Text == "")
            {
                MessageBox.Show("用户不能为空！");
                this.txtUser.Focus();
                return false;
            }
            var model = this.winUserBll.GetModelList().Where(p => p.ID != this.textBox1.Value).Where(p =>
                p.WindowID == Convert.ToInt32(this.txtWindow.Value)
                && p.UserID == Convert.ToInt32(this.txtUser.Value)
                ).FirstOrDefault();
            if (model != null)
            {
                MessageBox.Show("窗口及用户对应不能重复！");
                this.txtUser.Focus();
                return false;
            }
            return true;
        }

        #endregion
    }
}
