using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;

namespace BLL
{

    public class LockAction
    {
        public static void Run(FLockKey key, Action action)
        {
            DAL.LockAction.Run(key, action);
        }

        public static void RunWindowLock(string windowNo, Action action)
        {
            DAL.LockAction.RunWindowLock(windowNo, action);
        }
    }
}
