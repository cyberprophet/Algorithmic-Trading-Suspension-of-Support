using System;
using System.Collections.Generic;

namespace ShareInvest.Struct
{
    public struct FH0 : IBlock
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
            Inquiry.Enqueue(new FH0
            {
                Name = "InBlock",
                Field = "futcode",
                Property = Secret.Code
            });
            return Inquiry;
        }
        public Queue<IBlock> GetOutBlock(string res) => throw new NotImplementedException();
    }
}