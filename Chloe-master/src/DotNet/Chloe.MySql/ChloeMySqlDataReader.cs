using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.MySql
{
    public class ChloeMySqlDataReader : IDataReader, IDataRecord, IDisposable
    {
        IDataReader _reader;

        public ChloeMySqlDataReader(IDataReader reader)
        {
            Utils.CheckNull(reader);
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
            object obj = this._reader.GetValue(i);
            if (obj is byte)
                return (byte)obj;

            return Convert.ToByte(obj);
        }
        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return this._reader.GetBytes(i, fieldOffset, buffer, bufferoffset, length);
        }
        public char GetChar(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is char)
                return (char)obj;

            return Convert.ToChar(obj);
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
            object obj = this._reader.GetValue(i);
            if (obj is DateTime)
                return (DateTime)obj;

            return Convert.ToDateTime(obj);
        }
        public decimal GetDecimal(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is decimal)
                return (decimal)obj;

            return Convert.ToDecimal(obj);
        }
        public double GetDouble(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is double)
                return (double)obj;

            return Convert.ToDouble(obj);
        }
        public Type GetFieldType(int i)
        {
            return this._reader.GetFieldType(i);
        }
        public float GetFloat(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is float)
                return (float)obj;

            return Convert.ToSingle(obj);
        }
        public Guid GetGuid(int i)
        {
            return this._reader.GetGuid(i);
        }
        public short GetInt16(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is short)
                return (short)obj;

            return Convert.ToInt16(obj);
        }
        public int GetInt32(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is Int32)
                return (Int32)obj;

            return Convert.ToInt32(obj);
        }
        public long GetInt64(int i)
        {
            object obj = this._reader.GetValue(i);
            if (obj is Int64)
                return (Int64)obj;

            return Convert.ToInt64(obj);
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
}
