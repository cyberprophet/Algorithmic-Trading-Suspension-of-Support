using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Linq;
using AxKHOpenAPILib;
using ShareInvest.Catalog;
using ShareInvest.DelayRequest;
using ShareInvest.Interface;
using ShareInvest.OpenAPI;

namespace ShareInvest.Connect
{
    public class ConnectAPI : DistinctDate
    {
        public void SetAPI(AxKHOpenAPI axAPI)
        {
            AxAPI = axAPI;
            axAPI.OnEventConnect += OnEventConnect;
            axAPI.OnReceiveTrData += OnReceiveTrData;
            axAPI.OnReceiveRealData += OnReceiveRealData;
            axAPI.OnReceiveMsg += OnReceiveMsg;
        }
        public void StartProgress(IRealType type, SqlConnection sql)
        {
            if (AxAPI != null)
            {
                Sql = sql;
                Type = type;
                ErrorCode = AxAPI.CommConnect();
                sql.OpenAsync();

                if (ErrorCode != 0)
                    new Error(ErrorCode);

                return;
            }
            Environment.Exit(0);
        }
        public static ConnectAPI Get()
        {
            if (Api == null)
                Api = new ConnectAPI();

            return Api;
        }
        private void OnEventConnect(object sender, _DKHOpenAPIEvents_OnEventConnectEvent e)
        {
            int i;
            string exclusion, date = GetDistinctDate(CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now, CalendarWeekRule.FirstDay, DayOfWeek.Sunday) - CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(DateTime.Now.AddDays(1 - DateTime.Now.Day), CalendarWeekRule.FirstDay, DayOfWeek.Sunday) + 1);
            List<string> code = new List<string>
            {
                AxAPI.GetFutureCodeByIndex(e.nErrCode)
            };
            for (i = 2; i < 4; i++)
                foreach (var om in AxAPI.GetActPriceList().Split(';'))
                {
                    exclusion = AxAPI.GetOptionCode(om.Insert(3, "."), i, date);

                    if (code.Exists(o => o.Equals(exclusion)))
                        continue;

                    code.Add(exclusion);
                }
            code.RemoveAt(1);
            Sb = new StringBuilder(32);
            var absence = Sql.GetSchema("Columns").Rows;

