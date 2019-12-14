using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShareInvest.Log.Message;

namespace ShareInvest.SettingValue
{
    public class SaveSetting
    {
        public void SetSettingValue(StringBuilder param)
        {
            try
            {
                string path = Path.Combine(Application.StartupPath, @"..\SettingValue\");
                DirectoryInfo di = new DirectoryInfo(path);

                if (di.Exists == false)
                    di.Create();

                using (StreamWriter sw = new StreamWriter(string.Concat(path, DateTime.Now.Ticks, ".csv")))
                {
                    sw.WriteLine(param);
                }
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Exit();
            }
        }
    }
}