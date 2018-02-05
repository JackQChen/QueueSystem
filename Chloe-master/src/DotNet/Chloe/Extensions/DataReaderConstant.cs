using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Chloe.InternalExtensions;

namespace Chloe.Extensions
{
    static class DataReaderConstant
    {
        #region
        internal static MethodInfo GetReaderMethod(Type type)
        {
            MethodInfo result;
            bool isNullable = false;
            Type underlyingType;
            if (type.IsNullable(out underlyingType))
            {
                isNullable = true;
                type = underlyingType;
            }

            if (type.IsEnum)
            {
                result = (isNullable ? Reader_GetEnum_Nullable : Reader_GetEnum).MakeGenericMethod(type);
                return result;
            }

            TypeCode typeCode = Type.GetTypeCode(type);

            switch (typeCode)
            {
                case TypeCode.Int16:
                    result = isNullable ? Reader_GetInt16_Nullable : Reader_GetInt16;
                    break;
                case TypeCode.Int32:
                    result = isNullable ? Reader_GetInt32_Nullable : Reader_GetInt32;
                    break;
                case TypeCode.Int64:
                    result = isNullable ? Reader_GetInt64_Nullable : Reader_GetInt64;
                    break;
                case TypeCode.Decimal:
                    result = isNullable ? Reader_GetDecimal_Nullable : Reader_GetDecimal;
                    break;
                case TypeCode.Double:
                    result = isNullable ? Reader_GetDouble_Nullable : Reader_GetDouble;
                    break;
                case TypeCode.Single:
                    result = isNullable ? Reader_GetFloat_Nullable : Reader_GetFloat;
                    break;
                case TypeCode.Boolean:
                    result = isNullable ? Reader_GetBoolean_Nullable : Reader_GetBoolean;
                    break;
                case TypeCode.DateTime:
                    result = isNullable ? Reader_GetDateTime_Nullable : Reader_GetDateTime;
                    break;
                case TypeCode.Byte:
                    result = isNullable ? Reader_GetByte_Nullable : Reader_GetByte;
                    break;
                case TypeCode.Char:
                    result = isNullable ? Reader_GetChar_Nullable : Reader_GetChar;
                    break;
                case TypeCode.String:
                    result = Reader_GetString;
                    break;
                default:
                    if (type == UtilConstants.TypeOfGuid)
                    {
                        result = isNullable ? Reader_GetGuid_Nullable : Reader_GetGuid;
                    }
                    else if (type == UtilConstants.TypeOfObject)
                    {
                        result = Reader_GetValue;
                    }
                    else
                    {
                        result = (isNullable ? Reader_GetTValue_Nullable : Reader_GetTValue).MakeGenericMethod(type);
                    }
                    break;
            }
            return result;
        }

        #region
        internal static readonly MethodInfo Reader_GetInt16 = typeof(DataReaderExtension).GetMethod("GetInt16");
        internal static readonly MethodInfo Reader_GetInt16_Nullable = typeof(DataReaderExtension).GetMethod("GetInt16_Nullable");
        internal static readonly MethodInfo Reader_GetInt32 = typeof(DataReaderExtension).GetMethod("GetInt32");
        internal static readonly MethodInfo Reader_GetInt32_Nullable = typeof(DataReaderExtension).GetMethod("GetInt32_Nullable");
        internal static readonly MethodInfo Reader_GetInt64 = typeof(DataReaderExtension).GetMethod("GetInt64");
        internal static readonly MethodInfo Reader_GetInt64_Nullable = typeof(DataReaderExtension).GetMethod("GetInt64_Nullable");
        internal static readonly MethodInfo Reader_GetDecimal = typeof(DataReaderExtension).GetMethod("GetDecimal");
        internal static readonly MethodInfo Reader_GetDecimal_Nullable = typeof(DataReaderExtension).GetMethod("GetDecimal_Nullable");
        internal static readonly MethodInfo Reader_GetDouble = typeof(DataReaderExtension).GetMethod("GetDouble");
        internal static readonly MethodInfo Reader_GetDouble_Nullable = typeof(DataReaderExtension).GetMethod("GetDouble_Nullable");
        internal static readonly MethodInfo Reader_GetFloat = typeof(DataReaderExtension).GetMethod("GetFloat");
        internal static readonly MethodInfo Reader_GetFloat_Nullable = typeof(DataReaderExtension).GetMethod("GetFloat_Nullable");
        internal static readonly MethodInfo Reader_GetBoolean = typeof(DataReaderExtension).GetMethod("GetBoolean");
        internal static readonly MethodInfo Reader_GetBoolean_Nullable = typeof(DataReaderExtension).GetMethod("GetBoolean_Nullable");
        internal static readonly MethodInfo Reader_GetDateTime = typeof(DataReaderExtension).GetMethod("GetDateTime");
        internal static readonly MethodInfo Reader_GetDateTime_Nullable = typeof(DataReaderExtension).GetMethod("GetDateTime_Nullable");
        internal static readonly MethodInfo Reader_GetGuid = typeof(DataReaderExtension).GetMethod("GetGuid");
        internal static readonly MethodInfo Reader_GetGuid_Nullable = typeof(DataReaderExtension).GetMethod("GetGuid_Nullable");
        internal static readonly MethodInfo Reader_GetByte = typeof(DataReaderExtension).GetMethod("GetByte");
        internal static readonly MethodInfo Reader_GetByte_Nullable = typeof(DataReaderExtension).GetMethod("GetByte_Nullable");
        internal static readonly MethodInfo Reader_GetChar = typeof(DataReaderExtension).GetMethod("GetChar");
        internal static readonly MethodInfo Reader_GetChar_Nullable = typeof(DataReaderExtension).GetMethod("GetChar_Nullable");
        internal static readonly MethodInfo Reader_GetString = typeof(DataReaderExtension).GetMethod("GetString");
        internal static readonly MethodInfo Reader_GetValue = typeof(DataReaderExtension).GetMethod("GetValue");

        internal static readonly MethodInfo Reader_GetEnum = typeof(DataReaderExtension).GetMethod("GetEnum");
        internal static readonly MethodInfo Reader_GetEnum_Nullable = typeof(DataReaderExtension).GetMethod("GetEnum_Nullable");

        internal static readonly MethodInfo Reader_GetTValue = typeof(DataReaderExtension).GetMethod("GetTValue");
        internal static readonly MethodInfo Reader_GetTValue_Nullable = typeof(DataReaderExtension).GetMethod("GetTValue_Nullable");
        #endregion

        #endregion

    }
}
