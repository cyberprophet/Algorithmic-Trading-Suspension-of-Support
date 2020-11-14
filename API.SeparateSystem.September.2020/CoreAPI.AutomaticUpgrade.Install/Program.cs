using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.Versioning;

using Microsoft.Win32;

namespace ShareInvest
{
    class Program
    {
        static bool IsDebug
        {
            get; set;
        }
        [STAThread, SupportedOSPlatform("windows")]
        static void Main()
        {
            ChangePropertyToDebugMode();

            if (IsDebug && Security.IsAdministrator)
            {
                string name = Console.ReadLine(), version = string.Empty, generate_file;
                var address = Security.ChooseTheInstallationPath(name);
                var files = new List<string>();

                foreach (var file in new DirectoryInfo(address.Item2).GetFiles("*.*", SearchOption.AllDirectories))
                {
                    if (file.Name.Equals(Security.CoreAPI))
                    {
                        var temp = FileVersionInfo.GetVersionInfo(file.FullName).FileVersion.Replace(".", string.Empty)[2..];
                        version = string.Concat(temp[^4..], temp[0..^4]);
                    }
                    files.Add(file.FullName);
                }
                generate_file = string.Concat(Security.ShareInvest, name, version, Security.Zip);

                using (var zip = ZipFile.Open(generate_file, ZipArchiveMode.Create))
                    foreach (var file in files)
                        zip.CreateEntryFromFile(file, Path.GetFileName(file), CompressionLevel.Optimal);

                Console.WriteLine(Client.TransmitTheDataToUpdate(), address);
                new FileInfo(generate_file).Delete();
                GC.Collect();
                Process.GetCurrentProcess().Kill();
            }
            else
            {
                foreach (var name in Security.Names)
                    using (var registry = Registry.CurrentUser.OpenSubKey(Security.Run))
                        if (registry.GetValue(name) != null)
                        {
                            registry.Close();
                            Registry.CurrentUser.OpenSubKey(Security.Run, true).DeleteValue(name);
                        }
                using (var registry = Registry.CurrentUser.OpenSubKey(Security.Run))
                    if (registry.GetValue(Security.Names[2]) == null)
                    {
                        registry.Close();
                        Registry.CurrentUser.OpenSubKey(Security.Run, true).SetValue(Security.Names[2], string.Concat(Security.ShareInvest, Security.Names[2], ".exe"));
                    }
                var path = Security.ChooseTheInstallationPath();
                Server.StartProgress(path.Item1, path.Item2);
            }
        }
        [Conditional("DEBUG")]
        static void ChangePropertyToDebugMode() => IsDebug = true;
    }
}