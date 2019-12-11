using System;
using System.Runtime.InteropServices;

namespace ShareInvest.VolumeControl
{
    public class Volume
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
    }
}