using System.IO;
using System.Runtime.Versioning;

namespace ShareInvest
{
    public static class Repository
    {
        public static string Path => path;
        public static string Extension => extension;
        [SupportedOSPlatform("windows")]
        public static void RecodeToEncryption(string name) => new FileInfo(name).Encrypt();
        [SupportedOSPlatform("windows")]
        public static void ReadTheFile(string name) => new FileInfo(name).Decrypt();
        public static void CreateTheDirectory(DirectoryInfo info)
        {
            if (info.Exists == false)
                info.Create();
        }
        const string path = @"C:\Algorithmic Trading";
        const string extension = ".res";
    }
}