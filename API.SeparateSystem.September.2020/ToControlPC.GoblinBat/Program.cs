using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            var secrecy = new Secrecy(Verify.KeyDecoder.GetKey());
            secrecy.PublishTheDebuggedProgram();
            secrecy.CheckAndUpdateTheProgramVersion();
            StartProgess(secrecy.GetUserInformation());
            Process.GetCurrentProcess().Kill();
        }
        static void StartProgess(dynamic param)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Strategics.GoblinBat());
        }
    }
}