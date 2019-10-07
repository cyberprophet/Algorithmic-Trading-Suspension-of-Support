using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ShareInvest.AutoMessageBox
{
    public class Box
    {
        public static void Show(string text, string caption, int timeout)
        {
            new Box(text, caption, timeout);
        }
        private void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(null, _caption);

            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            _timeoutTimer.Dispose();
        }
        private Box(string text, string caption, int timeout)
        {
            _caption = caption;
            _timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, timeout, Timeout.Infinite);

            MessageBox.Show(text, caption);
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        private readonly System.Threading.Timer _timeoutTimer;
        private readonly string _caption;
        private const int WM_CLOSE = 0x0010;
    }
}