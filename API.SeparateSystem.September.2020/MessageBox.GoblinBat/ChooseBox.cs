using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace ShareInvest.Message
{
    public class ChooseBox
    {
        delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        public static DialogResult Show(string text, string caption, string yes, string no, string cancel)
        {
            ChooseBox.yes = yes;
            ChooseBox.cancel = cancel;
            ChooseBox.no = no;

            g_hHook = SetWindowsHookEx(5, new HookProc(HookWndProc), IntPtr.Zero, GetCurrentThreadId());

            return MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
        public static DialogResult Show(string text, string caption, string yes, string no)
        {
            ChooseBox.yes = yes;
            ChooseBox.no = no;

            g_hHook = SetWindowsHookEx(5, new HookProc(HookWndProc), IntPtr.Zero, GetCurrentThreadId());

            if (admin.Equals(caption))
                return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            else
                return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question, caption.Equals(welcome) ? MessageBoxDefaultButton.Button2 : MessageBoxDefaultButton.Button1);
        }
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int hook, HookProc callback, IntPtr hMod, uint dwThreadId);
        [DllImport("user32.dll")]
        static extern bool UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll")]
        static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern IntPtr GetDlgItem(IntPtr hDlg, DialogResult nIDDlgItem);
        [DllImport("user32.dll")]
        static extern bool SetDlgItemText(IntPtr hDlg, DialogResult nIDDlgItem, string lpString);
        [DllImport("kernel32.dll")]
        static extern uint GetCurrentThreadId();
        static int HookWndProc(int nCode, IntPtr wParam, IntPtr lParam)
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
        static IntPtr g_hHook;
        static string yes;
        static string cancel;
        static string no;
        const string admin = "Algorithmic Trading";
        const string welcome = "Welcome to the Algorithmic Trading";
    }
}