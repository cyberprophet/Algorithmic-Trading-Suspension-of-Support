using System.Collections.Generic;
using ShareInvest.GoblinBatContext;
using ShareInvest.Catalog;

namespace ShareInvest
{
    internal partial class Retrieve : CallUpGoblinBat
    {
        private Retrieve(char initial, string code) : base(initial)
        {
            Chart = GetChart(code);
        }
        internal Queue<Chart> Chart
        {
            get;
        }
        internal static void Dispose()
        {
            retrieve = null;
        }
        internal static Retrieve GetInstance(char initial, string code)
        {
            if (retrieve == null)
            {
                Code = code;
                retrieve = new Retrieve(initial, code);
            }
            else if (Code.Equals(code) == false)
            {
                Code = code;
                retrieve = new Retrieve(initial, code);
            }
            return retrieve;
        }
        private static string Code
        {
            get; set;
        }
        private static Retrieve retrieve;
    }
}