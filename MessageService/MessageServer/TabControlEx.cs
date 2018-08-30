using System.Windows.Forms;
namespace MessageServer
{
    public class TabControlEx : TabControl
    {

        public TabControlEx()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
                       | ControlStyles.OptimizedDoubleBuffer
                       , true);
        }

        protected override bool ShowFocusCues
        {
            get { return false; }
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}