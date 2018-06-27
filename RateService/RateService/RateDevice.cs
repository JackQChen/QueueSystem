using System;

namespace RateService
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {
        }

        public IntPtr ID { get; set; }

        public string WindowNumber { get; set; }

        public string UserID { get; set; }

        public string UserCode { get; set; }

        public string IP { get; set; }

        public string ConnTime { get; set; }
    }
}
