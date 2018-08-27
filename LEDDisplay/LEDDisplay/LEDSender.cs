using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices; // 用 DllImport 需用此 命名空间

namespace LEDDisplay
{
    //设备通讯参数
    public struct TDeviceParam
    {
        public ushort devType;
        public ushort comSpeed;
        public ushort comPort;
        public ushort comFlow;
        public ushort locPort;
        public ushort rmtPort;
        public ushort srcAddr;
        public ushort dstAddr;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public String rmtHost;
        public uint txTimeo;   //发送后等待应答时间 ====超时时间应为txTimeo*txRepeat
        public uint txRepeat;  //失败重发次数
        public uint txMovewin; //划动窗口
        public uint key;       //通讯密钥
        public int pkpLength;  //数据包大小

    }

    //设备控制参数
    public struct TSenderParam
    {
        public TDeviceParam devParam;
        public UInt32 wmHandle;
        public UInt32 wmMessage;
        public UInt32 wmLParam;
        public UInt32 notifyMode;
    }

    //设备应答消息参数
    public struct TNotifyParam
    {
        public ushort notify;
        public ushort command;
        public Int32 result;
        public Int32 status;
        public TSenderParam param;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] buffer;
        public UInt32 size;
    }

    //SYSTEMTIME型日期时间结构
    public struct TSystemTime
    {
        public ushort wYear;
        public ushort wMonth;
        public ushort wDayOfWeek;
        public ushort wDay;
        public ushort wHour;
        public ushort wMinute;
        public ushort wSecond;
        public ushort wMilliseconds;
    }

    //TimeStamp型日期时间结构
    public struct TTimeStamp
    {
        public Int32 time;
        public Int32 date;
    }

    public struct TPowerSchedule
    {
        public UInt32 Enabled;
        public UInt32 Mode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public TTimeStamp[] OpenTime;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 21)]
        public TTimeStamp[] CloseTime;
        public UInt32 Checksum;
    }

    public struct TBrightSchedule
    {
        public UInt32 Enabled;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] Bright;
        public UInt32 Checksum;
    }

    class CLEDSender
    {
        //通讯设备类型
        public readonly ushort DEVICE_TYPE_COM = 0;  //串口通讯
        public readonly ushort DEVICE_TYPE_UDP = 1;  //网络通讯
        public readonly ushort DEVICE_TYPE_485 = 2;  //485通讯

        //串口或者485通讯使用得通讯速度(波特率)
        public readonly ushort SBR_57600 = 0;
        public readonly ushort SBR_38400 = 1;
        public readonly ushort SBR_19200 = 2;
        public readonly ushort SBR_9600  = 3;

        //是否等待下位机应答，直接发送所有数据
        public readonly UInt32 NOTIFY_NONE  = 1;
        //是否阻塞方式；是则等到发送完成或者超时，才返回；否则立即返回
        public readonly UInt32 NOTIFY_BLOCK = 2;
        //多线程异步方式，一个本地端口发一个屏
        public readonly UInt32 NOTIFY_EVENT = 4;
        //一个本地端口并行发送多个屏
        public readonly UInt32 NOTIFY_MULTI = 8;

        public readonly Int32 R_DEVICE_READY    = 0;
        public readonly Int32 R_DEVICE_INVALID  = -1;
        public readonly Int32 R_DEVICE_BUSY     = -2;
        public readonly Int32 R_FONTSET_INVALID = -3;
        public readonly Int32 R_DLL_INIT_IVALID = -4;
        public readonly Int32 R_IGNORE_RESPOND  = -5;

        //下位机应答标识
        public readonly ushort LM_RX_COMPLETE = 1;
        public readonly ushort LM_TX_COMPLETE = 2;
        public readonly ushort LM_RESPOND     = 3;
        public readonly ushort LM_TIMEOUT     = 4;
        public readonly ushort LM_NOTIFY      = 5;
        public readonly ushort LM_PARAM       = 6;
        public readonly ushort LM_TX_PROGRESS = 7;
        public readonly ushort LM_RX_PROGRESS = 8;
        public readonly ushort RESULT_FLASH = 0xff;

        public readonly Int32 NOTIFY_ROOT_UPDATE          = 0x00010001;  //在线更新控制卡程序
        public readonly Int32 NOTIFY_ROOT_UPDATE_ERROR    = 0x00020001;  //下载字库失败
        public readonly Int32 NOTIFY_ROOT_FONTSET         = 0x00010002;  //下载字库成功
        public readonly Int32 NOTIFY_ROOT_FONTSET_ERROR   = 0x00020002;  //下载字库失败
        public readonly Int32 NOTIFY_ROOT_DOWNLOAD        = 0x00010003;  //更新Flash播放节目
        public readonly Int32 NOTIFY_SET_PARAM            = 0x00010004;  //设置参数
        public readonly Int32 NOTIFY_SET_POWER_SCHEDULE   = 0x00010005;  //设置定时开关屏
        public readonly Int32 NOTIFY_SET_BRIGHT_SCHEDULE  = 0x00010006;  //设置定时亮度调节
        public readonly Int32 NOTIFY_SET_POWER_FLASH      = 0x0001000a;  //设置电源状态并保存到Flash
        public readonly Int32 NOTIFY_SET_BRIGHT_FLASH     = 0x0001000b;  //设置亮度并保存到Flash
        public readonly Int32 NOTIFY_GET_PLAY_BUFFER      = 0x00011001;  //读取控制卡显示内容

        //命令代码定义
        public readonly ushort PKC_RESPOND     = 3;
        public readonly ushort PKC_QUERY       = 4;
        public readonly ushort PKC_OVERFLOW    = 5;
        public readonly ushort PKC_ADJUST_TIME = 6;
        public readonly ushort PKC_GET_PARAM   = 7;
        public readonly ushort PKC_SET_PARAM   = 8;
        public readonly ushort PKC_GET_POWER   = 9;
        public readonly ushort PKC_SET_POWER   = 10;
        public readonly ushort PKC_GET_BRIGHT  = 11;
        public readonly ushort PKC_SET_BRIGHT  = 12;
        public readonly ushort PKC_COM_TRANSFER = 21;
        public readonly ushort PKC_MODEM_TRANSFER = 22;
        public readonly ushort PKC_SET_EXSTRING = 29;
        public readonly ushort PKC_GET_TRANSFER_ACK = 33;
        public readonly ushort PKC_GET_TEMPERATURE_HUMIDITY = 24;
        public readonly ushort PKC_SET_POWER_SCHEDULE = 61;
        public readonly ushort PKC_SET_BRIGHT_SCHEDULE = 63;
        public readonly ushort PKC_GET_CHAPTER_COUNT = 66;
        public readonly ushort PKC_GET_CURRENT_CHAPTER = 67;
        public readonly ushort PKC_SET_CURRENT_CHAPTER = 68;

        //电源开关参数值
        public readonly Int32 LED_POWER_ON  = 1;
        public readonly Int32 LED_POWER_OFF = 0;

        //Chapter和Leaf中，播放时间控制
        public readonly ushort WAIT_USE_TIME = 0;  //按照指定的时间长度播放，到时间就切到下一个
        public readonly ushort WAIT_CHILD    = 1;  //等待子项目的播放，如果到了指定的时间长度，而子项目还没有播完，则等待播完

        //整形TRUE/FALSE值
        public readonly Int32 V_FALSE = 0;
        public readonly Int32 V_TRUE  = 1;

        //显示屏基色类型
        public readonly Int32 COLOR_MODE_MONO       = 1;  //单色
        public readonly Int32 COLOR_MODE_DOUBLE     = 2;  //双色
        public readonly Int32 COLOR_MODE_THREE      = 3;  //全彩无灰度
        public readonly Int32 COLOR_MODE_FULLCOLOR  = 4;  //全彩

        //显示数据命令
        public readonly Int32 ROOT_UPDATE       = 0x13;  //更新下位机程序
        public readonly Int32 ROOT_FONTSET      = 0x14;  //下载字库
        public readonly Int32 ROOT_PLAY         = 0x21;  //节目数据，保存到RAM，掉电丢失
        public readonly Int32 ROOT_DOWNLOAD     = 0x22;  //节目数据，保存到Flash
        public readonly Int32 ROOT_PLAY_CHAPTER = 0x23;  //插入或者更新某一节目
        public readonly Int32 ROOT_PLAY_REGION  = 0x25;  //插入或者更新某一区域/分区
        public readonly Int32 ROOT_PLAY_LEAF    = 0x27;  //插入或者更新某一页面
        public readonly Int32 ROOT_PLAY_OBJECT  = 0x29;  //插入或者更新某一对象

        public readonly Int32 ACTMODE_INSERT    = 0;     //插入操作
        public readonly Int32 ACTMODE_REPLACE   = 1;     //替换操作

        //RAM节目播放
        public readonly Int32 SURVIVE_ALWAYS = -1;

        //Windows字体类型定义
        public readonly Int32 WFS_NONE      = 0x0;   //普通样式
        public readonly Int32 WFS_BOLD      = 0x01;  //粗体
        public readonly Int32 WFS_ITALIC    = 0x02;  //斜体
        public readonly Int32 WFS_UNDERLINE = 0x04;  //下划线
        public readonly Int32 WFS_STRIKEOUT = 0x08;  //删除线

        //内码文字大小
        public readonly Int32 FONT_SET_16 = 0;      //16点阵字符
        public readonly Int32 FONT_SET_24 = 1;      //24点阵字符
  
        //正计时、倒计时type参数
        public readonly Int32 CT_COUNTUP = 0;        //目标正计时
        public readonly Int32 CT_COUNTDOWN = 1;      //目标倒计时
        public readonly Int32 CT_COUNTUP_EX = 2;     //普通正计时
        public readonly Int32 CT_COUNTDOWN_EX = 3;   //普通倒计时
        
        //正计时、倒计时format参数
        public readonly Int32 CF_HNS    = 0;      //时分秒（相对值）
        public readonly Int32 CF_HN     = 1;      //时分（相对值）
        public readonly Int32 CF_NS     = 2;      //分秒（相对值）
        public readonly Int32 CF_H      = 3;      //时（相对值）
        public readonly Int32 CF_N      = 4;      //分（相对值）
        public readonly Int32 CF_S      = 5;      //秒（相对值）
        public readonly Int32 CF_DAY    = 6;      //天数（绝对数量）
        public readonly Int32 CF_HOUR   = 7;      //小时数（绝对数量）
        public readonly Int32 CF_MINUTE = 8;     //分钟数（绝对数量）
        public readonly Int32 CF_SECOND = 9;      //秒数（绝对数量）

        //模拟时钟边框形状
        public readonly Int32 SHAPE_RECTANGLE = 0;      //方形
        public readonly Int32 SHAPE_ROUNDRECT = 1;      //圆角方形
        public readonly Int32 SHAPE_CIRCLE    = 2;      //圆形

        //节目播放计划一周有效日期定义
        public readonly ushort CS_SUN = 1;
        public readonly ushort CS_MON = 2;
        public readonly ushort CS_TUE = 4;
        public readonly ushort CS_WED = 8;
        public readonly ushort CS_THU = 16;
        public readonly ushort CS_FRI = 32;
        public readonly ushort CS_SAT = 64;
        public readonly ushort CS_EVERYDAY = 127;

        ////////////////////////////////////////////////////////////////////////////////////////////
        //接口函数导入
        ////////////////////////////////////////////////////////////////////////////////////////////

        //动态链接库初始化
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Startup();

        //动态链接库销毁
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cleanup();

        //创建控制卡在线监听服务
        //  serverindex 控制卡在线监听服务编号(可以在多个socket udp端口监听)
        //  localport 本地端口
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Report_CreateServer(Int32 serverindex, Int32 localport);

        //删除控制卡在线监听服务
        //  serverindex 控制卡在线监听服务编号
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Report_RemoveServer(Int32 serverindex);

        //删除全部控制卡在线监听服务
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Report_RemoveAllServer();

        //获得控制卡在线列表
        //必须先创建控制卡在线监听服务，即调用LED_Report_CreateServer后使用，否则返回值无效
        //  serverindex 控制卡在线监听服务编号
        //  plist 输出在线列表的用户外部缓冲区，
        //        如果传入空(NULL/0)，则输出到动态链接库内部的缓冲区，继续调用下面的接口取得详细信息
        //  count 最大读取个数
        //--返回值-- 小于0表示失败(未创建该在线监听服务)，大于等于0表示在线的控制卡数量
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Report_GetOnlineList(Int32 serverindex, Int32 listaddr, Int32 count);

        //获得某个在线控制卡的上报控制卡名称
        //必须先创建控制卡在线监听服务，即调用LED_Report_CreateServer后使用，否则返回值无效
        //  serverindex 控制卡在线监听服务编号
        //  itemindex 该监听服务的在线列表中，在线控制卡的编号
        //--返回值-- 在线控制卡的上报控制卡名称
        [DllImport("LEDSender2010.dll")]
        private static extern String LED_Report_GetOnlineItemName(Int32 serverindex, Int32 itemindex);

        //获得某个在线控制卡的上报控制卡IP地址
        //必须先创建控制卡在线监听服务，即调用LED_Report_CreateServer后使用，否则返回值无效
        //  serverindex 控制卡在线监听服务编号
        //  itemindex 该监听服务的在线列表中，在线控制卡的编号
        //--返回值-- 在线控制卡的IP地址
        [DllImport("LEDSender2010.dll")]
        private static extern String LED_Report_GetOnlineItemHost(Int32 serverindex, Int32 itemindex);

        //获得某个在线控制卡的上报控制卡远程UDP端口号
        //必须先创建控制卡在线监听服务，即调用LED_Report_CreateServer后使用，否则返回值无效
        //  serverindex 控制卡在线监听服务编号
        //  itemindex 该监听服务的在线列表中，在线控制卡的编号
        //--返回值-- 在线控制卡的远程UDP端口号
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Report_GetOnlineItemPort(Int32 serverindex, Int32 itemindex);

        //获得某个在线控制卡的上报控制卡地址
        //必须先创建控制卡在线监听服务，即调用LED_Report_CreateServer后使用，否则返回值无效
        //  serverindex 控制卡在线监听服务编号
        //  itemindex 该监听服务的在线列表中，在线控制卡的编号
        //--返回值-- 在线控制卡的硬件地址
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Report_GetOnlineItemAddr(Int32 serverindex, Int32 itemindex);

        //预览显示内容
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Preview(Int32 index, Int32 width, Int32 height, string previewfile);

        //预览显示内容
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_PreviewFile(Int32 width, Int32 height, string previewfile);

        //设置每次命令执行完成后，是否关闭设备。=0表示不关闭；=1表示关闭。
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_CloseDeviceOnTerminate(int AValue);

        //获取控制卡应答结果的数据
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetNotifyParam(ref TNotifyParam param, Int32 index);

        //获取控制卡应答结果的数据，并将附加数据保存到指定文件中
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetNotifyParam_BufferToFile(ref TNotifyParam param, String filename, int index);

        //复位控制卡节目播放，重新显示控制卡Flash中存储的节目
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_ResetDisplay(ref TSenderParam param);

        //重启控制卡
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Reboot(ref TSenderParam param);

        //读取控制卡温湿度值
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetTemperatureHumidity(ref TSenderParam param);

        //设置控制卡电源 value=LED_POWER_ON表示开启电源 value=LED_POWER_OFF表示关闭电源
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SetPower(ref TSenderParam param, Int32 value);

        //读取控制卡电源状态
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetPower(ref TSenderParam param);

        //设置控制卡亮度 value取值范围0-7
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SetBright(ref TSenderParam param, Int32 value);

        //读取控制卡亮度
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetBright(ref TSenderParam param);

        //校正时间，以当前计算机的系统时间校正控制卡的时钟
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_AdjustTime(ref TSenderParam param);

        //校正时间扩展，以指定的时间校正控制卡的时钟
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_AdjustTimeEx(ref TSenderParam param, ref TSystemTime time);

        //function LED_SetVarStringSingle(AParam: PSenderParam; AIndex: Integer; Str: PChar; Color: Integer): Integer; stdcall; external LedSender;
        //设置变量字符串
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SetVarStringSingle(ref TSenderParam param, Int32 index, string str, Int32 color);

        //设置定时开关屏
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SetPowerSchedule(ref TSenderParam param, ref TPowerSchedule schedule);

        //设置定时亮度调节
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SetBrightSchedule(ref TSenderParam param, ref TBrightSchedule schedule);

        //读取节目数量
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetChapterCount(ref TSenderParam param);

        //设置当前播放节目
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SetCurChapter(ref TSenderParam param, Int32 value);

        //读取当前播放节目
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetCurChapter(ref TSenderParam param);

        //发送节目数据 index为MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject函数的返回值
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SendToScreen(ref TSenderParam param, Int32 index);

        //直接发送字符串UDP包到显示屏
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SendStringDirectly(ref TSenderParam param, string text);

        //直接发送十六进制字符串UDP包到显示屏
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SendHexDirectly(ref TSenderParam param, string text);

        //232口转发协议数据（用于控制外接设备）
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_ComTransferHex(ref TSenderParam param, string text, Int32 delayaftertransfer);

        //485口转发协议数据（用于控制外接设备）
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_ModemTransferHex(ref TSenderParam param, string text, Int32 delayaftertransfer);

        //接收转发的应答数据（用于控制外接设备）
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetTransferAck(ref TSenderParam param);

        //设置UDP通讯的参数
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_UDP_SenderParam(Int32 param_index, Int32 locport, string host, Int32 rmtport, Int32 address, Int32 notifymode, Int32 wmhandle, Int32 wmmessage);

        //设置串口通讯的参数
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_COM_SenderParam(Int32 paramindex, Int32 comport, Int32 baudrate, Int32 address, Int32 notifymode, Int32 wmhandle, Int32 wmmessage);

        //发送节目数据 index为MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject函数的返回值
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_SendToScreen2(Int32 param_index, Int32 index);

        //压缩数据
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Compress(Int32 param_index);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //控制卡参数读取和设置相关API

        //读取控制卡参数，读取到的结果保存到动态链接库中
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam(ref TSenderParam param);

        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam2(Int32 param_index);

        //取得控制卡IP地址
        [DllImport("LEDSender2010.dll")]
        private static extern String LED_Cache_GetBoardParam_IP();

        //取得控制卡MAC地址
        [DllImport("LEDSender2010.dll")]
        private static extern String LED_Cache_GetBoardParam_Mac();

        //取得控制卡设备地址
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam_Addr();

        //取得控制卡设备宽度
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam_Width();

        //取得控制卡设备高度
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam_Height();

        //取得控制卡设备亮度
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam_Brightness();

        //取得控制卡设备单步时间
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_GetBoardParam_Frequency();

        //设置控制卡IP地址
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_IP(String value);

        //设置控制卡MAC地址
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_Mac(String value);

        //设置控制卡设备地址
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_Addr(Int32 value);

        //设置控制卡设备宽度
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_Width(Int32 value);

        //设置控制卡设备高度
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_Height(Int32 value);

        //设置控制卡设备亮度
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_Brightness(Int32 value);

        //设置控制卡设备单步时间
        [DllImport("LEDSender2010.dll")]
        private static extern void LED_Cache_SetBoardParam_Frequency(Int32 value);

        //设置控制卡参数
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_SetBoardParam(ref TSenderParam param);

        [DllImport("LEDSender2010.dll")]
        private static extern int LED_Cache_SetBoardParam2(Int32 param_index);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //读取控制卡当前显示内容相关API

        //读取控制卡当前显示内容
        [DllImport("LEDSender2010.dll")]
        private static extern int LED_GetPlayContent(ref TSenderParam param);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //节目数据组织形式
        //  ROOT
        //   |
        //   |---Chapter(节目)
        //   |      |
        //   |      |---Region(区域/分区)
        //   |      |     |
        //   |      |     |---Leaf(页面)
        //   |      |     |    |
        //   |      |     |    |---Object(对象[文字、时钟、图片等])
        //   |      |     |    |
        //   |      |     |    |---Object(对象[文字、时钟、图片等])
        //   |      |     |    |
        //   |      |     |    |   ......
        //   |      |     |    |
        //   |      |     |
        //   |      |     |---Leaf(页面)
        //   |      |     |
        //   |      |     |   ......
        //   |      |     |
        //   |      |
        //   |      |---Region(区域/分区)
        //   |      |
        //   |      |   ......
        //   |      |
        //   |---Chapter(节目)
        //   |
        //   |   ......

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //设置显示屏旋转 
        //  rotate 旋转方式 =0不旋转 =1逆时针旋转90度 =2顺时针旋转90度
        //  metrix_width 显示屏宽度
        //  metrix_height 显示屏高度
        [DllImport("LEDSender2010.dll")]
        private static extern int SetRotate(Int32 rotate, Int32 metrix_width, Int32 metrix_height);

        //生成节目数据（从VisionShow软件编辑的Vsq文件载入，并生成要下发的节目数据）
        //  RootType 为节目类型；=ROOT_PLAY表示更新控制卡RAM中的节目(掉电丢失)；=ROOT_DOWNLOAD表示更新控制卡Flash中的节目(掉电不丢失)
        //  ColorMode 为颜色模式；取值为COLOR_MODE_MONO或者COLOR
        //  survive 为RAM节目生存时间，在RootType=ROOT_PLAY时有效，当RAM节目播放达到时间后，恢复显示FLASH中的节目
        //  filename 由VisionShow软件编辑的节目文件
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeFromVsqFile(string filename, Int32 RootType, Int32 ColorMode, Int32 survive);

        //生成节目数据
        //  RootType 为节目类型；=ROOT_PLAY表示更新控制卡RAM中的节目(掉电丢失)；=ROOT_DOWNLOAD表示更新控制卡Flash中的节目(掉电不丢失)
        //  ColorMode 为颜色模式；取值为COLOR_MODE_MONO或者COLOR
        //  survive 为RAM节目生存时间，在RootType=ROOT_PLAY时有效，当RAM节目播放达到时间后，恢复显示FLASH中的节目
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeRoot(Int32 RootType, Int32 ColorMode, Int32 survive);

        //生成节目数据
        //  RootType 为节目类型；=ROOT_PLAY表示更新控制卡RAM中的节目(掉电丢失)；=ROOT_DOWNLOAD表示更新控制卡Flash中的节目(掉电不丢失)
        //  ColorMode 为颜色模式；取值为COLOR_MODE_MONO或者COLOR
        //  survive 为RAM节目生存时间，在RootType=ROOT_PLAY时有效，当RAM节目播放达到时间后，恢复显示FLASH中的节目
        //  rotate 旋转方式 =0不旋转 =1逆时针旋转90度 =2顺时针旋转90度
        //  metrix_width 显示屏宽度
        //  metrix_height 显示屏高度
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeRootEx(Int32 RootType, Int32 ColorMode, Int32 survive, Int32 rotate, Int32 metrix_width, Int32 metrix_height);

        //生成节目数据，后续需要调用[AddRegion]->[AddLeaf]->[AddObject]->[AddWindows/AddDateTime等]
        //  RootType 必须设为ROOT_PLAY_CHAPTER
        //  ActionMode 必须设为0
        //  ChapterIndex 要更新的节目序号
        //  ColorMode 同MakeRoot中的定义
        //  time 播放的时间长度
        //  wait 等待模式，=WAIT_CHILD，表示当达到播放时间长度时，需要等待子节目播放完成再切换；
        //                 =WAIT_USE_TIME，表示当达到播放时间长度时，不等待子节目播放完成，直接切换下一节目
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeChapter(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 ColorMode, UInt32 time, ushort wait);

        //生成区域/分区，后续需要调用[AddLeaf]->[AddObject]->[AddWindows/AddDateTime等]
        //  RootType 必须设为ROOT_PLAY_REGION
        //  ActionMode 必须设为0
        //  ChapterIndex 要更新的节目序号
        //  RegionIndex 要更新的区域/分区序号
        //  ColorMode 同MakeRoot中的定义
        //  left、top、width、height 左、上、宽度、高度
        //  border 流水边框
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeRegion(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 RegionIndex, 
            Int32 ColorMode, Int32 left, Int32 top, Int32 width, Int32 height, Int32 border);

        //生成页面，后续需要调用[AddObject]->[AddWindows/AddDateTime等]
        //  RootType 必须设为ROOT_PLAY_LEAF
        //  ActionMode 必须设为0
        //  ChapterIndex 要更新的节目序号
        //  RegionIndex 要更新的区域/分区序号
        //  LeafIndex 要更新的页面序号
        //  ColorMode 同MakeRoot中的定义
        //  time 播放的时间长度
        //  wait 等待模式，=WAIT_CHILD，表示当达到播放时间长度时，需要等待子节目播放完成再切换；
        //                 =WAIT_USE_TIME，表示当达到播放时间长度时，不等待子节目播放完成，直接切换下一页面
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeLeaf(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 RegionIndex, Int32 LeafIndex, 
            Int32 ColorMode, UInt32 time, ushort wait);

        //生成播放对象，后续需要调用[AddWindows/AddDateTime等]
        //  RootType 必须设为ROOT_PLAY_LEAF
        //  ActionMode 必须设为0
        //  ChapterIndex 要更新的节目序号
        //  RegionIndex 要更新的区域/分区序号
        //  LeafIndex 要更新的页面序号
        //  ObjectIndex 要更新的对象序号
        //  ColorMode 同MakeRoot中的定义
        [DllImport("LEDSender2010.dll")]
        private static extern int MakeObject(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 RegionIndex, Int32 LeafIndex, Int32 ObjectIndex, Int32 ColorMode);

        //添加节目
        //  num 节目数据缓冲区编号，是MakeRoot的返回值
        //  time 播放的时间长度(单位为毫秒)
        //  wait 等待模式，=WAIT_CHILD，表示当达到播放时间长度时，需要等待子节目播放完成再切换；
        //                 =WAIT_USE_TIME，表示当达到播放时间长度时，不等待子节目播放完成，直接切换下一节目
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChapter(ushort num, UInt32 time, ushort wait);

        //添加节目
        //  num 节目数据缓冲区编号，是MakeRoot的返回值
        //  time 播放的时间长度(单位为毫秒)
        //  wait 等待模式，=WAIT_CHILD，表示当达到播放时间长度时，需要等待子节目播放完成再切换；
        //                 =WAIT_USE_TIME，表示当达到播放时间长度时，不等待子节目播放完成，直接切换下一节目
        //  priority 优先级，如果同时有不同优先级的节目，那么只播放高优先级的节目
        //  kind 播放计划类型，=0始终播放，=1按照一周每日时间播放，=2按照指定起止日期时间播放，=3不播放
        //  week 一周有效日期，bit0到bit6表示周日到周六有效，当kind=1时，本参数起作用
        //  fromtime 有效起始时间
        //  totime 有效结束时间
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChapterEx(ushort num, UInt32 time, ushort wait, ushort priority, ushort kind, ushort week, ref TTimeStamp fromtime, ref TTimeStamp totime);

        //添加区域/分区
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  border 流水边框
        [DllImport("LEDSender2010.dll")]
        private static extern int AddRegion(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 border);

        //添加页面
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion的返回值
        //  time 播放的时间长度(单位为毫秒)
        //  wait 等待模式，=WAIT_CHILD，表示当达到播放时间长度时，需要等待子节目播放完成再切换；
        //                 =WAIT_USE_TIME，表示当达到播放时间长度时，不等待子节目播放完成，直接切换下一页面
        [DllImport("LEDSender2010.dll")]
        private static extern int AddLeaf(ushort num, UInt32 time, ushort wait);

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //添加日期时间显示
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  year_offset 年偏移量
        //  month_offset 月偏移量
        //  day_offset 日偏移量
        //  sec_offset 秒偏移量
        //  format 显示格式 
        //      #y表示年 #m表示月 #d表示日 #h表示时 #n表示分 #s表示秒 #w表示星期 #c表示农历
        //      举例： format="#y年#m月#d日 #h时#n分#s秒 星期#w 农历#c"时，显示为"2009年06月27日 12时38分45秒 星期六 农历五月初五"
        [DllImport("LEDSender2010.dll")]
        private static extern int AddDateTime(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
		    string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, 
            Int32 year_offset, Int32 month_offset, Int32 day_offset, Int32 sec_offset, string format);

        //添加日期时间显示
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  vertical 是否纵向显示 =0正常 =1逆时针旋转纵向显示 =2顺时针旋转纵向显示
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  year_offset 年偏移量
        //  month_offset 月偏移量
        //  day_offset 日偏移量
        //  sec_offset 秒偏移量
        //  format 显示格式 
        //      #y表示年 #m表示月 #d表示日 #h表示时 #n表示分 #s表示秒 #w表示星期 #c表示农历
        //      举例： format="#y年#m月#d日 #h时#n分#s秒 星期#w 农历#c"时，显示为"2009年06月27日 12时38分45秒 星期六 农历五月初五"
        [DllImport("LEDSender2010.dll")]
        private static extern int AddDateTimeEx(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 vertical,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle,
            Int32 year_offset, Int32 month_offset, Int32 day_offset, Int32 sec_offset, string format);

        //添加作战时间显示
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  format 显示格式 
        //      #y表示年 #m表示月 #d表示日 #h表示时 #n表示分 #s表示秒 #w表示星期
        //  basetime 作战时间
        //  fromtime 开始时间
        //  totime 结束时间
        //  step 计时走秒时间步长（多少毫秒走一秒）
        [DllImport("LEDSender2010.dll")]
        private static extern int AddCampaignEx(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle,
            string format, ref TTimeStamp basetime, ref TTimeStamp fromtime, ref TTimeStamp totime, Int32 step);

        //添加倒计时显示
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  countertype 计时类型 0(CT_COUNTUP)为正计时，1(CT_COUNTDOWN)为倒计时
        //  format 显示格式 
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  basetime 作战时间
        [DllImport("LEDSender2010.dll")]
        private static extern int AddCounter(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 countertype, Int32 format, 
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, ref TTimeStamp basetime);

        //添加模拟时钟
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  offset 秒偏移量
        //  bkcolor: 背景颜色
        //  bordercolor: 边框颜色
        //  borderwidth: 边框颜色
        //  bordershape: 边框形状 =0表示正方形；=1表示圆角方形；=2表示圆形
        //  dotradius: 刻度距离表盘中心半径
        //  adotwidth: 0369点刻度大小
        //  adotcolor: 0369点刻度颜色
        //  bdotwidth: 其他点刻度大小
        //  bdotcolor: 其他点刻度颜色
        //  hourwidth: 时针粗细
        //  hourcolor: 时针颜色
        //  minutewidth: 分针粗细
        //  minutecolor: 分针颜色
        //  secondwidth: 秒针粗细
        //  secondcolor: 秒针颜色
        [DllImport("LEDSender2010.dll")]
        private static extern int AddClock(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 offset,
		    UInt32 bkcolor, UInt32 bordercolor, UInt32 borderwidth, Int32 bordershape,
		    Int32 dotradius, Int32 adotwidth, UInt32 adotcolor, Int32 bdotwidth, UInt32 bdotcolor,
            Int32 hourwidth, UInt32 hourcolor, Int32 minutewidth, UInt32 minutecolor, Int32 secondwidth, UInt32 secondcolor);

        //添加模拟时钟
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  vertical 是否纵向显示 =0正常 =1逆时针旋转纵向显示 =2顺时针旋转纵向显示
        //  offset 秒偏移量
        //  bkcolor: 背景颜色
        //  bordercolor: 边框颜色
        //  borderwidth: 边框颜色
        //  bordershape: 边框形状 =0表示正方形；=1表示圆角方形；=2表示圆形
        //  dotradius: 刻度距离表盘中心半径
        //  adotwidth: 0369点刻度大小
        //  adotcolor: 0369点刻度颜色
        //  bdotwidth: 其他点刻度大小
        //  bdotcolor: 其他点刻度颜色
        //  hourwidth: 时针粗细
        //  hourcolor: 时针颜色
        //  minutewidth: 分针粗细
        //  minutecolor: 分针颜色
        //  secondwidth: 秒针粗细
        //  secondcolor: 秒针颜色
        [DllImport("LEDSender2010.dll")]
        private static extern int AddClockEx2(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 vertical, Int32 offset,
            UInt32 bkcolor, UInt32 bordercolor, UInt32 borderwidth, Int32 bordershape,
            Int32 dotradius, Int32 adotwidth, UInt32 adotcolor, Int32 bdotwidth, UInt32 bdotcolor,
            UInt32 cdotvisible, Int32 cdotwidth, UInt32 cdotcolor,
            Int32 hourwidth, UInt32 hourcolor, Int32 minutewidth, UInt32 minutecolor, Int32 secondwidth, UInt32 secondcolor);

        //添加动画
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  filename avi文件名
        //  stretch: 图像是否拉伸以适应对象大小
        [DllImport("LEDSender2010.dll")]
        private static extern int AddMovie(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, string filename, Int32 stretch);

        //添加图片组播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddWindows(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  dc 源图片DC句柄
        //  width 图片宽度
        //  height 图片高度
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildWindow(ushort num, UInt32 dc, Int32 width, Int32 height, Int32 alignment, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  filename 图片文件名
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildPicture(ushort num, string filename, Int32 alignment, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildText(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildTextEx(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 autofitsize, Int32 wordwrap, Int32 vertical, Int32 alignment,Int32 verticalspace, Int32 horizontalfit, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  dc 源图片DC句柄
        //  width 图片宽度
        //  height 图片高度
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        //  kind 播放计划类型，=0始终播放，=1按照一周每日时间播放，=2按照指定起止日期时间播放，=3不播放
        //  week 一周有效日期，bit0到bit6表示周日到周六有效，当kind=1时，本参数起作用
        //  fromtime 有效起始时间，格式请用“yyyy-mm-dd hh:nn:ss”
        //  totime 有效结束时间，格式请用“yyyy-mm-dd hh:nn:ss”
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildScheduleWindow(ushort num, UInt32 dc, Int32 width, Int32 height, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  filename 图片文件名
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        //  kind 播放计划类型，=0始终播放，=1按照一周每日时间播放，=2按照指定起止日期时间播放，=3不播放
        //  week 一周有效日期，bit0到bit6表示周日到周六有效，当kind=1时，本参数起作用
        //  fromtime 有效起始时间，格式请用“yyyy-mm-dd hh:nn:ss”
        //  totime 有效结束时间，格式请用“yyyy-mm-dd hh:nn:ss”
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildSchedulePicture(ushort num, string filename, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        //  kind 播放计划类型，=0始终播放，=1按照一周每日时间播放，=2按照指定起止日期时间播放，=3不播放
        //  week 一周有效日期，bit0到bit6表示周日到周六有效，当kind=1时，本参数起作用
        //  fromtime 有效起始时间，格式请用“yyyy-mm-dd hh:nn:ss”
        //  totime 有效结束时间，格式请用“yyyy-mm-dd hh:nn:ss”
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildScheduleText(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        //  kind 播放计划类型，=0始终播放，=1按照一周每日时间播放，=2按照指定起止日期时间播放，=3不播放
        //  week 一周有效日期，bit0到bit6表示周日到周六有效，当kind=1时，本参数起作用
        //  fromtime 有效起始时间，格式请用“yyyy-mm-dd hh:nn:ss”
        //  totime 有效结束时间，格式请用“yyyy-mm-dd hh:nn:ss”
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildScheduleTextEx(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 autofitsize, Int32 wordwrap, Int32 vertical, Int32 alignment, Int32 verticalspace, Int32 horizontalfit,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime);

        //添加内码文字组播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddStrings(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border);

        //添加图片组的子图片 此函数要跟在AddWindows后面调用
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  str 文字字符串
        //  fontset 字库 =FONTSET_16P表示16点阵字库；=FONTSET_24P表示24点阵字库
        //  color 颜色
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddChildString(ushort num, string str, Int32 fontset, Int32 color, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加图片点阵播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  dc 源图片DC句柄
        //  src_width 图片宽度
        //  src_height 图片高度
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddWindow(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
		    UInt32 dc, Int32 src_width, Int32 src_height, Int32 alignment, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加图片文件播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  filename 图片文件
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddPicture(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, string filename, Int32 alignment, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);
        //  stretch 图片拉伸 =1拉伸 =0不拉伸
        [DllImport("LEDSender2010.dll")]
        private static extern int AddPictureEx(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, string filename, Int32 alignment, Int32 stretch, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加表格
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  profile 表格配置文件
        //  content 表格内容，行之间以回车换行分割，列之间以'|'分割
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddTable(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, string profile, string content,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加文字播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddText(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
    		string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment, 
		    Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加文字播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  autofitsize 字体自适应显示区域
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  vertical 是否纵向显示 =0正常 =1逆时针旋转纵向显示 =2顺时针旋转纵向显示
        //  alignment 对齐方式 =0靠左 =1居中 =2靠右
        //  verticalspace 行间距
        //  horizontalfit 横向自适应
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddTextEx3(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 flag,
            string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 bkcolor, Int32 autofitsize, Int32 wordwrap, 
            Int32 vertical, Int32 alignment, Int32 verticalspace, Int32 horizontalfit,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加文字播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  flag 保留
        //  str 文字字符串
        //  charset 字符集，置0
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  autofitsize 字体自适应显示区域
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  vertical 是否纵向显示 =0正常 =1逆时针旋转纵向显示 =2顺时针旋转纵向显示
        //  alignment 对齐方式 =0靠左 =1居中 =2靠右
        //  verticalspace 行间距
        //  horizontalfit 横向自适应
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddTextEx4(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 flag,
            string str, Int32 charset, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 bkcolor, Int32 autofitsize, Int32 wordwrap,
            Int32 vertical, Int32 alignment, Int32 verticalspace, Int32 horizontalfit,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加RTF文字播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  flag 保留
        //  filename rtf文件
        //  alignment 对齐方式 =0靠左 =1居中 =2靠右
        //  wordwrap 是否自动换行 =1自动换行；=0不自动换行
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddRtf(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 flag,
            string filename, Int32 alignment, Int32 wordwrap, Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);


        //添加内码文字播放
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  str 文字字符串
        //  fontset 字库 =FONTSET_16P表示16点阵字库；=FONTSET_24P表示24点阵字库
        //  color 颜色
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddString(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
		    string str, Int32 fontset, Int32 color, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //添加温度显示
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        [DllImport("LEDSender2010.dll")]
        private static extern int AddTemperature(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle);

        //添加湿度显示
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        [DllImport("LEDSender2010.dll")]
        private static extern int AddHumidity(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle);

        //>>----------------------------------------------------------------------------------------------------------
        // 下面几个函数，是用户自定义绘制相关接口

        //初始化用户自定义绘制的画布图片大小
        //  width 画布图片宽度
        //  height 画布图片高度
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Init(Int32 width, Int32 height);

        //绘制直线
        //  X, Y 起点
        //  X1, Y1 终点
        //  linewidth 线宽
        //  linecolor 颜色
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Draw_Line(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor);

        //绘制矩形
        //  X, Y, X1, Y1 两个对角点坐标
        //  linewidth 线宽
        //  linecolor 边框颜色
        //  fillcolor 填充颜色
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Draw_Rectangle(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor, Int32 fillcolor);

        //绘制椭圆形
        //  X, Y, X1, Y1 两个对角点坐标
        //  linewidth 线宽
        //  linecolor 边框颜色
        //  fillcolor 填充颜色
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Draw_Ellipse(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor, Int32 fillcolor);

        //绘制圆角矩形
        //  X, Y, X1, Y1 两个对角点坐标
        //  linewidth 线宽
        //  linecolor 边框颜色
        //  fillcolor 填充颜色
        //  roundx, roundy 圆角弧度
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Draw_RoundRect(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor, Int32 fillcolor, Int32 roundx, Int32 roundy);

        //绘制文字
        //  left, top, width, height 左、上、宽度、高度，文字在画布上的显示区域
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Draw_Text(Int32 left, Int32 top, Int32 width, Int32 height, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 alignment);

        //计算自动换行文字的高度
        //  width 宽度
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Calc_MatrixHeight(Int32 width, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle);

        //绘制多行文字（自动换行）
        //  left, top, width, height 左、上、宽度、高度，文字在画布上的显示区域
        //  str 文字字符串
        //  fontname 字体名称
        //  fontsize 字体大小
        //  fontcolor 字体颜色
        //  fontstyle 字体样式 举例：=WFS_BOLD表示粗体；=WFS_ITALIC表示斜体；=WFS_BOLD+WFS_ITALIC表示粗斜体
        //  alignment 对齐方式 0=靠左，1=居中，2=靠右
        [DllImport("LEDSender2010.dll")]
        private static extern int UserCanvas_Draw_TextEx(Int32 left, Int32 top, Int32 width, Int32 height, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 alignment);

        //添加用户自定义绘制画布图片
        //  num 节目数据缓冲区编号，是MakeRoot、MakeChapter、MakeRegion、MakeLeaf、MakeObject的返回值
        //  left、top、width、height 左、上、宽度、高度
        //  transparent 是否透明 =1表示透明；=0表示不透明
        //  border 流水边框(未实现)
        //  inmethod 引入方式(下面有列表说明)
        //  inspeed 引入速度(取值范围0-5，从快到慢)
        //  outmethod 引出方式(下面有列表说明)
        //  outspeed 引出速度(取值范围0-5，从快到慢)
        //  stopmethod 停留方式(下面有列表说明)
        //  stopspeed 停留速度(取值范围0-5，从快到慢)
        //  stoptime 停留时间(单位毫秒)
        [DllImport("LEDSender2010.dll")]
        private static extern int AddUserCanvas(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime);

        //----------------------------------------------------------------------------------------------------------<<

        
        // ====引入动作方式列表(数值从0开始)====
        //    0 = '随机',
        //    1 = '立即显示',
        //    2 = '左滚显示',
        //    3 = '上滚显示',
        //    4 = '右滚显示',
        //    5 = '下滚显示',
        //    6 = '连续左滚显示',
        //    7 = '连续下滚显示',
        //    8 = '中间向上下展开',
        //    9 = '中间向两边展开',
        //   10 = '中间向四周展开',
        //   11 = '从右向左移入',
        //   12 = '从左向右移入',
        //   13 = '从左向右展开',
        //   14 = '从右向左展开',
        //   15 = '从右上角移入',
        //   16 = '从右下角移入',
        //   17 = '从左上角移入',
        //   18 = '从左下角移入',
        //   19 = '从上向下移入',
        //   20 = '从下向上移入',
        //   21 = '横向百叶窗',
        //   22 = '纵向百叶窗',
        // =====================================

        // ====引出动作方式列表(数值从0开始)====
        //    0 = '随机',
        //    1 = '不消失',
        //    2 = '立即消失',
        //    3 = '上下向中间合拢',
        //    4 = '两边向中间合拢',
        //    5 = '四周向中间合拢',
        //    6 = '从左向右移出',
        //    7 = '从右向左移出',
        //    8 = '从右向左合拢',
        //    9 = '从左向右合拢',
        //   10 = '从右上角移出',
        //   11 = '从右下角移出',
        //   12 = '从左上角移出',
        //   13 = '从左下角移出',
        //   14 = '从下向上移出',
        //   15 = '从上向下移出',
        //   16 = '横向百叶窗',
        //   17 = '纵向百叶窗'
        // =====================================

        // ====停留动作方式列表(数值从0开始)====
        //    0 = '静态显示',
        //    1 = '闪烁显示'
        // =====================================

        public int Do_LED_Startup()
        {
            return LED_Startup();
        }

        public int Do_LED_Cleanup()
        {
            return LED_Cleanup();
        }

        public int Do_LED_GetPlayContent(ref TSenderParam param)
        {
            return LED_GetPlayContent(ref param);
        }

        public int Do_LED_Cache_GetBoardParam(ref TSenderParam param)
        {
            return LED_Cache_GetBoardParam(ref param);
        }

        public String Do_LED_Cache_GetBoardParam_IP()
        {
            return LED_Cache_GetBoardParam_IP();
        }

        public String Do_LED_Cache_GetBoardParam_Mac()
        {
            return LED_Cache_GetBoardParam_Mac();
        }

        public int Do_LED_Cache_GetBoardParam_Addr()
        {
            return LED_Cache_GetBoardParam_Addr();
        }

        public int Do_LED_Cache_GetBoardParam_Width()
        {
            return LED_Cache_GetBoardParam_Width();
        }

        public int Do_LED_Cache_GetBoardParam_Height()
        {
            return LED_Cache_GetBoardParam_Height();
        }

        public int Do_LED_Cache_GetBoardParam_Brightness()
        {
            return LED_Cache_GetBoardParam_Brightness();
        }

        public int Do_LED_Cache_GetBoardParam_Frequency()
        {
            return LED_Cache_GetBoardParam_Frequency();
        }

        public void Do_LED_Cache_SetBoardParam_IP(String value)
        {
            LED_Cache_SetBoardParam_IP(value);
        }

        public void Do_LED_Cache_SetBoardParam_Mac(String value)
        {
            LED_Cache_SetBoardParam_Mac(value);
        }

        public void Do_LED_Cache_SetBoardParam_Addr(int value)
        {
            LED_Cache_SetBoardParam_Addr(value);
        }

        public void Do_LED_Cache_SetBoardParam_Width(int value)
        {
            LED_Cache_SetBoardParam_Width(value);
        }

        public void Do_LED_Cache_SetBoardParam_Height(int value)
        {
            LED_Cache_SetBoardParam_Height(value);
        }

        public void Do_LED_Cache_SetBoardParam_Brightness(int value)
        {
            LED_Cache_SetBoardParam_Brightness(value);
        }

        public void Do_LED_Cache_SetBoardParam_Frequency(int value)
        {
            LED_Cache_SetBoardParam_Frequency(value);
        }

        public int Do_LED_Cache_SetBoardParam(ref TSenderParam param)
        {
            return LED_Cache_SetBoardParam(ref param);
        }

        public int Do_LED_Report_CreateServer(Int32 serverindex, Int32 localport)
        {
            return LED_Report_CreateServer(serverindex, localport);
        }

        public void Do_LED_Report_RemoveServer(Int32 serverindex)
        {
            LED_Report_RemoveServer(serverindex);
        }

        public void Do_LED_Report_RemoveAllServer()
        {
            LED_Report_RemoveAllServer();
        }

        public int Do_LED_Report_GetOnlineList(Int32 serverindex)
        {
            return LED_Report_GetOnlineList(serverindex, 0, 0);
        }

        public String Do_LED_Report_GetOnlineItemName(Int32 serverindex, Int32 itemindex)
        {
            return LED_Report_GetOnlineItemName(serverindex, itemindex);
        }

        public String Do_LED_Report_GetOnlineItemHost(Int32 serverindex, Int32 itemindex)
        {
            return LED_Report_GetOnlineItemHost(serverindex, itemindex);
        }

        public int Do_LED_Report_GetOnlineItemPort(Int32 serverindex, Int32 itemindex)
        {
            return LED_Report_GetOnlineItemPort(serverindex, itemindex);
        }

        public int Do_LED_Report_GetOnlineItemAddr(Int32 serverindex, Int32 itemindex)
        {
            return LED_Report_GetOnlineItemAddr(serverindex, itemindex);
        }

        public void Do_LED_Preview(Int32 index, Int32 width, Int32 height, string previewfile)
        {
            LED_Preview(index, width, height, previewfile);
        }

        public void Do_LED_PreviewFile(Int32 width, Int32 height, string previewfile)
        {
            LED_PreviewFile(width, height, previewfile);
        }

        public void Do_LED_CloseDeviceOnTerminate(int AValue)
        {
            LED_CloseDeviceOnTerminate(AValue);
        }

        public int Do_LED_GetNotifyParam(ref TNotifyParam param, Int32 index)
        {
            return LED_GetNotifyParam(ref param, index);
        }

        public int Do_LED_GetNotifyParam_BufferToFile(ref TNotifyParam param, String filename, Int32 index)
        {
            return LED_GetNotifyParam_BufferToFile(ref param, filename, index);
        }

        public int Do_LED_ResetDisplay(ref TSenderParam param)
        {
            return LED_ResetDisplay(ref param);
        }

        public int Do_LED_Reboot(ref TSenderParam param)
        {
            return LED_Reboot(ref param);
        }

        public int Do_LED_GetTemperatureHumidity(ref TSenderParam param)
        {
            return LED_GetTemperatureHumidity(ref param);
        }

        public int Do_LED_SetPower(ref TSenderParam param, Int32 value)
        {
            return LED_SetPower(ref param, value);
        }

        public int Do_LED_GetPower(ref TSenderParam param)
        {
            return LED_GetPower(ref param);
        }

        public int Do_LED_SetBright(ref TSenderParam param, Int32 value)
        {
            return LED_SetBright(ref param, value);
        }

        public int Do_LED_GetBright(ref TSenderParam param)
        {
            return LED_GetBright(ref param);
        }

        public int Do_LED_AdjustTime(ref TSenderParam param)
        {
            return LED_AdjustTime(ref param);
        }

        public int Do_LED_AdjustTimeEx(ref TSenderParam param, ref TSystemTime time)
        {
            return LED_AdjustTimeEx(ref param, ref time);
        }

        public int Do_LED_SetVarStringSingle(ref TSenderParam param, Int32 index, string str, Int32 color)
        {
            return LED_SetVarStringSingle(ref param, index, str, color);
        }

        public int Do_LED_SetPowerSchedule(ref TSenderParam param, ref TPowerSchedule schedule)
        {
            return LED_SetPowerSchedule(ref param, ref schedule);
        }

        public int Do_LED_SetBrightSchedule(ref TSenderParam param, ref TBrightSchedule schedule)
        {
            return LED_SetBrightSchedule(ref param, ref schedule);
        }

        public int Do_LED_GetChapterCount(ref TSenderParam param)
        {
            return LED_GetChapterCount(ref param);
        }

        public int Do_LED_SetCurChapter(ref TSenderParam param, Int32 value)
        {
            return LED_SetCurChapter(ref param, value);
        }

        public int Do_LED_GetCurChapter(ref TSenderParam param)
        {
            return LED_GetCurChapter(ref param);
        }

        public int Do_LED_Compress(Int32 index)
        {
            return LED_Compress(index);
        }
        
        public int Do_LED_SendToScreen(ref TSenderParam param, Int32 index)
        {
            return LED_SendToScreen(ref param, index);
        }

        public int Do_LED_SendStringDirectly(ref TSenderParam param, string text)
        {
            return LED_SendStringDirectly(ref param, text);
        }

        public int Do_LED_SendHexDirectly(ref TSenderParam param, string text)
        {
            return LED_SendHexDirectly(ref param, text);
        }

        public int Do_LED_ComTransferHex(ref TSenderParam param, string text, Int32 delayaftertransfer)
        {
            return LED_ComTransferHex(ref param, text, delayaftertransfer);
        }

        public int Do_LED_ModemTransferHex(ref TSenderParam param, string text, Int32 delayaftertransfer)
        {
            return LED_ModemTransferHex(ref param, text, delayaftertransfer);
        }

        public int Do_LED_GetTransferAck(ref TSenderParam param)
        {
            return LED_GetTransferAck(ref param);
        }

        public int Do_LED_UDP_SenderParam(Int32 param_index, Int32 locport, string host, Int32 rmtport, Int32 address, Int32 notifymode, Int32 wmhandle, Int32 wmmessage)
        {
            return LED_UDP_SenderParam(param_index, locport, host, rmtport, address, notifymode, wmhandle, wmmessage);
        }

        public int Do_LED_COM_SenderParam(Int32 param_index, Int32 comport, Int32 baudrate, Int32 address, Int32 notifymode, Int32 wmhandle, Int32 wmmessage)
        {
            return LED_COM_SenderParam(param_index, comport, baudrate, address, notifymode, wmhandle, wmmessage);
        }

        public int Do_LED_SendToScreen2(Int32 param_index, Int32 index)
        {
            return LED_SendToScreen2(param_index, index);
        }

        public int Do_SetRotate(Int32 rotate, Int32 metrix_width, Int32 metrix_height)
        {
            return SetRotate(rotate, metrix_width, metrix_height);
        }

        public int Do_MakeFromVsqFile(string filename, Int32 RootType, Int32 ColorMode, Int32 survive)
        {
            return MakeFromVsqFile(filename, RootType, ColorMode, survive);
        }

        public int Do_MakeRoot(Int32 RootType, Int32 ColorMode, Int32 survive)
        {
            return MakeRoot(RootType, ColorMode, survive);
        }

        public int Do_MakeRootEx(Int32 RootType, Int32 ColorMode, Int32 survive, Int32 rotate, Int32 metrix_width, Int32 metrix_height)
        {
            return MakeRootEx(RootType, ColorMode, survive, rotate, metrix_width, metrix_height);
        }

        public int Do_MakeChapter(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 ColorMode, UInt32 time, ushort wait)
        {
            return MakeChapter(RootType, ActionMode, ChapterIndex, ColorMode, time, wait);
        }

        public int Do_MakeRegion(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 RegionIndex,
            Int32 ColorMode, Int32 left, Int32 top, Int32 width, Int32 height, Int32 border)
        {
            return MakeRegion(RootType, ActionMode, ChapterIndex, RegionIndex, ColorMode, left, top, width, height, border);
        }

        public int Do_MakeLeaf(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 RegionIndex, Int32 LeafIndex,
            Int32 ColorMode, UInt32 time, ushort wait)
        {
            return MakeLeaf(RootType, ActionMode, ChapterIndex, RegionIndex, LeafIndex, ColorMode, time, wait);
        }

        public int Do_MakeObject(Int32 RootType, Int32 ActionMode, Int32 ChapterIndex, Int32 RegionIndex, Int32 LeafIndex, Int32 ObjectIndex, Int32 ColorMode)
        {
            return MakeObject(RootType, ActionMode, ChapterIndex, RegionIndex, LeafIndex, ObjectIndex, ColorMode);
        }

        public int Do_AddChapter(ushort num, UInt32 time, ushort wait)
        {
            return AddChapter(num, time, wait);
        }

        public int Do_AddChapterEx(ushort num, UInt32 time, ushort wait, ushort priority, ushort kind, ushort week, ref TTimeStamp fromtime, ref TTimeStamp totime)
        {
            return AddChapterEx(num, time, wait, priority, kind, week, ref fromtime, ref totime);
        }

        public int Do_AddRegion(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 border)
        {
            return AddRegion(num, left, top, width, height, border);
        }

        public int Do_AddLeaf(ushort num, UInt32 time, ushort wait)
        {
            return AddLeaf(num, time, wait);
        }

        public int Do_AddDateTime(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle,
            Int32 year_offset, Int32 month_offset, Int32 day_offset, Int32 sec_offset, string format)
        {
            return AddDateTime(num, left, top, width, height, transparent, border, fontname, fontsize, fontcolor, fontstyle, 
                year_offset, month_offset, day_offset, sec_offset, format);
        }

        public int Do_AddDateTimeEx(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 vertical,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle,
            Int32 year_offset, Int32 month_offset, Int32 day_offset, Int32 sec_offset, string format)
        {
            return AddDateTimeEx(num, left, top, width, height, transparent, border, vertical, fontname, fontsize, fontcolor, fontstyle,
                year_offset, month_offset, day_offset, sec_offset, format);
        }

        public int Do_AddCampaign(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle,
            string format, ref TTimeStamp basetime, ref TTimeStamp fromtime, ref TTimeStamp totime, Int32 step)
        {
            return AddCampaignEx(num, left, top, width, height, transparent, border, fontname, fontsize, fontcolor, fontstyle,
                format, ref basetime, ref fromtime, ref totime, step);
        }

        public int Do_AddCounter(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 countertype, Int32 format,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, ref TTimeStamp basetime)
        {
            return AddCounter(num, left, top, width, height, transparent, border, countertype, format, fontname, fontsize, fontcolor, fontstyle, ref basetime);
        }

        public int Do_AddClock(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 offset,
            UInt32 bkcolor, UInt32 bordercolor, UInt32 borderwidth, Int32 bordershape,
            Int32 dotradius, Int32 adotwidth, UInt32 adotcolor, Int32 bdotwidth, UInt32 bdotcolor,
            Int32 hourwidth, UInt32 hourcolor, Int32 minutewidth, UInt32 minutecolor, Int32 secondwidth, UInt32 secondcolor)
        {
            return AddClock(num, left, top, width, height, transparent, border, offset, bkcolor, bordercolor, borderwidth, bordershape, 
                dotradius, adotwidth, adotcolor, bdotwidth, bdotcolor,
                hourwidth, hourcolor, minutewidth, minutecolor, secondwidth, secondcolor);
        }

        public int Do_AddClockEx2(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, Int32 vertical, Int32 offset,
            UInt32 bkcolor, UInt32 bordercolor, UInt32 borderwidth, Int32 bordershape,
            Int32 dotradius, Int32 adotwidth, UInt32 adotcolor, Int32 bdotwidth, UInt32 bdotcolor,
            UInt32 cdotvisible, Int32 cdotwidth, UInt32 cdotcolor,
            Int32 hourwidth, UInt32 hourcolor, Int32 minutewidth, UInt32 minutecolor, Int32 secondwidth, UInt32 secondcolor)
        {
            return AddClockEx2(num, left, top, width, height, transparent, border, vertical, offset, bkcolor, bordercolor, borderwidth, bordershape,
                dotradius, adotwidth, adotcolor, bdotwidth, bdotcolor, cdotvisible, cdotwidth, cdotcolor,
                hourwidth, hourcolor, minutewidth, minutecolor, secondwidth, secondcolor);
        }

        public int Do_AddMovie(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, string filename, Int32 stretch)
        {
            return AddMovie(num, left, top, width, height, transparent, border, filename, stretch);
        }

        public int Do_AddWindows(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border)
        {
            return AddWindows(num, left, top, width, height, transparent, border);
        }

        public int Do_AddChildWindow(ushort num, UInt32 dc, Int32 width, Int32 height, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddChildWindow(num, dc, width, height, alignment, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddChildPicture(ushort num, string filename, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddChildPicture(num, filename, alignment, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddChildText(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddChildText(num, str, fontname, fontsize, fontcolor, fontstyle, wordwrap, alignment, 
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddChildTextEx(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment, Int32 verticalspace,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddChildTextEx(num, str, fontname, fontsize, fontcolor, fontstyle, 0, wordwrap, 0, alignment, verticalspace, 0,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddChildScheduleWindow(ushort num, UInt32 dc, Int32 width, Int32 height, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime)
        {
            return AddChildScheduleWindow(num, dc, width, height, alignment, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime, kind, week, fromtime, totime);
        }

        public int Do_AddChildSchedulePicture(ushort num, string filename, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime)
        {
            return AddChildSchedulePicture(num, filename, alignment, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime, kind, week, fromtime, totime);
        }

        public int Do_AddChildScheduleText(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime)
        {
            return AddChildScheduleText(num, str, fontname, fontsize, fontcolor, fontstyle, wordwrap, alignment, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime, kind, week, fromtime, totime);
        }

        public int Do_AddChildScheduleTextEx(ushort num, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment, Int32 verticalspace,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime, ushort kind, ushort week, string fromtime, string totime)
        {
            return AddChildScheduleTextEx(num, str, fontname, fontsize, fontcolor, fontstyle, 0, wordwrap, 0, alignment, verticalspace, 0,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime, kind, week, fromtime, totime);
        }

        public int Do_AddStrings(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border)
        {
            return AddStrings(num, left, top, width, height, transparent, border);
        }

        public int Do_AddChildString(ushort num, string str, Int32 fontset, Int32 color,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddChildString(num, str, fontset, color,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddWindow(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
            UInt32 dc, Int32 src_width, Int32 src_height, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddWindow(num, left, top, width, height, transparent, border, dc, src_width, src_height, alignment,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddPicture(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, string filename, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddPicture(num, left, top, width, height, transparent, border, filename, alignment,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddPictureEx(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, string filename, Int32 alignment, Int32 stretch,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddPictureEx(num, left, top, width, height, transparent, border, filename, alignment, stretch,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddTable(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, string profile, string content,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddTable(num, left, top, width, height, transparent, profile, content,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddText(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
            string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 alignment,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddText(num, left, top, width, height, transparent, border, 
                str, fontname, fontsize, fontcolor, fontstyle, wordwrap, alignment,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddTextEx(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 wordwrap, Int32 vertical, Int32 alignment, Int32 verticalspace,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddTextEx4(num, left, top, width, height, transparent, 0, 
                str, 0, fontname, fontsize, fontcolor, fontstyle, 0, 1, wordwrap, vertical, alignment, verticalspace, 0,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddRtf(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, 
                    string filename, Int32 alignment, Int32 wordwrap, Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddRtf(num, left, top, width, height, transparent, 0,
                filename, alignment, wordwrap, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddString(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border, 
            string str, Int32 fontset, Int32 color,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddString(num, left, top, width, height, transparent, border, str, fontset, color,
                inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

        public int Do_AddTemperature(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle)
        {
            return AddTemperature(num, left, top, width, height, transparent, border, fontname, fontsize, fontcolor, fontstyle);
        }

        public int Do_AddHumidity(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle)
        {
            return AddHumidity(num, left, top, width, height, transparent, border, fontname, fontsize, fontcolor, fontstyle);
        }

        public int Do_UserCanvas_Init(Int32 width, Int32 height)
        {
            return UserCanvas_Init(width, height);
        }

        public int Do_UserCanvas_Draw_Line(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor)
        {
            return UserCanvas_Draw_Line(X, Y, X1, Y1, linewidth, linecolor);
        }

        public int Do_UserCanvas_Draw_Rectangle(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor, Int32 fillcolor)
        {
            return UserCanvas_Draw_Rectangle(X, Y, X1, Y1, linewidth, linecolor, fillcolor);
        }

        public int Do_UserCanvas_Draw_Ellipse(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor, Int32 fillcolor)
        {
            return UserCanvas_Draw_Ellipse(X, Y, X1, Y1, linewidth, linecolor, fillcolor);
        }

        public int Do_UserCanvas_Draw_RoundRect(Int32 X, Int32 Y, Int32 X1, Int32 Y1, Int32 linewidth, Int32 linecolor, Int32 fillcolor, Int32 roundx, Int32 roundy)
        {
            return UserCanvas_Draw_RoundRect(X, Y, X1, Y1, linewidth, linecolor, fillcolor, roundx, roundy);
        }

        public int Do_UserCanvas_Draw_Text(Int32 left, Int32 top, Int32 width, Int32 height, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 alignment)
        {
            return UserCanvas_Draw_Text(left, top, width, height, str, fontname, fontsize, fontcolor, fontstyle, alignment);
        }

        public int Do_UserCanvas_Calc_MatrixHeight(Int32 width, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle)
        {
            return UserCanvas_Calc_MatrixHeight(width, str, fontname, fontsize, fontcolor, fontstyle);
        }

        public int Do_UserCanvas_Draw_TextEx(Int32 left, Int32 top, Int32 width, Int32 height, string str, string fontname, Int32 fontsize, Int32 fontcolor, Int32 fontstyle, Int32 alignment)
        {
            return UserCanvas_Draw_TextEx(left, top, width, height, str, fontname, fontsize, fontcolor, fontstyle, alignment);
        }

        public int Do_AddUserCanvas(ushort num, Int32 left, Int32 top, Int32 width, Int32 height, Int32 transparent, Int32 border,
            Int32 inmethod, Int32 inspeed, Int32 outmethod, Int32 outspeed, Int32 stopmethod, Int32 stopspeed, Int32 stoptime)
        {
            return AddUserCanvas(num, left, top, width, height, transparent, border, inmethod, inspeed, outmethod, outspeed, stopmethod, stopspeed, stoptime);
        }

    }
}
