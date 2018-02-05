using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Chloe.Extensions
{
    public static class DataReaderExtension
    {
        public static short GetInt16(IDataReader reader, int ordinal)
        {
            return reader.GetInt16(ordinal);
        }
        public static short? GetInt16_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetInt16(ordinal);
        }

        public static int GetInt32(IDataReader reader, int ordinal)
        {
            return reader.GetInt32(ordinal);
        }
        public static int? GetInt32_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetInt32(ordinal);
        }

        public static long GetInt64(IDataReader reader, int ordinal)
        {
            return reader.GetInt64(ordinal);
        }
        public static long? GetInt64_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetInt64(ordinal);
        }

        public static decimal GetDecimal(IDataReader reader, int ordinal)
        {
            return reader.GetDecimal(ordinal);
        }
        public static decimal? GetDecimal_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetDecimal(ordinal);
        }

        public static double GetDouble(IDataReader reader, int ordinal)
        {
            return reader.GetDouble(ordinal);
        }
        public static double? GetDouble_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetDouble(ordinal);
        }

        public static float GetFloat(IDataReader reader, int ordinal)
        {
            return reader.GetFloat(ordinal);
        }
        public static float? GetFloat_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetFloat(ordinal);
        }

        public static bool GetBoolean(IDataReader reader, int ordinal)
        {
            return reader.GetBoolean(ordinal);
        }
        public static bool? GetBoolean_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetBoolean(ordinal);
        }

        public static DateTime GetDateTime(IDataReader reader, int ordinal)
        {
            return reader.GetDateTime(ordinal);
        }
        public static DateTime? GetDateTime_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetDateTime(ordinal);
        }

        public static Guid GetGuid(IDataReader reader, int ordinal)
        {
            return reader.GetGuid(ordinal);
        }
        public static Guid? GetGuid_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetGuid(ordinal);
        }

        public static byte GetByte(IDataReader reader, int ordinal)
        {
            return reader.GetByte(ordinal);
        }
        public static byte? GetByte_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetByte(ordinal);
        }

        public static char GetChar(IDataReader reader, int ordinal)
        {
            return reader.GetChar(ordinal);
        }
        public static char? GetChar_Nullable(this IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetChar(ordinal);
        }

        public static string GetString(IDataReader reader, int ordinal)
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return reader.GetString(ordinal);
        }

        public static object GetValue(IDataReader reader, int ordinal)
        {
            object val = reader.GetValue(ordinal);
            if (val == DBNull.Value)
            {
                return null;
            }

            return val;
        }

        public static TEnum GetEnum<TEnum>(this IDataReader reader, int ordinal) where TEnum : struct
        {
            Type fieldType = reader.GetFieldType(ordinal);

            object value;
            if (fieldType == UtilConstants.TypeOfByte)
                value = reader.GetByte(ordinal);
            else if (fieldType == UtilConstants.TypeOfInt16)
                value = reader.GetInt16(ordinal);
            else if (fieldType == UtilConstants.TypeOfInt64)
                value = reader.GetInt64(ordinal);
            else
                value = reader.GetInt32(ordinal);

            return (TEnum)Enum.ToObject(typeof(TEnum), value);
        }
        public static TEnum? GetEnum_Nullable<TEnum>(this IDataReader reader, int ordinal) where TEnum : struct
        {
            if (reader.IsDBNull(ordinal))
            {
                return null;
            }

            return GetEnum<TEnum>(reader, ordinal);
        }

        public static T GetTValue<T>(this IDataReader reader, int ordinal)
        {
            object val = GetValue(reader, ordinal);

            try
            {
                return (T)val;
            }
            catch (NullReferenceException)
            {
                throw new InvalidCastException("The column value could not be null.");
            }
        }
        public static T? GetTValue_Nullable<T>(this IDataReader reader, int ordinal) where T : struct
        {
            object val = reader.GetValue(ordinal);
            if (val == DBNull.Value)
            {
                return null;
            }

            return new Nullable<T>((T)val);
        }

    }
}
