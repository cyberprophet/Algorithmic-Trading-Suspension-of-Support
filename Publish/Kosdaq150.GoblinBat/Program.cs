using System;
using System.Windows.Forms;

namespace ShareInvest.Kosdaq150
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Kosdaq150());
        }
    }
}