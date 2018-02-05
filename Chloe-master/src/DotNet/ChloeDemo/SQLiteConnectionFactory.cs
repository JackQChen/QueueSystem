using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloeDemo
{
    public class SQLiteConnectionFactory : IDbConnectionFactory
    {
        string _connString = null;
        public SQLiteConnectionFactory(string connString)
        {
            this._connString = connString;
        }
        public IDbConnection CreateConnection()
        {
            /* You must add the 'System.Data.SQLite.dll' and implement this method  */

            //throw new NotImplementedException("You must add the System.Data.SQLite.dll and implement the method 'CreateConnection()'.");

            /*
             * If there is an error occurred because can not load the assembly 'SQLite.Interop.dll',the 'SQLite.Interop.dll' in the directories 'x64' and 'x86', you should copy them with folder to the directory 'bin\Debug' and try again.
             */

            SQLiteConnection conn = new SQLiteConnection(this._connString);
            return conn;
        }
    }
}
