using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace QueueClient
{
    public class TxPrn
    {


        [DllImport("TxPrnMod.dll")]
        extern static bool TxOpenPrinter(long type, long index);

        [DllImport("TxPrnMod.dll")]
        extern static long TxGetStatus();

        [DllImport("TxPrnMod.dll")]
        extern static long TxGetStatus2();

        [DllImport("TxPrnMod.dll")]
        extern static void TxClosePrinter();

        [DllImport("TxPrnMod.dll")]
        extern static void TxInit();

        [DllImport("TxPrnMod.dll")]
        extern static void TxOutputString(string str);

        [DllImport("TxPrnMod.dll")]
        extern static void TxNewline();

        [DllImport("TxPrnMod.dll")]
        extern static void TxOutputStringLn(string str);

        [DllImport("TxPrnMod.dll")]
        extern static void TxResetFont();

        [DllImport("TxPrnMod.dll")]
        extern static bool TxPrintImage(string filePath);

        [DllImport("TxPrnMod.dll")]
        extern static void TxSetupSerial(long attr);

        [DllImport("TxPrnMod.dll")]
        extern static bool TxPrintBarcode(long type, string str);

        [DllImport("TxPrnMod.dll")]
        extern static void TxDoFunction(long func, int param1, int param2);


        //打印机状态
        const long noerror = 0x0008;
        const long select = 0x0010;//处于联机状态
        const long paperend = 0x0020;//缺纸
        const long busy = 0x0080;//繁忙
        const long cover = 0x0200;//打印机机芯盖子打开
        const long error = 0x0400;//打印机错误
        const long rcv_err = 0x0800;//可恢复错误，需要人工干预
        const long cut_err = 0x1000;//切刀错误
        const long urcv_err = 0x2000;//不可恢复错误
        const long arcv_err = 0x4000;//可自动回复错误
        const long paper_ne = 0x8000;//快没有纸了


        public TxPrn()
        {

        }

        //打开并读取当前打印机状态
        public bool Open(long type, long index, out string mes)
        {
            mes = "";
            if (TxOpenPrinter(type, index))
            {
                long statu = TxGetStatus();
                if (statu == noerror)
                {
                    mes = "";
                }
                else if (statu == select)
                {
                    mes = "处于联机状态";
                }
                else if (statu == paperend)
                {
                    mes = "缺纸";
                }
                else if (statu == busy)
                {
                    mes = "繁忙";
                }
                else if (statu == cover)
                {
                    mes = "打印机机芯盖子打开";
                }
                else if (statu == error)
                {
                    mes = "打印机错误";
                }
                else if (statu == rcv_err)
                {
                    mes = "可恢复错误，需要人工干预";
                }
                else if (statu == cut_err)
                {
                    mes = "切刀错误";
                }
                else if (statu == urcv_err)
                {
                    mes = "不可恢复错误";
                }
                else if (statu == arcv_err)
                {
                    mes = "可自动回复错误";
                }
                else if (statu == 0x8000)
                {
                    mes = "快没有纸了";
                }
            }
            else
            {
                mes = "打印机打开失败";
                return false;
            }
            return true;
        }

        //初始化
        public void Init()
        {
            TxInit();
        }

        //设置串口
        public void SetSerialPort(SerialPort sp)
        {
            TxSetupSerial(sp.Baud | sp.Data | sp.Parity | sp.Stop | sp.Flow);
        }

        //输出字符（以\0结束），并自动回车换行
        public void OutPutStringLine(string str)
        {
            TxOutputStringLn(str);
        }

        //回车 换行
        public void NewLine()
        {
            TxNewline();
        }

        //恢复字体原始状态
        public void ResetFont()
        {
            TxResetFont();
        }

        //打印图片文件
        public bool PrintImage(string ImagePath)
        {
            return TxPrintImage(ImagePath);
        }

        //关闭打印机
        public void Close()
        {
            TxClosePrinter();
        }

        //打印条形码
        public void PrintBarCode(long barType, string str)
        {
            TxPrintBarcode(barType, str);
        }

        //特殊功能
        public void DoFunction(long func, int param1, int param2)
        {
            TxDoFunction(func, param1, param2);
        }

    }

    public class SerialPort
    {
        public SerialPort()
        {

        }

        public long Baud { get; set; }
        public long Data { get; set; }
        public long Parity { get; set; }
        public long Stop { get; set; }
        public long Flow { get; set; }

    }

}
