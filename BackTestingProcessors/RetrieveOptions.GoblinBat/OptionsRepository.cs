using System;

namespace ShareInvest.RetrieveOptions
{
    public class OptionsRepository : EventArgs
    {
        public string Code
        {
            get; private set;
        }
        public string Date
        {
            get; private set;
        }
        public string FileName
        {
            get; private set;
        }
        public double Price
        {
            get; private set;
        }
        public bool EndOfStream
        {
            get; private set;
        }
        public OptionsRepository(string file, string data, bool end)
        {
            string[] temp = file.Split('\\');
            FileName = temp[temp.Length - 2];
            temp = temp[temp.Length - 1].Split('.');
            Code = temp[0];
            temp = data.Split(',');
            Date = temp[0];
            Price = double.Parse(temp[1].Contains("-") ? temp[1].Substring(1) : temp[1]);
            EndOfStream = end;
        }
        public OptionsRepository(string date, double price)
        {
            Date = date;
            Price = price;
        }
    }
}