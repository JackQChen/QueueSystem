using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemConfig.Editor.Controls
{
    public class ValueTextBox : TextBox
    {
        object _value = null;

        public event Action<object> ValueInit;
        public event Action<object> ValueInitDone;
        public event Action<object> ValueChanged;

        public ValueTextBox()
        {
        }

        public void InitValue(object value)
        {
            if (ValueInit != null)
                ValueInit(value);
            this.Value = value;
            if (ValueInitDone != null)
                ValueInitDone(value);
        }

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                this._value = value;
                if (ValueChanged != null)
                    ValueChanged(_value);
            }
        }
    }
}
