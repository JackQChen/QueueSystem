using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Model;
using BLL;

namespace CallSystem
{
    public partial class frmQueueSet : Form
    {
        List<TWindowModel> wList;
        TWindowBLL wBll = new TWindowBLL();
        BCallBLL cBll = new BCallBLL();
        public frmQueueSet()
        {
            InitializeComponent();
        }

        private void frmQueueSet_Load(object sender, EventArgs e)
        {
            wList = wBll.GetModelList();
            cmbWindow.DataSource = wList;
            cmbWindow.DisplayMember = "Name";
            cmbWindow.ValueMember = "Number";
            cmbWindow.SelectedIndex = 0;
        }

        private void btnQuery_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            string winNum = cmbWindow.SelectedValue.ToString();
            string tiket = txtTicket.Text.Trim();
            var list = cBll.GetCall(winNum, tiket);
            foreach (var item in list)
            {
                ListViewItem lvItem = new ListViewItem();
                lvItem.Tag = item;
                lvItem.SubItems[0].Text=item.windowNumber;
                lvItem.SubItems.Add(item.ticketNumber);
                lvItem.SubItems.Add(item.handleTime.ToString("yyyy-MM-dd HH:mm:ss"));
                listView1.Items.Add(lvItem);
            }
            listView1.Refresh();
        }

        private void btnGiveUp_Click(object sender, EventArgs e)
        {
            object item = listView1.SelectedItems[0].Tag;
            var call = item as BCallModel;
            call.state = -1;
            cBll.Update(call);
            btnQuery_Click(null, null);
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            object item = listView1.SelectedItems[0].Tag;
            var call = item as BCallModel;
            call.state = 1;
            cBll.Update(call);
            btnQuery_Click(null, null);
        }
    }
}
