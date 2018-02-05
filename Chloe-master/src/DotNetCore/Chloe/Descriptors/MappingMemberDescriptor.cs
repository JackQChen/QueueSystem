using Chloe.Core;
using Chloe.DbExpressions;
using Chloe.Entity;
using System;
using System.Data;
using System.Reflection;
using System.Linq;
using System.Threading;
using Chloe.Core.Emit;
using Chloe.InternalExtensions;

namespace Chloe.Descriptors
{
    public class MappingMemberDescriptor : MemberDescriptor
    {
        Func<object, object> _valueGetter;
        Action<object, object> _valueSetter;
        public MappingMemberDescriptor(MemberInfo memberInfo, TypeDescriptor declaringTypeDescriptor)
            : base(memberInfo, declaringTypeDescriptor)
        {
            this.Initialize();
        }
        void Initialize()
        {
            ColumnAttribute columnFlag = (ColumnAttribute)this.MemberInfo.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
            if (columnFlag == null)
                columnFlag = new ColumnAttribute(this.MemberInfo.Name);
            else if (columnFlag.Name == null)
                columnFlag.Name = this.MemberInfo.Name;

            this.Column = new DbColumn(columnFlag.Name, this.MemberInfoType, columnFlag.GetDbType(), columnFlag.GetLength());
            this.IsPrimaryKey = columnFlag.IsPrimaryKey;
        }

        public bool IsPrimaryKey { get; set; }
        public bool IsAutoIncrement { get; set; }
        public DbColumn Column { get; private set; }

        public object GetValue(object instance)
        {
            if (null == this._valueGetter)
            {
                if (Monitor.TryEnter(this))
                {
                    try
                    {
                        if (null == this._valueGetter)
                            this._valueGetter = DelegateGenerator.CreateValueGetter(this.MemberInfo);
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                else
                {
                    return this.MemberInfo.GetMemberValue(instance);
                }
            }

            return this._valueGetter(instance);
        }
        public void SetValue(object instance, object value)
        {
            if (null == this._valueSetter)
            {
                if (Monitor.TryEnter(this))
                {
                    try
                    {
                        if (null == this._valueSetter)
                            this._valueSetter = DelegateGenerator.CreateValueSetter(this.MemberInfo);
                    }
                    finally
                    {
                        Monitor.Exit(this);
                    }
                }
                else
                {
                    this.MemberInfo.SetMemberValue(instance, value);
                    return;
                }
            }

            this._valueSetter(instance, value);
        }
    }
}
