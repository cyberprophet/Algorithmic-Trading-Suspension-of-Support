using System;
using System.Collections.Generic;
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
        Dictionary<string, StringBuilder> Storage
        {
            get;
        }
        public void ToCollect(string time, StringBuilder data)
        {
            if (Time.Equals(time) == false)
            {
                Time = time;
                Index = 0;
            }
            Storage[string.Concat(time, Index++.ToString("D3"))] = data;
        }
        public async Task TransmitStringData()
        {
            if (Storage.Count > 0 && await client.PostContextAsync(Storage, code) > 0)
                Storage.Clear();
        }
        public Collect(string code, string grant)
        {
            this.code = code;
            Time = DateTime.Now.ToString("HHmmss");
            Storage = new Dictionary<string, StringBuilder>();
            client = new Client.Collect(grant);
        }
        readonly string code;
        readonly Client.Collect client;
    }
}