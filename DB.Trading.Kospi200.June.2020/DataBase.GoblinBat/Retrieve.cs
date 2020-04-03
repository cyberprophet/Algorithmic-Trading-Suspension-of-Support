using System.Collections.Generic;
using ShareInvest.GoblinBatContext;
using ShareInvest.Catalog;

namespace ShareInvest
{
    internal partial class Retrieve : CallUpGoblinBat
    {
        internal Queue<Chart> Chart
        {
            get;
        }
        internal static void Dispose() => retrieve = null;
        internal static Retrieve GetInstance(string key, string code)
        {
            if (retrieve == null)
            {
                Code = code;
                retrieve = new Retrieve(key, code);
            }
            else if (Code.Equals(code) == false)
            {
                Code = code;
                retrieve = new Retrieve(key, code);
            }
            return retrieve;
        }
        static string Code
        {
            get; set;
        }
        Retrieve(string key, string code) : base(key) => Chart = GetChart(code);
        static Retrieve retrieve;
    }
}