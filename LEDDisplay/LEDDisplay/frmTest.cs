using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices; // 用 DllImport 需用此 命名空间
using System.Configuration;
using System.Threading;

namespace LEDDisplay
{
    public partial class frmTest : Form
    {
        private const int WM_LED_NOTIFY = 1025;

        CLEDSender LEDSender = new CLEDSender();

        public frmTest()
        {
            InitializeComponent();
        }

        private void OnLEDNotify(UInt32 msg, UInt32 wParam, UInt32 lParam)
        {
            TNotifyParam notifyparam = new TNotifyParam();
            LEDSender.Do_LED_GetNotifyParam_BufferToFile(ref notifyparam, "C:\\play.dat", (int)wParam);

            if (notifyparam.notify == LEDSender.LM_TIMEOUT)
            {
                Text = "命令执行超时";
            }
            else if (notifyparam.notify == LEDSender.LM_TX_COMPLETE)
            {
                if (notifyparam.result == LEDSender.RESULT_FLASH)
                {
                    Text = "数据传送完成，正在写入Flash";
                }
                else
                {
                    Text = "数据传送完成";
                }
            }
            else if (notifyparam.notify == LEDSender.LM_NOTIFY)
            {
                if (notifyparam.result == LEDSender.NOTIFY_GET_PLAY_BUFFER)
                {
                    Text = "当前播放节目数据读取完成";
                    LEDSender.Do_LED_PreviewFile(128, 64, "C:\\play.dat");
                }
            }
            else if (notifyparam.notify == LEDSender.LM_RESPOND)
            {
                if (notifyparam.command == LEDSender.PKC_GET_POWER)
                {
                    if (notifyparam.status == LEDSender.LED_POWER_ON) Text = "读取电源状态完成，当前为电源开启状态";
                    else if (notifyparam.status == LEDSender.LED_POWER_OFF) Text = "读取电源状态完成，当前为电源关闭状态";
                }
                else if (notifyparam.command == LEDSender.PKC_SET_POWER)
                {
                    if (notifyparam.result == 99) Text = "当前为定时开关屏模式";
                    else if (notifyparam.status == LEDSender.LED_POWER_ON) Text = "设置电源状态完成，当前为电源开启状态";
                    else Text = "设置电源状态完成，当前为电源关闭状态";
                }
                else if (notifyparam.command == LEDSender.PKC_GET_BRIGHT)
                {
                    Text = string.Format("读取亮度完成，当前亮度={0:D}", notifyparam.status);
                }
                else if (notifyparam.command == LEDSender.PKC_SET_BRIGHT)
                {
                    if (notifyparam.result == 99) Text = "当前为定时亮度调节模式";
                    else Text = string.Format("设置亮度完成，当前亮度={0:D}", notifyparam.status);
                }
                else if (notifyparam.command == LEDSender.PKC_ADJUST_TIME)
                {
                    Text = "校正显示屏时间完成";
                }
                else if (notifyparam.command == LEDSender.PKC_GET_TEMPERATURE_HUMIDITY)
                {
                    Text = string.Format("读取亮度完成，当前温度={0:D}, 湿度={1:D}", notifyparam.status & 0xFFFF, notifyparam.status >> 16);
                }
            }
        }

        protected override void DefWndProc(ref System.Windows.Forms.Message m)
        {
            switch (m.Msg)
            {
                case WM_LED_NOTIFY:
                    OnLEDNotify((UInt32)m.Msg, (UInt32)m.WParam, (UInt32)m.LParam);
                    break;
                default:
                    base.DefWndProc(ref m);//调用基类函数处理非自定义消息。
                    break;
            }
        }

        private void GetDeviceParam(ref TDeviceParam param)
        {
            switch (cmbDevType.SelectedIndex)
            {
                case 0:
                    param.devType = LEDSender.DEVICE_TYPE_COM;
                    break;
                case 1:
                    param.devType = LEDSender.DEVICE_TYPE_UDP;
                    break;
            }
            param.comPort = (ushort)Convert.ToInt16(eCommPort.Text);
            param.comSpeed = (ushort)cmbBaudRate.SelectedIndex;
            param.locPort = (ushort)Convert.ToInt16(eLocalPort.Text);
            param.rmtHost = eRemoteHost.Text;
            param.rmtPort = 6666;
            param.dstAddr = 0;
        }

        private void GetDeviceParamWithoutStruct(Int32 param_index, Int32 notifymode, Int32 wmhandle, Int32 wmmessage)
        {
            switch (cmbDevType.SelectedIndex)
            {
                case 0:
                    LEDSender.Do_LED_COM_SenderParam(param_index, (Int32)Convert.ToInt16(eCommPort.Text), (Int32)cmbBaudRate.SelectedIndex, 0, notifymode, wmhandle, wmmessage);
                    break;
                case 1:
                    LEDSender.Do_LED_UDP_SenderParam(param_index, (Int32)Convert.ToInt16(eLocalPort.Text), eRemoteHost.Text, 6666, 0, notifymode, wmhandle, wmmessage);
                    break;
            }
        }

        public void EncodeDateTime(ref TSystemTime t, ushort year, ushort month, ushort day, ushort hour, ushort minute, ushort second, ushort milliseconds)
        {
            t.wYear = year;
            t.wMonth = month;
            t.wDay = day;
            t.wHour = hour;
            t.wMinute = minute;
            t.wSecond = second;
            t.wMilliseconds = milliseconds;
        }

