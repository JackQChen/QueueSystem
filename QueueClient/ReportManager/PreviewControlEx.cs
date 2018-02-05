using System;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Reflection;
using System.Windows.Forms;
using DevComponents.DotNetBar;
using FastReport.Preview;
using FastReport.Utils;
using ReportManager.Properties;

namespace ReportManager
{
    public class PreviewControlEx : PreviewControl
    {
        private ButtonItem btnDesign;
        private ComboBoxItem cmb;
        private LabelItem lab;

        public event Action<object> BeforePrint;
        public event Action<object> AfterPrint;

        public bool PrintButtonVisible
        {
            get
            {
                return this.ToolBar.Items["btnPrint"].Visible;
            }
            set
            {
                this.ToolBar.Items["btnPrint"].Visible = value;
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "GetKeyState")]
        public static extern int GetKeyState(int nVirtKey);

        public PreviewControlEx()
        {
            this.ToolBar.Items["btnPrint"].Click += new System.EventHandler((obj, args) =>
            {
                if (AfterPrint != null)
                    this.AfterPrint(obj);
            });
            this.ToolBar.Items["btnPrint"].MouseUp += new System.Windows.Forms.MouseEventHandler((obj, args) =>
            {
                if (BeforePrint != null)
                    this.BeforePrint(obj);
            });
            lab = new LabelItem("labStyle", "外观样式");
            lab.ItemAlignment = eItemAlignment.Far;
            cmb = new DevComponents.DotNetBar.ComboBoxItem("cmbStyle");
            cmb.Items.AddRange(System.Enum.GetNames(typeof(UIStyle)));
            cmb.ComboWidth = 125;
            cmb.SelectedIndexChanged += new System.EventHandler(cmb_SelectedIndexChanged);
            cmb.SelectedIndex = 0;
            if (PrintManager.CanDesign)
            {
                lab.BeginGroup = true;
                btnDesign = new ButtonItem("btnDesign", "设计报表");
                btnDesign.ItemAlignment = eItemAlignment.Far;
                btnDesign.ButtonStyle = eButtonStyle.ImageAndText;
                btnDesign.Image = Resources.Design;
                btnDesign.Click += new System.EventHandler(btnDesign_Click);
                this.ToolBar.Items.Add(btnDesign);
            }
            this.ToolBar.Items.Add(lab);
            this.ToolBar.Items.Add(cmb);
            this.PageChanged += PreviewControlEx_PageChanged;
        }

