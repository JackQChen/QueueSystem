using Chloe.Core;
using Chloe.Core.Emit;
using Chloe.Query.Mapping;
using Chloe.InternalExtensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using Chloe.Infrastructure;

namespace Chloe.Mapper
{
    public class EntityMemberMapper
    {
        Dictionary<MemberInfo, IMRM> _mappingMemberMappers;
        Dictionary<MemberInfo, Action<object, object>> _complexMemberSetters;

        EntityMemberMapper(Type t)
        {
            this.Type = t;
            this.Init();
        }

        void Init()
        {
            Type t = this.Type;
            var members = t.GetMembers(BindingFlags.Public | BindingFlags.Instance);

            Dictionary<MemberInfo, IMRM> mappingMemberMappers = new Dictionary<MemberInfo, IMRM>();
            Dictionary<MemberInfo, Action<object, object>> complexMemberSetters = new Dictionary<MemberInfo, Action<object, object>>();

            foreach (var member in members)
            {
                Type memberType = null;
                PropertyInfo prop = null;
                FieldInfo field = null;

                if ((prop = member as PropertyInfo) != null)
                {
                    if (prop.GetSetMethod() == null)
                        continue;//对于没有公共的 setter 直接跳过
                    memberType = prop.PropertyType;
                }
                else if ((field = member as FieldInfo) != null)
                {
                    memberType = field.FieldType;
                }
                else
                    continue;//只支持公共属性和字段

                if (MappingTypeSystem.IsMappingType(memberType))
                {
                    IMRM mrm = MRMHelper.CreateMRM(member);
                    mappingMemberMappers.Add(member, mrm);
                }
                else
                {
                    if (prop != null)
                    {
                        Action<object, object> valueSetter = DelegateGenerator.CreateValueSetter(prop);
                        complexMemberSetters.Add(member, valueSetter);
                    }
                    else if (field != null)
                    {
                        Action<object, object> valueSetter = DelegateGenerator.CreateValueSetter(field);
                        complexMemberSetters.Add(member, valueSetter);
                    }
                    else
                        continue;

                    continue;
                }
            }

            this._mappingMemberMappers = Utils.Clone(mappingMemberMappers);
            this._complexMemberSetters = Utils.Clone(complexMemberSetters);
        }

        public Type Type { get; private set; }

        public IMRM TryGetMappingMemberMapper(MemberInfo memberInfo)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.Type);
            IMRM mapper = null;
            this._mappingMemberMappers.TryGetValue(memberInfo, out mapper);
            return mapper;
        }
        public Action<object, object> TryGetComplexMemberSetter(MemberInfo memberInfo)
        {
            memberInfo = memberInfo.AsReflectedMemberOf(this.Type);
            Action<object, object> valueSetter = null;
            this._complexMemberSetters.TryGetValue(memberInfo, out valueSetter);
            return valueSetter;
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, EntityMemberMapper> InstanceCache = new System.Collections.Concurrent.ConcurrentDictionary<Type, EntityMemberMapper>();

        public static EntityMemberMapper GetInstance(Type type)
        {
            EntityMemberMapper instance;
            if (!InstanceCache.TryGetValue(type, out instance))
            {
                lock (type)
                {
                    if (!InstanceCache.TryGetValue(type, out instance))
                    {
                        instance = new EntityMemberMapper(type);
                        InstanceCache.GetOrAdd(type, instance);
                    }
                }
            }

            return instance;
        }
    }
}
