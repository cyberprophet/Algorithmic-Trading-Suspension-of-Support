using System.Collections.Generic;
using ShareInvest.GoblinBatContext;
using ShareInvest.Interface.Struct;

namespace ShareInvest.Strategy
{
    internal partial class Retrieve : CallUpGoblinBat
    {
        private Retrieve(string code)
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
        internal static Retrieve GetInstance(string code)
        {
            if (retrieve == null)
            {
                Code = code;
                retrieve = new Retrieve(code);
            }
            else if (Code.Equals(code) == false)
            {
                Code = code;
                retrieve = new Retrieve(code);
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