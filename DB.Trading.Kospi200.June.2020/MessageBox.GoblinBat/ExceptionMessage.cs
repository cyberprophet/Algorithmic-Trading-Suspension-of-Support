using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareInvest.Message
{
    public class ExceptionMessage
    {
        public ExceptionMessage(string message, string code)
        {
            this.code = code;
            new Task(() => Record(message)).Start();
        }
        void Record(string message)
        {
            try
            {
                var path = Path.Combine(Application.StartupPath, @"Message\");
                var di = new DirectoryInfo(path);
                var date = int.Parse(DateTime.Now.AddDays(-30).ToString("yyMMdd"));

                if (di.Exists)
                    foreach (var file in Directory.GetFiles(path))
                    {
                        string[] recent = file.Split('\\');
                        recent = recent[recent.Length - 1].Split('.');

                        if (date < int.Parse(recent[recent.Length - 2]))
                            continue;

                        new FileInfo(file).Delete();
                    }
                else
                    di.Create();

                using (var sw = new StreamWriter(string.Concat(path, DateTime.Now.ToString("yyMMdd"), ".txt"), true))
                {
                    if (code != null)
                        sw.WriteLine(code);

                    sw.WriteLine(DateTime.Now.ToShortTimeString());
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Record(ex.StackTrace);
            }
        }
        public ExceptionMessage(string message) => new Task(() => Record(message)).Start();
        readonly string code;
    }
}