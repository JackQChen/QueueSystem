using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Model;

namespace QueueClient
{
    public class PrintHelper
    {
        #region
        #region TxDoFunction的func参数 
        UInt32 TX_FONT_SIZE = 1; /*放大系数，0为原始大小，1为增大1倍，如此类推，最大为7；参数1为宽，参数2为高*/
        UInt32 TX_FONT_ULINE = 2;/*下划线*/
        UInt32 TX_FONT_BOLD = 3;/*粗体*/
        UInt32 TX_SEL_FONT = 4;/*选择英文字体*/
        UInt32 TX_FONT_ROTATE = 5;/*旋转90度*/
        UInt32 TX_ALIGN = 6;/*参数为TX_ALIGN_XXX*/
        UInt32 TX_CHINESE_MODE = 7;/*开关中文模式*/
        UInt32 TX_FEED = 10;/*执行走纸*/
        UInt32 TX_UNIT_TYPE = 11;/*设置动作单位*/
        UInt32 TX_CUT = 12;/*执行切纸，第一参数指明类型，第二参数指明切纸前的走纸距离*/
        UInt32 TX_HOR_POS = 13;/*绝对水平定位*/
        UInt32 TX_LINE_SP = 14;/*设置行间距*/
        UInt32 TX_BW_REVERSE = 15;/*设置字体黑白翻转*/
        UInt32 TX_UPSIDE_DOWN = 16;/*设置倒置打印*/
        UInt32 TX_INET_CHARS = 17;/*选择国际字符集*/
        UInt32 TX_CODE_PAGE = 18;/*选择字符代码表*/
        UInt32 TX_CH_ROTATE = 19;/*设定汉字旋转*/
        UInt32 TX_CHK_BMARK = 20;/*寻找黑标*/
        UInt32 TX_SET_BMARK = 21;/*设置黑标相关偏移量*/
        UInt32 TX_PRINT_LOGO = 22;/*打印已下载好的LOGO*/
        UInt32 TX_BARCODE_HEIGHT = 23;/*设定条码高度*/
        UInt32 TX_BARCODE_WIDTH = 24;/*设定条码宽度*/
        UInt32 TX_BARCODE_FONT = 25;/*选择HRI字符的打印位置*/
        UInt32 TX_FEED_REV = 26;/*执行逆向走纸*/
        UInt32 TX_QR_DOTSIZE = 27;/*设置QR码的点大小，2<param1<10*/
        UInt32 TX_QR_ERRLEVEL = 28;/*设置QR码的交错等级*/
        #endregion
        #region TxDoFunction的参数（需对应功能） 
        UInt32 TX_ON = 1;
        UInt32 TX_OFF = 0;
        UInt32 TX_CUT_FULL = 0; /*对应TX_CUT*/
        UInt32 TX_CUT_PARTIAL = 1;
        UInt32 TX_PURECUT_FULL = 2; /*与黑标无关的切纸*/
        UInt32 TX_PURECUT_PARTIAL = 3;
        UInt32 TX_FONT_A = 0; /*对应TX_SEL_FONT*/
        UInt32 TX_FONT_B = 1;
        UInt32 TX_ALIGN_LEFT = 0; /*对应TX_ALIGN*/
        UInt32 TX_ALIGN_CENTER = 1;
        UInt32 TX_ALIGN_RIGHT = 2;
        UInt32 TX_SIZE_1X = 0; /*对应TX_FONT_SIZE*/
        UInt32 TX_SIZE_2X = 1;
        UInt32 TX_SIZE_3X = 2;
        UInt32 TX_SIZE_4X = 3;
        UInt32 TX_SIZE_5X = 4;
        UInt32 TX_SIZE_6X = 5;
        UInt32 TX_SIZE_7X = 6;
        UInt32 TX_SIZE_8X = 7;
        UInt32 TX_UNIT_PIXEL = 0; /*对应TX_UNIT_TYPE*/
        UInt32 TX_UNIT_MM = 1;
        UInt32 TX_CH_ROTATE_NONE = 0; /*对应TX_CH_ROTATE*/
        UInt32 TX_CH_ROTATE_LEFT = 1;
        UInt32 TX_CH_ROTATE_RIGHT = 2;
        UInt32 TX_BM_START = 1; /*起始打印位置相对于黑标检测位置的偏移量*/
        UInt32 TX_BM_TEAR = 2; /*切/撕纸位置相对于黑标检测位置的偏移量*/
        UInt32 TX_LOGO_1X1 = 0; /*对应TX_PRINT_LOGO*/
        UInt32 TX_LOGO_1X2 = 1; /*203x101*/
        UInt32 TX_LOGO_2X1 = 2; /*101x203*/
        UInt32 TX_LOGO_2X2 = 3; /*101x101*/
        UInt32 TX_BAR_FONT_NONE = 0; /*对应TX_BARCODE_FONT*/
        UInt32 TX_BAR_FONT_UP = 1;
        UInt32 TX_BAR_FONT_DOWN = 2;
        UInt32 TX_BAR_FONT_BOTH = 3;
        #endregion
        /*
        * @brief 连接打印机。需在所有函数前执行。
        * @param Type  : TX_TYPE_XXX
        * @param Idx   : 从0开始
        * @return 操作是否成功
        */
        [DllImport("TxPrnMod.dl")]
        public static extern bool TxOpenPrinter(long type, long index);

