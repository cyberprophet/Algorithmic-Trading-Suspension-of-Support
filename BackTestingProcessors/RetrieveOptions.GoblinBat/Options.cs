using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ShareInvest.RetrieveOptions
{
    public class Options : IOptions
    {
        public Dictionary<string, Dictionary<string, Dictionary<string, double>>> Repository
        {
            get; private set;
        }
        public Options()
        {
            list = new Dictionary<string, double>(512);
            temp = new Dictionary<string, Dictionary<string, double>>(512);
            Repository = new Dictionary<string, Dictionary<string, Dictionary<string, double>>>(512);

            foreach (string file in Directory.GetFiles(Path.Combine(Application.StartupPath, @"..\Chart\"), "*.csv", SearchOption.AllDirectories))
                if (!file.Contains("Day") && !file.Contains("Tick"))
                    ReadCSV(file);
        }
        private void ReadCSV(string file)
        {
            try
            {
                using StreamReader sr = new StreamReader(file);
                if (sr != null)
                    while (sr.EndOfStream == false)
                        OnReceiveOptions(new OptionsRepository(file, sr.ReadLine(), sr.EndOfStream));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private void OnReceiveOptions(OptionsRepository e)
        {
            if (e.Code.Equals(Code) || Code == null)
            {
                if (Code == null)
                {
                    Code = e.Code;
                    FileName = e.FileName;
                }
                list[e.Date] = e.Price;

                if (e.EndOfStream)
                {
                    temp[Code] = list;
                    Repository[FileName] = temp;
                }
                return;
            }
            temp[Code] = list;

            if (!e.Code.Equals(Code) && !e.FileName.Equals(FileName))
            {
                Repository[FileName] = temp;
                temp = new Dictionary<string, Dictionary<string, double>>(512);
                FileName = e.FileName;
            }
            list = new Dictionary<string, double>(512)
            {
                { e.Date, e.Price }
            };
            Code = e.Code;
            GC.Collect();
        }
        private string Code
        {
            get; set;
        }
        private string FileName
        {
            get; set;
        }
        private Dictionary<string, Dictionary<string, double>> temp;
        private Dictionary<string, double> list;
    }
}