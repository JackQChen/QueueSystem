using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using BLL;
using SystemConfig.DataGrids;
using SystemConfig.Properties;

namespace SystemConfig
{
    public partial class FrmMain : Form
    {
        //Sunisoft.IrisSkin.SkinEngine skinEngine;
        public FrmMain()
        {
            //skinEngine = new Sunisoft.IrisSkin.SkinEngine(this, new MemoryStream(Resources.skin));
            InitializeComponent();
            this.Icon = System.Drawing.Icon.FromHandle(Resources.Setting.GetHicon());
        }

        Dictionary<string, object> dicBLL = new Dictionary<string, object>();
        Dictionary<string, UserControl> dicDataGridView = new Dictionary<string, UserControl>();
        Dictionary<string, Type> dicEditor = new Dictionary<string, Type>();
        ListViewItem lastSelectItem;

        void initDic(string[] strArr)
        {
            Assembly bllAsm = typeof(TUserBLL).Assembly;
            Assembly currAsm = this.GetType().Assembly;
            foreach (var mName in strArr)
            {
                dicBLL.Add(mName, Activator.CreateInstance(bllAsm.GetType("BLL." + mName + "BLL")));
                var grid = Activator.CreateInstance(currAsm.GetType("SystemConfig.DataGrids.uc" + mName + "Grid")) as UserControl;
                grid.Dock = DockStyle.Fill;
                var dgv = (grid as IDataGrid).DataGridView;
                dgv.MultiSelect = false;
                dgv.AutoGenerateColumns = false;
                dgv.BackgroundColor = Color.White;
                var model = grid as IExtension;
                if (model == null)
                    dgv.CellMouseDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0)
                            this.btnView.PerformClick();
                    };
                else
                    model.ClickDataGridView.CellMouseDoubleClick += (s, e) =>
                    {
                        if (e.RowIndex >= 0)
                            this.btnView.PerformClick();
                    };
                dicDataGridView.Add(mName, grid);
                this.plGrid.Visible = false;
                this.plGrid.Controls.Add(grid);
                this.plGrid.Visible = true;
                dicEditor.Add(mName, currAsm.GetType("SystemConfig.Editor.uc" + mName + "Editor"));
            }
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            initDic(new string[] {
                "TWindow",
                "TWindowArea",
                "TUser",
                "TWindowUser",
                "TUnit",
                "TBusiness",
                "TBusinessAttribute",
                "TWindowBusiness",
                "TLedController",
                "TLedWindow"
            });
            this.Icon = System.Drawing.Icon.FromHandle(Resources.Setting.GetHicon());
            this.SizeChanged += (s, a) =>
            {
                this.splitContainer1.SplitterDistance = 200;
                this.columnHeader1.Width = this.listView1.Width;
            };
            this.listView1.Items[0].Selected = true;
            this.splitContainer1.SplitterDistance = 200;
            this.columnHeader1.Width = this.listView1.Width;
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.listView1.SelectedItems.Count != 1)
                return;
            lastSelectItem = this.listView1.SelectedItems[0];
            if (lastSelectItem.Tag == null)
                return;
            var mName = lastSelectItem.Tag.ToString();
            var dataGrid = dicDataGridView[mName];
            dataGrid.BringToFront();
            var dgv = (dataGrid as IDataGrid).DataGridView;
            this.currDataGrid = dgv;
            var tp = this.dicBLL[mName].GetType();
            var lastIndex = 0;
            if (dgv.SelectedRows.Count == 1)
                lastIndex = dgv.SelectedRows[0].Index;
            dgv.DataSource = tp.GetMethod("GetGridData").Invoke(this.dicBLL[mName], new object[] { });
            if (dgv.Rows.Count > 0)
            {
                if (lastIndex >= dgv.Rows.Count)
                    lastIndex = dgv.Rows.Count - 1;
                //触发datagridview的CurrentCellChanged事件
                dgv.Rows[lastIndex].Cells[1].Selected = true;
                dgv.Rows[lastIndex].Cells[0].Selected = true;
            }
        }

        DataGridView currDataGrid;

        private void btnNew_Click(object sender, EventArgs e)
        {
            using (FrmEditor frm = new FrmEditor())
            {
                var mName = this.lastSelectItem.Tag.ToString();
                var editor = Activator.CreateInstance(dicEditor[mName]) as UserControl;
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = dicBLL[mName];
                var dataGrid = dicDataGridView[mName];
                var ext = dataGrid as IExtension;
                if (ext == null)
                {
                    frm.Model = Activator.CreateInstance(typeof(Model.TUserModel).Assembly.GetType("Model." + mName + "Model"));
                    frm.Operate = OperateType.New;
                }
                else
                {
                    frm.Model = ext.GetCreateModel();
                    frm.Operate = OperateType.Save;
                }
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.增加.GetHicon());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    listView1_SelectedIndexChanged(sender, e);
                }
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (currDataGrid.Rows.Count == 0)
                return;
            using (FrmEditor frm = new FrmEditor())
            {
                var mName = this.lastSelectItem.Tag.ToString();
                var editor = Activator.CreateInstance(dicEditor[mName]) as UserControl;
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = dicBLL[mName];
                var dataGrid = dicDataGridView[mName];
                var ext = dataGrid as IExtension;
                if (ext == null)
                {
                    var viewModel = currDataGrid.SelectedRows[0].DataBoundItem;
                    frm.Model = viewModel.GetType().GetProperty("Model").GetValue(viewModel, null);
                    frm.Operate = OperateType.Edit;
                }
                else
                {
                    if (ext.Model == null)
                        return;
                    frm.Model = ext.Model;
                    frm.Operate = OperateType.Save;
                }
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.修改.GetHicon());
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    listView1_SelectedIndexChanged(sender, e);
                }
            }
        }

        private void btnView_Click(object sender, EventArgs e)
        {
            if (currDataGrid.Rows.Count == 0)
                return;
            using (FrmEditor frm = new FrmEditor())
            {
                var mName = this.lastSelectItem.Tag.ToString();
                var editor = Activator.CreateInstance(dicEditor[mName]) as UserControl;
                editor.Dock = DockStyle.Fill;
                frm.Content = editor;
                frm.BLL = dicBLL[mName];
                object model = null;
                var dataGrid = dicDataGridView[mName];
                var ext = dataGrid as IExtension;
                if (ext == null)
                {
                    var viewModel = currDataGrid.SelectedRows[0].DataBoundItem;
                    model = viewModel.GetType().GetProperty("Model").GetValue(viewModel, null);
                }
                else
                {
                    if (ext.Model == null)
                        return;
                    model = ext.Model;
                }
                frm.Model = model;
                frm.Operate = OperateType.View;
                frm.Icon = System.Drawing.Icon.FromHandle(Resources.查找.GetHicon());
                frm.ShowDialog();
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (currDataGrid.SelectedRows.Count != 1)
                return;
            object model = null;
            var mName = this.lastSelectItem.Tag.ToString();
            var dataGrid = dicDataGridView[mName];
            var ext = dataGrid as IExtension;
            if (ext == null)
            {
                var viewModel = currDataGrid.SelectedRows[0].DataBoundItem;
                model = viewModel.GetType().GetProperty("Model").GetValue(viewModel, null);
            }
            else
            {
                if (ext.Model == null)
                    return;
                model = ext.Model;
            }
            if (MessageBox.Show(
                "确定删除该记录吗?",
                "提示",
                MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
                var tp = this.dicBLL[mName].GetType();
                tp.GetMethod("Delete").Invoke(this.dicBLL[mName], new object[] { model });
                tp.GetMethod("ResetIndex").Invoke(this.dicBLL[mName], new object[] { });
                listView1_SelectedIndexChanged(sender, e);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            listView1_SelectedIndexChanged(sender, e);
        }

        private void btnLog_Click(object sender, EventArgs e)
        {
            using (FrmLogQuery frm = new FrmLogQuery())
            {
                frm.ShowDialog(this);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
