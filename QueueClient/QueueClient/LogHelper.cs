using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace QueueClient
{
    public class LogHelper
    {
        public static void WriterInterfaceLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\InterfaceLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime .Now.ToString()+" : "+ logString);
            }
        }

        public static void WriterQueueLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\QueueLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " : " + logString);
            }
        }

        public static void WriterReadIdCardLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\ReadIdCardLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " : " + logString);
            }
        }

        public static void WriterEvaluateLog(string logString)
        {
            string dir = AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd");
            string path = dir + "\\EvaluateLog.txt";
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            using (StreamWriter sw = new StreamWriter(path, true))
            {
                sw.WriteLine(DateTime.Now.ToString() + " : " + logString);
            }
        }
    }
}
