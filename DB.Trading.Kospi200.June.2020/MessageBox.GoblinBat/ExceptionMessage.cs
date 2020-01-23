using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareInvest.Message
{
    public class ExceptionMessage
    {
        public ExceptionMessage(string message)
        {
            new Task(() => Record(message)).Start();
        }
        public ExceptionMessage(string message, string code)
        {
            this.code = code;
            new Task(() => Record(message)).Start();
        }
        private void Record(string message)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, @"Message\");
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using (StreamWriter sw = new StreamWriter(string.Concat(path, DateTime.Now.ToString("yyMMdd"), ".txt"), true))
                {
                    if (code != null)
                        sw.WriteLine(code);

                    sw.WriteLine(DateTime.Now.ToShortTimeString());
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Record(ex.ToString());
            }
        }
        private readonly string code;
    }
}