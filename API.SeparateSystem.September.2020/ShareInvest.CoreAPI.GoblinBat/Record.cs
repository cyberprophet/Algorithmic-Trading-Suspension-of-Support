using System;
using System.IO;
using System.Threading.Tasks;

namespace ShareInvest
{
    public static class Record
    {
        public static async Task SendToErrorMessage(string name, string message)
        {
            var directory = new DirectoryInfo(path);
            var now = DateTime.Now;

            if (directory.Exists == false)
                directory.Create();

            using var sw = new StreamWriter(string.Concat(path, now.ToString(format), file), true);
            foreach (var str in new string[] { now.ToLongTimeString(), name, message })
                await sw.WriteLineAsync(str);
        }
        const string path = @"C:\ShareInvest\Res\";
        const string file = @".res";
        const string format = "yyMMdd";
    }
}