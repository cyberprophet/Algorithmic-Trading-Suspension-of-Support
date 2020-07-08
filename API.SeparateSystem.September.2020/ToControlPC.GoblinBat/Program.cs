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
            secrecy.CheckAndUpdateTheProgramVersion();
            StartProgess(secrecy.Key);
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