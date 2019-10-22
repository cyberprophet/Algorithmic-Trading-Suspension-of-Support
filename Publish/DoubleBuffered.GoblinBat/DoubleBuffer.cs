using System.Reflection;
using System.Windows.Forms;

namespace ShareInvest.DoubleBuffered
{
    public static class DoubleBuffer
    {
        public static void SetBuffered(Control control)
        {
            if (SystemInformation.TerminalServerSession)
                return;

            typeof(Control).GetProperty("DoubleBuffered", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(control, true, null);
        }
    }
}