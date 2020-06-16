using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Microsoft.Win32;

using ShareInvest.Message;
using ShareInvest.Strategy;
using ShareInvest.Verify;

namespace ShareInvest
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            if (ShowWindow(GetConsoleWindow(), secret.Hide) && secret.GetIdentify(str))
            {
                var path = Path.Combine(Application.StartupPath, secret.Indentify);
                var registry = Registry.CurrentUser.OpenSubKey(new Secret().Path);
                var initial = secret.GetPort(str);
                var remaining = secret.GetIsSever(str) ? 1 : ran.Next(initial.Equals((char)Port.Seriate) ? 11 : 9, 15);
                var cts = new CancellationTokenSource();
                var retrieve = new Strategy.Retrieve(str, initial);
                var count = secret.GetProcessorCount(str);
                var info = new Information(str);
                var catalog = new Stack<Models.Simulations>();

                if (secret.GetDirectoryInfoExists(path))
                {
                    if (registry.GetValue(secret.GoblinBat) == null || DateTime.Now.Date.Equals(new DateTime(2020, 4, 3)))
                    {
                        registry.Close();
                        registry = Registry.CurrentUser.OpenSubKey(new Secret().Path, true);
                        registry.SetValue(secret.GoblinBat, Array.Find(Directory.GetFiles(Application.StartupPath, "*.exe", SearchOption.AllDirectories), o => o.Contains(string.Concat(secret.GodSword, ".exe"))));
                    }
                    while (remaining > 0)
                        if (TimerBox.Show(new Secret(remaining--).RemainingTime, secret.GetIdentify(), MessageBoxButtons.OK, MessageBoxIcon.Information, 60000U).Equals(DialogResult.OK) && remaining == 0)
                        {
                            if (DateTime.Now.Hour == 15 && (secret.GetHoliday(DateTime.Now) == false || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) == false || DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) == false) && secret.GetIsSever(str))
                            {
                                retrieve.GetInitialzeTheCode();
                                info.GetUserIdentity(initial);
                                Application.EnableVisualStyles();
                                Application.SetCompatibleTextRenderingDefault(false);
                                Application.Run(new GoblinBat(initial, secret, str, cts, retrieve));

                                return;
                            }
                            new Task(() =>
                            {
                                retrieve.GetRecentDate(DateTime.Now);
                                retrieve.GetInitialzeTheCode();
                                info.GetUserIdentity(initial);
                                catalog = count == 0.75 ? info.GetStatistics(secret.GetExternal(str), secret.rate, secret.commission) : info.GetStatistics(count);

                                if (initial.Equals((char)Port.Collecting) == false)
                                {
                                    var mine = retrieve.OnReceiveMyStrategy();
                                    new BackTesting(initial, mine, str);
                                    Count++;

                                    foreach (var my in info.GetStatistics(mine, secret.rate[0]))
                                        catalog.Push(my);
                                }
                                if (secret.GetIsSever(str))
                                {
                                    count = 0.25;
                                    info.SetInsertBaseStrategy(secret.strategy, secret.rate, secret.commission);

                                    foreach (var best in info.GetBestStrategy())
                                        catalog.Push(best);
                                }
                                var po = new ParallelOptions
                                {
                                    CancellationToken = cts.Token,
                                    MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * count * (initial.Equals((char)Port.Collecting) ? 1 : 2))
                                };
                                try
                                {
                                    Parallel.ForEach(catalog, po, new Action<Models.Simulations>((number) =>
                                    {
                                        if (cts.IsCancellationRequested)
                                            po.CancellationToken.ThrowIfCancellationRequested();

                                        if (retrieve.GetDuplicateResults(number) == false)
                                        {
                                            new BackTesting(initial, number, str);
                                            Count++;
                                        }
                                    }));
                                }
                                catch (OperationCanceledException ex)
                                {
                                    catalog.Clear();
                                    new ExceptionMessage(ex.StackTrace);
                                }
                                catch (Exception ex)
                                {
                                    if (Array.Exists(Information.RemainingDay, o => o.Equals(DateTime.Now.ToString(format))) == false && secret.GetIsSever(str) == false && (DateTime.Now.Hour > 4 && DateTime.Now.Hour < 9 || DateTime.Now.Hour > 15 && DateTime.Now.Hour < 18))
                                        Process.Start("shutdown.exe", "-r");

                                    else
                                    {
                                        new ExceptionMessage(ex.TargetSite.Name);

                                        while (catalog.Count > 0)
                                        {
                                            var pop = catalog.Pop();

                                            if (retrieve.GetDuplicateResults(pop) == false)
                                            {
                                                new BackTesting(initial, pop, str);
                                                Count++;
                                            }
                                        }
                                    }
                                    cts.Dispose();
                                    new ExceptionMessage(ex.StackTrace, ex.TargetSite.Name);
                                }
                            }).Start();
                        }
                    while (TimerBox.Show(secret.StartProgress, string.Concat("N0.", Count.ToString("N0")), MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, 30000U).Equals(DialogResult.Cancel))
                        if (secret.GetHoliday(DateTime.Now) == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) == false)
                        {
                            if (initial.Equals((char)Port.Collecting) && (DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17) && DateTime.Now.Minute > 39 && ran.Next(0, 10) == 9)
                                break;

                            if ((DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17) && (DateTime.Now.Minute > 54 || DateTime.Now.Minute > 49 && ran.Next(0, 5) == 3))
                                break;
                        }
                    if (initial.Equals((char)126) == false)
                    {
                        if (initial.Equals((char)Port.Collecting) == false && cts.IsCancellationRequested == false && DateTime.Now.Hour == 8)
                            try
                            {
                                cts.Cancel();
                            }
                            catch (Exception ex)
                            {
                                new ExceptionMessage(ex.StackTrace);
                            }
                            finally
                            {
                                catalog.Clear();
                                cts.Dispose();
                                GC.Collect();
                            }
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new GoblinBat(initial, secret, str, cts, retrieve));
                    }
                    else
                        new ExceptionMessage(str);
                }
                else if (ShowWindow(GetConsoleWindow(), secret.Show) == false)
                {
                    while (secret.SetIndentify(path, str) == false)
                        Thread.Sleep(ran.Next(1, 100));

                    Process.Start("shutdown.exe", "-r");
                }
            }
        }
        static readonly Random ran = new Random(Guid.NewGuid().GetHashCode());
        static readonly Secret secret = new Secret();
        static readonly string str = KeyDecoder.GetWindowsProductKeyFromRegistry();
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        internal static uint Count
        {
            get; set;
        }
        const string format = "yyMMdd";
    }
}