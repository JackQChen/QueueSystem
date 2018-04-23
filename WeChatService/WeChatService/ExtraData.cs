using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WeChatService
{
    public class ExtraData
    {

        private const int headLength = 4;
        public int Position { get; set; }
        public byte[] Head { get; set; }
        public byte[] Data { get; set; }
        public string GUID { get; set; }
        public bool Access { get; set; }

        public ExtraData()
        {
            this.Head = new byte[headLength];
        }
    }
}
