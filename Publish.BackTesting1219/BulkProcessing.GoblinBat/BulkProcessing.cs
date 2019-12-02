using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using ShareInvest.Communication;
using ShareInvest.Log.Message;

namespace ShareInvest.MassProcessingTechnology
{
    public class BulkProcessing
    {
        public BulkProcessing(string path)
        {
            Dictionary<string, StringBuilder> date = new Dictionary<string, StringBuilder>(64);
            Dictionary<string, StringBuilder> material = new Dictionary<string, StringBuilder>(1024);
            StringBuilder sb = new StringBuilder(64);
            StringBuilder enginery = new StringBuilder(32);
            string strategy = string.Empty;
            string[] temp = null, param;

            foreach (string str in new Enumerate())
            {
                temp = str.Split(',');

                if (!strategy.Equals(string.Empty) && !strategy.Equals(temp[0]))
                {
                    material[strategy] = sb;
                    strategy = temp[0];
                    sb = new StringBuilder(64);
                }
                else if (strategy.Equals(string.Empty))
                    strategy = temp[0];

                Application.DoEvents();
                sb.Append(temp[1]).Append(';').Append(temp[2]).Append(';').Append(temp[3]).Append(';').Append(temp[4]).Append('*');
            }
            if (temp != null)
            {
                material[temp[0]] = sb;
                sb = new StringBuilder(32);
                GC.Collect();
            }
            foreach (KeyValuePair<string, StringBuilder> kv in material)
            {
                temp = kv.Value.ToString().Split('*');

                foreach (string str in temp)
                    if (!str.Equals(string.Empty))
                    {
                        param = str.Split(';');
                        Application.DoEvents();

                        if (!date.ContainsKey(param[0]))
                        {
                            date[param[0]] = new StringBuilder(16).Append(',').Append(param[2]);

                            continue;
                        }
                        if (date.ContainsKey(param[0]) && !param[0].Equals(temp[temp.Length - 2].Substring(0, 8)))
                        {
                            date[param[0]] = date[param[0]].Append(',').Append(param[2]);

                            continue;
                        }
                        if (date.ContainsKey(param[0]))
                            date[param[0]] = date[param[0]].Append(',').Append(long.Parse(param[2]) + long.Parse(param[1]));
                    }
                temp = temp[temp.Length - 2].Split(';');
                sb.Append(',').Append(long.Parse(temp[1]) + long.Parse(temp[3]));
                enginery.Append(',').Append(kv.Key);
            }
            Statistics(enginery, path);
            material.Clear();
            enginery.Clear();
            GC.Collect();

            foreach (KeyValuePair<string, StringBuilder> kv in date)
                Statistics(new StringBuilder(16).Append(kv.Key).Append(kv.Value), path);

            Statistics(sb, path);
        }
        private void Statistics(StringBuilder sb, string path)
        {
            Application.DoEvents();

            try
            {
                DirectoryInfo di = new DirectoryInfo(string.Concat(Path.Combine(Application.StartupPath, @"..\"), @"\Statistics\"));

                if (di.Exists == false)
                    di.Create();

                using StreamWriter sw = new StreamWriter(path, true);
                sw.WriteLine(sb);
            }
            catch (Exception ex)
            {
                new LogMessage().Record("Exception", ex.ToString());
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
    }
}