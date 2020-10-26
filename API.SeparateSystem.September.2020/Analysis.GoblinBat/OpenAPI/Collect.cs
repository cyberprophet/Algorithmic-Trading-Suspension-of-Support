using System;
using System.Text;
using System.Threading.Tasks;

namespace ShareInvest.Analysis.OpenAPI
{
    public class Collect
    {
        uint Index
        {
            get; set;
        }
        string Time
        {
            get; set;
        }
        StringBuilder Date
        {
            get;
        }
        public StringBuilder Data
        {
            get;
        }
        public void ToCollect(string time)
        {
            if (Time.Equals(time) == false)
            {
                Time = time;
                Index = 0;
            }
            Date.Append(time).Append(Index++.ToString("D3")).Append('|');
        }
        public async Task TransmitStringData()
        {
            if (Date.Length > 0xA && await client.PostContextAsync(Data.Remove(Data.Length - 1, 1), Date.Remove(Date.Length - 1, 1), code) > 0)
            {
                Date.Clear();
                Data.Clear();
            }
        }
        public Collect(string code, string grant)
        {
            this.code = code;
            Time = DateTime.Now.ToString("HHmmss");
            Date = new StringBuilder(code.Length);
            Data = new StringBuilder(grant.Length * Time.Length);
            client = new Client.Collect(grant);
        }
        readonly string code;
        readonly Client.Collect client;
    }
}