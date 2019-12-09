using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest.NetFramework
{
    public class Net
    {
        public void StartProgress()
        {
            try
            {
                Process.Start("HardwareMonitor.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}