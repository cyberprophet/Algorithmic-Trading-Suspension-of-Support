using System;
using System.Collections.Generic;
using System.IO;
using ShareInvest.AutoMessageBox;

namespace ShareInvest.RetrieveInformation
{
    public class Retrieve
    {
        protected List<string> ReadCSV(string file, List<string> list)
        {
            try
            {
                using (sr = new StreamReader(file))
                {
                    if (sr != null)
                        while (sr.EndOfStream == false)
                            list.Add(sr.ReadLine());
                }
            }
            catch (Exception ex)
            {
                Box.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", 3750);
                Environment.Exit(0);
            }
            return list;
        }
        private StreamReader sr;
    }
}