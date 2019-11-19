using System;
using System.Collections.Generic;
using System.IO;

namespace ShareInvest.RetrieveOptions
{
    public class Options : IOptions
    {
        public static Options Get()
        {
            if (options == null)
                options = new Options();

            return options;
        }
        public Dictionary<string, Dictionary<string, List<OptionsRepository>>> Repository
        {
            get; internal set;
        }
        internal void ReadCSV(string file)
        {
            try
            {
                using StreamReader sr = new StreamReader(file);
                if (sr != null)
                    while (sr.EndOfStream == false)
                        SendRepository?.Invoke(this, new OptionsRepository(file, sr.ReadLine(), sr.EndOfStream));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private Options()
        {
            Repository = new Dictionary<string, Dictionary<string, List<OptionsRepository>>>(4096);
        }
        private static Options options;
        public event EventHandler<OptionsRepository> SendRepository;
    }
}