using System;
using System.Collections.Generic;
using System.Data;
using Chloe.InternalExtensions;

namespace Chloe.Infrastructure
{
    public static class MappingTypeSystem
    {
        static readonly object _lockObj = new object();
        static readonly Dictionary<Type, DbType> _defaultTypeMap;
        static readonly Dictionary<Type, DbType> _typeMap;

        static MappingTypeSystem()
        {
            Dictionary<Type, DbType> defaultTypeMap = new Dictionary<Type, DbType>();
            defaultTypeMap[typeof(byte)] = DbType.Byte;
            defaultTypeMap[typeof(sbyte)] = DbType.SByte;
            defaultTypeMap[typeof(short)] = DbType.Int16;
            defaultTypeMap[typeof(ushort)] = DbType.UInt16;
            defaultTypeMap[typeof(int)] = DbType.Int32;
            defaultTypeMap[typeof(uint)] = DbType.UInt32;
            defaultTypeMap[typeof(long)] = DbType.Int64;
            defaultTypeMap[typeof(ulong)] = DbType.UInt64;
            defaultTypeMap[typeof(float)] = DbType.Single;
            defaultTypeMap[typeof(double)] = DbType.Double;
            defaultTypeMap[typeof(decimal)] = DbType.Decimal;
            defaultTypeMap[typeof(bool)] = DbType.Boolean;
            defaultTypeMap[typeof(string)] = DbType.String;
            defaultTypeMap[typeof(Guid)] = DbType.Guid;
            defaultTypeMap[typeof(DateTime)] = DbType.DateTime;
            defaultTypeMap[typeof(DateTimeOffset)] = DbType.DateTimeOffset;
            defaultTypeMap[typeof(TimeSpan)] = DbType.Time;
            defaultTypeMap[typeof(byte[])] = DbType.Binary;
            //defaultTypeMap[typeof(Object)] = DbType.Object; // ignore typeof(Object).

            _defaultTypeMap = Utils.Clone(defaultTypeMap);
            _typeMap = Utils.Clone(defaultTypeMap);
        }
        /// <summary>
        /// 注册一个需要映射的类型。
        /// </summary>
        /// <param name="type">新增的映射类型</param>
        /// <param name="dbTypeToMap">映射的 DbType。如果是扩展的 DbType，务必对原生的 System.Data.IDataParameter 进行包装，拦截 IDataParameter.DbType 属性的 setter 以对 dbTypeToMap 处理。</param>
        public static void Register(Type type, DbType dbTypeToMap)
        {
            Utils.CheckNull(type);

            type = type.GetUnderlyingType();
            lock (_lockObj)
            {
                if (!_typeMap.ContainsKey(type))
                    _typeMap.Add(type, dbTypeToMap);
            }
        }
        public static DbType? GetDbType(Type type)
        {
            if (type == null)
                return null;

            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                underlyingType = Enum.GetUnderlyingType(underlyingType);

            DbType ret;
            if (_typeMap.TryGetValue(underlyingType, out ret))
                return ret;

            return null;
        }
        public static bool IsMappingType(Type type)
        {
            Type underlyingType = type.GetUnderlyingType();
            if (underlyingType.IsEnum)
                return true;

            return _typeMap.ContainsKey(underlyingType);
        }
    }
}
