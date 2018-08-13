using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QueueClient
{
    public static class BoShiPrinter
    {
        /// <summary>
        /// 打开打印机连接
        /// 这个函数是在使用打印机时，第一要用到的函数
        /// 其中的 Type 见前面的定义，若等于 1就是使用 USB 口，Idx 从 0 开始，是指端口号
        /// 比如使用串口的话，Idx=0,就是用 com1,=1 就是用 com2。
        /// 使用 USB 的话，Idx=0，就是本打印机第 1 次插到电脑的 USB 口生成的那个 USB00x,如果
        /// 同时电脑的 USB 同时插了
        /// 2台 USB 的打印机，那么 Idx=1,就是使用第 2 台的打印
        /// </summary>
        /// <param name="type"></param>
        /// <param name="idx"></param>
        /// <returns></returns>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxOpenPrinter")]
        public static extern bool TxOpenPrinter(int type, int idx);
        /// <summary>
        /// 关闭打印机连接
        /// </summary>
        /// <returns></returns>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxClosePrinter")]
        public static extern int TxClosePrinter();
        /// <summary>
        /// 获取打印机状态
        /// </summary>
        /// <returns>
        /// #define TX_STAT_NOERROR 0x0008 无故障
        /// #define TX_STAT_SELECT 0x0010 处于联机状态
        /// #define TX_STAT_PAPEREND 0x0020 缺纸
        /// #define TX_STAT_BUSY 0x0080 繁忙
        /// #define TX_STAT_DRAW_HIGH 0x0100 钱箱接口的电平（整机使用的，模块无用）
        /// #define TX_STAT_COVER 0x0200 打印机机芯的盖子打开
        /// #define TX_STAT_ERROR 0x0400 打印机错误
        /// #define TX_STAT_RCV_ERR 0x0800 可恢复错误（需要人工干预）
        /// #define TX_STAT_CUT_ERR 0x1000 切刀错误
        /// #define TX_STAT_URCV_ERR 0x2000 不可恢复错误
        /// #define TX_STAT_ARCV_ERR 0x4000 可自动恢复的错误
        /// #define TX_STAT_PAPER_NE 0x8000 快要没有纸了
        /// 返回值为 TX_STAT_XXXX，调用失败则为 0。
        /// 返回值 TX_STAT_XXXX 见上面的位定义，是各种状态的组合，是按位来处理的，实际判断
        /// 时要将返回的
        /// TX_STAT_XXXX按上面的位定义来按位判断，得到打印的状态
        /// 特别提示：在使用 USB 口时最好先使用这个函数来读取打印机的状态,可以很快获得打印机
        /// 的是否忙，是否有纸和是否错误的状态，其它的状态不能获得。若打印机有纸和无故障的不
        /// 忙的情况下，可以调用下面的函数来获取其它的信息，比如纸将尽等等
        /// </returns>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxGetStatus")]
        public static extern int TxGetStatus();
        /// <summary>
        /// 获取打印机状态
        /// 与上一函数类似，区别是 USB 接口通过指令查询状态。这个函数下，USB 可以获得打印机
        /// 更多的信息。最好是在调用前一个函数确认打印有纸，无故障，不忙的情况下调用此函数。
        /// </summary>
        /// <returns>
        /// 同TxGetStatus
        /// </returns>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxGetStatus2")]
        public static extern int TxGetStatus2();
        /// <summary>
        /// 初始化打印机
        /// </summary>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxInit")]
        public static extern void TxInit();
        /// <summary>
        /// 输出字符串（以\0 结束） 。就是把字符串直接发到打印机
        /// </summary>
        /// <param name="str"></param>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxOutputString")]
        public static extern void TxOutputString(string str);
        /// <summary>
        /// 输出回车、换行，就是向打印机发送 0x0D,0x0A 的数据，打印机接收到后，会将缓冲区中的数据打印并走纸 1行
        /// </summary>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxNewline")]
        public static extern void TxNewline();
        /// <summary>
        /// 输出字符串（以\0 结束） ，并自动添加回车、换行
        /// </summary>
        /// <param name="str"></param>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxOutputStringLn")]
        public static extern void TxOutputStringLn(string str);
        /// <summary>
        /// 打印条码
        /// type 是打印的条码的类型，后面是符合要求的条码数据
        /// 如下例子是打印 UPCA 的条码，数据为“12345678
        /// TxPrintBarcode(TX_BAR_UPCA,"12345678901");
        /// </summary>
        /// <param name="type">
        /// #define TX_BAR_UPCA 65
        /// #define TX_BAR_UPCE 66
        /// #define TX_BAR_EAN13 67
        /// #define TX_BAR_EAN8 68
        /// #define TX_BAR_CODE39 69
        /// #define TX_BAR_ITF 70
        /// #define TX_BAR_CODABAR 71
        /// #define TX_BAR_CODE93 72
        /// #define TX_BAR_CODE128 73
        /// </param>
        /// <param name="str"></param>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxPrintBarcode")]
        public static extern void TxPrintBarcode(int type, string str);
        /**
         * @brief 输出指定的缓冲
         * @param buf  : 为缓冲区指针
         * @param len  : 为缓冲区长度
         * @return 操作是否成功
         */
        [DllImport("TxPrnMod.dll", EntryPoint = "TxWritePrinter")]
        public static extern bool TxWritePrinter(char[] buf, int len);

        /**
         * @brief 从打印机读取数据
         * @param buf  : 为接收缓冲
         * @param len  : 为欲读取的字节数
         * @return 返回实际的字节数
         */
        [DllImport("TxPrnMod.dll", EntryPoint = "TxReadPrinter")]
        public static extern int TxReadPrinter(ref char[] buf, int len);

        /// <summary>
        /// 特殊操作
        /// 执行特殊功能。就是向打印机发送设置指令，或着走纸指令，定位指令等，具体要见下面做
        /// 
        /// 好的定义
        /// 有的功能需要传入两个参数，有的功能只需要一个参数（此时 param2=0 就可以了） 。
        /// TxDoFunction 的 func 的功能类型定义
        /// #define TX_FONT_SIZE 1
        /// 控制字体的放大倍数功能，后面的 2个参数 0 为正常大小，递增 1为增大 1 倍，如此类推，
        /// 最大为 7；参数 1(param1)为字体宽的倍数，参数 2(param2)为字体高的倍数。
        /// 下面的例子就是使用 4 倍宽 3倍高的字体：
        /// TxDoFunction(TX_FONT_SIZE,TX_SIZE_3X,TX_SIZE_2X);
        /// #define TX_FONT_ULINE 2
        /// 控制是否有下划线功能，只需 1 个参数，param1=TX_ON(有下划线)，=TX_OFF(无小划
        /// 线）,param2=0 就可以了。
        /// 下面的例子就是使用下划线的调用：
        /// TxDoFunction(TX_FONT_ULINE,TX_ON,0);#define TX_FONT_BOLD 3
        /// 控制是否粗体功能，只需 1 个参数，param1=TX_ON(使用粗体)，=TX_OFF(不使用粗
        /// 体),param2=0 就可以了。
        /// 下面的例子就是使用粗体的调用
        /// TxDoFunction(TX_FONT_BOLD,TX_ON,0);
        /// #define TX_SEL_FONT 4
        /// 选 择 英 文 字 体 功 能 ， 也 是 只 需 要 1 个 参 数 ， TX_FONT_A(12X24) 或
        /// TX_FONT_B(9X17),param2=0 就可以了。
        /// 下面的例子就是使用 9X17的字体调用的
        /// TxDoFunction(TX_SEL_FONT,TX_FONT_B,0);
        /// #define TX_FONT_ROTATE 5
        /// 字体是否旋转 90度功能，只需要 1 个参数。
        /// 下面的例子就是使用 9X17的字体调用的
        /// TxDoFunction(TX_FONT_ROTATE,TX_ON,0);
        /// #define TX_ALIGN 6
        /// 控制对齐功能，参数为 TX_ALIGN_XXX，只需要 1个参数。
        /// #define TX_ALIGN_LEFT 0 左对齐的设置参数
        /// #define TX_ALIGN_CENTER 1 对中设置的参数
        /// #define TX_ALIGN_RIGHT 2 右对齐的设置参数
        /// 下面例子是对中打印的调用
        /// TxDoFunction(TX_ALIGN,TX_ALIGN_CENTER,0);
        /// TxOutputStringLn("center");
        /// #define TX_CHINESE_MODE 7
        /// 中文/英文模式切换功能，只需要 1 个参数。
        /// 下面的例子是进入中文方式的调用
        /// TxDoFunction(TX_CHINESE_MODE,TX_ON,0);
        /// #define TX_FEED 10
        /// 执行走纸功能，只需要 1个参数，参数以毫米为单位。
        /// 下面的例子是走纸 30毫米的调用
        /// TxDoFunction(TX_FEED,30,0);
        /// #define TX_UNIT_TYPE 11
        /// 设置动作单位(无效)
        /// #define TX_CUT 12
        /// 执行切纸功能，第一参数指明类型，第二参数指明切纸前的走纸距离
        /// 切纸的类型有 2种:#define TX_CUT_FULL 0 全切的设置参数(和黑标检测关联）
        /// #define TX_CUT_PARTIAL 1 半切的设置参数（和黑标检测关联）
        /// #define TX_PURECUT_FULL 2 全切（与黑标无关的切纸）
        /// #define TX_PURECUT_PARTIAL 3 半切（与黑标无关的切纸）
        /// 下面的例子就是不走纸直接全切的调用。
        /// TxDoFunction(TX_CUT, PURECUT_FULL ,0);
        /// #define TX_HOR_POS 13
        /// 绝对水平定位功能，只需要 1个参数，参数以毫米为单位。
        /// 下面的例子就是定位在离左边 20mm 的地方开始处理字符数据（或着说打印也可以）
        /// TxDoFunction(TX_HOR_POS,20,0);
        /// #define TX_LINE_SP 14
        /// 设置行间距功能，只需要 1 个参数，参数是以点数为单位的。
        /// 下面的例子就是设置行间距为 30 点（默认的参数）
        /// TxDoFunction(TX_LINE_SP,30,0);
        /// #define TX_BW_REVERSE 15
        /// 设置字体是否黑白翻转功能，只需要 1个参数
        /// 下面的例子是使用黑白翻转打印功能的调用。
        /// TxDoFunction(TX_BW_REVERSE,TX_ON,0);
        /// #define TX_UPSIDE_DOWN 16
        /// 设置是否倒置打印功能，只需要 1个参数
        /// 下面的例子是使用倒置打印功能的调用。
        /// TxDoFunction(TX_UPSIDE_DOWN,TX_ON,0);
        /// #define TX_INET_CHARS 17
        /// 选择国际字符集功能，只需要 1 个参数,通常这个设置也是在英文方式下使用的，只是针对
        /// 12个特定的 ASCII 码不同国家的使用的字形不同，默认是 0，表示是使用美国的字符集。
        /// 下面的例子是设置国际字符集为 1
        /// TxDoFunction(TX_INET_CHARS,1,0);
        /// #define TX_CODE_PAGE 18
        /// 选择字符代码页功能，只需要 1个参数，通常是 0~n,表示选择的代码页参数，祥见打印机说
        /// 明书
        /// 一般默认是 0，是表示 PC437的代码页，这个功能要只在英文方式下有效。
        /// 下面的例子是设置字符代码页为 3
        /// TxDoFunction(TX_CODE_PAGE,3,0);
        /// #define TX_CH_ROTATE 19
        /// 设定汉字旋转功能，只需要 1个参数，可以表示 3 种选择，如下所示：
        /// #define TX_CH_ROTATE_NONE 0 不旋转
        /// #define TX_CH_ROTATE_LEFT 1 向左旋转#define TX_CH_ROTATE_RIGHT 2 向右旋转
        /// 下面的例子是设置汉字向左旋转的功能
        /// TxDoFunction(TX_CH_ROTATE,TX_CH_ROTATE_LEFT,0);
        /// #define TX_CHK_BMARK 20
        /// 寻找黑标黑标
        /// TX_CHK_BMARK 是执行黑标检测，这个动作本身是不需要参数的，所以就是
        /// TxDoFunction(TX_CHK_BMARK,0,0)
        /// #define TX_SET_BMARK 21
        /// 设置黑标相关偏移量功能
        /// 这里有 2 个参数要设置，起始打印位置相对于黑标检测位置的偏移量和切/撕纸位置相对于
        /// 黑标检测位置的偏移量。
        /// TxDoFunction(TX_SET_BMARK,TX_BM_START,参数)，这样是设置起始打印位置相对
        /// 于黑标检测位置的偏移量。
        /// TxDoFunction(TX_SET_BMARK,TX_BM_TEAR ,参数)，这样是切/撕纸位置相对于黑标
        /// 检测位置的偏移量。
        /// #define TX_PRINT_LOGO 22
        /// 打印已下载好的 LOGO 的功能，只需 1 个参数，可以表示 4 种选择，如下所示：
        /// #define TX_LOGO_1X1 0 对应 203X203 的点密度，就是正常大小。
        /// #define TX_LOGO_1X2 1 对应 203x101 的点密度，就是 LOG 在水平方向上放大
        /// 到了 2倍。
        /// #define TX_LOGO_2X1 2 对应 101x203 的点密度，就是 LOG 在垂直方向上放大
        /// 到了 2倍。
        /// #define TX_LOGO_2X2 3 对应 101x101 的点密度，就是 LOG 在水平和垂直方向
        /// 上放大到了 2 倍。
        /// 下面的例子就是正常打印下载的 LOGO
        /// TxDoFunction(TX_PRINT_LOGO,TX_LOGO_1x1,0);
        /// #define TX_BARCODE_HEIGHT 23
        /// 设定条码高度功能,只需要 1 个参数,单位为点数
        /// 下面的例子是设置的条码打印的高度为 15 点：
        /// TxDoFunction(TX_BARCODE_HEIGHT,15,0);
        /// #define TX_BARCODE_WIDTH 24
        /// 设定条码宽度功能,只需要 1 个参数,单位为点数, 不能小于 2.
        /// 下面的例子是设置的条码打印的宽度高度为 3 点：
        /// TxDoFunction(TX_BARCODE_WIDTH,3,0);
        /// #define TX_BARCODE_FONT 25
        /// 选择条码 HRI 字符的打印位置,只需要 1 个参数，可以表示 4 种选择，如下所示：#define TX_BAR_FONT_NONE 0 不打印
        /// #define TX_BAR_FONT_UP 1 打印在条码的上面
        /// #define TX_BAR_FONT_DOWN 2 打印在条码的下面
        /// #define TX_BAR_FONT_BOTH 3 条码的上面下面都打印
        /// 下面的例子就是设置打印条码时条码的 HRI 字符打印在条码的下面
        /// TxDoFunction(TX_BARCODE_FONT,TX_BAR_FONT_DOWN,0);
        /// #define TX_FEED_REV 26
        /// 执行反向走纸功能，只需要 1个参数，参数以毫米为单位。
        /// #define下面的例子是反向走纸 30毫米的调用
        /// #defineTxDoFunction(TX_FEED_REV,30,0);
        /// #define TX_QR_DOTSIZE 27
        /// #define设置 QR 码的点大小，2《param1《10
        /// #define下面的例子是设置 QR 码 6点大小的调用
        /// #defineTxDoFunction(TX_QR_DOTSIZE,6,0);
        /// #define TX_QR_ERRLEVEL 28
        /// #define设置 QR 码的纠错等级，有 4 个等级可以选择，等级越高，可以支持的字符数越少（在同一
        /// #define个版本的情况下）
        /// #define TX_QR_ERRLEVEL_L 0x31
        /// #define TX_QR_ERRLEVEL_M 0x32
        /// #define TX_QR_ERRLEVEL_Q 0x33
        /// #define TX_QR_ERRLEVEL_H 0x34
        /// #define下面的例子是设置为 L 的纠错等级的调用
        /// #defineTxDoFunction(TX_QR_ERRLEVEL,TX_QR_ERRLEVEL_L,0);
        /// #defineTxDoFunction 的参数的定义（需和功能类型对应）
        /// #define TX_ON 1 功能设置有效的参数
        /// #define TX_OFF 0 功能设置无效的参数
        /// #define TX_CUT_FULL 0 全切的设置参数(和黑标检测关联）
        /// #define TX_CUT_PARTIAL 1 半切的设置参数（和黑标检测关联）
        /// #define TX_PURECUT_FULL 2 全切（与黑标无关的切纸）
        /// #define TX_PURECUT_PARTIAL 3 半切（与黑标无关的切纸）
        /// #define TX_FONT_A 0 12X24点阵的设置参数
        /// #define TX_FONT_B 1 9X17点阵的设置参数
        /// #define TX_ALIGN_LEFT 0 左对齐的设置参数
        /// #define TX_ALIGN_CENTER 1 对中设置的参数
        /// #define TX_ALIGN_RIGHT 2 右对齐的设置参数控制字体倍数的参数设置
        /// #define TX_SIZE_1X 0 正常大小
        /// #define TX_SIZE_2X 1 2 倍大小
        /// #define TX_SIZE_3X 2 3 倍大小
        /// #define TX_SIZE_4X 3 4 倍大小
        /// #define TX_SIZE_5X 4 5 倍大小
        /// #define TX_SIZE_6X 5 6 倍大小
        /// #define TX_SIZE_7X 6 7 倍大小
        /// #define TX_SIZE_8X 7 8 倍大小
        /// #define TX_UNIT_PIXEL 0 对应 TX_UNIT_TYPE
        /// #define TX_UNIT_MM 1
        /// #define TX_CH_ROTATE_NONE 0 对应 TX_CH_ROTATE
        /// #define TX_CH_ROTATE_LEFT 1
        /// #define TX_CH_ROTATE_RIGHT 2
        /// #define TX_BM_START 1 起始打印位置相对于黑标检测位置的偏移量
        /// #define TX_BM_TEAR 2 切/撕纸位置相对于黑标检测位置的偏移量
        /// #define TX_LOGO_1X1 0 对应 TX_PRINT_LOGO
        /// #define TX_LOGO_1X2 1 /*203x101*/
        /// #define TX_LOGO_2X1 2 /*101x203*/
        /// #define TX_LOGO_2X2 3 /*101x101*/
        /// #define TX_BAR_FONT_NONE 0 对应 TX_BARCODE_FONT
        /// #define TX_BAR_FONT_UP 1
        /// #define TX_BAR_FONT_DOWN 2
        /// #define TX_BAR_FONT_BOTH 3
        /// QR 码纠错等级参数
        /// #define TX_QR_ERRLEVEL_L 0x31
        /// #define TX_QR_ERRLEVEL_M 0x32
        /// #define TX_QR_ERRLEVEL_Q 0x33
        /// #define TX_QR_ERRLEVEL_H 0x34
        /// </summary>
        /// <param name="func"></param>
        /// <param name="param1"></param>
        /// <param name="param2"></param>
        /// <returns></returns>
        [DllImport("TxPrnMod.dll", EntryPoint = "TxDoFunction")]
        public static extern int TxDoFunction(int func, int param1, int param2);

        /// <summary>
        /// 发送指令判断打印是否成功
        /// </summary>
        /// <returns></returns>
        public static bool CheckIsPrintSuccess()
        {
            char[] bufW = new char[3];
            bufW[0] = (char)0x1D;
            bufW[1] = (char)0x49;
            bufW[2] = (char)0x01;
            bool isWrited = TxWritePrinter(bufW, bufW.Length);
            char[] bufR = new char[1];
            bufR[0] = (char)0x00;
            int re = TxReadPrinter(ref bufR, 1);
            return re > 0;
        }
        const long Paperend = 0x0020;//缺纸
        const long Paper_ne = 0x8000;//快没有纸了
        public static bool IfPaperend()
        {
            if (TxOpenPrinter(1, 0))
            {
                var status = TxGetStatus();
                if ((status & Paperend) != 0 || (status & Paper_ne) != 0)
                {
                    return true;
                }
                TxClosePrinter();
            }
            return false;
        }
    }
}
