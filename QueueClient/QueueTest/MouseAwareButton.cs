using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;


public class MouseAwarePictureBox : PictureBox
{
    private const int WM_LBUTTONDOWN = 0x201;
    private const int WM_LBUTTONUP = 0x202;
    Control prevParent = null;

    public new event MouseEventHandler MouseUp;
    public new event MouseEventHandler MouseDown;

    protected override void OnParentChanged(EventArgs e)
    {
        if (prevParent != null)
        {
            WndProcHooker.UnhookWndProc(prevParent, WM_LBUTTONDOWN);
            WndProcHooker.UnhookWndProc(prevParent, WM_LBUTTONUP);
        }
        prevParent = Parent;

        if (Parent != null)
        {
            WndProcHooker.HookWndProc(Parent, WM_LBUTTONDOWN_Handler, WM_LBUTTONDOWN);
            WndProcHooker.HookWndProc(Parent, WM_LBUTTONUP_Handler, WM_LBUTTONUP);
        }
        base.OnParentChanged(e);
    }

    int WM_LBUTTONDOWN_Handler(IntPtr hwnd, uint msg, uint wParam, int lParam, ref bool handled)
    {
        int x = lParam & 0xffff, y = lParam >> 16;
        MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 0, x, y, 0);
        if (MouseDown != null)
            MouseDown(this, args);
        return 0;
    }

    int WM_LBUTTONUP_Handler(IntPtr hwnd, uint msg, uint wParam, int lParam, ref bool handled)
    {
        int x = lParam & 0xffff, y = lParam >> 16;
        MouseEventArgs args = new MouseEventArgs(MouseButtons.Left, 0, x, y, 0);
        if (MouseUp != null)
            MouseUp(this, args);
        return 0;
    }
}

