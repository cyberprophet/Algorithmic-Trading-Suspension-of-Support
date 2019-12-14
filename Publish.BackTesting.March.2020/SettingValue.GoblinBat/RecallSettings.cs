using System;
using System.IO;
using System.Windows.Forms;
using ShareInvest.Log.Message;

namespace ShareInvest.SettingValue
{
    public class RecallSettings
    {
        public string[] GetSettingValue()
        {
            string[] temp;
            ulong recent = 0;
            string value = string.Empty, path = Path.Combine(Application.StartupPath, @"..\SettingValue\");

            try
            {
                foreach (string val in Directory.GetFiles(path))
                {
                    temp = val.Split('\\');
                    temp = temp[temp.Length - 1].Split('.');
                    ulong count = ulong.Parse(temp[0]);

                    if (count > recent)
                        recent = count;
                }
                using (StreamReader sr = new StreamReader(string.Concat(path, recent, ".csv")))
                {
                    if (sr != null)
                        while (sr.EndOfStream == false)
                            value = sr.ReadLine();
                }
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
            return value.Split(',');
        }
    }
}