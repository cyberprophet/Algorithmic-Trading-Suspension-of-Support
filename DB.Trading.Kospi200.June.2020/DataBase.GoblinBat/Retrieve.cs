using System.Collections.Generic;
using System.IO;

using ShareInvest.Catalog;
using ShareInvest.GoblinBatContext;

namespace ShareInvest
{
    sealed partial class Retrieve : CallUpGoblinBat
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
        Retrieve(string key, string code) : base(key) => Chart = GetChart(code, new Queue<Chart>(1048576), new DirectoryInfo(Path));
        static Retrieve retrieve;
    }
}