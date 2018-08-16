using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


public class MPictureBox : PictureBox
{
    public event Action<object> down;
    public event Action<object> up;
    private const int WM_LBUTTONDOWN = 0x201;
    private const int WM_LBUTTONUP = 0x202;
    protected override void WndProc(ref Message m)
    {
        switch (m.Msg)
        {
            case WM_LBUTTONDOWN:
                {
                    if (down != null)
                        down(this); ;
                    break;
                }
            case WM_LBUTTONUP:
                {
                    if (up != null)
                        up(this);
                    break;
                }
        }
        base.WndProc(ref m);
    }
}