        public int MonthDays(int ryear, int month)
        {
            int r = 0;
            switch (ryear)
            {
                case 0:
                    switch (month)
                    {
                        case 0:
                        case 2:
                        case 4:
                        case 6:
                        case 7:
                        case 9:
                        case 11:
                            r = 31;
                            break;
                        case 3:
                        case 5:
                        case 8:
                        case 10:
                            r = 30;
                            break;
                        case 1:
                            r = 28;
                            break;
                    }
                    break;
                case 1:
                    switch (month)
                    {
                        case 0:
                        case 2:
                        case 4:
                        case 6:
                        case 7:
                        case 9:
                        case 11:
                            r = 31;
                            break;
                        case 3:
                        case 5:
                        case 8:
                        case 10:
                            r = 30;
                            break;
                        case 1:
                            r = 29;
                            break;
                    }
                    break;
            }
            return r;
        }

        public void SystemTimeToTimeStamp(ref TSystemTime itime, ref TTimeStamp otime)
        {
            Int32 i, y, m, d;

            y = itime.wYear - 1;
            for (m = 0, d = itime.wYear; d > 100; d -= 100, m++) ;

            if (((itime.wYear & 3) == 0) && ((m & 3) == 0 || (d != 0))) d = 1;
            else d = 0;

            otime.date = itime.wDay;
            for (i = 1; i <= itime.wMonth - 1; i++) otime.date += MonthDays(d, i - 1);
            otime.date += y * 365 + (y >> 2) - (y / 100) + (y / 400);

            otime.time = itime.wHour * 60 * 60000 + itime.wMinute * 60000 + itime.wSecond * 1000 + itime.wMilliseconds;
        }

        private void Parse(Int32 K)
        {
            if (K == LEDSender.R_DEVICE_READY) Text = "正在执行命令或者发送数据...";
            else if (K == LEDSender.R_DEVICE_INVALID) Text = "打开通讯设备失败(串口不存在、或者串口已被占用、或者网络端口被占用)";
            else if (K == LEDSender.R_DEVICE_BUSY) Text = "设备忙，正在通讯中...";
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            cmbDevType.SelectedIndex = 1;
            cmbBaudRate.SelectedIndex = 0;
            LEDSender.Do_LED_Startup();
            //LEDSender.Do_LED_CloseDeviceOnTerminate(1);

            ptr = (UInt32)this.Handle;
            fontName = ConfigurationManager.AppSettings["FontName"];
            fontColor = Convert.ToInt32(ConfigurationManager.AppSettings["FontColor"], 16);
            fontSize = Convert.ToInt32(ConfigurationManager.AppSettings["FontSize"]);
            fontStyle = Convert.ToInt32(ConfigurationManager.AppSettings["FontStyle"]);
            var position = ConfigurationManager.AppSettings["Position"].Split(',');
            rectText = new Rectangle(
                Convert.ToInt32(position[0]),
                Convert.ToInt32(position[1]),
                Convert.ToInt32(position[2]),
                Convert.ToInt32(position[3]));
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            LEDSender.Do_LED_Cleanup();
        }

