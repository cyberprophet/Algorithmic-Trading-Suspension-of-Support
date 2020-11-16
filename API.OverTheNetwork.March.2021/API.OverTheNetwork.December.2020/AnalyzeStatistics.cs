using System;
using System.IO;
using System.IO.Pipes;
using System.Security.Principal;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;

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
        internal static void TryToConnectThePipeStream() => new Task(async () =>
        {
            var client = new NamedPipeClientStream(".", Key, PipeDirection.InOut, PipeOptions.Asynchronous, TokenImpersonationLevel.Impersonation);
            await client.ConnectAsync();
            TellTheClientConnectionStatus(client.IsConnected);

            if (client.IsConnected)
                OnReceivePipeClientMessage(client);
        }).Start();
        static bool Collection
        {
            get; set;
        }
        static void OnReceivePipeClientMessage(NamedPipeClientStream client)
        {
            bool repeat = true;
            using (var sr = new StreamReader(client))
                try
                {
                    while (client.IsConnected)
                    {
                        var param = sr.ReadLine();

                        if (string.IsNullOrEmpty(param) == false)
                        {
                            string[] temp = param.Split('|'), price;

                            if (temp[0].Length != 5 && temp[0].Length != 0xA && Collection && Security.Collection.TryGetValue(temp[1], out Statistical.Analysis analysis))
                                switch (temp[0].Length)
                                {
                                    case 4 when temp[0].Equals("주식시세") == false:
                                        price = temp[^1].Split(';');
                                        new Task(() => analysis.AnalyzeTheConclusion(price)).Start();
                                        analysis.Collection.Enqueue(new Collect
                                        {
                                            Time = price[0],
                                            Datum = temp[^1][7..]
                                        });
                                        break;

                                    case 6 when temp[0].Equals("주식우선호가") == false:
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
                                    switch (Enum.ToObject(typeof(Operation), number))
                                    {
                                        case Operation.장시작:
                                            Collection = operation[1].Equals("090000") && operation[^1].Equals("000000");
                                            break;

                                        case Operation.장시작전 when operation[1].Equals("085500"):

                                            break;
                                    }
                                else if (char.TryParse(operation[0], out char charactor))
                                    switch (Enum.ToObject(typeof(Operation), charactor))
                                    {
                                        case Operation.선옵_장마감전_동시호가_종료:

                                            break;

                                        case Operation.시간외_단일가_매매종료:
                                            repeat = false;
                                            Host.Dispose();
                                            break;
                                    }
                            }
                            else if (temp[0].Length == 0xA)
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    client.Close();
                    client.Dispose();
                    TellTheClientConnectionStatus(client.IsConnected);
                }
            if (repeat && Console.ReadLine().Equals("Try to Connect"))
                TryToConnectThePipeStream();
        }
        static void TellTheClientConnectionStatus(bool is_connected) => Console.WriteLine("Client is connected on {0}", is_connected);
    }
}