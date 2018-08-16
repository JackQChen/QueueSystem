using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BLL;

namespace RemotingConfig
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }
        string strServer = @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<configuration>
  <system.runtime.remoting>
    <application>
      <service>{0}
      </service>
      <channels>
        <channel ref=""tcp"" secure=""false"" port=""5566""/>
      </channels>
    </application>
    <customErrors mode=""off"" />
  </system.runtime.remoting>
</configuration>
";
        string strClient = @"<?xml version=""1.0"" encoding=""utf-8""?>
<configuration>
  <system.runtime.remoting>
    <application>
      <client>{0}
      </client>
    </application>
  </system.runtime.remoting>
</configuration>
";

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            var asm = typeof(TUserBLL).Assembly;
            StringBuilder sbServer = new StringBuilder();
            StringBuilder sbClient = new StringBuilder();
            foreach (var tp in asm.GetTypes().Where(p => p.IsSubclassOf(typeof(MarshalByRefObject))))
            {
                if (tp.IsGenericType)
                    continue;
                sbServer.Append(string.Format(@"
        <wellknown mode=""SingleCall"" type=""{0},BLL"" objectUri=""{1}""/>",
                    tp.FullName, tp.Name));
                sbClient.Append(string.Format(@"
        <wellknown type=""{0},BLL"" url=""tcp://{1}/{2}""/>",
                    tp.FullName, this.txtUri.Text, tp.Name));
            }
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "RemotingConfig_Server.xml", string.Format(strServer, sbServer));
            File.WriteAllText(AppDomain.CurrentDomain.BaseDirectory + "RemotingConfig.xml", string.Format(strClient, sbClient));
            MessageBox.Show("完成");
        }
    }
}
