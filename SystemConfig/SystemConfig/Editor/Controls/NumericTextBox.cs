using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SystemConfig.Editor.Controls
{
    public class NumericTextBox : TextBox
    {
        private const int ES_NUMBER = 0x2000;

        protected override System.Windows.Forms.CreateParams CreateParams
        {
            get
            {
                System.Windows.Forms.CreateParams cp = base.CreateParams;
                cp.Style |= ES_NUMBER;
                return cp;
            }
        }

        public NumericTextBox()
        {
            this.Text = "0";
        }

        public int Value
        {
            get
            {
                if (this.Text == "新增")
                    return 0;
                return Convert.ToInt32(this.Text);
            }
        }
    }
}