            for (i = 0; i < code.Count; i++)
            {
                if (absence.Cast<DataRow>().Where(o => o.ItemArray.Contains(code[i])).Any() == false)
                {
                    var sql = Sql.CreateCommand();
                    sql.CommandText = GetQuery(code[i], date);
                    sql.CommandTimeout = 0;
                    sql.CommandType = CommandType.Text;
                    sql.BeginExecuteNonQuery();
                }
                Sb.Append(code[i]);

                if (i > 0 && i % 99 == 0 || i == code.Count - 1)
                {
                    AxAPI.CommKwRqData(Sb.ToString(), 0, 100, 3, "OPTFOFID", new Random().Next(1000, 10000).ToString());
                    Sb = new StringBuilder(32);

                    continue;
                }
                Sb.Append(';');
            }
            code.Clear();
        }
        private string GetQuery(string code)
        {
            var name = AxAPI.GetMasterCodeName(code).Trim();

            return string.Concat("BEGIN CREATE TABLE [", name, "] ([", code, "] BIGINT NOT NULL CONSTRAINT pk", code, " PRIMARY KEY,[Date] DATETIME NOT NULL,[Price] INT NOT NULL,[Volume] INT NOT NULL) END");
        }
        private string GetQuery(string code, string date)
        {
            var name = AxAPI.GetMasterCodeName(code);

            if (name.Equals(string.Empty))
            {
                name = code.Replace(code.Substring(3, 2), date).Replace("201", "C").Replace("301", "P");

                if (code.Last().Equals('2') || code.Last().Equals('7'))
                    name = string.Concat(name, ".5");
            }
            else
                name = name.Replace(" ", string.Empty);

            return string.Concat("BEGIN CREATE TABLE [", name, "] ([", code, "] BIGINT NOT NULL CONSTRAINT pk", code, " PRIMARY KEY,[Date] DATETIME NOT NULL,[Price] FLOAT NOT NULL,[Volume] INT NOT NULL) END");
        }
        private void OnReceiveTrData(object sender, _DKHOpenAPIEvents_OnReceiveTrDataEvent e)
        {
            /*
            if (e.sTrCode.Equals("opt50028") || e.sTrCode.Equals("opt50066"))
            {
                var temp = AxAPI.GetCommDataEx(e.sTrCode, e.sRQName);

                if (temp != null)
                {
                    string[,] ts = new string[((object[,])temp).GetUpperBound(0) + 1, ((object[,])temp).GetUpperBound(1) + 1];
                    int x, y, lx = ((object[,])temp).GetUpperBound(0), ly = ((object[,])temp).GetUpperBound(1);

                    for (x = 0; x <= lx; x++)
                    {
                        Sb = new StringBuilder(64);

                        for (y = 0; y <= ly; y++)
                        {
                            ts[x, y] = (string)((object[,])temp)[x, y];

                            if (ts[x, y].Length > 13 && e.sRQName.Substring(8).Equals(ts[x, y].Substring(2)))
                            {
                                Sb = new StringBuilder(it);
                                e.sPrevNext = "0";

                                break;
                            }
                            Sb.Append(ts[x, y]).Append(';');
                        }
                        if (!it.Equals(Sb.ToString()))
                        {
                            SendMemorize?.Invoke(this, new Memorize(Sb));

                            continue;
                        }
                        if (it.Equals(Sb.ToString()))
                            break;
                    }
                    if (e.sRQName.Substring(0, 3).Equals("101") && e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50028 { Value = e.sRQName.Substring(0, 8), RQName = e.sRQName, PrevNext = 2 })));

                        return;
                    }
                    if (e.sPrevNext.Equals("2"))
                    {
                        request.RequestTrData(new Task(() => InputValueRqData(new Opt50066 { Value = e.sRQName.Substring(0, 8), RQName = e.sRQName, PrevNext = 2 })));

                        return;
                    }
                    if (e.sPrevNext.Equals("0"))
                        SendMemorize?.Invoke(this, new Memorize(e.sPrevNext, e.sRQName.Substring(0, 8)));
                }
                Request(e.sRQName);
                return;
            }
            */
            if (e.sTrCode.Contains("KOA"))
                return;

            Sb = new StringBuilder(512);
            int i, cnt = AxAPI.GetRepeatCnt(e.sTrCode, e.sRQName);

            for (i = 0; i < (cnt > 0 ? cnt : cnt + 1); i++)
            {
                foreach (string item in Array.Find(catalog, o => o.ToString().Contains(e.sTrCode.Substring(1))))
                    Sb.Append(AxAPI.GetCommData(e.sTrCode, e.sRQName, i, item).Trim()).Append(';');

                if (cnt > 0)
                {
                    Sb.Append("*");

                    if (DeadLine && Sb.ToString().Substring(0, 3).Equals("101"))
                    {
                        string[] temp = Sb.ToString().Split(';');
                        DeadLine = false;
                    }
                }
            }
        }
        private void OnReceiveRealData(object sender, _DKHOpenAPIEvents_OnReceiveRealDataEvent e)
        {
            Sb = new StringBuilder(512);
            int index = Array.FindIndex(Enum.GetNames(typeof(IRealType.RealType)), o => o.Equals(e.sRealType));

            foreach (int fid in Type.Catalog[index])
                Sb.Append(AxAPI.GetCommRealData(e.sRealKey, fid)).Append(';');

            switch (index)
            {
                case 1:
                    if (e.sRealKey.Substring(0, 3).Equals("101"))
                    {

                    }
                    break;

                case 5:
                    string[] find = Sb.ToString().Split(';');
                    break;

                    /*
                case 7:
                    if (Sb.ToString().Substring(0, 1).Equals("e") && DeadLine == false)
                    {
                        DeadLine = true;
                        Delay.delay = 4150;

                        if (RemainingDate.Equals(DateTime.Now.ToString("yyyyMMdd")))
                        {
                            Squence = 0;
                            AxAPI.SetRealRemove("ALL", AxAPI.GetFutureCodeByIndex(1));
                            RemainingDay(AxAPI.GetFutureCodeByIndex(0));
                        }
                        Request(Code[0].Substring(0, 8));
                    }
                    break;
                    */
            }
        }
        private void OnReceiveMsg(object sender, _DKHOpenAPIEvents_OnReceiveMsgEvent e)
        {

        }
        private int ErrorCode
        {
            get; set;
        }
        private bool DeadLine
        {
            get; set;
        }
        private IRealType Type
        {
            get; set;
        }
        private SqlConnection Sql
        {
            get; set;
        }
        private AxKHOpenAPI AxAPI
        {
            get; set;
        }
        private StringBuilder Sb
        {
            get; set;
        }
        private ConnectAPI()
        {
            request = Delay.GetInstance(3605);
            request.Run();
        }
        private static ConnectAPI Api
        {
            get; set;
        }
        private readonly Delay request;
    }
}