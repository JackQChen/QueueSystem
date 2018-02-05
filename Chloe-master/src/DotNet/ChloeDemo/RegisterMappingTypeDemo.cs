using Chloe.Infrastructure;
using Chloe.SqlServer;
using Database;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChloeDemo
{
    public static class RegisterMappingTypeDemo
    {
        /*sql script:
        CREATE TABLE [dbo].[ExtensionMappingType](
        	[Id] [int] IDENTITY(1,1) NOT NULL,
        	[Name] [nvarchar](100) NULL,
        	[F_Char] [nvarchar](1) NULL,
        	[F_Time] [time](7) NULL,
         CONSTRAINT [PK_ExtensionMappingType] PRIMARY KEY CLUSTERED 
        (
        	[Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY]
        */

        public static void RunDemo()
        {
            //step 1:
            /* 原生 Chloe 不支持 char 和 TimeSpan 类型映射，需要我们自己注册，注册映射类必须在程序启动时进行 */
            MappingTypeSystem.Register(typeof(char), DbType.StringFixedLength);
            MappingTypeSystem.Register(typeof(TimeSpan), DbType.Time);

            //step 2:
            /* 因为我们新增了 MappingType，所以需要对原生的 SqlConnection、SqlServerCommand、SqlServerDataReader、SqlServerParameter 包装处理，所以，我们需要自个儿实现 IDbConnectionFactory 工厂  */
            SqlServerDbConnectionFactory sqlServerDbConnectionFactory = new SqlServerDbConnectionFactory(DbHelper.ConnectionString);
            MsSqlContext context = new MsSqlContext(sqlServerDbConnectionFactory);

            /* 经过上述封装，我们就可以支持 char 和 TimeSpan 类型映射了 */

            ExtensionMappingType entity = new ExtensionMappingType();
            entity.Name = "test";
            entity.F_Char = 'A';
            entity.F_Time = TimeSpan.FromHours(12);
            context.Insert(entity);
            Console.WriteLine(entity.Id);

            TimeSpan ts = TimeSpan.FromHours(12);
            ExtensionMappingType model = context.Query<ExtensionMappingType>().Where(a => a.F_Time == ts && a.Id == entity.Id).First();
            Console.WriteLine(model.Id == entity.Id);


            Console.WriteLine("Yeah!!");
            Console.ReadKey();
        }
    }

    public class ExtensionMappingType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        [Chloe.Entity.Column(DbType = DbType.StringFixedLength)]/* 因为 char 是新增的映射类型，必须指定 DbType */
        public char? F_Char { get; set; }
        [Chloe.Entity.Column(DbType = DbType.Time)]/* 因为 TimeSpan 是新增的映射类型，必须指定 DbType */
        public TimeSpan? F_Time { get; set; }
    }

    /*
     * 自定义一个 DbConnectionFactory，实现 CreateConnection 方法，其返回我们自定义的 SqlServerConnection
     */
    public class SqlServerDbConnectionFactory : IDbConnectionFactory
    {
        string _connString;
        public SqlServerDbConnectionFactory(string connString)
        {
            this._connString = connString;
        }
        public IDbConnection CreateConnection()
        {
            SqlConnection conn = new SqlConnection(this._connString);

            /*
             tips：因为我们新增了 MappingType，所以需要对原生的 SqlConnection、SqlServerCommand、SqlServerDataReader、SqlServerParameter 包装处理
             */
            return new SqlServerConnection(conn);
        }
    }


    /*
     * 对于新增映射类型，需要包装驱动的的 DataParameter，拦截 DbType 和 Value 属性。
     */
    public class SqlServerParameter : IDbDataParameter, IDataParameter
    {
        internal SqlParameter _sqlParameter;
        IDbDataParameter Parameter
        {
            get
            {
                return (IDbDataParameter)this._sqlParameter;
            }
        }

        public SqlServerParameter(SqlParameter sqlParameter)
        {
            this._sqlParameter = sqlParameter;
        }

        public DbType DbType
        {
            get
            {
                return this.Parameter.DbType;
            }
            set
            {
                DbType dbType = value;
                if (value == System.Data.DbType.Time)
                {
                    /* 需要设置 SqlDbType 为 SqlDbType.Time，否则运行会报错 */
                    this.Parameter.DbType = dbType;
                    this._sqlParameter.SqlDbType = SqlDbType.Time;
                    return;
                }

                this.Parameter.DbType = dbType;
            }
        }
        public object Value
        {
            get
            {
                return this.Parameter.Value;
            }
            set
            {
                if (value != null)
                {
                    Type valueType = value.GetType();
                    if (valueType == typeof(TimeSpan))
                    {
                        this.DbType = System.Data.DbType.Time;
                    }
                    else if (valueType == typeof(char))
                    {
                        DbType dbType = this.DbType;
                        if (dbType != System.Data.DbType.String && dbType != System.Data.DbType.StringFixedLength && dbType != System.Data.DbType.AnsiString && dbType != System.Data.DbType.AnsiStringFixedLength)
                        {
                            this.DbType = System.Data.DbType.String;
                        }

                        this.Parameter.Value = new string(new char[1] { (char)value });
                        return;
                    }
                }

                this.Parameter.Value = value;
            }
        }

        public byte Precision
        {
            get
            {
                return this.Parameter.Precision;
            }
            set
            {
                this.Parameter.Precision = value;
            }
        }
        public byte Scale
        {
            get
            {
                return this.Parameter.Scale;
            }
            set
            {
                this.Parameter.Scale = value;
            }
        }
        public int Size
        {
            get
            {
                return this.Parameter.Size;
            }
            set
            {
                this.Parameter.Size = value;
            }
        }
        public ParameterDirection Direction
        {
            get
            {
                return this.Parameter.Direction;
            }
            set
            {
                this.Parameter.Direction = value;
            }
        }
        public bool IsNullable
        {
            get
            {
                return this.Parameter.IsNullable;
            }
        }
        public string ParameterName
        {
            get
            {
                return this.Parameter.ParameterName;
            }
            set
            {
                this.Parameter.ParameterName = value;
            }
        }
        public string SourceColumn
        {
            get
            {
                return this.Parameter.SourceColumn;
            }
            set
            {
                this.Parameter.SourceColumn = value;
            }
        }
        public DataRowVersion SourceVersion
        {
            get
            {
                return this.Parameter.SourceVersion;
            }
            set
            {
                this.Parameter.SourceVersion = value;
            }
        }

    }

    /*
     * 对驱动原生的 DataReader 包装
     */
    public class SqlServerDataReader : IDataReader, IDataRecord, IDisposable
    {
        IDataReader _reader;

        public SqlServerDataReader(IDataReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException();

            this._reader = reader;
        }

        #region IDataReader
        public int Depth { get { return this._reader.Depth; } }
        public bool IsClosed { get { return this._reader.IsClosed; } }
        public int RecordsAffected { get { return this._reader.RecordsAffected; } }

        public void Close()
        {
            this._reader.Close();
        }
        public DataTable GetSchemaTable()
        {
            return this._reader.GetSchemaTable();
        }
        public bool NextResult()
        {
            return this._reader.NextResult();
        }
        public bool Read()
        {
            return this._reader.Read();
        }

        public void Dispose()
        {
            this._reader.Dispose();
        }
        #endregion

        public int FieldCount { get { return this._reader.FieldCount; } }

        public object this[int i] { get { return this._reader[i]; } }
        public object this[string name] { get { return this._reader[name]; } }

        public bool GetBoolean(int i)
        {
            return this._reader.GetBoolean(i);
        }
        public byte GetByte(int i)
        {
            return this._reader.GetByte(i);
        }
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }

        /*
         *因为 SqlServer 不支持 char 类型，所以，如果我们需要支持 char 类型映射，在数据库里我们只能使用 char、varchar 等字符串类型映射，因此必须拦截 DataReader 的 GetChar() 方法处理
         */
        public char GetChar(int i)
        {
            Type fieldType = this._reader.GetFieldType(i);
            if (fieldType == typeof(string))
            {
                if (this._reader.IsDBNull(i) == false)
                {
                    string str = this._reader.GetString(i);
                    if (str.Length == 1)
                    {
                        return str[0];
                    }
                }
            }

            return this._reader.GetChar(i);
        }
        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetChars(i, fieldoffset, buffer, bufferoffset, length);
        }
        public IDataReader GetData(int i)
        {
            return this._reader.GetData(i);
        }
        public string GetDataTypeName(int i)
        {
            return this._reader.GetDataTypeName(i);
        }
        public DateTime GetDateTime(int i)
        {
            return this._reader.GetDateTime(i);
        }
        public decimal GetDecimal(int i)
        {
            return this._reader.GetDecimal(i);
        }
        public double GetDouble(int i)
        {
            return this._reader.GetDouble(i);
        }
        public Type GetFieldType(int i)
        {
            return this._reader.GetFieldType(i);
        }
        public float GetFloat(int i)
        {
            return this._reader.GetFloat(i);
        }
        public Guid GetGuid(int i)
        {
            return this._reader.GetGuid(i);
        }
        public short GetInt16(int i)
        {
            return this._reader.GetInt16(i);
        }
        public int GetInt32(int i)
        {
            return this._reader.GetInt32(i);
        }
        public long GetInt64(int i)
        {
            return this._reader.GetInt64(i);
        }
        public string GetName(int i)
        {
            return this._reader.GetName(i);
        }
        public int GetOrdinal(string name)
        {
            return this._reader.GetOrdinal(name);
        }
        public string GetString(int i)
        {
            return this._reader.GetString(i);
        }
        public object GetValue(int i)
        {
            return this._reader.GetValue(i);
        }
        public int GetValues(object[] values)
        {
            return this._reader.GetValues(values);
        }
        public bool IsDBNull(int i)
        {
            return this._reader.IsDBNull(i);
        }
    }

    /*
     * 对驱动原生的 SqlCommand 包装
     */
    public class SqlServerCommand : IDbCommand, IDisposable
    {
        SqlCommand _dbCommand;
        DataParameterCollection _dataParameterCollection;
        public SqlServerCommand(SqlCommand dbCommand)
        {
            if (dbCommand == null)
                throw new ArgumentNullException();

            this._dbCommand = dbCommand;
            this._dataParameterCollection = new DataParameterCollection(dbCommand.Parameters);
        }

        public string CommandText
        {
            get
            {
                return this._dbCommand.CommandText;
            }
            set
            {
                this._dbCommand.CommandText = value;
            }
        }
        public int CommandTimeout
        {
            get
            {
                return this._dbCommand.CommandTimeout;
            }
            set
            {
                this._dbCommand.CommandTimeout = value;
            }
        }
        public CommandType CommandType
        {
            get
            {
                return this._dbCommand.CommandType;
            }
            set
            {
                this._dbCommand.CommandType = value;
            }
        }
        public IDbConnection Connection
        {
            get
            {
                return ((IDbCommand)this._dbCommand).Connection;
            }
            set
            {
                ((IDbCommand)this._dbCommand).Connection = value;
            }
        }
        public IDataParameterCollection Parameters
        {
            get
            {
                return this._dataParameterCollection;
            }
        }
        public IDbTransaction Transaction
        {
            get
            {
                return ((IDbCommand)this._dbCommand).Transaction;
            }
            set
            {
                ((IDbCommand)this._dbCommand).Transaction = value;
            }
        }
        public UpdateRowSource UpdatedRowSource
        {
            get
            {
                return this._dbCommand.UpdatedRowSource;
            }
            set
            {
                this._dbCommand.UpdatedRowSource = value;
            }
        }

        public void Cancel()
        {
            this._dbCommand.Cancel();
        }
        /*
         *返回自定义的 SqlServerParameter
         */
        public IDbDataParameter CreateParameter()
        {
            return new SqlServerParameter(this._dbCommand.CreateParameter());
        }
        public int ExecuteNonQuery()
        {
            return this._dbCommand.ExecuteNonQuery();
        }
        public IDataReader ExecuteReader()
        {
            return new SqlServerDataReader(this._dbCommand.ExecuteReader());
        }
        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new SqlServerDataReader(this._dbCommand.ExecuteReader(behavior));
        }
        public object ExecuteScalar()
        {
            return this._dbCommand.ExecuteScalar();
        }
        public void Prepare()
        {
            this._dbCommand.Prepare();
        }
        public void Dispose()
        {
            this._dbCommand.Dispose();
        }



        public class DataParameterCollection : IDataParameterCollection
        {
            IDataParameterCollection _parameterCollection;
            Dictionary<SqlParameter, SqlServerParameter> _paramters = new Dictionary<SqlParameter, SqlServerParameter>();
            public DataParameterCollection(IDataParameterCollection parameterCollection)
            {
                this._parameterCollection = parameterCollection;
            }

            public object this[string parameterName]
            {
                get
                {
                    throw new NotImplementedException();

                    //SqlParameter sqlParameter = this._parameterCollection[parameterName] as SqlParameter;

                    //if (sqlParameter != null)
                    //{
                    //    return this._paramters[sqlParameter];
                    //}

                    //return null;
                }
                set
                {
                    throw new NotImplementedException();

                    //SqlServerParameter sqlServerParameter = (SqlServerParameter)value;
                    //if(sqlServerParameter==null)

                }
            }

            public bool Contains(string parameterName)
            {
                return this._parameterCollection.Contains(parameterName);
            }

            public int IndexOf(string parameterName)
            {
                throw new NotImplementedException();
            }

            public void RemoveAt(string parameterName)
            {
                throw new NotImplementedException();
            }

            public bool IsFixedSize { get { return this._parameterCollection.IsFixedSize; } }
            public bool IsReadOnly { get { return this._parameterCollection.IsReadOnly; } }

            public object this[int index]
            {
                get
                {
                    throw new NotImplementedException();
                }
                set
                {
                    throw new NotImplementedException();
                }
            }

            public int Add(object value)
            {
                SqlServerParameter sqlServerParameter = (SqlServerParameter)value;

                int index = this._parameterCollection.Add(sqlServerParameter._sqlParameter);
                var xx = this._parameterCollection[sqlServerParameter._sqlParameter.ParameterName];
                if (!this._paramters.ContainsKey(sqlServerParameter._sqlParameter))
                {
                    this._paramters.Add(sqlServerParameter._sqlParameter, sqlServerParameter);
                }

                return index;

                throw new NotImplementedException();
            }
            public void Clear()
            {
                this._parameterCollection.Clear();
                this._paramters.Clear();
            }
            public bool Contains(object value)
            {
                throw new NotImplementedException();
            }
            public int IndexOf(object value)
            {
                throw new NotImplementedException();
            }
            public void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }
            public void Remove(object value)
            {
                throw new NotImplementedException();
            }
            public void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }


            public int Count
            {
                get
                {
                    return this._parameterCollection.Count;
                }
            }
            public bool IsSynchronized
            {
                get
                {
                    return this._parameterCollection.IsSynchronized;
                }
            }
            public object SyncRoot
            {
                get
                {
                    return this._parameterCollection.SyncRoot;
                }
            }

            public void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public IEnumerator GetEnumerator()
            {
                foreach (var item in this._parameterCollection)
                {
                    yield return this._paramters[(SqlParameter)item];
                }
            }
        }
    }

    /*
     * 对驱动原生的 SqlConnection 包装
     */
    public class SqlServerConnection : IDbConnection, IDisposable, ICloneable
    {
        SqlConnection _dbConnection;
        public SqlServerConnection(SqlConnection dbConnection)
        {
            if (dbConnection == null)
                throw new ArgumentNullException();

            this._dbConnection = dbConnection;
        }

        public string ConnectionString
        {
            get { return this._dbConnection.ConnectionString; }
            set { this._dbConnection.ConnectionString = value; }
        }
        public int ConnectionTimeout
        {
            get { return this._dbConnection.ConnectionTimeout; }
        }
        public string Database
        {
            get { return this._dbConnection.Database; }
        }
        public ConnectionState State
        {
            get { return this._dbConnection.State; }
        }

        public IDbTransaction BeginTransaction()
        {
            return this._dbConnection.BeginTransaction();
        }
        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return this._dbConnection.BeginTransaction(il);
        }
        public void ChangeDatabase(string databaseName)
        {
            this._dbConnection.ChangeDatabase(databaseName);
        }
        public void Close()
        {
            this._dbConnection.Close();
        }

        /*
         * 返回我们自定义的 SqlServerCommand
         */
        public IDbCommand CreateCommand()
        {
            return new SqlServerCommand(this._dbConnection.CreateCommand());
        }
        public void Open()
        {
            this._dbConnection.Open();
        }

        public void Dispose()
        {
            this._dbConnection.Dispose();
        }
        public object Clone()
        {
            if (this._dbConnection is ICloneable)
            {
                return new SqlServerConnection((SqlConnection)((ICloneable)this._dbConnection).Clone());
            }

            throw new NotSupportedException();
        }
    }

}
