using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using QueueMessage;

namespace QueueService
{
    public class ClientInfo
    {
        public IntPtr ID { get; set; }
        public string IP { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ConnTime { get; set; }
    }
}
