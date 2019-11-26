using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest.OpenAPI
{
    public class Setup
    {
        public void StartProgress()
        {
            try
            {
                Process.Start("OpenAPISetup.exe");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}