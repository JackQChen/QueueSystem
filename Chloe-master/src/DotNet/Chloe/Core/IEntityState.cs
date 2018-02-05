using Chloe.Descriptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Chloe.Core
{
    public interface IEntityState
    {
        object Entity { get; }
        TypeDescriptor TypeDescriptor { get; }
        bool HasChanged(MappingMemberDescriptor memberDescriptor, object val);
        void Refresh();
    }

    class EntityState : IEntityState
    {
        Dictionary<MemberInfo, object> _fakes;
        object _entity;
        TypeDescriptor _typeDescriptor;

        public EntityState(TypeDescriptor typeDescriptor, object entity)
        {
            this._typeDescriptor = typeDescriptor;
            this._entity = entity;
            this.Refresh();
        }

        public object Entity { get { return this._entity; } }
        public TypeDescriptor TypeDescriptor { get { return this._typeDescriptor; } }

        public bool HasChanged(MappingMemberDescriptor memberDescriptor, object val)
        {
            object oldVal;
            if (!this._fakes.TryGetValue(memberDescriptor.MemberInfo, out oldVal))
            {
                return true;
            }

            if (memberDescriptor.MemberInfoType == UtilConstants.TypeOfByteArray)
            {
                //byte[] is a big big hole~
                return !AreEqual((byte[])oldVal, (byte[])val);
            }

            return !Utils.AreEqual(oldVal, val);
        }
        public void Refresh()
        {
            Dictionary<MemberInfo, MappingMemberDescriptor> mappingMemberDescriptors = this.TypeDescriptor.MappingMemberDescriptors;

            if (this._fakes == null)
            {
                this._fakes = new Dictionary<MemberInfo, object>(mappingMemberDescriptors.Count);
            }
            else
            {
                this._fakes.Clear();
            }

            object entity = this._entity;
            foreach (var kv in mappingMemberDescriptors)
            {
                MemberInfo key = kv.Key;
                MappingMemberDescriptor memberDescriptor = kv.Value;

                var val = memberDescriptor.GetValue(entity);

                //I hate the byte[].
                if (memberDescriptor.MemberInfoType == UtilConstants.TypeOfByteArray)
                {
                    val = Clone((byte[])val);
                }

                this._fakes[key] = val;
            }
        }

        static byte[] Clone(byte[] arr)
        {
            if (arr == null)
                return null;

            byte[] ret = new byte[arr.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                ret[i] = arr[i];
            }

            return ret;
        }
        static bool AreEqual(byte[] obj1, byte[] obj2)
        {
            if (obj1 == obj2)
                return true;

            if (obj1 != null && obj2 != null)
            {
                if (obj1.Length != obj2.Length)
                    return false;

                for (int i = 0; i < obj1.Length; i++)
                {
                    if (obj1[i] != obj2[i])
                        return false;
                }

                return true;
            }

            return false;
        }
    }
}
