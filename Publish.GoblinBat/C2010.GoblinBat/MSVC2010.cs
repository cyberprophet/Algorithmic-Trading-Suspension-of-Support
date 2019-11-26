using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest.C2010
{
    public class MSVC2010
    {
        public void StartProgress()
        {
            try
            {
                Process.Start("2010.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}