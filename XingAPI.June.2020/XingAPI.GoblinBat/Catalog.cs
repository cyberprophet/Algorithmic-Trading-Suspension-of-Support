using System.Collections.Generic;
using ShareInvest.Struct;

namespace ShareInvest.XingAPI
{
    public partial class Catalog
    {
        protected readonly IBlock[] catalog =
        {
            new T9943(),
            new CFOBQ10500(),
            new T0441(),
            new T2105()
        };
        protected readonly Dictionary<string, IBlock> real = new Dictionary<string, IBlock>()
        {
            { "FC0.res", new FC0() },
            { "FH0.res", new FH0() },
            { "JIF.res", new JIF() }
        };
    }
}