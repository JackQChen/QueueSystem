using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Db
{
    public class DbHelper
    {
        public static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["connString"].ConnectionString;

        public static DbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }

        /// <summary>
        /// 返回一个DataSet
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static DataSet ExecuteDataSet(string sqlString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                {
                    if (cmdParams != null)
                    {
                        cmd.Parameters.AddRange(cmdParams);
                    }

                    DataSet ds = new DataSet();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);
                    return ds;
                }
            }

        }
        /// <summary>
        /// 返回结果集的第一行第一列
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static object ExecuteScalar(string sqlString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                {
                    if (cmdParams != null)
                    {
                        cmd.Parameters.AddRange(cmdParams);
                    }
                    conn.Open();
                    object obj = cmd.ExecuteScalar();
                    cmd.Parameters.Clear();
                    return obj;
                }
            }
        }
        /// <summary>
        /// 返回受影响的行数
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static int ExecuteNonQurey(string sqlString, params SqlParameter[] cmdParams)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlString, conn))
                {
                    if (cmdParams != null)
                    {
                        cmd.Parameters.AddRange(cmdParams);
                    }
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }
        }
        /// <summary>
        /// 返回SqlDataReader 调用完此方法需将reader关闭
        /// </summary>
        /// <param name="sqlString"></param>
        /// <param name="cmdParams"></param>
        /// <returns></returns>
        public static SqlDataReader ExecuteReader(string sqlString, params SqlParameter[] cmdParams)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            SqlCommand cmd = new SqlCommand(sqlString, conn);
            if (cmdParams != null)
            {
                cmd.Parameters.AddRange(cmdParams);
            }
            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection);
            return reader;
        }

        public static DataTable FillDataTable(IDataReader reader)
        {
            DataTable dt = new DataTable();
            int fieldCount = reader.FieldCount;
            for (int i = 0; i < fieldCount; i++)
            {
                DataColumn dc = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                dt.Columns.Add(dc);
            }
            while (reader.Read())
            {
                DataRow dr = dt.NewRow();
                for (int i = 0; i < fieldCount; i++)
                {
                    dr[i] = reader[i];
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public static DataSet FillDataSet(IDataReader reader)
        {
            DataSet ds = new DataSet();
            var dt = FillDataTable(reader);
            ds.Tables.Add(dt);

            while (reader.NextResult())
            {
                dt = FillDataTable(reader);
                ds.Tables.Add(dt);
            }

            return ds;
        }

    }

}
