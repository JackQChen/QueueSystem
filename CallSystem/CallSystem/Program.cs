using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Runtime.Remoting;
using System.Configuration;

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
            var remotingConfigPath = AppDomain.CurrentDomain.BaseDirectory + "RemotingConfig.xml";
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
                //有新的更新内容
                if (bool.Parse(args[1]))
                {
                    frmMain.SetConfigValue("RemotingConfig", "192.168.0.253:5566");
                    var config = File.ReadAllText(remotingConfigPath).Replace("0.0.0.0:0000", ConfigurationManager.AppSettings["RemotingConfig"]);
                    File.WriteAllText(remotingConfigPath, config);
                }
            }
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            RemotingConfiguration.Configure(remotingConfigPath, false);
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
