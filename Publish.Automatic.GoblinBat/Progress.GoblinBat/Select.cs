using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest.Progress
{
    public class Select : Form
    {
        public void StartProcess(string process)
        {
            Process.Start(string.Concat(Application.StartupPath, process));
            Close();
            Dispose();
            Application.ExitThread();
        }
    }
}