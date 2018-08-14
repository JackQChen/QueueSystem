using System;

namespace ScreenDisplayService
{
    public class DeviceInfo
    {
        public DeviceInfo()
        {
        }

        public IntPtr ID { get; set; }

        public string DeviceName { get; set; }

        public string IP { get; set; }

        public string ConnTime { get; set; }
    }
}
