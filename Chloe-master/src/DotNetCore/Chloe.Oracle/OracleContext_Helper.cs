using Chloe.Core;
using Chloe.Core.Visitors;
using Chloe.DbExpressions;
using Chloe.Descriptors;
using Chloe.Entity;
using Chloe.Exceptions;
using Chloe.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Chloe.Oracle
{
    public partial class OracleContext : DbContext
    {
        static MappingMemberDescriptor GetDefineSequenceMemberDescriptor(TypeDescriptor typeDescriptor, out string sequenceName)
        {
            sequenceName = null;
            MappingMemberDescriptor defineSequenceMemberDescriptor = null;

            foreach (MappingMemberDescriptor memberDescriptor in typeDescriptor.MappingMemberDescriptors.Values)
            {
                string sequenceNameT;
                if (!HasSequenceAttribute(memberDescriptor, out sequenceNameT))
                {
                    continue;
                }

                if (defineSequenceMemberDescriptor != null)
                    throw new ChloeException(string.Format("Mapping type '{0}' can not define multiple identity members.", typeDescriptor.EntityType.FullName));

                sequenceName = sequenceNameT;
                defineSequenceMemberDescriptor = memberDescriptor;
            }

            if (defineSequenceMemberDescriptor != null)
                EnsureDefineSequenceMemberType(defineSequenceMemberDescriptor);

            return defineSequenceMemberDescriptor;
        }
        static bool HasSequenceAttribute(MappingMemberDescriptor memberDescriptor, out string sequenceName)
        {
            sequenceName = null;

            SequenceAttribute attr = (SequenceAttribute)memberDescriptor.GetCustomAttribute(typeof(SequenceAttribute));
            if (attr != null)
            {
                if (string.IsNullOrEmpty(attr.Name))
                    throw new ChloeException("Sequence name can not be empty.");

                sequenceName = attr.Name;
                return true;
            }

            return false;
        }
        static void EnsureDefineSequenceMemberType(MappingMemberDescriptor defineSequenceMemberDescriptor)
        {
            if (defineSequenceMemberDescriptor.MemberInfoType != UtilConstants.TypeOfInt16 && defineSequenceMemberDescriptor.MemberInfoType != UtilConstants.TypeOfInt32 && defineSequenceMemberDescriptor.MemberInfoType != UtilConstants.TypeOfInt64)
            {
                throw new ChloeException("Identity type must be Int16,Int32 or Int64.");
            }
        }
        static void EnsureMappingTypeHasPrimaryKey(TypeDescriptor typeDescriptor)
        {
            if (!typeDescriptor.HasPrimaryKey())
                throw new ChloeException(string.Format("Mapping type '{0}' does not define a primary key.", typeDescriptor.EntityType.FullName));
        }
        static object ConvertIdentityType(object identity, Type conversionType)
        {
            if (identity.GetType() != conversionType)
                return Convert.ChangeType(identity, conversionType);

            return identity;
        }
        static Dictionary<MappingMemberDescriptor, object> CreateKeyValueMap(TypeDescriptor typeDescriptor)
        {
            Dictionary<MappingMemberDescriptor, object> keyValueMap = new Dictionary<MappingMemberDescriptor, object>();
            foreach (MappingMemberDescriptor keyMemberDescriptor in typeDescriptor.PrimaryKeys)
            {
                keyValueMap.Add(keyMemberDescriptor, null);
            }

            return keyValueMap;
        }
        static DbExpression MakeCondition(Dictionary<MappingMemberDescriptor, object> keyValueMap, DbTable dbTable)
        {
            DbExpression conditionExp = null;
            foreach (var kv in keyValueMap)
            {
                MappingMemberDescriptor keyMemberDescriptor = kv.Key;
                object keyVal = kv.Value;

                if (keyVal == null)
                    throw new ArgumentException(string.Format("The primary key '{0}' could not be null.", keyMemberDescriptor.MemberInfo.Name));

                DbExpression left = new DbColumnAccessExpression(dbTable, keyMemberDescriptor.Column);
                DbExpression right = DbExpression.Parameter(keyVal, keyMemberDescriptor.MemberInfoType);
                DbExpression equalExp = new DbEqualExpression(left, right);
                conditionExp = conditionExp == null ? equalExp : DbExpression.And(conditionExp, equalExp);
            }

            return conditionExp;
        }

    }
}
