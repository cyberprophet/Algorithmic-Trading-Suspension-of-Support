using System;
using ShareInvest.Catalog;
using ShareInvest.EventHandler;

namespace ShareInvest.XingAPI.Catalog
{
    internal class JIF : Real, IReals, IEvents<NotifyIconText>
    {
        internal JIF() : base()
        {
            Console.WriteLine(GetType().Name);
        }
        protected override void OnReceiveRealData(string szTrCode)
        {
            if (int.TryParse(GetFieldData(OutBlock, Enum.GetName(typeof(J), J.jangubun)), out int field) && int.TryParse(GetFieldData(OutBlock, Enum.GetName(typeof(J), J.jstatus)), out int choice) && (field == 5 || field == 7) && (choice == 21 || choice == 41))
                Send?.Invoke(this, new NotifyIconText((char)choice));
        }
        public void OnReceiveRealTime(string code)
        {
            if (LoadFromResFile(new Secret().GetResFileName(GetType().Name)))
            {
                SetFieldData(GetInBlock(), Enum.GetName(typeof(J), J.jangubun), ((int)Attribute.Common).ToString());
                AdviseRealData();
            }
        }
        private enum Attribute
        {
            Common = 0,
            코스피 = 1,
            코스닥 = 2,
            선물옵션 = 5,
            CME야간선물 = 7,
            EUREX야간옵션선물 = 8,
            미국주식 = 9,
            중국주식오전 = 'A',
            중국주식오후 = 'B',
            홍콩주식오전 = 'C',
            홍콩주식오후 = 'D',
            장전동시호가개시 = 11,
            장시작 = 21,
            장개시10초전 = 22,
            장개시1분전 = 23,
            장개시5분전 = 24,
            장개시10분전 = 25,
            장후동시호가개시 = 31,
            장마감 = 41,
            장마감10초전 = 42,
            장마감1분전 = 43,
            장마감5분전 = 44,
            시간외종가매매개시 = 51,
            시간외종가매매종료 = 52,
            시간외단일가매매종료 = 54,
            파생상품장종료 = 61,
            호가접수개시 = 62,
            장중동시마감 = 63
        }
        private enum J
        {
            jangubun = 0,
            jstatus = 1
        }
        public event EventHandler<NotifyIconText> Send;
    }
}