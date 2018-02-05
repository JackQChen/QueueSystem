using Chloe.Core;
using Chloe.Core.Emit;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Chloe.Mapper
{
    public interface IMRM
    {
        void Map(object instance, IDataReader reader, int ordinal);
    }

    static class MRMHelper
    {
        public static IMRM CreateMRM(MemberInfo member)
        {
            Type type = ClassGenerator.CreateMRMType(member);
            IMRM obj = (IMRM)type.GetConstructor(Type.EmptyTypes).Invoke(null);
            return obj;
        }
    }

    class MRM : IMRM
    {
        Action<object, IDataReader, int> _mapper;
        public MRM(Action<object, IDataReader, int> mapper)
        {
            this._mapper = mapper;
        }

        public void Map(object instance, IDataReader reader, int ordinal)
        {
            this._mapper(instance, reader, ordinal);
        }
    }
}
