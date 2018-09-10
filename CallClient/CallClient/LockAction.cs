using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BLL;
using Model;

namespace CallClient
{
    public class LockAction
    {
        static LockDictionary lockDic = new LockDictionary();
        LockBLL bll = new LockBLL();
        public void lockWin(string windowNo, Action action)
        {
            try
            {
                lock (lockDic.GetLockObject(windowNo))
                {
                    Task.Factory.StartNew(() =>
                    {
                        var winLock = true;
                        int count = 0;
                        while (true)
                        {
                            winLock = bll.lockWin(windowNo);
                            if (winLock)
                                break;
                            else
                            {
                                Interlocked.Increment(ref count);
                                Thread.Sleep(100);
                                if (count > 600)
                                { 
                                    //等1分钟无响应 就手动释放窗口 防止被锁死。
                                    bll.releaseWin(windowNo);
                                }
                            }
                        }
                        if (winLock)
                        {
                            action();
                            bll.releaseWin(windowNo);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "log\\" + DateTime.Now.ToString("yyyy-MM-dd") + "\\Exception.txt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " : " + ex.Message + "\r\n");
            }
        }
    }
}
