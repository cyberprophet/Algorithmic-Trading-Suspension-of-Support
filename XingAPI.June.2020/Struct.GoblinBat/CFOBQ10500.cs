using System;
using System.Collections.Generic;

namespace ShareInvest.Struct
{
    public struct CFOBQ10500 : IBlock
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
            int i = 0;
            string name = string.Empty;
            Inquiry = new Queue<IBlock>();

            foreach (var str in res.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (str.Contains("레코드명:") && str.Contains("InBlock"))
                {
                    name = str.Replace("*", string.Empty).Replace("레코드명:", string.Empty).Trim();

                    continue;
                }
                else if (str.Contains("레코드명:") && str.Contains("OutBlock"))
                    break;

                else if (str.Contains("No,한글명,필드명,영문명,"))
                    continue;

                var temp = str.Split(',');
                Inquiry.Enqueue(new CFOBQ10500
                {
                    Name = name,
                    Field = temp[2],
                    Occurs = int.Parse(temp[7]),
                    Property = Secret.CFOBQ10500[i++]
                });
            }
            return Inquiry;
        }
        public Queue<IBlock> GetOutBlock(string res)
        {
            string name = string.Empty;
            Inquiry = new Queue<IBlock>();

            foreach (var str in res.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (str.Contains("레코드명:") && str.Contains("InBlock"))
                    continue;

                else if (str.Contains("레코드명:") && str.Contains("OutBlock"))
                {
                    name = str.Replace("*", string.Empty).Replace("레코드명:", string.Empty).Trim();

                    continue;
                }
                else if (str.Contains("No,한글명,필드명,영문명,"))
                    continue;

                var temp = str.Split(',');
                Inquiry.Enqueue(new CFOBQ10500
                {
                    Name = name,
                    Field = temp[2]
                });
            }
            return Inquiry;
        }
    }
}