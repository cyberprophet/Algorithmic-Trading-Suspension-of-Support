using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Runtime.Versioning;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;

using Newtonsoft.Json;

using ShareInvest.Catalog.Models;
using ShareInvest.Catalog.OpenAPI;

namespace ShareInvest
{
    static class AnalyzeStatistics
    {
        internal static IWebHost Host
        {
            private get; set;
        }
        internal static string Key
        {
            private get; set;
        }
        internal static StreamWriter Server
        {
            get; private set;
        }
        [SupportedOSPlatform("windows")]
        internal static void TryToConnectThePipeStream()
        {
            var server = new NamedPipeServerStream(Process.GetCurrentProcess().ProcessName, PipeDirection.Out, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous);
            var client = new NamedPipeClientStream(".", Key, PipeDirection.In, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
            new Task(async () =>
            {
                await server.WaitForConnectionAsync();
                TellTheClientConnectionStatus(server.GetType().Name, server.IsConnected);

                if (server.IsConnected)
                {
                    Server = new StreamWriter(server)
                    {
                        AutoFlush = true
                    };
                    Server.WriteLine("{0}API Connects via Pipe. . .", Security.SecuritiesCompany == 'O' ? "Open" : "Xing");
                }
            }).Start();
            new Task(async () =>
            {
                await client.ConnectAsync();
                TellTheClientConnectionStatus(client.GetType().Name, client.IsConnected);

                if (client.IsConnected)
                    OnReceivePipeClientMessage(client, server);
            }).Start();
        }
        [SupportedOSPlatform("windows")]
        static void OnReceivePipeClientMessage(NamedPipeClientStream client, NamedPipeServerStream server)
        {
            bool repeat = true, collection = false, stocks = false;
            using (var sr = new StreamReader(client))
                try
                {
                    while (client.IsConnected)
                    {
                        var param = sr.ReadLine();

                        if (string.IsNullOrEmpty(param) == false)
                        {
                            string[] temp = param.Split('|'), price;

                            if (temp[0].Length != 5 && temp[0].Length != 0xA && collection && Security.Collection.TryGetValue(temp[1], out Statistical.Analysis analysis))
                                switch (temp[0].Length)
                                {
                                    case 4 when temp[0].Equals("주식시세") == false && (stocks && temp[1].Length == 6 || temp[1].Length == 8):
                                        price = temp[^1].Split(';');
                                        new Task(() => analysis.AnalyzeTheConclusion(price)).Start();
                                        analysis.Collection.Enqueue(new Collect
                                        {
                                            Time = price[0],
                                            Datum = temp[^1][7..]
                                        });
                                        analysis.Collector = true;
                                        break;

                                    case 6 when temp[0].Equals("주식우선호가") == false && analysis.Collector:
                                        price = temp[^1].Split(';');
                                        new Task(() => analysis.AnalyzeTheQuotes(price)).Start();
                                        analysis.Collection.Enqueue(new Collect
                                        {
                                            Time = price[0],
                                            Datum = temp[^1][7..]
                                        });
                                        break;

                                    case 8 when temp[1][1] > '0':
                                    case 4 when temp[0].Equals("주식시세"):
                                        price = temp[^1].Split(';');
                                        analysis.Current = int.TryParse(price[0][0] == '-' ? price[0][1..] : price[0], out int current) ? current : 0;
                                        analysis.Offer = int.TryParse(price[1][0] == '-' ? price[1][1..] : price[1], out int stocks_offer) ? stocks_offer : 0;
                                        analysis.Bid = int.TryParse(price[^1][0] == '-' ? price[^1][1..] : price[^1], out int stocks_bid) ? stocks_bid : 0;
                                        break;

                                    case 8 when temp[1][1] == '0':
                                        price = temp[^1].Split(';');
                                        analysis.Current = double.TryParse(price[0][0] == '-' ? price[0][1..] : price[0], out double options_current) ? options_current : 0D;
                                        analysis.Offer = double.TryParse(price[1][0] == '-' ? price[1][1..] : price[1], out double options_offer) ? options_offer : 0D;
                                        analysis.Bid = double.TryParse(price[^1][0] == '-' ? price[^1][1..] : price[^1], out double options_bid) ? options_bid : 0D;
                                        break;

                                    case 6 when temp[0].Equals("주식우선호가"):
                                        price = temp[^1].Split(';');
                                        analysis.Offer = int.TryParse(price[0][0] == '-' ? price[0][1..] : price[0], out int offer) ? offer : 0;
                                        analysis.Bid = int.TryParse(price[^1][0] == '-' ? price[^1][1..] : price[^1], out int bid) ? bid : 0;
                                        break;
                                }
                            else if (temp[0].Length == 5 && temp[0].Equals("장시작시간"))
                            {
                                var operation = temp[^1].Split(';');

                                if (int.TryParse(operation[0], out int number))
                                {
                                    switch (Enum.ToObject(typeof(Operation), number))
                                    {
                                        case Operation.장시작:
                                            collection = operation[1].Equals("090000") && operation[^1].Equals("000000");
                                            stocks = true;
                                            break;

                                        case Operation.장마감:
                                            stocks = false;
                                            new Task(() =>
                                            {
                                                foreach (var collect in Security.Collection)
                                                    if (collect.Key.Length == 6)
                                                    {
                                                        var convert = collect.Value.SortTheRecordedInformation;
                                                        Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1), collect.Key, convert.Item2, convert.Item3);
                                                    }
                                            }).Start();
                                            Server.WriteLine(string.Concat(typeof(Operation).Name, '|', operation[0]));
                                            break;

                                        case Operation.장시작전 when operation[1].Equals("085500"):
                                            Server.WriteLine(string.Concat(typeof(Operation).Name, '|', operation[1]));
                                            break;

                                        case Operation.장마감전_동시호가 when operation[1].Equals("152000"):
                                            new Task(() =>
                                            {
                                                foreach (var stop in Security.Collection)
                                                    if (stop.Key.Length == 6)
                                                        stop.Value.Collector = false;
                                            }).Start();
                                            Server.WriteLine(string.Concat(typeof(Operation).Name, '|', operation[1]));
                                            break;
                                    }
                                    Base.SendMessage(string.Concat(DateTime.Now.ToString("HH:mm:ss.ffff"), '_', Enum.GetName(typeof(Operation), number), '_', operation[1]), typeof(Operation));
                                }
                                else if (char.TryParse(operation[0], out char charactor))
                                {
                                    switch (Enum.ToObject(typeof(Operation), charactor))
                                    {
                                        case Operation.선옵_장마감전_동시호가_종료 when repeat:
                                            repeat = false;
                                            new Task(() =>
                                            {
                                                foreach (var collect in Security.Collection)
                                                    if (collect.Key.Length == 8)
                                                    {
                                                        var convert = collect.Value.SortTheRecordedInformation;
                                                        Repository.KeepOrganizedInStorage(JsonConvert.SerializeObject(convert.Item1), collect.Key, convert.Item2, convert.Item3);
                                                    }
                                            }).Start();
                                            break;

                                        case Operation.시간외_단일가_매매종료:
                                            Server.WriteLine(string.Concat(typeof(Operation).Name, '|', operation[0]));
                                            Process.Start("shutdown.exe", "-r");
                                            Host.Dispose();
                                            break;
                                    }
                                    Base.SendMessage(string.Concat(DateTime.Now.ToString("HH:mm:ss.ffff"), '_', Enum.GetName(typeof(Operation), charactor), '_', operation[1]), typeof(Operation));
                                }
                            }
                            else if (temp.Length == 1)
                            {
                                if (param.Equals("조회"))
                                {
                                    collection = true;
                                    stocks = true;
                                }
                                else if (param.Length == 0xA)
                                    Base.SendMessage(param, typeof(AnalyzeStatistics));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Base.SendMessage(ex.StackTrace, typeof(AnalyzeStatistics));
                }
                finally
                {
                    client.Close();
                    client.Dispose();
                    Server.Close();
                    Server.Dispose();

                    if (server.IsConnected)
                        server.Disconnect();

                    server.Close();
                    server.Dispose();
                    TellTheClientConnectionStatus(server.GetType().Name, server.IsConnected);
                    TellTheClientConnectionStatus(client.GetType().Name, client.IsConnected);
                }
            if (repeat && Console.ReadLine().Equals("Try to Connect"))
            {
                TryToConnectThePipeStream();
                Console.WriteLine("Wait for the {0}API to Restart. . .", Security.SecuritiesCompany == 'O' ? "Open" : "Xing");
            }
            else
            {
                Process.Start("shutdown.exe", "-r");
                Host.Dispose();
            }
        }
        static void TellTheClientConnectionStatus(string name, bool is_connected) => Console.WriteLine("{0} is connected on {1}", name, is_connected);
    }
}