using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

namespace ServiceHost
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                MessageBox.Show("只能通过宿主程序运行服务");
                return;
            }
            Mutex instance = null;
            while (true)
            {
                bool createdNew = false;
                instance = new Mutex(true, Process.GetCurrentProcess().MainModule.FileName.Replace("\\", "/"), out createdNew);
                if (createdNew)
                    break;
                else
                {
                    Thread.Sleep(1000);
                    instance.Close();
                }
            }
            try
            {
                new Thread(() =>
                {
                    var serverProc = Process.GetProcessById(Convert.ToInt32(args[0]));
                    while (true)
                    {
                        if (serverProc.HasExited)
                            Application.Exit();
                        Thread.Sleep(10000);
                    }
                })
                { IsBackground = true }.Start();
                var mainDirectory = Directory.GetParent(Application.StartupPath).FullName + "\\";
                var currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
                File.Delete(currentDirectory + "ServiceHost.exe.config");
                File.Copy(mainDirectory + "MessageServer.exe.config", currentDirectory + "ServiceHost.exe.config");
                ConfigurationManager.RefreshSection("appSettings");
                ConfigurationManager.RefreshSection("connectionStrings");

                //WebApi
                string bllPath = mainDirectory + "BLL.dll", serviceNewPath = mainDirectory + "ApplicationService.dll";
                string servicePath = currentDirectory + "ApplicationService.dll";
                var types = Assembly.LoadFrom(bllPath).GetTypes();
                ServiceCreator.Create(types.Where(p => !p.ContainsGenericParameters && p.IsSubclassOf(typeof(MarshalByRefObject))).ToArray());
                File.Delete(serviceNewPath);
                File.Copy(servicePath, serviceNewPath);
                Assembly.LoadFrom(serviceNewPath);
                string baseAddress = "http://localhost:5567/";
                Microsoft.Owin.Hosting.WebApp.Start<Startup>(url: baseAddress);

                //Remoting
                var remotingConfigPath = mainDirectory + "Service.xml";
                RemotingConfiguration.Configure(remotingConfigPath, false);
                var frm = new Form();
                frm.Shown += (s, a) =>
                {
                    frm.Visible = false;
                };
                Application.Run(frm);
                instance.ReleaseMutex();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "服务容器");
            }
        }
    }
}
