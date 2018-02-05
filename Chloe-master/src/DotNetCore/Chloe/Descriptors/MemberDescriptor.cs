using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using Chloe.InternalExtensions;

namespace Chloe.Descriptors
{
    public abstract class MemberDescriptor
    {
        Dictionary<Type, Attribute> _customAttributes = new Dictionary<Type, Attribute>();
        MemberInfo _memberInfo;
        TypeDescriptor _declaringTypeDescriptor;
        protected MemberDescriptor(MemberInfo memberInfo, TypeDescriptor declaringTypeDescriptor)
        {
            this._declaringTypeDescriptor = declaringTypeDescriptor;
            this._memberInfo = memberInfo;
        }

        public TypeDescriptor DeclaringTypeDescriptor { get { return this._declaringTypeDescriptor; } }
        public MemberInfo MemberInfo { get { return this._memberInfo; } }
        public Type MemberInfoType
        {
            get
            {
                return this._memberInfo.GetMemberType();
            }
        }

        public virtual Attribute GetCustomAttribute(Type attributeType)
        {
            Attribute val;
            if (!this._customAttributes.TryGetValue(attributeType, out val))
            {
                val = this.MemberInfo.GetCustomAttributes(attributeType, false).FirstOrDefault() as Attribute;
                lock (this._customAttributes)
                {
                    this._customAttributes[attributeType] = val;
                }
            }

            return val;
        }
        public bool IsDefined(Type attributeType)
        {
            return this.MemberInfo.IsDefined(attributeType, false);
        }
    }
}
