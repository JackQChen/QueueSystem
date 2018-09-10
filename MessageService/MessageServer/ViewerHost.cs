using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MessageServer
{
    public class ViewerHost : Panel
    {
        public event Action OnEmbed;

        public ViewerHost()
        {
        }

        public void Start()
        {
            Start(string.Empty);
        }

        public void Start(string args)
        {
            if (m_AppProcess != null)
            {
                Stop();
            }
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(this.m_AppFileName);
                info.UseShellExecute = true;
                info.WindowStyle = ProcessWindowStyle.Minimized;
                info.Arguments = args;
                m_AppProcess = Process.Start(info);
                Application.Idle += Application_Idle;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Stop();
            }
        }

        void Application_Idle(object sender, EventArgs e)
        {
            if (this.m_AppProcess == null || this.m_AppProcess.HasExited)
            {
                this.m_AppProcess = null;
                Application.Idle -= Application_Idle;
                return;
            }
            if (m_AppProcess.MainWindowHandle == IntPtr.Zero)
                return;
            Application.Idle -= Application_Idle;
            EmbedProcess(m_AppProcess, this);
            if (this.OnEmbed != null)
                this.OnEmbed();
        }

        public void Stop()
        {
            if (m_AppProcess != null)
            {
                try
                {
                    if (!m_AppProcess.HasExited)
                        m_AppProcess.Kill();
                }
                catch (Exception)
                {
                }
                m_AppProcess = null;
            }
        }

        public void SendMessage(string strContent)
        {
            this.SendMessage(0, strContent);
        }

        public void SendMessage(uint param, string strContent)
        {
            if (this.m_AppProcess != null)
                SendMessage(this.m_AppProcess.MainWindowHandle, WM_SETTEXT, param, strContent);
        }

        public void PostMessage(string strContent)
        {
            this.PostMessage(0, strContent);
        }

        public void PostMessage(uint param, string strContent)
        {
            if (this.m_AppProcess != null)
                PostMessage(this.m_AppProcess.MainWindowHandle, WM_SETTEXT, param, strContent);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            Stop();
            base.OnHandleDestroyed(e);
        }

        protected override void OnResize(EventArgs eventargs)
        {
            if (m_AppProcess != null)
            {
                MoveWindow(m_AppProcess.MainWindowHandle, 0, 0, this.Width, this.Height, true);
            }
            base.OnResize(eventargs);
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            this.Invalidate();
            base.OnSizeChanged(e);
        }

        #region 属性

        Process m_AppProcess = null;
        public Process AppProcess
        {
            get { return this.m_AppProcess; }
            set { this.m_AppProcess = value; }
        }

        private string m_AppFileName = "";
        public string AppFileName
        {
            get { return m_AppFileName; }
            set { m_AppFileName = value; }
        }

        public bool IsStarted { get { return (this.m_AppProcess != null); } }

        #endregion 属性

        #region Win32 API
        [DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", SetLastError = true,
             CharSet = CharSet.Unicode, ExactSpelling = true,
             CallingConvention = CallingConvention.StdCall)]
        private static extern long GetWindowThreadProcessId(long hWnd, long lpdwProcessId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern long SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongA", SetLastError = true)]
        private static extern long GetWindowLong(IntPtr hwnd, int nIndex);

        public static IntPtr SetWindowLong(HandleRef hWnd, int nIndex, int dwNewLong)
        {
            if (IntPtr.Size == 4)
            {
                return SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
            }
            return SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
        }

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
        public static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern long SetWindowPos(IntPtr hwnd, long hWndInsertAfter, long x, long y, long cx, long cy, long wFlags);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool MoveWindow(IntPtr hwnd, int x, int y, int cx, int cy, bool repaint);

        [DllImport("user32.dll", EntryPoint = "PostMessageA", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hwnd, uint Msg, uint wParam, string lParam);

        [DllImport("user32.dll", EntryPoint = "SendMessageA", SetLastError = true)]
        private static extern bool SendMessage(IntPtr hwnd, uint Msg, uint wParam, string lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr GetParent(IntPtr hwnd);

        [DllImport("user32.dll", EntryPoint = "ShowWindow", SetLastError = true)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SWP_NOOWNERZORDER = 0x200;
        private const int SWP_NOREDRAW = 0x8;
        private const int SWP_NOZORDER = 0x4;
        private const int SWP_SHOWWINDOW = 0x0040;
        private const int WS_EX_MDICHILD = 0x40;
        private const int SWP_FRAMECHANGED = 0x20;
        private const int SWP_NOACTIVATE = 0x10;
        private const int SWP_ASYNCWINDOWPOS = 0x4000;
        private const int SWP_NOMOVE = 0x2;
        private const int SWP_NOSIZE = 0x1;
        private const int GWL_STYLE = (-16);
        private const int WS_VISIBLE = 0x10000000;
        private const int WM_CLOSE = 0x10;
        private const int WM_SETTEXT = 0x0C;
        private const int WS_CHILD = 0x40000000;

        private const int SW_HIDE = 0; //{隐藏, 并且任务栏也没有最小化图标}
        private const int SW_SHOWNORMAL = 1; //{用最近的大小和位置显示, 激活}
        private const int SW_NORMAL = 1; //{同 SW_SHOWNORMAL}
        private const int SW_SHOWMINIMIZED = 2; //{最小化, 激活}
        private const int SW_SHOWMAXIMIZED = 3; //{最大化, 激活}
        private const int SW_MAXIMIZE = 3; //{同 SW_SHOWMAXIMIZED}
        private const int SW_SHOWNOACTIVATE = 4; //{用最近的大小和位置显示, 不激活}
        private const int SW_SHOW = 5; //{同 SW_SHOWNORMAL}
        private const int SW_MINIMIZE = 6; //{最小化, 不激活}
        private const int SW_SHOWMINNOACTIVE = 7; //{同 SW_MINIMIZE}
        private const int SW_SHOWNA = 8; //{同 SW_SHOWNOACTIVATE}
        private const int SW_RESTORE = 9; //{同 SW_SHOWNORMAL}
        private const int SW_SHOWDEFAULT = 10; //{同 SW_SHOWNORMAL}
        private const int SW_MAX = 10; //{同 SW_SHOWNORMAL}

        //const int PROCESS_ALL_ACCESS = 0x1F0FFF;
        //const int PROCESS_VM_READ = 0x0010;
        //const int PROCESS_VM_WRITE = 0x0020;     
        #endregion Win32 API

        private void EmbedProcess(Process app, Control control)
        {
            if (app == null || app.MainWindowHandle == IntPtr.Zero || control == null)
                return;
            try
            {
                SetParent(app.MainWindowHandle, control.Handle);
                SetWindowLong(new HandleRef(this, app.MainWindowHandle), GWL_STYLE, WS_VISIBLE);
                MoveWindow(app.MainWindowHandle, 0, 0, control.Width, control.Height, true);
            }
            catch
            {
            }
        }
    }
}
