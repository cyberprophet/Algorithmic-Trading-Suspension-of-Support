using System.Collections;
using ShareInvest.CallUpDataBase;

namespace ShareInvest.Analysis
{
    public class Retrieve : CallUpGoblinBat
    {
        private Retrieve(string code)
        {
            Chart = GetChart(code);
        }
        public IList Chart
        {
            get;
        }
        public static Retrieve GetInstance(string code)
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