using System;
using System.Windows.Forms;

namespace ShareInvest.Kospi200
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new GoblinBat());
        }
    }
}