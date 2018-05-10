using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace CallClient
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            bool createdNew;
            Mutex instance = new Mutex(true, "CallClient", out createdNew);
            var ini = new OperateIni(Application.StartupPath + @"\WindowConfig.ini");
            if (bool.Parse(ini.ReadString("WindowSet", "Restart", "false")))
            {
                createdNew = true;
                ini.DelKey("WindowSet", "Restart");
            }
            if (createdNew)
            {
                var updatePath = AppDomain.CurrentDomain.BaseDirectory + "AutoUpdate.exe";
                if (args.Length == 0)
                {
                    if (File.Exists(updatePath))
                    {
                        System.Diagnostics.Process.Start(updatePath, "CallClient.exe");
                        return;
                    }
                }
                else
                {
                    var newUpdatePath = AppDomain.CurrentDomain.BaseDirectory + "AutoUpdate.exe.tmp";
                    if (File.Exists(newUpdatePath))
                    {
                        File.Delete(updatePath);
                        File.Move(newUpdatePath, updatePath);
                    }
                }
                string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
                Application.Run(new frmMain());
            }
            else
            {
                MessageBox.Show("程序已运行，不能重复启动！");
            }
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Exception.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + ex.StackTrace.ToString());
        }
    }
}
