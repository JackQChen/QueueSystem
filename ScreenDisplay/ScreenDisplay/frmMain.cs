using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using Model;
using BLL;
using System.Runtime.InteropServices;
using System.Configuration;
namespace ScreenDisplay
{
    public partial class frmMain : Form
    {
        private string temp;//字幕临时
        private string messInfo = System.Configuration.ConfigurationManager.AppSettings["MsgInfo"]; //滚动字幕
        private string msgErrorInfo = System.Configuration.ConfigurationManager.AppSettings["MsgErrorInfo"];
        private int isShowError = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["IsShowError"]);
        private int areaNo = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["AreaNo"]);
        private string areaList = System.Configuration.ConfigurationManager.AppSettings["AreaNo"];
        private int interval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Interval"]);
        private int distance = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["Distance"]);
        private int rowCount = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RowCount"]);
        private int rowHeight = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RowHeight"]);
        private int msgHeight = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MsgHeight"]);
        private string colorGreenRGB = System.Configuration.ConfigurationManager.AppSettings["ColorGreenRGB"];
        private string colorGrayRGB = System.Configuration.ConfigurationManager.AppSettings["ColorGrayRGB"];
        private string colorBlueRGB = System.Configuration.ConfigurationManager.AppSettings["ColorBlueRGB"];
        private string colorTicketRGB = System.Configuration.ConfigurationManager.AppSettings["ColorTicketRGB"];
        private string colorWindowRGB = System.Configuration.ConfigurationManager.AppSettings["ColorWindowRGB"];
        private string colorOtherRGB = System.Configuration.ConfigurationManager.AppSettings["ColorOtherRGB"];
        private string colorBackRGB = System.Configuration.ConfigurationManager.AppSettings["ColorBackRGB"];
        private int refreshInterval = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["RefreshInterval"]);
        private string fontName = System.Configuration.ConfigurationManager.AppSettings["FontName"];
        private int fontSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["FontSize"]);
        private string msgFontName = System.Configuration.ConfigurationManager.AppSettings["MsgFontName"];
        private int msgFontSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["MsgFontSize"]);
        private string ExitPstion = System.Configuration.ConfigurationManager.AppSettings["ExitPstion"];
        private string strVipColorRGB;
        private Font msgf = null;// new Font("黑体", 65, FontStyle.Bold); //显示字体大小 
        private Color c;//背景颜色
        private PointF p;  //绘制文本的左上角
        private int ScreenWidth = 1080;
        private TCallBLL cBll = new TCallBLL();
        private Color main;
        private Color green;// System.Drawing.Color.FromArgb(136, 240, 146);
        private Color gray;//= System.Drawing.Color.FromArgb(61, 60, 60);
        private Color blue;//= System.Drawing.Color.FromArgb(32, 126, 192);
        private Color ticket;
        private Color window;
        private Color other;
        private Color vipColor;
        private bool ShowError = false;
        private int VIPFontSize;
        TBusinessAttributeBLL baBll = new TBusinessAttributeBLL();
        TWindowBLL wBll = new TWindowBLL();
        List<TWindowModel> wList = new List<TWindowModel>();
        List<TBusinessAttributeModel> baList = new List<TBusinessAttributeModel>();//
        TWindowBusinessBLL wbBll = new TWindowBusinessBLL();
        List<TWindowBusinessModel> wbList = new List<TWindowBusinessModel>();
        TQueueBLL qBll = new TQueueBLL();
        string[] areaStrList;
        public frmMain()
        {
            InitializeComponent();
            msgf = new Font(msgFontName, msgFontSize, FontStyle.Bold);
            string[] sGre = colorGreenRGB.Split(',');
            string[] sGra = colorGrayRGB.Split(',');
            string[] sBlu = colorBlueRGB.Split(',');
            string[] sTic = colorTicketRGB.Split(',');
            string[] sWin = colorWindowRGB.Split(',');
            string[] sOth = colorOtherRGB.Split(',');
            string[] sMai = colorBackRGB.Split(',');
            green = System.Drawing.Color.FromArgb(Convert.ToInt32(sGre[0]), Convert.ToInt32(sGre[1]), Convert.ToInt32(sGre[2]));
            gray = System.Drawing.Color.FromArgb(Convert.ToInt32(sGra[0]), Convert.ToInt32(sGra[1]), Convert.ToInt32(sGra[2]));
            blue = System.Drawing.Color.FromArgb(Convert.ToInt32(sBlu[0]), Convert.ToInt32(sBlu[1]), Convert.ToInt32(sBlu[2]));
            ticket = System.Drawing.Color.FromArgb(Convert.ToInt32(sTic[0]), Convert.ToInt32(sTic[1]), Convert.ToInt32(sTic[2]));
            window = System.Drawing.Color.FromArgb(Convert.ToInt32(sWin[0]), Convert.ToInt32(sWin[1]), Convert.ToInt32(sWin[2]));
            other = System.Drawing.Color.FromArgb(Convert.ToInt32(sOth[0]), Convert.ToInt32(sOth[1]), Convert.ToInt32(sOth[2]));
            main = System.Drawing.Color.FromArgb(Convert.ToInt32(sMai[0]), Convert.ToInt32(sMai[1]), Convert.ToInt32(sMai[2]));
            pnMain.BackColor = main;
            c = main;
            this.pnTip.Height = msgHeight;

        }

        public static void SetConfigValue(string key, string value)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                if (config.AppSettings.Settings[key] == null)
                    config.AppSettings.Settings.Add(key, value);
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch
            {
            }
        }

        private void BindData()
        {
            List<msInfo> mList = new List<msInfo>();
            #region 正式数据
            var vList = new List<TCallModel>();
            try
            {
                vList = cBll.ScreenShowByArea(areaList).Take(rowCount).ToList();
                ShowError = false;
            }
            catch
            {
                ShowError = (isShowError == 0 ? false : true);
            }
            int i = 0;
            Font vipFont = new Font(fontName, VIPFontSize, FontStyle.Bold);
            Font msgFont = new Font(fontName, fontSize, FontStyle.Bold);
            if (vList == null || vList.Count == 0)
            {
                this.pnMain.Controls.Clear();
                this.pnMain.ResumeLayout();
                return;
            }

            #region
            foreach (var v in vList)
            {
                var bam = baList.Where(b => b.busiSeq == v.busiSeq && b.unitSeq == v.unitSeq).FirstOrDefault();
                var queue = qBll.GetModel(q => q.id == v.qId);
                string strVip = "";
                if (bam != null && bam.isGreenChannel == 1)
                {
                    strVip = "绿色\r\n通道";
                }
                else
                {
                    if ((queue != null && queue.appType == 1 && queue.reserveStartTime <= v.handleTime && queue.reserveEndTime >= v.handleTime))
                    {
                        strVip = "网上\r\n预约";
                    }
                }
                msInfo ms = new msInfo();
                ms.Index = (i + 1);
                ms.RowHeight = rowHeight;
                ms.TextFont = msgFont;
                ms.WindowColor = window;
                ms.TicketColor = ticket;
                ms.VIPColor = vipColor;
                ms.VIPFont = vipFont;
                ms.VIPText = strVip;
                ms.OtherColor = other;
                ms.WindowNumber = v.windowNumber;
                ms.QueueNumber = v.ticketNumber;
                ms.msClick += ms_Click;
                if (i % 2 != 0)
                {
                    ms.BackColorPage = blue;
                }
                else
                {
                    ms.BackColorPage = gray;
                }
                mList.Add(ms);
                i++;
            }
            #endregion

            #endregion

            if (pnMain.Controls.Count == 0)
            {
                int t = 0;
                foreach (var ms in mList)
                {
                    ms.Location = new Point(0, t);
                    this.pnMain.Controls.Add(ms);
                    t += ms.Height;
                }
                pnMain.ResumeLayout();
            }
            else
            {
                var deleteControl = new List<Control>();
                int jCount = pnMain.Controls.Count;
                for (int j = 0; j < jCount; j++)
                {
                    if ((j + 1) <= mList.Count)
                    {
                        var ctl = pnMain.Controls[j] as msInfo;
                        ctl.WindowNumber = mList[j].WindowNumber;
                        ctl.QueueNumber = mList[j].QueueNumber;
                        ctl.VIPText = mList[j].VIPText;
                        ctl.Refresh();
                        ctl.Invalidate();
                    }
                    else
                    {
                        deleteControl.Add(pnMain.Controls[j]);
                    }
                }
                foreach (var con in deleteControl)
                    pnMain.Controls.Remove(con);
                pnMain.Refresh();
                pnMain.Invalidate();
                pnMain.ResumeLayout();
                int tCount = pnMain.Controls.Count;
                if (tCount < mList.Count)
                {
                    int t = (pnMain.Controls[0] as msInfo).Height * pnMain.Controls.Count;
                    for (int m = 0; m < mList.Count - tCount; m++)
                    {
                        var ms = mList[m + tCount];
                        ms.Location = new Point(0, t);
                        this.pnMain.Controls.Add(ms);
                        t += ms.Height;
                    }
                    pnMain.Refresh();
                    pnMain.Invalidate();
                    pnMain.ResumeLayout();
                }
            }
        }
        int clickTime = 0;
        void ms_Click()
        {
            int mx = Control.MousePosition.X;
            int my = Control.MousePosition.Y;
            string[] pos = ExitPstion.Split(',');
            int x1 = Convert.ToInt32(pos[0]);
            int y1 = Convert.ToInt32(pos[1]);
            int x2 = Convert.ToInt32(pos[2]);
            int y2 = Convert.ToInt32(pos[3]);
            if (mx > x1 && mx < x2 && my > y1 && my < y2)
            {
                if (clickTime > 5)
                    this.Close();
                else
                    clickTime++;
            }
        }
        [DllImport("user32.dll", EntryPoint = "ShowCursor", CharSet = CharSet.Auto)]
        public static extern void ShowCursor(int status);
        private void frmMain_Load(object sender, EventArgs e)
        {
            SetConfigValue("ColorVIPRGB", "136,240,146");
            SetConfigValue("VIPFontSize", "25");
            VIPFontSize = Convert.ToInt32(System.Configuration.ConfigurationManager.AppSettings["VIPFontSize"]);
            strVipColorRGB = System.Configuration.ConfigurationManager.AppSettings["ColorVIPRGB"];
            string[] sVip = strVipColorRGB.Split(',');
            vipColor = System.Drawing.Color.FromArgb(Convert.ToInt32(sVip[0]), Convert.ToInt32(sVip[1]), Convert.ToInt32(sVip[2]));
            ShowCursor(0);
            BindData();
            bmp = new Bitmap(this.pictureBox1.Width, this.pictureBox1.Height);
            timer1.Interval = interval;
            timer2.Interval = refreshInterval * 1000;
            areaStrList = areaList.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            wList = wBll.GetModelList(w => areaStrList.Contains(w.AreaName.ToString())).ToList();
            var windowList = wList.Select(s => s.ID).ToList();
            wbList = wbBll.GetModelList(w => windowList.Contains(w.WindowID)).ToList();
            var busyList = wbList.Select(w => w.busiSeq).ToList();
            var unitList = wbList.Select(w => w.unitSeq).ToList();
            baList = baBll.GetModelList(b => busyList.Contains(b.busiSeq) && unitList.Contains(b.unitSeq)).ToList();
        }

        Bitmap bmp;

        private void timer1_Tick(object sender, EventArgs e)
        {
            var img = this.bmp.Clone() as Image;
            Graphics g = Graphics.FromImage(img);
            SizeF s = new SizeF();
            Brush brush = new SolidBrush(green);
            g.Clear(c);//清除背景  
            if (temp != (ShowError ? msgErrorInfo : messInfo))//文字改变时,重新显示 
            {
                p = new PointF(ScreenWidth, 0);
                temp = (ShowError ? msgErrorInfo : messInfo);
            }
            else
                p = new PointF(p.X - distance, 0);//每次偏移
            s = g.MeasureString(temp, msgf);//测量文字长度  
            if (p.X <= -s.Width)
                p = new PointF(ScreenWidth, 0);
            g.DrawString(temp, msgf, brush, p);
            this.pictureBox1.Image = img;
            this.pictureBox1.Refresh();
        }

        private void frmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.Close();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            BindData();
        }

        private void pnMain_Click(object sender, EventArgs e)
        {
            ms_Click();
        }
    }
}
