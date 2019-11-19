using System.Collections.Generic;

namespace ShareInvest.RetrieveOptions
{
    public interface IOptions
    {
        Dictionary<string, Dictionary<string, List<OptionsRepository>>> Repository
        {
            get;
        }
    }
}