using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using FastReport;
using FastReport.Data;
using FastReport.Utils;
using ReportManager.Properties;

namespace ReportManager
{
    public class PrintManager : IDisposable
    {
        private FrmPreview frmPrint;
        private string title;

        public FrmPreview Form
        {
            get
            {
                if (this.frmPrint == null)
                    this.frmPrint = new FrmPreview();
                return this.frmPrint;
            }
        }

        public PreviewControlEx PreviewControl
        {
            get
            {
                return this.Form.PreviewControl;
            }
        }

        public static bool CanDesign
        {
            get;
            set;
        }

        static EnvironmentSettings setttings = new EnvironmentSettings();

        public static bool ShowProgress
        {
            get
            {
                return setttings.ReportSettings.ShowProgress;
            }
            set
            {
                setttings.ReportSettings.ShowProgress = value;
            }
        }

        public Report Report { get; set; }

        static PrintManager()
        {
            //报表进行汉化
            Res.LoadLocale(new MemoryStream(Encoding.UTF8.GetBytes(Resources.CHS)));
            AddCustomMethods();
        }

        public PrintManager()
        {
            this.Report = new Report();
            List<string> listRefAssembly = new List<string>(this.Report.ReferencedAssemblies);
            //添加自身程序集引用
            if (!listRefAssembly.Contains("ReportManager.dll"))
            {
                listRefAssembly.Add("ReportManager.dll");
                this.Report.ReferencedAssemblies = listRefAssembly.ToArray();
            }
            //SystemVariable sv1 = new SystemVariable();
            //SystemVariable sv2 = new SystemVariable();
            //sv1.Name = "操作员ID";
            //sv1.Value = "01";
            //sv2.Name = "操作员姓名";
            //sv2.Value = "admin";
            //this.Report.Dictionary.SystemVariables.AddRange(new Base[]{
            //    sv1,sv2
            //});
        }

        public static void WriteReportXml(string fileName, DataTable dtData)
        {
            if (string.IsNullOrEmpty(dtData.TableName))
                dtData.TableName = "report";
            string folder = Application.StartupPath + "\\ReportXML";
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
            dtData.WriteXml(string.Format("{0}\\{1}_Data.xml", folder, fileName));
            dtData.WriteXmlSchema(string.Format("{0}\\{1}_Schema.xsd", folder, fileName));
        }

        private static void AddCustomMethods()
        {
            //RegisteredObjects.AddCategory("Variables,CustomData", FastReport.Utils.Res.GetImage(66), "CustomData");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("Contains"), "Text");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("ToFloat"), "Conversion");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("ToDateTimeEx"), "Conversion");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("ToChineseUpper"), "Text");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("GetDisplayOrder"), "Conversion");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("SwapPageOrder"), "Conversion");
            RegisteredObjects.AddFunction(typeof(ReportMethod).GetMethod("GetDataByIndex"), "Conversion");
            //RegisteredObjects.AddFunctionCategory("CustomMethod", "CustomMethod");
            //RegisteredObjects.AddFunction(typeof(CommonExpand).GetMethod("GetBrithDateByCardId"), "CustomMethod");

        }

        #region IDisposable 成员

