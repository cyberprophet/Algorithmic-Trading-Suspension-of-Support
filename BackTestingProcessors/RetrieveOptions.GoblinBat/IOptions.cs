using System.Collections.Generic;

namespace ShareInvest.RetrieveOptions
{
    public interface IOptions
    {
        Dictionary<string, Dictionary<string, Dictionary<ulong, double>>> Repository
        {
            get;
        }
    }
}