using System;
using System.IO;
using System.Windows.Forms;

namespace ShareInvest.Log.Message
{
    public class LogMessage
    {
        public void Record(string directory, string message)
        {
            string path = string.Concat(Path.Combine(Application.StartupPath, @"..\Message\"), directory, @"\");

            try
            {
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using StreamWriter sw = new StreamWriter(string.Concat(path, DateTime.Now.ToString("yyMMdd"), ".txt"), true);
                sw.WriteLine(message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
    }
}