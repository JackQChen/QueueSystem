using System;
using System.Windows.Forms;

namespace RateService
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
            this.service.deviceListChanged = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!this.service.deviceListChanged)
                return;
            this.listView1.Items.Clear();
            foreach (var client in service.deviceList.Dictionary.Values)
            {
                var item = this.listView1.Items.Add(client.ID.ToString());
                item.SubItems.Add(client.IP);
                item.SubItems.Add(client.WindowNumber);
                item.SubItems.Add(client.UserCode);
                item.SubItems.Add(client.ConnTime);
            }
            this.service.deviceListChanged = false;
        }
    }
}
