using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest.C2012
{
    public class MSVC2012
    {
        public void StartProgress()
        {
            try
            {
                Process.Start("2012.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}