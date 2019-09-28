using System;
using System.Collections.Generic;
using System.IO;

namespace ShareInvest.Chart
{
    public class Fetch
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

                    return list;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return list;
        }
        protected List<string> list;

        private StreamReader sr;
    }
}