        public void Dispose()
        {
            if (this.Report != null)
                this.Report.Dispose();
            if (this.frmPrint != null)
            {
                if (this.frmPrint.PreviewControl != null)
                    this.frmPrint.PreviewControl.Dispose();
                this.frmPrint.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="reportName">报表名称</param>
        public bool InitReport(string reportName)
        {
            string reportPath = Application.StartupPath + "\\Reports\\" + reportName + ".frx";
            if (System.IO.File.Exists(reportPath))
            {
                this.Report.Load(reportPath);
                this.title = reportName;
                return true;
            }
            else
            {
                MessageBox.Show("[" + reportName + "] 报表文件未找到\r\n" +
                "报表全路径为:" + reportPath, "提示");
                return false;
            }
        }

        public void AddData(DataTable dataTable)
        {
            this.AddData(dataTable, dataTable.TableName);
        }

        public void AddData(DataTable dataTable, string dataSourceName)
        {
            if (this.Report.Dictionary.DataSources.Count > 0)
                foreach (Column clm in this.Report.Dictionary.DataSources[0].Columns)
                {
                    if (!dataTable.Columns.Contains(clm.PropName)
                        && !clm.Calculated)
                    {
                        dataTable.Columns.Add(clm.PropName);
                        foreach (DataRow dr in dataTable.Rows)
                            dr[clm.PropName] = this.Report.Dictionary.DataSources[0].PropName + "." + clm.PropName;
                    }
                }
            this.Report.RegisterData(dataTable, dataSourceName);

        }

        public bool AddData(string materDataName, DataTable dataTable, string dataSourceName)
        {
            AddData(dataTable, dataSourceName);
            DataBand db = this.Report.FindObject(materDataName) as DataBand;
            if (db == null)
            {
                MessageBox.Show("未找到控件 [" + materDataName + "]", "提示");
                return false;
            }
            else
            {
                FastReport.Data.DataSourceBase source = this.Report.GetDataSource(dataSourceName);
                if (source == null)
                {
                    MessageBox.Show("没有找到名为[" + dataSourceName + "] 的数据源", "提示");
                    return false;
                }
                else
                {
                    db.DataSource = source;
                    return true;
                }
            }
        }

        public bool AddParam(string paramName, object value)
        {
            try
            {
                FastReport.Data.Parameter param = this.Report.Parameters.FindByName(paramName);
                if (param == null)
                {
                    FastReport.Data.Parameter paramNew = new FastReport.Data.Parameter(paramName);
                    paramNew.Value = value;
                    this.Report.Parameters.Add(paramNew);
                }
                else
                {
                    param.Value = value;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 设置预览报表标题
        /// </summary>
        /// <param name="title"></param>
        public void SetReportTitle(string title)
        {
            this.title = title;
        }

        /// <summary>
        /// 添加主从关系
        /// </summary>
        /// <param name="ChildControlName">
        /// 子控件名称
        /// </param>
        /// <param name="ParentColumnName">
        /// 对应父控件列名
        /// </param>
        /// <param name="ChildColumnName">
        /// 对应子控件列名
        /// </param>
        public void AddRelation(string childControlName, string parentColumnName, string childColumnName)
        {
            DataBand db = this.Report.FindObject(childControlName) as DataBand;
            FastReport.Data.Relation relation = new FastReport.Data.Relation();
            relation.ParentDataSource = (db.Parent as DataBand).DataSource;
            relation.ParentColumns = new string[] { parentColumnName };
            relation.ChildColumns = new string[] { childColumnName };
            db.Relation = relation;
        }

        /// <summary>
        /// 通过控件名称获得控件
        /// </summary>
        /// <param name="ControlName"></param>
        /// <returns></returns>
        public T GetControlByName<T>(string ControlName) where T : Base
        {
            return (T)this.Report.FindObject(ControlName);
        }

        /// <summary>
        /// 进行打印
        /// </summary>
        public void Print()
        {
            try
            {
                if (!this.Report.IsRunning)
                    this.Report.Print();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示");
            }
        }

        /// <summary>
        /// 进行预览
        /// </summary>
        public void PreView()
        {
            try
            {
                if (!this.Report.IsRunning)
                {
                    this.Report.Preview = this.Form.PreviewControl;
                    this.Report.Show(this.Form);
                    this.Form.Text = this.title;
                    this.Form.ShowDialog();
                    this.Dispose();
                }
            }
            catch (Exception ex)
            {
                if (PrintManager.CanDesign)
                {
                    if (MessageBox.Show((ex.Message.Length > 100 ? ex.Message.Substring(0, 100) + "\r\n和更多错误内容" : ex.Message)
                        + "\r\n\r\n" + "是否需要设计该报表?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2)
                        == DialogResult.Yes)
                    {
                        DesignerEx de = new DesignerEx(this.Report);
                        de.ShowDialog();
                    }
                }
                else
                {
                    MessageBox.Show(ex.Message, "提示");
                }
            }
        }
    }

}
