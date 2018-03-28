using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace DataUpload
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
                    System.Diagnostics.Process.Start(updatePath, "DataUpload.exe");
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
                //有新的更新内容
                //if (bool.Parse(args[2]))
                //{
                //}
            }
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);
            Application.Run(new FrmMain());
        }
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            try
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Exception.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + ex.StackTrace.ToString());
            }
            catch
            { 
            
            }
        }
    }
}
