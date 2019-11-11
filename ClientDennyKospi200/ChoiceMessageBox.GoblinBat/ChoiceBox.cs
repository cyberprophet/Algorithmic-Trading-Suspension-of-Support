using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace ShareInvest.ChoiceMessageBox
{
    public class ChoiceBox
    {
        public static DialogResult Show(string text, string caption, int timeout, MessageBoxIcon icon, MessageBoxButtons buttons = 0, DialogResult timerResult = DialogResult.None)
        {
            return new ChoiceBox(text, caption, timeout, icon, buttons, timerResult).result;
        }
        private void OnTimerElapsed(object state)
        {
            IntPtr mbWnd = FindWindow(messageBox, caption);

            if (mbWnd != IntPtr.Zero)
                SendMessage(mbWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);

            timeoutTimer.Dispose();
            result = timerResult;
        }
        private ChoiceBox(string text, string caption, int timeout, MessageBoxIcon icon, MessageBoxButtons buttons = 0, DialogResult timerResult = DialogResult.None)
        {
            timeoutTimer = new System.Threading.Timer(OnTimerElapsed, null, timeout, Timeout.Infinite);
            this.caption = caption;
            this.timerResult = timerResult;
            using (timeoutTimer)
                result = MessageBox.Show(text, caption, buttons, icon);
        }
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private readonly string caption;
        private readonly System.Threading.Timer timeoutTimer;
        private readonly DialogResult timerResult;
        private const int WM_CLOSE = 0x0010;
        private const string messageBox = "#32770";
        private DialogResult result;
    }
}