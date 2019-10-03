using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareInvest
{
    public class Choose
    {
        public static DialogResult Show(string text, string caption, string yes, string no, string cancel)
        {
            Choose.yes = yes;
            Choose.cancel = cancel;
            Choose.no = no;

            g_hHook = SetWindowsHookEx(5, new HookProc(HookWndProc), IntPtr.Zero, GetCurrentThreadId());

            return MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int hook, HookProc callback, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDlgItem(IntPtr hDlg, DialogResult nIDDlgItem);

        [DllImport("user32.dll")]
        private static extern bool SetDlgItemText(IntPtr hDlg, DialogResult nIDDlgItem, string lpString);

        [DllImport("kernel32.dll")]
        private static extern uint GetCurrentThreadId();

        private static int HookWndProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode == 5)
            {
                if (GetDlgItem(wParam, DialogResult.Yes) != null)
                    SetDlgItemText(wParam, DialogResult.Yes, yes);

                if (GetDlgItem(wParam, DialogResult.No) != null)
                    SetDlgItemText(wParam, DialogResult.No, no);

                if (GetDlgItem(wParam, DialogResult.Cancel) != null)
                    SetDlgItemText(wParam, DialogResult.Cancel, cancel);

                UnhookWindowsHookEx(g_hHook);
            }
            else
                CallNextHookEx(g_hHook, nCode, wParam, lParam);

            return 0;
        }
        private static IntPtr g_hHook;
        private static string yes;
        private static string cancel;
        private static string no;
    }
}