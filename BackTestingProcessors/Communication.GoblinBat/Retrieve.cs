using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace ShareInvest.Communication
{
    public class Retrieve : IFetch
    {
        private List<string> ReadCSV(string file, List<string> list)
        {
            try
            {
                using StreamReader sr = new StreamReader(file);
                if (sr != null)
                    while (sr.EndOfStream == false)
                        list.Add(sr.ReadLine());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
            return list;
        }
        private Retrieve()
        {
            DayChart = ReadCSV(Array.Find(Directory.GetFiles(Path.Combine(Application.StartupPath, @"..\"), "*.csv", SearchOption.AllDirectories), o => o.Contains("Day")), DayChart);
            TickChart = ReadCSV(Array.Find(Directory.GetFiles(Path.Combine(Application.StartupPath, @"..\"), "*.csv", SearchOption.AllDirectories), o => o.Contains("Tick")), TickChart);
        }
        public static Retrieve Get()
        {
            if (retrieve == null)
                retrieve = new Retrieve();

            return retrieve;
        }
        public List<string> DayChart
        {
            get; private set;
        } = new List<string>(256);
        public List<string> TickChart
        {
            get; private set;
        } = new List<string>(2097152);
        private static Retrieve retrieve;
    }
}