using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Entity;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using Chloe.InternalExtensions;
using Chloe.Query.Visitors;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Chloe.Descriptors
{
    public class TypeDescriptor
    {
        Dictionary<MemberInfo, MappingMemberDescriptor> _mappingMemberDescriptors;
        Dictionary<MemberInfo, DbColumnAccessExpression> _memberColumnMap;
        ReadOnlyCollection<MappingMemberDescriptor> _primaryKeys;
        MappingMemberDescriptor _autoIncrement = null;

        DefaultExpressionParser _expressionParser = null;

        TypeDescriptor(Type t)
        {
            this.EntityType = t;
            this.Init();
        }

        void Init()
        {
            this.InitTableInfo();
            this.InitMemberInfo();
            this.InitMemberColumnMap();
        }
        void InitTableInfo()
        {
            Type entityType = this.EntityType;
            TableAttribute tableFlag = entityType.GetCustomAttributes<TableAttribute>(false).FirstOrDefault();

            if (tableFlag == null)
            {
                tableFlag = new TableAttribute(entityType.Name);
            }
            else if (tableFlag.Name == null)
                tableFlag.Name = entityType.Name;

            this.Table = new DbTable(tableFlag.Name, tableFlag.Schema);
        }
        void InitMemberInfo()
        {
            List<MappingMemberDescriptor> mappingMemberDescriptors = this.ExtractMappingMemberDescriptors();

            List<MappingMemberDescriptor> primaryKeys = mappingMemberDescriptors.Where(a => a.IsPrimaryKey).ToList();

            if (primaryKeys.Count == 0)
            {
                //如果没有定义任何主键，则从所有映射的属性中查找名为 id 的属性作为主键
                MappingMemberDescriptor idNameMemberDescriptor = mappingMemberDescriptors.Find(a => a.MemberInfo.Name.ToLower() == "id" && !a.IsDefined(typeof(ColumnAttribute)));

                if (idNameMemberDescriptor != null)
                {
                    idNameMemberDescriptor.IsPrimaryKey = true;
                    primaryKeys.Add(idNameMemberDescriptor);
                }
            }

            this._primaryKeys = primaryKeys.AsReadOnly();

            List<MappingMemberDescriptor> autoIncrementMemberDescriptors = mappingMemberDescriptors.Where(a => a.IsDefined(typeof(AutoIncrementAttribute))).ToList();
            if (autoIncrementMemberDescriptors.Count > 1)
            {
                throw new NotSupportedException(string.Format("The entity type '{0}' can not define multiple auto increment members.", this.EntityType.FullName));
            }
            else if (autoIncrementMemberDescriptors.Count == 1)
            {
                MappingMemberDescriptor autoIncrementMemberDescriptor = autoIncrementMemberDescriptors[0];
                if (autoIncrementMemberDescriptor.IsDefined(typeof(NonAutoIncrementAttribute)))
                {
                    throw new ChloeException(string.Format("Can't define both 'AutoIncrementAttribute' and 'NotAutoIncrementAttribute' on the same mapping member '{0}'.", autoIncrementMemberDescriptor.MemberInfo.Name));
                }

                if (!IsAutoIncrementType(autoIncrementMemberDescriptor.MemberInfoType))
                {
                    throw new ChloeException("Auto increment member type must be Int16, Int32 or Int64.");
                }

                if (autoIncrementMemberDescriptor.IsPrimaryKey && primaryKeys.Count > 1)
                {
                    /* 自增列不能作为联合主键 */
                    throw new ChloeException("Auto increment member can not be union key.");
                }

                autoIncrementMemberDescriptor.IsAutoIncrement = true;
                this._autoIncrement = autoIncrementMemberDescriptor;
            }
            else if (primaryKeys.Count == 1)
            {
                /* 如果没有显示定义自增成员，并且主键只有 1 个，如果该主键满足一定条件，则默认其是自增列 */
                MappingMemberDescriptor primaryKeyDescriptor = primaryKeys[0];
                if (IsAutoIncrementType(primaryKeyDescriptor.MemberInfoType) && !primaryKeyDescriptor.IsDefined(typeof(NonAutoIncrementAttribute)))
                {
                    primaryKeyDescriptor.IsAutoIncrement = true;
                    this._autoIncrement = primaryKeyDescriptor;
                }
            }

            this._mappingMemberDescriptors = new Dictionary<MemberInfo, MappingMemberDescriptor>(mappingMemberDescriptors.Count);
            foreach (MappingMemberDescriptor mappingMemberDescriptor in mappingMemberDescriptors)
            {
                this._mappingMemberDescriptors.Add(mappingMemberDescriptor.MemberInfo, mappingMemberDescriptor);
            }
        }
        void InitMemberColumnMap()
        {
            Dictionary<MemberInfo, DbColumnAccessExpression> memberColumnMap = new Dictionary<MemberInfo, DbColumnAccessExpression>(this._mappingMemberDescriptors.Count);
            foreach (var kv in this._mappingMemberDescriptors)
            {
                memberColumnMap.Add(kv.Key, new DbColumnAccessExpression(this.Table, kv.Value.Column));
            }

            this._memberColumnMap = memberColumnMap;
        }

        List<MappingMemberDescriptor> ExtractMappingMemberDescriptors()
        {
            var members = this.EntityType.GetMembers(BindingFlags.Public | BindingFlags.Instance);

            List<MappingMemberDescriptor> mappingMemberDescriptors = new List<MappingMemberDescriptor>();
            foreach (var member in members)
            {
                if (ShouldMap(member) == false)
                    continue;

                if (MappingTypeSystem.IsMappingType(member.GetMemberType()))
                {
                    MappingMemberDescriptor memberDescriptor = new MappingMemberDescriptor(member, this);
                    mappingMemberDescriptors.Add(memberDescriptor);
                }
            }

            return mappingMemberDescriptors;
        }

        static bool IsAutoIncrementType(Type t)
        {
            return t == UtilConstants.TypeOfInt16 || t == UtilConstants.TypeOfInt32 || t == UtilConstants.TypeOfInt64;
        }
        static bool ShouldMap(MemberInfo member)
        {
            var ignoreFlags = member.GetCustomAttributes(typeof(NotMappedAttribute), false);
            if (ignoreFlags.Count() > 0)
                return false;

            if (member.MemberType == MemberTypes.Property)
            {
                if (((PropertyInfo)member).GetSetMethod() == null)
                    return false;//对于没有公共的 setter 直接跳过
                return true;
            }
            else if (member.MemberType == MemberTypes.Field)
            {
                return true;
            }
            else
                return false;//只支持公共属性和字段
        }

        public Type EntityType { get; private set; }
        public DbTable Table { get; private set; }

        public ReadOnlyCollection<MappingMemberDescriptor> PrimaryKeys { get { return this._primaryKeys; } }
        /* It will return null if an entity has no auto increment member. */
        public MappingMemberDescriptor AutoIncrement { get { return this._autoIncrement; } }

        public DefaultExpressionParser GetExpressionParser(DbTable explicitDbTable)
        {
            if (explicitDbTable == null)
            {
                if (this._expressionParser == null)
                    this._expressionParser = new DefaultExpressionParser(this, null);
                return this._expressionParser;
            }
            else
                return new DefaultExpressionParser(this, explicitDbTable);
        }

        public Dictionary<MemberInfo, MappingMemberDescriptor> MappingMemberDescriptors { get { return this._mappingMemberDescriptors; } }

        public bool HasPrimaryKey()
        {
            return this._primaryKeys.Count > 0;
        }
        public MappingMemberDescriptor TryGetMappingMemberDescriptor(MemberInfo memberInfo)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.EntityType);
            MappingMemberDescriptor memberDescriptor;
            this._mappingMemberDescriptors.TryGetValue(memberInfo, out memberDescriptor);
            return memberDescriptor;
        }
        public DbColumnAccessExpression TryGetColumnAccessExpression(MemberInfo memberInfo)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.EntityType);
            DbColumnAccessExpression dbColumnAccessExpression;
            this._memberColumnMap.TryGetValue(memberInfo, out dbColumnAccessExpression);
            return dbColumnAccessExpression;
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, TypeDescriptor> InstanceCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, TypeDescriptor>();

        public static TypeDescriptor GetDescriptor(Type type)
        {
            TypeDescriptor instance;
            if (!InstanceCache.TryGetValue(type, out instance))
            {
                lock (type)
                {
                    if (!InstanceCache.TryGetValue(type, out instance))
                    {
                        instance = new TypeDescriptor(type);
                        InstanceCache.GetOrAdd(type, instance);
                    }
                }
            }

            return instance;
        }
    }
}
