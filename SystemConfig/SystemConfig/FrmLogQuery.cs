using System.Windows.Forms;
using System;
using System.IO;
using System.Text;

namespace SystemConfig
{
    public partial class FrmLogQuery : Form
    {
        BLL.TOprateLogBLL logBll = new BLL.TOprateLogBLL();

        public FrmLogQuery()
        {
            InitializeComponent();
        }

        private void FrmLogQuery_Load(object sender, System.EventArgs e)
        {
            this.dataGridView1.AutoGenerateColumns = false;
            this.dtStart.Value = DateTime.Now.Date;
            this.dtEnd.Value = DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            var dic = logBll.GetQueryParams();
            this.cmbTerminalType.Items.AddRange(dic["tType"].ToArray());
            this.cmbTerminalName.Items.AddRange(dic["tName"].ToArray());
            this.cmbOperateType.Items.AddRange(dic["oType"].ToArray());
        }

        private void btnQuery_Click(object sender, System.EventArgs e)
        {
            this.dataGridView1.DataSource = this.logBll.Query(
                this.cmbTerminalType.Text,
                this.cmbTerminalName.Text,
                this.cmbOperateType.Text,
                this.dtStart.Value,
                this.dtEnd.Value,
                this.txtLogContent.Text);
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportExcel();
        }

        public void ExportExcel()
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Excel文件(*.xls)|*.xls";
                saveFileDialog.FilterIndex = 0;
                saveFileDialog.RestoreDirectory = true;
                saveFileDialog.Title = "导出数据";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Stream myStream;
                    myStream = saveFileDialog.OpenFile();
                    StreamWriter sw = new StreamWriter(myStream, Encoding.Default);
                    string str = "";
                    try
                    {
                        for (int i = 0; i < dataGridView1.ColumnCount; i++)
                        {
                            if (i > 0)
                            {
                                str += "\t";
                            }
                            str += dataGridView1.Columns[i].HeaderText;
                        }
                        sw.WriteLine(str);
                        for (int j = 0; j < dataGridView1.Rows.Count; j++)
                        {
                            string tempStr = "";
                            for (int k = 0; k < dataGridView1.Columns.Count; k++)
                            {
                                if (k > 0)
                                {
                                    tempStr += "\t";
                                }
                                tempStr += dataGridView1.Rows[j].Cells[k].Value.ToString();
                            }
                            sw.WriteLine(tempStr);
                        }
                        sw.Close();
                        myStream.Close();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.ToString());
                    }
                    finally
                    {
                        sw.Close();
                        myStream.Close();
                    }
                    MessageBox.Show("导出完成！");
                }
            }
        }
    }
}
