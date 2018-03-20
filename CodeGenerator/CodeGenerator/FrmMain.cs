using System;
using System.Configuration;
using System.Data;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Office.Interop.Word;
using MySql.Data.MySqlClient;

namespace CodeGenerator
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            this.txtTableName.Items.Clear();
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString))
            {
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(@"select table_name from information_schema.tables where table_schema='" + conn.Database + "'", conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    this.txtTableName.Items.Add(Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(row["table_name"].ToString()));
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tableName = this.txtTableName.Text;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString))
            {
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(@"
select COLUMN_NAME,DATA_TYPE,COLUMN_COMMENT 
from information_schema.COLUMNS 
where  TABLE_SCHEMA = '" + conn.Database + "' and table_name = '" + tableName + @"'", conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                string strProps = "";
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    string strProp = string.Format(@"

        /// <summary>
        /// {2}
        /// </summary>
        public {0} {1} {{ get; set; }}",
                  getCSharpType(row["DATA_TYPE"].ToString()),
                  row["COLUMN_NAME"],
                  row["COLUMN_COMMENT"]
                );
                    strProps += strProp;
                }
                string strCode = string.Format(@"using Chloe.Entity;

namespace Model
{{
    [Table(""{0}"")]
    public class {1}Model
        {{{2}

        }}
}}

", tableName, tableName.Replace("_", ""), strProps);
                this.txtCode.Text = strCode;
            }
        }

        string getCSharpType(string dbType)
        {
            switch (dbType.ToLower())
            {
                case "varchar":
                    return "string";
                case "datetime":
                    return "DateTime";
                case "int":
                    return "int";
            }
            return "";
        }

        private void txtCode_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = sender as TextBox;
            if (e.Control && e.KeyCode == Keys.A)
                t.SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var tableName = this.txtTableName.Text;
            string strCode = string.Format(@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Chloe;
using Model;

namespace DAL
{{
    public class {0}
    {{

        private DbContext db;

        public {0}()
        {{
            this.db = Factory.Instance.CreateDbContext();
        }}

        #region CommonMethods

        public List<{1}> GetModelList()
        {{
            return db.Query<{1}>().ToList();
        }}

        public List<{1}> GetModelList(Expression<Func<{1}, bool>> predicate)
        {{
            return db.Query<{1}>().Where(predicate).ToList();
        }}

        public {1} GetModel(int id)
        {{
            return db.Query<{1}>().Where(p => p.id == id).FirstOrDefault();
        }}

        public {1} GetModel(Expression<Func<{1}, bool>> predicate)
        {{
            return db.Query<{1}>().Where(predicate).FirstOrDefault();
        }}

        public {1} Insert({1} model)
        {{
            return db.Insert(model);
        }}

        public int Update({1} model)
        {{
            return this.db.Update(model);
        }}

        public int Delete({1} model)
        {{
            return this.db.Delete(model);
        }}

        #endregion
    }}
}}
", tableName.Replace("_", "") + "DAL", tableName.Replace("_", "") + "Model");
            this.txtCode.Text = strCode;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            var tableName = this.txtTableName.Text;
            string strCode = string.Format(@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using DAL;
using Model;

namespace BLL
{{
    public class {0}
    {{

        private {1} dal;

        public {0}()
        {{
            this.dal = new {1}();
        }}

        public {0}(string dbKey)
        {{
            this.dal = new {1}(dbKey);
        }}

        #region CommonMethods

        public List<{2}> GetModelList()
        {{
            return this.dal.GetModelList();
        }}

        public List<{2}> GetModelList(Expression<Func<{2}, bool>> predicate)
        {{
            return this.dal.GetModelList(predicate);
        }}

        public {2} GetModel(int id)
        {{
            return this.dal.GetModel(id);
        }}

        public {2} GetModel(Expression<Func<{2}, bool>> predicate)
        {{
            return this.dal.GetModel(predicate);
        }}

        public {2} Insert({2} model)
        {{
            return this.dal.Insert(model);
        }}

        public int Update({2} model)
        {{
            return this.dal.Update(model);
        }}

        public int Delete({2} model)
        {{
            return this.dal.Delete(model);
        }}

        #endregion
    }}
}}
", tableName.Replace("_", "") + "BLL", tableName.Replace("_", "") + "DAL", tableName.Replace("_", "") + "Model");
            this.txtCode.Text = strCode;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            var tableName = this.txtTableName.Text;
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["MySQL"].ConnectionString))
            {
                conn.Open();
                MySqlDataAdapter adapter = new MySqlDataAdapter(@"
select COLUMN_NAME,DATA_TYPE,IS_NULLABLE,COLUMN_COMMENT 
from information_schema.COLUMNS 
where  TABLE_SCHEMA = '" + conn.Database + "' and table_name = '" + tableName + @"'", conn);
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                //Word操作
                var app = new Microsoft.Office.Interop.Word.Application();
                Document doc = app.Documents.Add();
                app = doc.Application;
                Bookmark bk = doc.Bookmarks.Add("Database");
                var rowCount = ds.Tables[0].Rows.Count;
                Table tb = doc.Tables.Add(bk.Range, rowCount + 1, 4);
                tb.set_Style("网格型");
                tb.Cell(1, 1).Range.Text = "列名";
                tb.Cell(1, 2).Range.Text = "数据类型";
                tb.Cell(1, 3).Range.Text = "可空";
                tb.Cell(1, 4).Range.Text = "描述";
                for (int i = 0; i < rowCount; i++)
                {
                    DataRow row = ds.Tables[0].Rows[i];
                    tb.Cell(i + 2, 1).Range.Text = row["COLUMN_NAME"].ToString();
                    tb.Cell(i + 2, 2).Range.Text = row["DATA_TYPE"].ToString();
                    tb.Cell(i + 2, 3).Range.Text = row["IS_NULLABLE"].ToString();
                    tb.Cell(i + 2, 4).Range.Text = row["COLUMN_COMMENT"].ToString();
                }
                doc.SaveAs(AppDomain.CurrentDomain.BaseDirectory + "Database.doc");
                doc.Close();
                app.Quit();
            }
        }
    }
}
