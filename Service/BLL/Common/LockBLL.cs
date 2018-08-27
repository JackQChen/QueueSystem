using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Model;
using DAL;

namespace BLL
{

    public class LockBLL : BLLBase<LockDAL, FLockModel>
    {
        public LockBLL()
            : base()
        {
        }

        public LockBLL(string connName)
            : base(connName)
        {
        }

        public LockBLL(string connName, string areaNo)
            : base(connName, areaNo)
        {
        }

        public bool lockWin(string windowNo)
        {
            return this.CreateDAL().lockWin(windowNo);
        }

        public void releaseWin(string windowNo)
        {
            this.CreateDAL().releaseWin(windowNo);
        }
    }
}
