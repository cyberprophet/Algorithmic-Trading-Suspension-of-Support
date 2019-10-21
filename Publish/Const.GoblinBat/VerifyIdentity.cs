using System.Collections.Generic;
using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class VerifyIdentity : IConfirm
    {
        public string Confirm
        {
            get; private set;
        }
        public bool Identify(string id, string name)
        {
            Confirm = string.Concat(id.Substring(0, 1).ToUpper(), id.Substring(1));

            return unique.ContainsKey(id) && unique.ContainsValue(name);
        }
        private readonly Dictionary<string, string> unique = new Dictionary<string, string>()
        {

        };
    }
}