        /// <summary>
        /// 获取打印机状态 调用失败则为0。
        /// </summary>
        [DllImport("TxPrnMod.dl")]
        public static extern short TxGetStatus2();
        /// <summary>
        /// 关闭连接
        /// </summary>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxClosePrinter")]
        public static extern void TxClosePrinter();

        /// <summary>
        /// 发送初始化
        /// </summary>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxInit")]
        public static extern void TxInit();

        /// <summary>
        /// 输出字符串（以\0结束）
        /// </summary>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxOutputString")]
        public static extern void TxOutputString(string outString);

        /// <summary>
        /// 输出回车、换行
        /// </summary>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxNewline")]
        public static extern void TxNewline();

        /// <summary>
        /// 输出字符串（以\0结束），并自动添加回车、换行
        /// </summary>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxOutputStringLn")]
        public static extern void TxOutputStringLn(string outString);

        /// <summary>
        /// 恢复字体效果（大小、粗体等）为原始状态。
        /// </summary>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxResetFont")]
        public static extern void TxResetFont();

        /// <summary>
        /// 执行特殊功能。
        /// 有的功能需要传入两个参数，有的功能只需要一个参数（param2=0）
        /// </summary>
        /// <param name="func">功能类型</param>
        /// <param name="param1">参数1</param>
        /// <param name="param2"></param>
        [DllImport("TxPrnMod.dl", EntryPoint = "TxDoFunction")]
        public static extern void TxDoFunction(System.UInt32 func, UInt32 param1, UInt32 param2);

        #endregion

        public PrintHelper()
        {

        }

        public void Print(BQueueModel model, string area, int wait)
        {
            var open= TxOpenPrinter(0, 0);
            var state = TxGetStatus2();
            TxInit();//初始化
            TxDoFunction(TX_FEED, 15, 0);//走纸，留空行 15毫米
            TxDoFunction(TX_HOR_POS, 15, 0);//距离左边15毫米
            TxDoFunction(TX_FONT_SIZE, TX_SIZE_3X, TX_SIZE_2X);//设置标题字体大小
            TxOutputStringLn("阳江市票务大厅排队叫号系统");
            TxDoFunction(TX_FONT_SIZE, TX_SIZE_1X, TX_SIZE_1X);//设置字体
            TxOutputStringLn("排队区域：" + area);
            TxOutputStringLn("排队部门：" + model.unitName);
            TxOutputStringLn("排队业务：" + model.busTypeName);
            TxOutputString("当前号码：");
            TxDoFunction(TX_FONT_BOLD, TX_ON, 0);//使用粗体
            TxDoFunction(TX_FONT_SIZE, TX_SIZE_3X, TX_SIZE_3X);//设置字体
            TxOutputStringLn(model.ticketNumber);
            TxDoFunction(TX_FONT_SIZE, TX_SIZE_1X, TX_SIZE_1X);//设置字体
            TxDoFunction(TX_FONT_BOLD, TX_OFF, 0);//关闭粗体
            TxOutputStringLn(string.Format("你前面还有 {0} 人在等待。", wait.ToString()));
            TxOutputStringLn("请在休息区等候，注意呼叫及大屏显示信息。");
            TxOutputStringLn("过号无效，请您重新取号。");
            TxOutputStringLn(string.Format("              {0}    ", DateTime.Now));
            TxDoFunction(TX_CUT, TX_CUT_PARTIAL, 5);//切纸，切纸前 走纸5毫米
            TxClosePrinter();
        }

    }
}
