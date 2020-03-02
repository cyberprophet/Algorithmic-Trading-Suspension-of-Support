using System;
using System.Windows.Forms;
using XA_SESSIONLib;

namespace ShareInvest.GoblinBat
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new StartUp(new XASessionClass()));
        }
    }
}