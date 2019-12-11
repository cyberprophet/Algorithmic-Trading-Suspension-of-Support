using System;
using ShareInvest.Progress;

namespace ShareInvest.Automatic
{
    static class GoblinBat
    {
        static void Main()
        {
            new Select().StartProcess(DateTime.Now.Hour < 16 ? @"\Kospi200.exe" : @"\BackTesting.exe");
            Environment.Exit(0);
        }
    }
}