using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public class DbHelper
    {
        public static readonly string ConnectionString = "Data Source = .;Initial Catalog = Chloe;Integrated Security = SSPI;";

        public static DbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(ConnectionString);
            return conn;
        }
    }
}
