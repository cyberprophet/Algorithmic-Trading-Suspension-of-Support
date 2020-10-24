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
        StringBuilder Data
        {
            get;
        }
        public void ToCollect(string[] param)
        {
            if (Time.Equals(param[0]) == false)
            {
                Time = param[0];
                Index = 0;
            }
            Date.Append(param[0]).Append(Index++.ToString("D3")).Append('|');
            Data.Append(param[1]).Append(';').Append(param[2]).Append(';').Append(param[3]).Append(';').Append(param[4]).Append('|');
        }
        public async Task TransmitStringData() => await client.PostContextAsync(Data.Remove(Data.Length - 1, 1), Date.Remove(Date.Length - 1, 1), code);
        public Collect(string code, string grant)
        {
            this.code = code;
            Time = DateTime.Now.ToString("HHmmss");
            Date = new StringBuilder(grant.Length);
            Data = new StringBuilder(grant.Length * code.Length);
            client = new Client.Collect(grant);
        }
        readonly string code;
        readonly Client.Collect client;
    }
}