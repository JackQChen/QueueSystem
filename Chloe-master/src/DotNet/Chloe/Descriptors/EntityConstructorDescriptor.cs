using Chloe.InternalExtensions;
using Chloe.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Chloe.Descriptors
{
    public class EntityConstructorDescriptor
    {
        EntityMemberMapper _mapper = null;
        EntityConstructor _entityConstructor = null;
        EntityConstructorDescriptor(ConstructorInfo constructorInfo)
        {
            this.ConstructorInfo = constructorInfo;
            this.Init();
        }

        void Init()
        {
            ConstructorInfo constructor = this.ConstructorInfo;
            Type type = constructor.DeclaringType;

            if (ReflectionExtension.IsAnonymousType(type))
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                this.MemberParameterMap = new Dictionary<MemberInfo, ParameterInfo>(parameters.Length);
                foreach (ParameterInfo parameter in parameters)
                {
                    PropertyInfo prop = type.GetProperty(parameter.Name);
                    this.MemberParameterMap.Add(prop, parameter);
                }
            }
            else
                this.MemberParameterMap = new Dictionary<MemberInfo, ParameterInfo>(0);
        }

        public ConstructorInfo ConstructorInfo { get; private set; }
        public Dictionary<MemberInfo, ParameterInfo> MemberParameterMap { get; private set; }
        public Func<IDataReader, ReaderOrdinalEnumerator, ObjectActivatorEnumerator, object> GetInstanceCreator()
        {
            EntityConstructor entityConstructor = null;
            if (null == this._entityConstructor)
            {
                this._entityConstructor = EntityConstructor.GetInstance(this.ConstructorInfo);
            }

            entityConstructor = this._entityConstructor;
            return entityConstructor.InstanceCreator;
        }
        public EntityMemberMapper GetEntityMemberMapper()
        {
            EntityMemberMapper mapper = null;
            if (null == this._mapper)
            {
                this._mapper = EntityMemberMapper.GetInstance(this.ConstructorInfo.DeclaringType);
            }

            mapper = this._mapper;
            return mapper;
        }

        static readonly System.Collections.Concurrent.ConcurrentDictionary<ConstructorInfo, EntityConstructorDescriptor> InstanceCache = new System.Collections.Concurrent.ConcurrentDictionary<ConstructorInfo, EntityConstructorDescriptor>();

        public static EntityConstructorDescriptor GetInstance(ConstructorInfo constructorInfo)
        {
            EntityConstructorDescriptor instance;
            if (!InstanceCache.TryGetValue(constructorInfo, out instance))
            {
                lock (constructorInfo)
                {
                    if (!InstanceCache.TryGetValue(constructorInfo, out instance))
                    {
                        instance = new EntityConstructorDescriptor(constructorInfo);
                        InstanceCache.GetOrAdd(constructorInfo, instance);
                    }
                }
            }

            return instance;
        }
    }

}
