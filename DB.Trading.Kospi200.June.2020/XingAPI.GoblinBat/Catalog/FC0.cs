using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler.XingAPI;

namespace ShareInvest.XingAPI.Catalog
{
    internal class FC0 : Real, IReals, IEvents<Datum>
    {
        internal FC0() : base()
        {
            Console.WriteLine(GetType().Name);
        }
        protected override void OnReceiveRealData(string szTrCode)
        {
            string[] array = Enum.GetNames(typeof(C)), temp = new string[array.Length - 1];

            for (int i = 0; i < array.Length - 1; i++)
                temp[i] = GetFieldData(OutBlock, array[i]);

            Send?.Invoke(this, new Datum(temp[0], temp[4], string.Concat(temp[8], temp[9])));
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
        public event EventHandler<Datum> Send;
    }
}