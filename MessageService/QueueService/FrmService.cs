using System;
using System.Linq;
using System.Windows.Forms;
using QueueMessage;

namespace QueueService
{
    public partial class FrmService : Form
    {

        Service service;
        public FrmService(Service service)
        {
            InitializeComponent();
            this.service = service;
        }

        private void FrmService_Load(object sender, EventArgs e)
        {
            this.service.clientList.Changed = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.service.clientList.Changed)
                return;
            this.listView1.Items.Clear();
            foreach (var client in service.clientList.Dictionary.Values.ToArray())
            {
                var item = this.listView1.Items.Add(client.ID.ToString());
                item.SubItems.Add(client.IP);
                item.SubItems.Add(client.Name);
                item.SubItems.Add(client.Type);
                item.SubItems.Add(client.ConnTime);
                item.Tag = client.ID;
            }
            this.service.clientList.Changed = false;
        }

        private void btnRestart_Click(object sender, EventArgs e)
        {
            foreach (ListViewItem item in this.listView1.SelectedItems)
            {
                var restartMsg = new RestartMessage();
                this.service.SendMessage((IntPtr)item.Tag, restartMsg);
            }
        }
    }
}
