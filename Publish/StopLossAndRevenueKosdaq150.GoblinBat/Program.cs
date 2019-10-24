using System;
using System.Windows.Forms;

namespace ShareInvest.Kosdaq150.StopLossAndRevenue
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