using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;
using ShareInvest.EventHandler.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    internal class NC0 : Real, IReals, IEvents<Datum>, ITrends<Trends>
    {
        protected override void OnReceiveRealData(string szTrCode)
        {
            string[] array = Enum.GetNames(typeof(C)), temp = new string[array.Length];

            for (int i = 0; i < array.Length; i++)
                temp[i] = GetFieldData(OutBlock, array[i]);

            Send?.Invoke(this, new Datum(temp[0], temp[4]));

            if (int.TryParse(string.Concat(temp[8], temp[9]), out int volume))
            {
                API.Volume += volume;
                SendTrend?.Invoke(this, new Trends(API.Trend, API.Volume));
            }
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
            {
                var param = GetInBlock(code);
                SetFieldData(param.Block, param.Field, param.Data);
                AdviseRealData();
            }
        }
        internal NC0() : base() => Console.WriteLine(GetType().Name);
        public event EventHandler<Datum> Send;
        public event EventHandler<Trends> SendTrend;
    }
}