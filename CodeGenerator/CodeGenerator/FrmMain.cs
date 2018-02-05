using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace CodeGenerator
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var tableName = this.txtTableName.Text;
            MySqlConnection conn = new MySqlConnection(@"Database='QueueDB';Data Source='cysoft.uicp.net';User Id='root';Password='admin88';charset='utf8';pooling=true");
            conn.Open();
            MySqlDataAdapter adapter = new MySqlDataAdapter(@"
select COLUMN_NAME,DATA_TYPE,COLUMN_COMMENT 
from information_schema.COLUMNS 
where  TABLE_SCHEMA = 'QueueDB' and table_name = '" + tableName + @"'", conn);
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
            string strCode = string.Format(@"using System.Collections.Generic;
using Chloe;
using Model;

namespace DAL
{{
    public class {0}
    {{
        DbContext db;
        public {0}()
        {{
            this.db = Factory.Instance.CreateDbContext();
        }}

        #region CommonMethods

        public List<{1}> GetModelList()
        {{
            return db.Query<{1}>().ToList();
        }}

        public {1} GetModel(int id)
        {{
            return db.Query<{1}>().Where(p => p.id == id).FirstOrDefault();
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
            string strCode = string.Format(@"using System.Collections.Generic;
using DAL;
using Model;

namespace BLL
{{
    public class {0}
    {{
        public {0}()
        {{
        }}

        #region CommonMethods

        public List<{2}> GetModelList()
        {{
            return new {1}().GetModelList();
        }}

        public {2} GetModel(int id)
        {{
            return new {1}().GetModel(id);
        }}

        public {2} Insert({2} model)
        {{
            return new {1}().Insert(model);
        }}

        public int Update({2} model)
        {{
            return new {1}().Update(model);
        }}

        public int Delete({2} model)
        {{
            return new {1}().Delete(model);
        }}

        #endregion
    }}
}}
", tableName.Replace("_", "") + "BLL", tableName.Replace("_", "") + "DAL", tableName.Replace("_", "") + "Model");
            this.txtCode.Text = strCode;
        }
    }
}
