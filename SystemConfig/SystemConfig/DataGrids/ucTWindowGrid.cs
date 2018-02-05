using System;
using System.Collections;
using System.Windows.Forms;
using BLL;
using Model;
using SystemConfig.Editor;
using SystemConfig.Properties;

namespace SystemConfig.DataGrids
{
    public partial class ucTWindowGrid : UserControl, IDataGrid
    {
        TWindowUserBLL winUserBll = new TWindowUserBLL();
        TWindowBusinessBLL winBusiBll = new TWindowBusinessBLL();

        public ucTWindowGrid()
        {
            InitializeComponent();
            this.dgvBusi.AutoGenerateColumns = false;
            this.dgvUser.AutoGenerateColumns = false;
        }

        #region IDataGrid 成员

        public DataGridView DataGridView
        {
            get
            {
                return this.dgvWindow;
            }
        }

        #endregion

        int winId;
        object busiModel, userModel;

        private void dgvWindow_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvWindow.CurrentRow == null)
                return;
            winId = Convert.ToInt32(this.dgvWindow.CurrentRow.Cells[0].Value);
            var busiList = this.winBusiBll.GetGridDetailData(winId) as IList;
            this.dgvBusi.DataSource = busiList;
            if (busiList.Count == 0)
                this.busiModel = null;
            var userList = this.winUserBll.GetGridDetailData(winId) as IList;
            this.dgvUser.DataSource = userList;
            if (userList.Count == 0)
                this.userModel = null;
        }

        private void dgvBusi_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvBusi.CurrentRow == null)
                return;
            var data = this.dgvBusi.CurrentRow.DataBoundItem;
            this.busiModel = data.GetType().GetProperty("Model").GetValue(data, null);
        }

        private void dgvUser_CurrentCellChanged(object sender, EventArgs e)
        {
            if (dgvUser.CurrentRow == null)
                return;
            var data = this.dgvUser.CurrentRow.DataBoundItem;
            this.userModel = data.GetType().GetProperty("Model").GetValue(data, null);
        }

        #region 业务操作

        private void btnBusiNew_Click(object sender, EventArgs e)
        {
            using (FrmEditor frm = new FrmEditor())
            {
                var editor = new ucTWindowBusinessEditor();
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = this.winBusiBll;
                var m = new TWindowBusinessModel();
                m.WindowID = winId;
                frm.Model = m;
                frm.Operate = OperateType.Save;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.增加.GetHicon());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.btnRefresh.PerformClick();
                }
            }
        }

        private void btnBusiEdit_Click(object sender, EventArgs e)
        {
            if (this.dgvBusi.Rows.Count == 0)
                return;
            using (FrmEditor frm = new FrmEditor())
            {
                var editor = new ucTWindowBusinessEditor();
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = this.winBusiBll;
                if (this.busiModel == null)
                    return;
                frm.Model = this.busiModel;
                frm.Operate = OperateType.Save;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.修改.GetHicon());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.btnRefresh.PerformClick();
                }
            }
        }

        private void btnBusiView_Click(object sender, EventArgs e)
        {
            if (this.dgvBusi.Rows.Count == 0)
                return;
            using (FrmEditor frm = new FrmEditor())
            {
                var editor = new ucTWindowBusinessEditor();
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = this.winBusiBll;
                if (this.busiModel == null)
                    return;
                frm.Model = this.busiModel;
                frm.Operate = OperateType.View;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.查找.GetHicon());
                frm.ShowDialog();
            }
        }

        private void btnBusiDelete_Click(object sender, EventArgs e)
        {
            if (this.dgvBusi.Rows.Count == 0)
                return;
            if (this.busiModel == null)
                return;
            if (MessageBox.Show(
                "确定删除该记录吗?",
                "提示",
                MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                this.winBusiBll.Delete(this.busiModel as TWindowBusinessModel);
                this.winBusiBll.ResetIndex();
                this.btnRefresh.PerformClick();
            }
        }

        private void btnBusiRefresh_Click(object sender, EventArgs e)
        {
            var lastIndex = 0;
            if (this.dgvBusi.SelectedRows.Count == 1)
                lastIndex = dgvBusi.SelectedRows[0].Index;
            dgvBusi.DataSource = this.winBusiBll.GetGridDetailData(winId);
            if (dgvBusi.Rows.Count > 0)
            {
                if (lastIndex >= dgvBusi.Rows.Count)
                    lastIndex = dgvBusi.Rows.Count - 1;
                //触发datagridview的CurrentCellChanged事件
                dgvBusi.Rows[lastIndex].Cells[1].Selected = true;
                dgvBusi.Rows[lastIndex].Cells[0].Selected = true;
            }
        }

        private void dgvBusi_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                this.btnView.PerformClick();
        }

        #endregion

        #region 用户操作

        private void btnUserNew_Click(object sender, EventArgs e)
        {
            using (FrmEditor frm = new FrmEditor())
            {
                var editor = new ucTWindowUserEditor();
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = this.winUserBll;
                var m = new TWindowUserModel();
                m.WindowID = winId;
                m.CreateDateTime = DateTime.Now;
                frm.Model = m;
                frm.Operate = OperateType.Save;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.增加.GetHicon());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.btnRefresh2.PerformClick();
                }
            }
        }

        private void btnUserEdit_Click(object sender, EventArgs e)
        {
            if (this.dgvUser.Rows.Count == 0)
                return;
            using (FrmEditor frm = new FrmEditor())
            {
                var editor = new ucTWindowUserEditor();
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = this.winUserBll;
                if (this.userModel == null)
                    return;
                frm.Model = this.userModel;
                frm.Operate = OperateType.Save;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.修改.GetHicon());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    this.btnRefresh2.PerformClick();
                }
            }
        }

        private void btnUserView_Click(object sender, EventArgs e)
        {
            if (this.dgvUser.Rows.Count == 0)
                return;
            using (FrmEditor frm = new FrmEditor())
            {
                var editor = new ucTWindowUserEditor();
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = this.winUserBll;
                if (this.userModel == null)
                    return;
                frm.Model = this.userModel;
                frm.Operate = OperateType.View;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.查找.GetHicon());
                frm.ShowDialog();
            }
        }

        private void btnUserDelete_Click(object sender, EventArgs e)
        {
            if (this.dgvUser.Rows.Count == 0)
                return;
            if (this.userModel == null)
                return;
            if (MessageBox.Show(
                "确定删除该记录吗?",
                "提示",
                MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                this.winUserBll.Delete(this.userModel as TWindowUserModel);
                this.winUserBll.ResetIndex();
                this.btnRefresh2.PerformClick();
            }
        }

        private void btnUserRefresh_Click(object sender, EventArgs e)
        {
            var lastIndex = 0;
            if (this.dgvUser.SelectedRows.Count == 1)
                lastIndex = dgvUser.SelectedRows[0].Index;
            dgvUser.DataSource = this.winUserBll.GetGridDetailData(winId);
            if (dgvUser.Rows.Count > 0)
            {
                if (lastIndex >= dgvUser.Rows.Count)
                    lastIndex = dgvUser.Rows.Count - 1;
                //触发datagridview的CurrentCellChanged事件
                dgvUser.Rows[lastIndex].Cells[1].Selected = true;
                dgvUser.Rows[lastIndex].Cells[0].Selected = true;
            }
        }

        private void dgvUser_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
                this.btnView2.PerformClick();
        }

        #endregion
    }
}
