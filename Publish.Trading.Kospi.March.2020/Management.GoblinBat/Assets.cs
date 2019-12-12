using System;
using System.IO;
using System.Windows.Forms;
using ShareInvest.Log.Message;

namespace ShareInvest.Management
{
    public class Assets
    {
        public string ReadCSV()
        {
            string[] temp;
            ulong recent = 0;
            string assets = string.Empty;

            try
            {
                foreach (string val in Directory.GetFiles(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\BasicMaterial\BackTesting\"), "*.csv", SearchOption.AllDirectories))
                {
                    temp = val.Split('\\');
                    temp = temp[temp.Length - 1].Split('.');
                    ulong count = ulong.Parse(temp[0]);

                    if (count > recent)
                        recent = count;
                }
                using StreamReader sr = new StreamReader(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\BasicMaterial\BackTesting\", recent, ".csv"));
                if (sr != null)
                    while (sr.EndOfStream == false)
                        assets = sr.ReadLine();
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Error", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            return assets;
        }
    }
}