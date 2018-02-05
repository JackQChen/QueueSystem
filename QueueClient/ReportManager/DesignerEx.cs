
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;
using FastReport.Design.StandardDesigner;
using FastReport.Design.ToolWindows;
using Microsoft.Win32;
namespace ReportManager
{
    public class DesignerEx : DesignerForm
    {
        public DesignerEx(FastReport.Report rpt)
        {
            this.Designer.MainMenu.miHelpAbout.Visible = false;
            this.Designer.UIStyle = FastReport.Utils.UIStyle.Office2007Blue;
            this.Text = "报表设计器";
            this.Designer.Report = rpt;
            //对字典项内容进行排序
            ((TreeView)
                ((DictionaryWindow)
                this.Designer.Plugins.Find("DictionaryWindow")).ParentControl.Controls[0])
                .Sort();
            #region 增加字体
            /*
            //增加部分字体 
            ((ToolbarBase)this.Designer.Plugins.Find("TextToolbar")).Items.Remove("cbxTextSize");
            ComboBoxItem cmb = new ComboBoxItem();
            cmb.Name = "cbxTextSize";
            cmb.Text = "大小";
            cmb.ComboBoxEx.DropDownStyle = ComboBoxStyle.DropDown;
            ((ToolbarBase)this.Designer.Plugins.Find("TextToolbar")).Items.Add(cmb, 1);
            cmb.ComboWidth = 60;
            cmb.ComboBoxEx.DisabledBackColor = UIStyleUtils.GetControlColor(this.Designer.UIStyle);
            cmb.Items.AddRange(new ChsFont[] 
            {
                #region 增加字体
              new ChsFont(   "初号",42),
                new ChsFont(   "小初", 36 ),
                new ChsFont(   "一号", 26.25f),
                  new ChsFont( "小一",24),
                  new ChsFont( "二号",21.75f),
                  new ChsFont( "小二",18),
                  new ChsFont( "三号",15.75f),
                  new ChsFont( "小三",15f),
                  new ChsFont( "四号",14.25f),
                  new ChsFont( "小四",12f),
                  new ChsFont( "五号",10.5f),
                  new ChsFont( "小五",9f),
                  new ChsFont( "六号",7.5f),
                  new ChsFont( "5",5),
                  new ChsFont( "6",6),
                  new ChsFont( "7",7),
                  new ChsFont( "8",8),
                  new ChsFont( "9",9),
                  new ChsFont( "10",10),
                  new ChsFont( "11",11),
                  new ChsFont( "12",12),
                  new ChsFont( "14",14),
                  new ChsFont( "16",16),
                  new ChsFont( "18",18),
                  new ChsFont( "20",20),
                  new ChsFont( "22",22),
                  new ChsFont( "24",24),
                  new ChsFont( "26",26),
                  new ChsFont( "28",28),
                  new ChsFont( "36",36),
                  new ChsFont( "48",48),
                  new ChsFont( "72",72) 
                #endregion
            });

            ((ToolbarBase)this.Designer.Plugins.Find("TextToolbar")).Items["cbxTextName"].EnabledChanged
                += new System.EventHandler((obj, args) =>
                {
                    if (this.Designer.SelectedTextObjects.First != null)
                        cmb.ControlText = Converter.DecreasePrecision(
                             this.Designer.SelectedTextObjects.First.Font.Size
                            , 2).ToString();
                    else
                        cmb.ControlText = "";
                    cmb.Enabled = (obj as ComboBoxItem).Enabled;
                });
            //    private void UpdateText()
            //{
            //    this.FUpdating = true;
            //    this.FUpdating = false;
            //}
            //this.Designer.SelectedTextObjects  
            cmb.ComboBoxEx.KeyDown += new KeyEventHandler((obj, args) =>
            {
                if (args.KeyCode == Keys.Return)
                    SetSelectedTextFontSize(Converter.StringToFloat(cmb.ControlText, true));
            });
            cmb.SelectedIndexChanged += new System.EventHandler((obj, args) =>
            {
                SetSelectedTextFontSize((cmb.SelectedItem as ChsFont).FontSize);
            });

        }

        private void SetSelectedTextFontSize(float fontSize)
        {
            if (fontSize == 0)
                return;
            this.Designer.SelectedTextObjects.SetFontSize(fontSize);
        }

        public class ChsFont
        {
            public string ChsSize { get; set; }
            public float FontSize { get; set; }

            public ChsFont()
            {
            }

            public ChsFont(string chsSize, float size)
            {
                this.ChsSize = chsSize;
                this.FontSize = size;
            }

            public override string ToString()
            {
                return this.ChsSize;
            }
             */
            #endregion
        }

        protected override void Dispose(bool disposing)
        {
            #region 释放资源
            List<Delegate> listRemove = new List<Delegate>();
            FieldInfo field = typeof(SystemEvents).GetField("_handlers",
                BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding);
            IEnumerable e = field.GetValue(null) as IEnumerable;
            foreach (var m in e)
            {
                IEnumerable list = m.GetType().GetProperty("Value").GetValue(m, null) as IEnumerable;
                foreach (var n in list)
                {
                    Delegate del = n.GetType().GetField("_delegate", BindingFlags.Instance | BindingFlags.GetField | BindingFlags.NonPublic | BindingFlags.ExactBinding)
                         .GetValue(n) as Delegate;
                    if (del.Target is DevComponents.DotNetBar.Bar)
                    {
                        DevComponents.DotNetBar.Bar bar = del.Target as DevComponents.DotNetBar.Bar;
                        if (bar.FindForm() == null)
                            listRemove.Add(del);
                        else if (bar.FindForm().GetType().Equals(this.GetType()))
                        {
                            listRemove.Add(del);
                        }
                    }
                }
            }
            MethodInfo method = typeof(SystemEvents).GetMethod("RemoveEventHandler",
                 BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.ExactBinding);
            MethodInfo method2 = typeof(DevComponents.DotNetBar.DotNetBarManager).Assembly
                .GetType("DevComponents.DotNetBar.MessageHandler")
                .GetMethod("UnregisterMessageClient",
                BindingFlags.Static | BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.ExactBinding);
            foreach (var v in listRemove)
            {
                method.Invoke(null, new object[] { 
                     typeof(SystemEvents).GetField("OnUserPreferenceChangedEvent",
                BindingFlags.Static | BindingFlags.GetField | BindingFlags.NonPublic  | BindingFlags.ExactBinding )
                .GetValue(null),v});
                method2.Invoke(null, new object[] { v.Target });
            }
            this.Designer.Report.Designer = null;
            #endregion
            base.Dispose(disposing);
        }
    }

}
