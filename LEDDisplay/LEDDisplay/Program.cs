using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Configuration;
using System.Runtime.Remoting;

namespace LEDDisplay
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var updatePath = AppDomain.CurrentDomain.BaseDirectory + "AutoUpdate.exe";
            if (args.Length == 0)
            {
                if (File.Exists(updatePath))
                {
                    System.Diagnostics.Process.Start(updatePath, "LEDDisplay.exe");
                    return;
                }
            }
            else if (args[1] == "AutoUpdate")
            {
                var newUpdatePath = AppDomain.CurrentDomain.BaseDirectory + "AutoUpdate.exe.tmp";
                if (File.Exists(newUpdatePath))
                {
                    File.Delete(updatePath);
                    File.Move(newUpdatePath, updatePath);
                }
                //var remotingConfigPath = AppDomain.CurrentDomain.BaseDirectory + "RemotingConfig.xml";
                //有新的更新内容
                //if (bool.Parse(args[2]))
                //{
                //    var config = File.ReadAllText(remotingConfigPath).Replace("0.0.0.0:0000", ConfigurationManager.AppSettings["RemotingConfig"]);
                //    File.WriteAllText(remotingConfigPath, config);
                //}
                //RemotingConfiguration.Configure(remotingConfigPath, false);
            }
            //if (!Validate.Check())
            //    return;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FrmMain());
        }
    }
}