        ContextMenuBar barMain;
        ButtonItem itemRoot, item3, item4;
        ComboBoxItem item1, item2;
        TextBoxItem item5;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            Control tabControl = this.GetType().BaseType.GetField("tabControl",
                     BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding)
                     .GetValue(this) as Control;
            barMain = new ContextMenuBar();
            itemRoot = new ButtonItem("Root");
            item1 = new ComboBoxItem("item1");
            item1.Text = "显示比例";
            ButtonItem btn1 = new ButtonItem("b1", "标准");
            ButtonItem btn2 = new ButtonItem("b2", "适应");
            ButtonItem btn3 = new ButtonItem("b3", "页面宽度");
            item1.Items.AddRange(new BaseItem[] { btn1, btn2, btn3 });
            item1.ComboWidth = 80;
            item1.DropDownStyle = ComboBoxStyle.DropDownList;
            item1.SelectedIndex = 0;
            item1.Click += (obj, args) =>
            {
                switch (item1.SelectedIndex)
                {
                    case 0: this.Zoom = 1f; break;
                    case 1: this.ZoomWholePage(); break;
                    case 2: this.ZoomPageWidth(); break;
                    default: break;
                }
            };
            item2 = new ComboBoxItem("item2");
            item2.Text = "页面位置";
            ButtonItem btn4 = new ButtonItem("b4", "前一页");
            ButtonItem btn5 = new ButtonItem("b5", "后一页");
            ButtonItem btn6 = new ButtonItem("b7", "首页");
            ButtonItem btn7 = new ButtonItem("b8", "尾页");
            item2.Items.AddRange(new BaseItem[] { btn4, btn5, btn6, btn7 });
            item2.ComboWidth = 80;
            item2.DropDownStyle = ComboBoxStyle.DropDownList;
            item2.SelectedIndex = 0;
            item2.Click += (obj, args) =>
            {
                switch (item2.SelectedIndex)
                {
                    case 0: this.Prior(); break;
                    case 1: this.Next(); break;
                    case 2: this.First(); break;
                    case 3: this.Last(); break;
                    default: break;
                }
            };
            item3 = new ButtonItem("item3");
            item3.BeginGroup = true;
            item3.Text = "打印";
            item3.Image = Res.GetImage(195);
            item3.Click += (obj, args) =>
            {
                this.Print();
            };
            item4 = new ButtonItem("item4");
            item4.Text = "打印当前页";
            item4.Click += (obj, args) =>
            {
                this.Report.PrintSettings.PageNumbers = this.PageNo.ToString();
                this.Report.PrintSettings.PageRange = FastReport.PageRange.PageNumbers;
                this.Report.Print();
                this.Report.PrintSettings.PageNumbers = "";
                this.Report.PrintSettings.PageRange = FastReport.PageRange.All;
            };
            item5 = new TextBoxItem("item5");
            item5.Text = "页码范围";
            item5.Click += (obj, args) =>
            {
                try
                {
                    this.Report.PrintSettings.PageNumbers = item5.ControlText;
                    this.Report.PrintSettings.PageRange = FastReport.PageRange.PageNumbers;
                    this.Report.Print();
                    this.Report.PrintSettings.PageNumbers = "";
                    this.Report.PrintSettings.PageRange = FastReport.PageRange.All;
                }
                catch
                {
                    MessageBoxEx.Show("输入的页码范围无效，请核对后重试", "提示");
                }
            };
            itemRoot.SubItems.AddRange(new BaseItem[] { item1, item2, item3, item4, item5 });
            barMain.SetContextMenuEx(tabControl, itemRoot);
            this.Controls.Add(barMain);
            tabControl.Controls[0].MouseDown += (obj, args) =>
            {
                itemRoot.ClosePopup();
            };
            //tabControl.Controls[0].LostFocus += (obj, args) =>
            //{ 
            //    itemRoot.ClosePopup();
            //};
        }

        void PreviewControlEx_PageChanged(object sender, EventArgs e)
        {
            this.PageChanged -= PreviewControlEx_PageChanged;
            //此后Report已经进行绑定
            #region 释放资源
            foreach (var datasource in this.Report.Dictionary.DataSources)
            {
                EventHandlerList list = typeof(Component).GetProperty("Events",
         BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.ExactBinding)
         .GetValue(datasource, null) as EventHandlerList;
                object head = typeof(EventHandlerList).GetField("head",
                 BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding)
                 .GetValue(list);
                if (head == null)
                    continue;
                Type tp = typeof(EventHandlerList).Assembly.GetType("System.ComponentModel.EventHandlerList+ListEntry");
                object key = tp.GetField("key",
                      BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding)
                      .GetValue(head);
                Delegate del = tp.GetField("handler",
                    BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding)
                    .GetValue(head) as Delegate;
                list.RemoveHandler(key, del);
            }
            #endregion
            if ((GetKeyState((int)System.Windows.Forms.Keys.D) & 0x8000) != 0)
            {
                btnDesign_Click(this.btnDesign, EventArgs.Empty);
            }
        }

        private void btnDesign_Click(object sender, System.EventArgs e)
        {
            using (DesignerEx de = new DesignerEx(this.Report))
            {
                de.ShowDialog();
            }
        }

        private void cmb_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            this.UIStyle = (UIStyle)System.Enum.Parse(typeof(UIStyle), cmb.SelectedItem.ToString());
            this.lab.Style = (eDotNetBarStyle)this.UIStyle;
            this.cmb.Style = (eDotNetBarStyle)this.UIStyle;
        }
    }

}
