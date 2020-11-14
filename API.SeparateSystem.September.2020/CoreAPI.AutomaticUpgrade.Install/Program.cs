using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Versioning;

namespace ShareInvest
{
    class Program
    {
        static bool IsDebug
        {
            get; set;
        }
        const string core_api = ".zip";
        const string path = @"C:\ShareInvest\";
        [STAThread, SupportedOSPlatform("windows")]
        static void Main()
        {
            ChangePropertyToDebugMode();

            if (IsDebug && Security.IsAdministrator)
            {
                string name = Console.ReadLine(), version = string.Empty;
                var address = Security.ChooseTheInstallationPath(name);
                var files = new List<string>();

                foreach (var file in new DirectoryInfo(address.Item2).GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (file.Name.Equals("CoreAPI.exe"))
                    {
                        var temp = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion.Replace(".", string.Empty)[2..];
                        version = string.Concat(temp[4..], temp[0..^4]);
                    }
                    files.Add(file.FullName);
                }
                using (var zip = ZipFile.Open(string.Concat(path, name, version, core_api), ZipArchiveMode.Create))
                    foreach (var file in files)
                        zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);

                Console.WriteLine(new Client().TransmitTheDataToUpdate(), address);
            }
            else
            {
                new Server();

                return;
            }
            GC.Collect();
            Process.GetCurrentProcess().Kill();
        }
        [Conditional("DEBUG")]
        static void ChangePropertyToDebugMode() => IsDebug = true;
    }
}