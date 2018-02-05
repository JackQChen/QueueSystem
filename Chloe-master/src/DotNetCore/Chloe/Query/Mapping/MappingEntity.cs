using Chloe.Mapper;
using Chloe.Descriptors;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Chloe.Query.Mapping
{
    public class MappingEntity : IObjectActivatorCreator
    {
        public MappingEntity(EntityConstructorDescriptor constructorDescriptor)
        {
            this.ConstructorDescriptor = constructorDescriptor;
            this.ConstructorParameters = new Dictionary<ParameterInfo, int>();
            this.ConstructorEntityParameters = new Dictionary<ParameterInfo, IObjectActivatorCreator>();
            this.MappingMembers = new Dictionary<MemberInfo, int>();
            this.ComplexMembers = new Dictionary<MemberInfo, IObjectActivatorCreator>();
        }
        public int? CheckNullOrdinal { get; set; }
        public EntityConstructorDescriptor ConstructorDescriptor { get; private set; }
        public Dictionary<ParameterInfo, int> ConstructorParameters { get; private set; }
        public Dictionary<ParameterInfo, IObjectActivatorCreator> ConstructorEntityParameters { get; private set; }

        /// <summary>
        /// 映射成员集合。以 MemberInfo 为 key，读取 DataReader 时的 Ordinal 为 value
        /// </summary>
        public Dictionary<MemberInfo, int> MappingMembers { get; private set; }
        /// <summary>
        /// 复杂类型成员集合。
        /// </summary>
        public Dictionary<MemberInfo, IObjectActivatorCreator> ComplexMembers { get; private set; }

        public IObjectActivator CreateObjectActivator()
        {
            return this.CreateObjectActivator(null);
        }
        public IObjectActivator CreateObjectActivator(IDbContext dbContext)
        {
            EntityMemberMapper mapper = this.ConstructorDescriptor.GetEntityMemberMapper();
            List<IValueSetter> memberSetters = new List<IValueSetter>(this.MappingMembers.Count + this.ComplexMembers.Count);
            foreach (var kv in this.MappingMembers)
            {
                IMRM mMapper = mapper.TryGetMappingMemberMapper(kv.Key);
                MappingMemberBinder binder = new MappingMemberBinder(mMapper, kv.Value);
                memberSetters.Add(binder);
            }

            foreach (var kv in this.ComplexMembers)
            {
                Action<object, object> del = mapper.TryGetComplexMemberSetter(kv.Key);
                IObjectActivator memberActivtor = kv.Value.CreateObjectActivator();
                ComplexMemberBinder binder = new ComplexMemberBinder(del, memberActivtor);
                memberSetters.Add(binder);
            }

            Func<IDataReader, ReaderOrdinalEnumerator, ObjectActivatorEnumerator, object> instanceCreator = this.ConstructorDescriptor.GetInstanceCreator();

            List<int> readerOrdinals = this.ConstructorParameters.Select(a => a.Value).ToList();
            List<IObjectActivator> objectActivators = this.ConstructorEntityParameters.Select(a => a.Value.CreateObjectActivator()).ToList();

            ObjectActivator ret;
            if (dbContext != null)
                ret = new ObjectActivatorWithTracking(instanceCreator, readerOrdinals, objectActivators, memberSetters, this.CheckNullOrdinal, dbContext);
            else
                ret = new ObjectActivator(instanceCreator, readerOrdinals, objectActivators, memberSetters, this.CheckNullOrdinal);

            return ret;
        }
    }
}
