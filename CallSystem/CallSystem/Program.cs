using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;

namespace CallSystem
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
                    System.Diagnostics.Process.Start(updatePath, "CallSystem.exe");
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
        static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Exception.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + ex.StackTrace.ToString());
        }
    }

    class test
    {
        class ParamInfo
        {
            string Type { get; set; }
            string Info { get; set; }
        }

        Queue<ParamInfo> Queue = new Queue<ParamInfo>();

        void Init()
        {
            new Thread(() =>
            {
                while (true)
                {
                    while (Queue.Count > 0)
                    {
                        var x = Queue.Dequeue();
                        //Process(Queue.Dequeue());
                    }

                }
            }) { IsBackground = true }.Start();
        }

        void Invoke()
        {
            //construct
            var p = new ParamInfo();
            Queue.Enqueue(p);
        }

    }
}
