using Chloe.Core;
using Chloe.Mapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.Query.Mapping
{
    public class MappingField : IObjectActivatorCreator
    {
        Type _type;
        public MappingField(Type type, int readerOrdinal)
        {
            this._type = type;
            this.ReaderOrdinal = readerOrdinal;
        }
        public int ReaderOrdinal { get; private set; }
        public int? CheckNullOrdinal { get; set; }

        public IObjectActivator CreateObjectActivator()
        {
            return this.CreateObjectActivator(null);
        }
        public IObjectActivator CreateObjectActivator(IDbContext dbContext)
        {
            Func<IDataReader, int, object> fn = MappingTypeConstructor.GetInstance(this._type).InstanceCreator;
            MappingFieldActivator act = new MappingFieldActivator(fn, this.ReaderOrdinal);
            return act;
        }
    }
}
