using System;
using System.Collections.Generic;
using System.Data;

namespace Chloe.Core
{
    class InternalDataReader : IDataReader, IDisposable, IDataRecord
    {
        InternalAdoSession _adoSession;
        IDataReader _reader;
        IDbCommand _cmd;
        List<OutputParameter> _outputParameters;
        bool _disposed = false;


        public InternalDataReader(InternalAdoSession adoSession, IDataReader reader, IDbCommand cmd, List<OutputParameter> outputParameters)
        {
            Utils.CheckNull(adoSession);
            Utils.CheckNull(reader);
            Utils.CheckNull(cmd);

            this._adoSession = adoSession;
            this._reader = reader;
            this._cmd = cmd;
            this._outputParameters = outputParameters;
        }

        #region IDataReader
        public int Depth { get { return this._reader.Depth; } }
        public bool IsClosed { get { return this._reader.IsClosed; } }
        public int RecordsAffected { get { return this._reader.RecordsAffected; } }

        public void Close()
        {
            if (!this._reader.IsClosed)
            {
                try
                {
                    this._reader.Close();
                    this._reader.Dispose();/* Tips：.NET Core 的 SqlServer 驱动 System.Data.SqlClient(4.1.0) 中，调用 DataReader.Dispose() 方法后才能拿到 Output 参数值，这算是坑爹么？？ */
                    OutputParameter.CallMapValue(this._outputParameters);
                }
                finally
                {
                    this._adoSession.Complete();
                }
            }
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
            if (this._disposed)
                return;

            this.Close();
            this._cmd.Dispose();

            this._disposed = true;
        }
        #endregion

        #region IDataRecord
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
        public char GetChar(int i)
        {
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
        #endregion
    }
}