        private void btnPowerOn_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_SetPower(ref param, 1));
        }

        private void btnPowerOff_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_SetPower(ref param, 0));
        }

        private void btnGetPower_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_GetPower(ref param));
        }

        private void btnSetBright_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_SetBright(ref param, 4));
        }

        private void btnGetBright_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_GetBright(ref param));
        }

        private void btnAdjustTime_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_AdjustTime(ref param));
        }

        private void btnGetDisplay_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_GetPlayContent(ref param));
        }

        private void btnChapterEx_Click(object sender, EventArgs e)
        {
            TTimeStamp fromtime = new TTimeStamp();
            TTimeStamp totime = new TTimeStamp();
            TSystemTime T = new TSystemTime();

            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            //此例程，按照每周周一、周三的8点到13点播放该节目
            //开始时间 08:00:00  当按照每日时间播放时，fromtime和totime中的日期部分将被忽略掉 
            EncodeDateTime(ref T, 2013, 1, 1, 8, 0, 0, 0);
            SystemTimeToTimeStamp(ref T, ref fromtime);
            //结束时间 13:00:00
            EncodeDateTime(ref T, 2013, 1, 1, 12, 30, 0, 0);
            SystemTimeToTimeStamp(ref T, ref totime);
            //LEDSender.Do_AddChapterEx(K, 30000, LEDSender.WAIT_CHILD, 1, (ushort)(LEDSender.CS_MON + LEDSender.CS_WED), ref fromtime, ref totime);
            //LEDSender.Do_AddChapterEx(K, 30000, LEDSender.WAIT_CHILD, 1, 127, ref fromtime, ref totime);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //自动换行的文字
            LEDSender.Do_AddTextEx(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 0, 6, 5, 1, 5, 0, 0, 3000);

            //此例程，按照每周周一、周三的8点到13点播放该节目
            //开始时间 08:00:00  当按照每日时间播放时，fromtime和totime中的日期部分将被忽略掉 
            EncodeDateTime(ref T, 2013, 1, 1, 12, 31, 0, 0);
            SystemTimeToTimeStamp(ref T, ref fromtime);
            //结束时间 13:00:00
            EncodeDateTime(ref T, 2013, 1, 1, 12, 35, 0, 0);
            SystemTimeToTimeStamp(ref T, ref totime);
            //LEDSender.Do_AddChapterEx(K, 30000, LEDSender.WAIT_CHILD, 1, (ushort)(LEDSender.CS_MON + LEDSender.CS_WED), ref fromtime, ref totime);
            //LEDSender.Do_AddChapterEx(K, 30000, LEDSender.WAIT_CHILD, 1, 127, ref fromtime, ref totime);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //自动换行的文字
            LEDSender.Do_AddTextEx(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff00, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 0, 6, 5, 1, 5, 0, 0, 3000);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnText_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //自动换行的文字
            LEDSender.Do_AddTextEx(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 0, 6, 5, 1, 5, 0, 0, 3000);
            //非自动换行的文字
            //LEDSender.Do_AddTextEx(K, 0, 16, 64, 32, LEDSender.V_TRUE, 0, "Hello world! Hello world! Hello World!", "宋体", 12, 0xff00, LEDSender.WFS_NONE, LEDSender.V_FALSE, 0, 0, 0, 7, 5, 1, 5, 0, 0, 3000);

            //第2页面
            //LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //非自动换行的文字(此函数可支持纵向显示)
            //LEDSender.Do_AddTextEx(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world!", "宋体", 12, 0xffff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 0, 0, 5, 0, 5, 0, 0, 5000);

            //Parse(LEDSender.Do_LED_SendToScreen(ref param, K));

            //此接口用于预览生成节目的显示效果
            LEDSender.Do_LED_Preview(K, 160, 128, "C:\\play.dat");
        }

        private void btnTextNoStruct_Click(object sender, EventArgs e)
        {
            Int32 param_index = 0;
            ushort K;

            GetDeviceParamWithoutStruct(param_index, (Int32)LEDSender.NOTIFY_EVENT, (Int32)Handle, WM_LED_NOTIFY);

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 160, 128, 0);
            //添加表头
            LEDSender.Do_AddLeaf(K, 10000000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, 0, 0, 64, 16, 1, 0, "目的地        货物  数量  车长", "宋体", 12, 255, 0, 0, 0, 1, 5, 1, 5, 0, 0, 1000);
            LEDSender.Do_AddWindows(K, 0, 16, 64, 48, 1, 0);
            LEDSender.Do_AddChildText(K, "第1行\r\n第2行\r\n第3行\r\n第4行\r\n第5行", "宋体", 12, 255, 0, 0, 0, 3, 5, 0, 5, 0, 0, 20000);
            LEDSender.Do_AddChildText(K, "第6行\r\n第7行\r\n第8行\r\n第9行\r\n第10行", "宋体", 12, 255, 0, 0, 0, 3, 5, 0, 5, 0, 0, 20000);


            Parse(LEDSender.Do_LED_SendToScreen2(param_index, K));
        }

        private void btnDib_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;
            Graphics G;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            G = Graphics.FromHwnd(pictureBox.Handle);
            LEDSender.Do_AddWindow(K, 0, 0, 64, 64, LEDSender.V_TRUE, 0, (uint)G.GetHdc(), pictureBox.Image.Width, pictureBox.Image.Height, 0, 1, 1, 1, 1, 1, 0, 3000);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnPicFile_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddPicture(K, 0, 0, 128, 16, LEDSender.V_TRUE, 0, "C:\\Demo.bmp", 0, 2, 1, 1, 1, 1, 0, 1000);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnString_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //16点阵字体
            LEDSender.Do_AddString(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! Hello world! Hello World!", LEDSender.FONT_SET_16, 0xff, 0, 1, 1, 1, 0, 1, 1000);

            //第2页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //24点阵字体
            LEDSender.Do_AddString(K, 0, 0, 64, 32, LEDSender.V_TRUE, 0, "Hello world! Hello world! Hello World!", LEDSender.FONT_SET_24, 0xff00, 3, 1, 1, 1, 1, 1, 1000);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnDateTime_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddDateTimeEx(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, 0, "Times New Roman", 12, 0xffff, LEDSender.WFS_NONE, 0, 0, 0, 0, "#y-#m-#d");

            //第2页面
            //LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //此函数支持纵向显示
            //LEDSender.Do_AddDateTimeEx(K, 0, 16, 64, 32, LEDSender.V_TRUE, 0, 0, "Times New Roman", 12, 0xff00, LEDSender.WFS_NONE, 0, 0, 0, 0, "#h:#n:#s");

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnCountUp_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;
            TTimeStamp basetime = new TTimeStamp();
            TSystemTime T = new TSystemTime();
            DateTime dtNow = System.DateTime.Now;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);

            //目标时间正计时
            //    是指以指定的时间，开始计时。
            //    比如当前是2012-5-18 17:41:30，你调用函数时，指定的目标时间为2012-5-18 16:41:30，那么就是相对于2012-5-18 16:41:30这个时间的正计时，
            //    显示效果：01:00:00/01:00:01/01:00:02/......
            EncodeDateTime(ref T, (ushort)dtNow.Year, (ushort)dtNow.Month, (ushort)dtNow.Day, 1, 0, 0, 0);
            SystemTimeToTimeStamp(ref T, ref basetime);
            LEDSender.Do_AddCounter(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, LEDSender.CT_COUNTUP, LEDSender.CF_HNS, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, ref basetime);

            //普通正计时
            //    从00:00:00开始计时，到指定的时间停止计时
            //    显示效果：00:00:00/00:00:01/00:00:02/....../01:28:59/01:29:00
            EncodeDateTime(ref T, 1899, 1, 1, 1, 29, 0, 0);
            SystemTimeToTimeStamp(ref T, ref basetime);
            LEDSender.Do_AddCounter(K, 0, 16, 64, 16, LEDSender.V_TRUE, 0, LEDSender.CT_COUNTUP_EX, LEDSender.CF_HNS, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, ref basetime);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnCountDown_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;
            TTimeStamp basetime = new TTimeStamp();
            TSystemTime T = new TSystemTime();
            DateTime dtNow = System.DateTime.Now;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);

            //目标时间倒计时
            //    是指以指定的时间为目标，进行倒计时。
            //    比如当前是2012-5-18 17:41:30，你调用函数时，指定的目标时间为2012-5-18 19:41:30，那么就是相对于2012-5-18 19:41:30这个时间的倒计时，
            //    显示效果：02:00:00/01:59:59/01:59:58/......
            EncodeDateTime(ref T, (ushort)dtNow.Year, (ushort)dtNow.Month, (ushort)dtNow.Day, 23, 59, 59, 0);
            SystemTimeToTimeStamp(ref T, ref basetime);
            LEDSender.Do_AddCounter(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, LEDSender.CT_COUNTDOWN, LEDSender.CF_HNS, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, ref basetime);

            //普通倒计时
            //    从指定的时间开始计时，到00:00:00停止计时
            //    显示效果：01:29:00/01:28:59/01:28:58/....../00:00:01/00:00:00
            EncodeDateTime(ref T, 1899, 1, 1, 1, 29, 0, 0);
            SystemTimeToTimeStamp(ref T, ref basetime);
            LEDSender.Do_AddCounter(K, 0, 16, 64, 16, LEDSender.V_TRUE, 0, LEDSender.CT_COUNTDOWN_EX, LEDSender.CF_HNS, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, ref basetime);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnCampaign_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;
            TTimeStamp basetime = new TTimeStamp();
            TTimeStamp fromtime = new TTimeStamp();
            TTimeStamp totime = new TTimeStamp();
            TSystemTime T = new TSystemTime();

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddDateTime(K, 0, 0, 256, 16, LEDSender.V_TRUE, 0, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, 0, 0, 0, 0, "#y-#m-#d");

            //作战时间 2020-06-01 07:00:00
            EncodeDateTime(ref T, 2020, 6, 1, 7, 0, 0, 0);
            SystemTimeToTimeStamp(ref T, ref basetime);
            //开始时间 2012-05-02 09:00:00
            EncodeDateTime(ref T, 2012, 5, 2, 9, 0, 0, 0);
            SystemTimeToTimeStamp(ref T, ref fromtime);
            //结束时间 2012-05-02 10:00:00
            EncodeDateTime(ref T, 2012, 5, 2, 10, 0, 0, 0);
            SystemTimeToTimeStamp(ref T, ref totime);
            LEDSender.Do_AddCampaign(K, 0, 16, 256, 16, LEDSender.V_TRUE, 0, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, "#h-#n-#s", ref basetime, ref fromtime, ref totime, 1000);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnClock_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddClockEx2(K, 0, 0, 64, 64, LEDSender.V_TRUE, 0, 0, 0, 0x0, 0xffff, 1, LEDSender.SHAPE_ROUNDRECT, 30, 3, 0xff00, 2, 0xffff, 0, 1, 0xff, 3, 0xffff, 2, 0xff00, 1, 0xff);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnTemperature_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 64, 64, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddTemperature(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Times New Roman", 12, 0xffff, LEDSender.WFS_NONE);
            LEDSender.Do_AddHumidity(K, 0, 16, 64, 16, LEDSender.V_TRUE, 0, "Times New Roman", 12, 0xffff, LEDSender.WFS_NONE);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnGetTemperature_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Parse(LEDSender.Do_LED_GetTemperatureHumidity(ref param));
        }

        private void btnVsqFile_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            K = (ushort)LEDSender.Do_MakeFromVsqFile("D:\\MyWorks\\ACard2008\\MyPlayer\\demo.vsq", LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void Parse2(Int32 K)
        {
            TNotifyParam notifyparam = new TNotifyParam();
            if (K >= 0)
            {
                LEDSender.Do_LED_GetNotifyParam(ref notifyparam, K);

                if (notifyparam.notify == LEDSender.LM_TIMEOUT)
                {
                    Text = "命令执行超时";
                }
                else if (notifyparam.notify == LEDSender.LM_TX_COMPLETE)
                {
                    if (notifyparam.result == LEDSender.RESULT_FLASH)
                    {
                        Text = "数据传送完成，正在写入Flash";
                    }
                    else
                    {
                        Text = "数据传送完成";
                    }
                }
                else if (notifyparam.notify == LEDSender.LM_RESPOND)
                {
                    if (notifyparam.command == LEDSender.PKC_GET_POWER)
                    {
                        if (notifyparam.status == LEDSender.LED_POWER_ON) Text = "读取电源状态完成，当前为电源开启状态";
                        else if (notifyparam.status == LEDSender.LED_POWER_OFF) Text = "读取电源状态完成，当前为电源关闭状态";
                    }
                    else if (notifyparam.command == LEDSender.PKC_SET_POWER)
                    {
                        if (notifyparam.result == 99) Text = "当前为定时开关屏模式";
                        else if (notifyparam.status == LEDSender.LED_POWER_ON) Text = "设置电源状态完成，当前为电源开启状态";
                        else Text = "设置电源状态完成，当前为电源关闭状态";
                    }
                    else if (notifyparam.command == LEDSender.PKC_GET_BRIGHT)
                    {
                        Text = string.Format("读取亮度完成，当前亮度={0:D}", notifyparam.status);
                    }
                    else if (notifyparam.command == LEDSender.PKC_SET_BRIGHT)
                    {
                        if (notifyparam.result == 99) Text = "当前为定时亮度调节模式";
                        else Text = string.Format("设置亮度完成，当前亮度={0:D}", notifyparam.status);
                    }
                    else if (notifyparam.command == LEDSender.PKC_ADJUST_TIME)
                    {
                        Text = "校正显示屏时间完成";
                    }
                }
            }
            else if (K == LEDSender.R_DEVICE_INVALID) Text = "打开通讯设备失败(串口不存在、或者串口已被占用、或者网络端口被占用)";
            else if (K == LEDSender.R_DEVICE_BUSY) Text = "设备忙，正在通讯中...";
        }

        private void btnPowerOn2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SetPower(ref param, 1));
        }

        private void btnPowerOff2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SetPower(ref param, 0));
        }

        private void btnGetPower2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_GetPower(ref param));
        }

        private void btnSetBright2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SetBright(ref param, 4));
        }

        private void btnGetBright2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_GetBright(ref param));
        }

        private void btnAdjustTime2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_AdjustTime(ref param));
        }

        private void btnText2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //自动换行的文字
            LEDSender.Do_AddText(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 1, 1, 1, 0, 1, 3);
            //非自动换行的文字
            LEDSender.Do_AddText(K, 0, 16, 64, 32, LEDSender.V_TRUE, 0, "Hello world! Hello world! Hello World!", "宋体", 12, 0xff00, LEDSender.WFS_NONE, LEDSender.V_FALSE, 0, 2, 1, 1, 1, 1, 1, 3);

            //第2页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //非自动换行的文字
            LEDSender.Do_AddText(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world!", "宋体", 12, 0xffff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 1, 1, 1, 1, 1, 5);

            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnDib2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //LEDSender.Do_AddWindow(K, 0, 0, 64, 16, LEDSender.V_TRUE, pictureBox.Handle, pictureBox.Image.Width, pictureBox.Image.Height, 0, 0, 1, 1, 1, 0, 1, 3);

            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnPicFile2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddPicture(K, 0, 0, 128, 16, LEDSender.V_TRUE, 0, "C:\\Demo.bmp", 0, 2, 1, 1, 1, 1, 0, 1000);

            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnString2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //16点阵字体
            LEDSender.Do_AddString(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! Hello world! Hello World!", LEDSender.FONT_SET_16, 0xff, 0, 1, 1, 1, 0, 1, 1000);

            //第2页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            //24点阵字体
            LEDSender.Do_AddString(K, 0, 0, 64, 32, LEDSender.V_TRUE, 0, "Hello world! Hello world! Hello World!", LEDSender.FONT_SET_24, 0xff00, 3, 1, 1, 1, 1, 1, 1000);

            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnDateTime2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddDateTime(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Times New Roman", 12, 0xff, LEDSender.WFS_NONE, 0, 0, 0, 0, "#y-#m-#d");

            //第2页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddDateTime(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Times New Roman", 12, 0xff00, LEDSender.WFS_NONE, 0, 0, 0, 0, "#h:#n:#s");

            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnClock2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddClock(K, 0, 0, 64, 64, LEDSender.V_TRUE, 0, 0, 0x0, 0xffff, 1, LEDSender.SHAPE_ROUNDRECT, 30, 3, 0xff00, 2, 0xffff, 3, 0xffff, 2, 0xff00, 1, 0xff);

            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnVsqFile2_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            K = (ushort)LEDSender.Do_MakeFromVsqFile("D:\\MyWorks\\ACard2008\\MyPlayer\\demo.vsq", LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);

            Text = "正在执行命令或者发送数据...";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnChapter_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            //这个操作中，ChapterIndex=0，只更新控制卡内第1个节目
            //如果ChapterIndex=1，只更新控制卡内第2个节目
            //以此类推
            K = (ushort)LEDSender.Do_MakeChapter(LEDSender.ROOT_PLAY_CHAPTER, LEDSender.ACTMODE_REPLACE, 0, LEDSender.COLOR_MODE_DOUBLE, 5000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 1, 1, 1, 0, 1, 3);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnRegion_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            //这个操作中，ChapterIndex=0，RegionIndex=0，只更新控制卡内第1个节目中的第1个分区
            //如果ChapterIndex=1，RegionIndex=2，只更新控制卡内第2个节目中的第3个分区
            //以此类推
            K = (ushort)LEDSender.Do_MakeRegion(LEDSender.ROOT_PLAY_REGION, LEDSender.ACTMODE_REPLACE, 0, 0, LEDSender.COLOR_MODE_DOUBLE, 0, 0, 128, 32, 0);
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 1, 1, 1, 0, 1, 3);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnLeaf_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            //这个操作中，ChapterIndex=0，RegionIndex=0，LeafIndex=0，只更新控制卡内第1个节目中的第1个分区中的第1个页面
            //如果ChapterIndex=1，RegionIndex=2，LeafIndex=1，只更新控制卡内第2个节目中的第3个分区中的第2个页面
            //以此类推
            K = (ushort)LEDSender.Do_MakeLeaf(LEDSender.ROOT_PLAY_LEAF, LEDSender.ACTMODE_REPLACE, 0, 0, 0, LEDSender.COLOR_MODE_DOUBLE, 5000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, 0, 0, 64, 16, LEDSender.V_TRUE, 0, "Hello world! HELLO WORLD!", "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 0, 1, 1, 1, 0, 1, 3);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnOnlineServerStartup_Click(object sender, EventArgs e)
        {
            if (LEDSender.Do_LED_Report_CreateServer(0, 8888) > 0)
            {
                Text = "在线控制卡监听服务启动成功，在8888端口进行监听";
            }
            else
            {
                Text = "在线控制卡监听服务已启动，8888端口当前被占用，请检查是否有其它应用程序使用该端口";
            }
            //可以创建多个监听服务，例如继续调用LED_Report_CreateServer(1, 8889);
            //则表示创建了两个监听，一个在8888端口，一个在8889端口
        }

        private void btnOnlineServerCleanup_Click(object sender, EventArgs e)
        {
            LEDSender.Do_LED_Report_RemoveServer(0);
            //或者调用 LED_Report_RemoveAllServer();
        }

        private void btnOnlineGetList_Click(object sender, EventArgs e)
        {
            int devcount;
            int I;
            String S;

            //将在线控制卡列表保存在动态链接库的缓冲区中，然后调用相应接口读取详细信息
            devcount = LEDSender.Do_LED_Report_GetOnlineList(0);

            S = "在线控制卡数量=" + Convert.ToString(devcount) + "\r\n名称 IP地址 端口 硬件地址\r\n";
            for (I = 0; I < devcount; I++)
            {
                S = S + LEDSender.Do_LED_Report_GetOnlineItemName(0, I) + " " +
                        LEDSender.Do_LED_Report_GetOnlineItemHost(0, I) + " " +
                        Convert.ToString(LEDSender.Do_LED_Report_GetOnlineItemPort(0, I)) + " " +
                        Convert.ToString(LEDSender.Do_LED_Report_GetOnlineItemAddr(0, I)) + "\r\n";
            }

            System.Windows.Forms.MessageBox.Show(S);
        }
        int i = 322;
        private void btnObject_Click(object sender, EventArgs e)
        {
            i++;
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            //这个操作中，ChapterIndex=0，RegionIndex=0，LeafIndex=0，ObjectIndex=0 只更新控制卡内第1个节目中的第1个分区中的第1个页面中的第1个对象
            //如果ChapterIndex=1，RegionIndex=2，LeafIndex=1，ObjectIndex=2只更新控制卡内第2个节目中的第3个分区中的第2个页面中的第3个对象
            //以此类推
            K = (ushort)LEDSender.Do_MakeObject(LEDSender.ROOT_PLAY_OBJECT, LEDSender.ACTMODE_REPLACE, 0, 0, 0, 3, LEDSender.COLOR_MODE_DOUBLE);
            LEDSender.Do_AddText(K, 85, 29, 128, 20, LEDSender.V_TRUE, 0, "请C" + i + "号", "宋体", 12, 0x0000FF, LEDSender.WFS_BOLD, LEDSender.V_FALSE, 0, 1, 5, 1, 5, 0, 1, 3);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnTimerTest_Click(object sender, EventArgs e)
        {
            if (timerMain.Enabled)
            {
                timerMain.Enabled = false;
                btnTimerTest.Text = "启动定时发送测试";
            }
            else
            {
                timerMain.Interval = 5000;
                timerMain.Enabled = true;
                btnTimerTest.Text = "停止定时发送测试";
            }
        }

        UInt32 serialno = 0;

        private void timerMain_Tick(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort I, K;
            string text;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            for (I = 166; I <= 169; I++)
            {
                param.devParam.rmtHost = "192.168.1." + I;
                K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
                LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
                LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);
                LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);
                text = "IP=" + I + "\r\n" + "序列号=" + serialno;
                serialno++;
                LEDSender.Do_AddText(K, 0, 0, 256, 64, LEDSender.V_TRUE, 0, text, "宋体", 12, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 5, 1, 1, 0, 1, 3000);
                Text = "<" + param.devParam.rmtHost + ">正在发送数据...";
                Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
            }
        }

        private void btnTable_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);
            LEDSender.Do_AddChapter(K, 30000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddRegion(K, 0, 0, 128, 32, 0);

            //第1页面
            LEDSender.Do_AddLeaf(K, 10000, LEDSender.WAIT_CHILD);

            LEDSender.Do_AddTable(K, 0, 0, 256, 64, LEDSender.V_TRUE, "TableParamDemo.ini", "aaa|bbb|ccc\r\n111|222|333", 1, 5, 1, 1, 0, 1, 3000);

            Parse(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        private void btnBoardParam_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = (UInt32)WM_LED_NOTIFY;
            Text = "正在执行命令或者发送数据...";
            if (LEDSender.Do_LED_Cache_GetBoardParam(ref param) >= 0)
            {
                LEDSender.Do_LED_Cache_SetBoardParam_IP("192.168.0.99");
                LEDSender.Do_LED_Cache_SetBoardParam_Mac("01-01-F1-DE-1A-02");
                LEDSender.Do_LED_Cache_SetBoardParam_Addr(0);
                LEDSender.Do_LED_Cache_SetBoardParam_Width(256);
                LEDSender.Do_LED_Cache_SetBoardParam_Height(64);
                LEDSender.Do_LED_Cache_SetBoardParam_Brightness(7);
                Parse2(LEDSender.Do_LED_Cache_SetBoardParam(ref param));
            }
            else
            {
                Text = "读取控制卡参数失败";
            }
        }

        private void btnMultiCard_Click(object sender, EventArgs e)
        {
            TSenderParam param = new TSenderParam();
            ushort K;
            int x, y, cx, cy;

            GetDeviceParam(ref param.devParam);
            param.notifyMode = LEDSender.NOTIFY_BLOCK;
            param.wmHandle = (UInt32)Handle;
            param.wmMessage = WM_LED_NOTIFY;

            x = 0;
            y = 0;
            cx = 320;
            cy = 176;

            //----------------------------

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);

            LEDSender.Do_AddChapter(K, 1000, LEDSender.WAIT_USE_TIME);
            LEDSender.Do_AddRegion(K, x, y, cx, cy, 0);
            LEDSender.Do_AddLeaf(K, 1000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, x, y, cx, cy, LEDSender.V_TRUE, 0, "行人通过马路时，请注意安全。珍惜生命，严禁酒后驾车。开车时请您系好安全带。", "宋体", 24, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 1, 1, 1, 0, 1, 3);

            LEDSender.Do_AddChapter(K, 1000, LEDSender.WAIT_USE_TIME);
            LEDSender.Do_AddRegion(K, x, y, cx, cy, 0);
            LEDSender.Do_AddLeaf(K, 1000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, x, y, cx, cy, LEDSender.V_TRUE, 0, "行人通过马路时，请注意安全。珍惜生命，严禁酒后驾车。开车时请您系好安全带。", "宋体", 24, 0xff00, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 1, 1, 1, 0, 1, 3);

            Text = "正在发送1...";
            param.devParam.rmtHost = "43.35.162.36";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));

            //----------------------------

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);

            LEDSender.Do_AddChapter(K, 1000, LEDSender.WAIT_USE_TIME);
            LEDSender.Do_AddRegion(K, x, y, cx, cy, 0);
            LEDSender.Do_AddLeaf(K, 1000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, x, y, cx, cy, LEDSender.V_TRUE, 0, "行人通过马路时，请注意安全。珍惜生命，严禁酒后驾车。开车时请您系好安全带。", "宋体", 24, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 1, 1, 1, 0, 1, 3);

            LEDSender.Do_AddChapter(K, 1000, LEDSender.WAIT_USE_TIME);
            LEDSender.Do_AddRegion(K, x, y, cx, cy, 0);
            LEDSender.Do_AddLeaf(K, 1000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, x, y, cx, cy, LEDSender.V_TRUE, 0, "行人通过马路时，请注意安全。珍惜生命，严禁酒后驾车。开车时请您系好安全带。", "宋体", 24, 0xff00, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 1, 1, 1, 0, 1, 3);

            Text = "正在发送2...";
            param.devParam.rmtHost = "43.35.162.37";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));

            //----------------------------

            K = (ushort)LEDSender.Do_MakeRoot(LEDSender.ROOT_PLAY, LEDSender.COLOR_MODE_DOUBLE, LEDSender.SURVIVE_ALWAYS);

            LEDSender.Do_AddChapter(K, 1000, LEDSender.WAIT_USE_TIME);
            LEDSender.Do_AddRegion(K, x, y, cx, cy, 0);
            LEDSender.Do_AddLeaf(K, 1000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, x, y, cx, cy, LEDSender.V_TRUE, 0, "行人通过马路时，请注意安全。珍惜生命，严禁酒后驾车。开车时请您系好安全带。", "宋体", 24, 0xff, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 1, 1, 1, 0, 1, 3);

            LEDSender.Do_AddChapter(K, 1000, LEDSender.WAIT_USE_TIME);
            LEDSender.Do_AddRegion(K, x, y, cx, cy, 0);
            LEDSender.Do_AddLeaf(K, 1000, LEDSender.WAIT_CHILD);
            LEDSender.Do_AddText(K, x, y, cx, cy, LEDSender.V_TRUE, 0, "行人通过马路时，请注意安全。珍惜生命，严禁酒后驾车。开车时请您系好安全带。", "宋体", 24, 0xff00, LEDSender.WFS_NONE, LEDSender.V_TRUE, 0, 1, 1, 1, 1, 0, 1, 3);

            Text = "正在发送3...";
            param.devParam.rmtHost = "43.35.162.35";
            Parse2(LEDSender.Do_LED_SendToScreen(ref param, K));
        }

        int fontColor = 0, fontSize = 0, fontStyle = 0;
        string fontName = "";
        Rectangle rectText = Rectangle.Empty;
        UInt32 ptr = 0;
        private void button1_Click(object sender, EventArgs e)
        {
            ptr = (UInt32)this.Handle;
            fontName = ConfigurationManager.AppSettings["FontName"];
            fontColor = Convert.ToInt32(ConfigurationManager.AppSettings["FontColor"], 16);
            fontSize = Convert.ToInt32(ConfigurationManager.AppSettings["FontSize"]);
            fontStyle = Convert.ToInt32(ConfigurationManager.AppSettings["FontStyle"]);
            var position = ConfigurationManager.AppSettings["Position"].Split(',');
            rectText = new Rectangle(
                Convert.ToInt32(position[0]),
                Convert.ToInt32(position[1]),
                Convert.ToInt32(position[2]),
                Convert.ToInt32(position[3]));
            //this.SendLEDMessage(ptr, this.eRemoteHost.Text, ushort.Parse(eLocalPort.Text), "0", "0,0,0,3", "请A003号");
            //new Thread(() =>
            //{ 
            //    this.SendLEDMessage(ptr, this.eRemoteHost.Text, ushort.Parse(eLocalPort.Text), "0", "0,0,0,3", "请A003号");
            //    Thread.Sleep(1000);
            //    this.SendLEDMessage(ptr, this.eRemoteHost.Text, ushort.Parse(eLocalPort.Text), "0", "0,0,0,3", " ");
            //    Thread.Sleep(1000);
            //    this.SendLEDMessage(ptr, this.eRemoteHost.Text, ushort.Parse(eLocalPort.Text), "0", "0,0,0,3", "请A003号");
            //    Thread.Sleep(1000);
            //    this.SendLEDMessage(ptr, this.eRemoteHost.Text, ushort.Parse(eLocalPort.Text), "0", "0,0,0,3", " ");
            //    Thread.Sleep(1000);
            //    this.SendLEDMessage(ptr, this.eRemoteHost.Text, ushort.Parse(eLocalPort.Text), "0", "0,0,0,3", "请A003号");
            //})
            //{ IsBackground = true }.Start();
        }

        int SendLEDMessage(string ip, ushort port, string deviceAddr, string position, string text, bool isFlash)
        {
            var strArr = position.Split(',');
            int ChapterIndex = Convert.ToInt32(strArr[0]),
                RegionIndex = Convert.ToInt32(strArr[1]),
                LeafIndex = Convert.ToInt32(strArr[2]),
                ObjectIndex = Convert.ToInt32(strArr[3]);
            TSenderParam param = new TSenderParam();
            param.devParam.devType = LEDSender.DEVICE_TYPE_UDP;
            param.devParam.rmtHost = ip;
            param.devParam.locPort = port;
            param.devParam.rmtPort = 6666;
            param.devParam.dstAddr = ushort.Parse(deviceAddr);
            param.notifyMode = LEDSender.NOTIFY_EVENT;
            param.wmHandle = this.ptr;
            param.wmMessage = WM_LED_NOTIFY;
            //这个操作中，ChapterIndex=0，RegionIndex=0，LeafIndex=0，ObjectIndex=0 只更新控制卡内第1个节目中的第1个分区中的第1个页面中的第1个对象
            //如果ChapterIndex=1，RegionIndex=2，LeafIndex=1，ObjectIndex=2只更新控制卡内第2个节目中的第3个分区中的第2个页面中的第3个对象
            //以此类推 
            ushort K = (ushort)LEDSender.Do_MakeObject(LEDSender.ROOT_PLAY_OBJECT, LEDSender.ACTMODE_REPLACE,
                ChapterIndex, RegionIndex, LeafIndex, ObjectIndex,
                LEDSender.COLOR_MODE_FULLCOLOR
                );

            LEDSender.Do_AddText(K, rectText.Left, rectText.Top, rectText.Width, rectText.Height, LEDSender.V_TRUE, 0,
                text, fontName, fontSize, fontColor, fontStyle, 0, 1,
                1, 5, 1, 5, 0, 1000, 10000);
            //CLEDSender.AddTextEx3(K, rectText.Left, rectText.Top, rectText.Width, rectText.Height, LEDSender.V_TRUE, 0,
            //     text, "宋体", 24, 0xff, LEDSender.WFS_NONE, 0, 1, LEDSender.V_TRUE, 0, 0, 0, 1,
            //     1, 5, 1, 5, 0, 1000, 10000);

            //CLEDSender.AddTextEx3(K, rectText.Left, rectText.Top, rectText.Width, rectText.Height, LEDSender.V_TRUE, 0,
            //     text, fontName, fontSize, fontColor, fontStyle, 0, LEDSender.V_TRUE, 0, 0, 1, 0, LEDSender.V_TRUE,
            //     1, 5, 1, 5, 0, 1000, 10000);

            var result = LEDSender.Do_LED_SendToScreen(ref param, K);
            return result;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.SendLEDMessage(txtip.Text, ushort.Parse(txtport.Text), "0", txtposition.Text.Trim(), "请A003号测试 ", false);
        }

    }
}
