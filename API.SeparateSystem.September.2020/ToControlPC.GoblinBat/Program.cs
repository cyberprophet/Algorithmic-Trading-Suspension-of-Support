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
            secrecy.PublishTheDebuggedProgram();

            if (secrecy.IsProcessing == false && secrecy.CheckAndUpdateTheProgramVersion() == false)
                StartProgess(secrecy.Key);

            if (secrecy.IsProcessing == false && secrecy.IsDebugging == false)
                Process.Start("shutdown.exe", "-r");

            Process.GetCurrentProcess().Kill();
        }
        static void StartProgess(dynamic param)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Strategics.GoblinBat(param));
        }
        static readonly Secrecy secrecy = new Secrecy(Verify.KeyDecoder.GetKey());
    }
}