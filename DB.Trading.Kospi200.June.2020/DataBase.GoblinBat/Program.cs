using System;
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
                var registry = Registry.CurrentUser.OpenSubKey(new Secret().Path);
                var classfication = secret.GetPort(str).Equals((char)Port.Trading) && (DateTime.Now.Hour == 15 && DateTime.Now.Minute >= 45 || DateTime.Now.Hour > 4 && DateTime.Now.Hour < 6);
                var remaining = secret.GetIsSever(str) ? ran.Next(classfication ? 25 : 5, classfication ? 31 : 11) : 1;
                var path = Path.Combine(Application.StartupPath, secret.Indentify);
                var initial = secret.GetPort(str);
                var cts = new CancellationTokenSource();

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
                            var retrieve = new Strategy.Retrieve(str);
                            var list = retrieve.SetInitialzeTheCode();
                            int num = list.Count;
                            ParallelOptions po;
                            Task task;

                            while (num-- > 1)
                            {
                                var shuffle = ran.Next(num + 1);
                                var value = list[shuffle];
                                list[shuffle] = list[num];
                                list[num] = value;
                            }
                            if (secret.GetIsSever(str))
                            {
                                po = new ParallelOptions
                                {
                                    CancellationToken = cts.Token,
                                    MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * secret.GetProcessorCount(str))
                                };
                                task = new Task(() =>
                                {
                                    try
                                    {
                                        Parallel.ForEach(list, po, new Action<long>((number) =>
                                        {
                                            po.CancellationToken.ThrowIfCancellationRequested();

                                            if (retrieve.GetDuplicateResults(number) == false)
                                                new BackTesting(initial, retrieve.OnReceiveStrategy(number), str);
                                        }));
                                    }
                                    catch (OperationCanceledException ex)
                                    {
                                        new ExceptionMessage(ex.StackTrace);
                                    }
                                    catch (Exception ex)
                                    {
                                        new ExceptionMessage(ex.StackTrace);
                                    }
                                });
                            }
                            else
                            {
                                var info = new Information(str);
                                task = new Task(() =>
                                {
                                    try
                                    {
                                        po = new ParallelOptions
                                        {
                                            CancellationToken = cts.Token,
                                            MaxDegreeOfParallelism = Environment.ProcessorCount
                                        };
                                        Parallel.ForEach(info.GetUserIdentity(), po, new Action<string[]>((identify) =>
                                        {
                                            po.CancellationToken.ThrowIfCancellationRequested();
                                            info.SetInsertStrategy(identify);
                                        }));
                                        po = new ParallelOptions
                                        {
                                            CancellationToken = cts.Token,
                                            MaxDegreeOfParallelism = (int)(Environment.ProcessorCount * 0.375)
                                        };
                                        Parallel.ForEach(list, po, new Action<long>((number) =>
                                        {
                                            po.CancellationToken.ThrowIfCancellationRequested();

                                            if (retrieve.GetDuplicateResults(number) == false)
                                                new BackTesting(initial, retrieve.OnReceiveStrategy(number), str);
                                        }));
                                    }
                                    catch (OperationCanceledException ex)
                                    {
                                        new ExceptionMessage(ex.StackTrace);
                                    }
                                    catch (Exception ex)
                                    {
                                        new ExceptionMessage(ex.StackTrace);
                                    }
                                });
                            }
                            task.Start();
                        }
                    while (TimerBox.Show(secret.StartProgress, secret.GetIdentify(), MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, 30000U).Equals(DialogResult.Cancel))
                        if (secret.GetHoliday(DateTime.Now) == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Saturday) == false && DateTime.Now.DayOfWeek.Equals(DayOfWeek.Sunday) == false)
                        {
                            if (initial.Equals((char)Port.Collecting) && (DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17) && DateTime.Now.Minute > 35 && ran.Next(0, 10) == 9)
                                break;

                            if ((DateTime.Now.Hour == 8 || DateTime.Now.Hour == 17) && DateTime.Now.Minute > 50)
                                break;
                        }
                    if (initial.Equals((char)126) == false)
                    {
                        Application.EnableVisualStyles();
                        Application.SetCompatibleTextRenderingDefault(false);
                        Application.Run(new GoblinBat(initial, secret, str, cts));
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
    }
}