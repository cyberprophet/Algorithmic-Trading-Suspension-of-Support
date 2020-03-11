using System;
using System.Collections.Generic;

namespace ShareInvest.Struct
{
    public struct JIF : IBlock
    {
        public int Occurs
        {
            get; private set;
        }
        public string Name
        {
            get; private set;
        }
        public string Field
        {
            get; private set;
        }
        public string Property
        {
            get; private set;
        }
        public Queue<IBlock> Inquiry
        {
            get; private set;
        }
        public Queue<IBlock> GetInBlock(string res)
        {
            Inquiry = new Queue<IBlock>();
            Inquiry.Enqueue(new JIF
            {
                Name = "InBlock",
                Field = "jangubun",
                Property = ((int)(DateTime.Now.Hour < 16 ? Attribute.선물옵션 : Attribute.CME야간선물)).ToString()
            });
            return Inquiry;
        }
        public Queue<IBlock> GetOutBlock(string res)
        {
            Inquiry = new Queue<IBlock>();

            foreach (var property in Enum.GetNames(typeof(FieldProperty)))
                Inquiry.Enqueue(new JIF
                {
                    Name = "OutBlock",
                    Field = property
                });
            return Inquiry;
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
        private enum FieldProperty
        {
            jangubun = 1,
            jstatus = 2
        }
    }
}