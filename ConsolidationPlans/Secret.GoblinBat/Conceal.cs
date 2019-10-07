using System.Collections.Generic;
using System.Text;

namespace ShareInvest.Secret
{
    public class Conceal
    {
        private readonly Dictionary<string, string> unique = new Dictionary<string, string>()
        {
            { "jhy7264", "주식회사공유인베" },
            { "share9", "주식회사공유인베" },
            { "prophet8", "전혜영" },
            { "prophet9", "박상우" }
        };
        protected readonly string[] unique_account =
        {
            "8744695931",
            "8744158731",
            "8744131431",
            "8743861631",
            "8744716231",
            "8744725631",
            "8744760731",
            "8744791131",
            "8744791231",
            "8744791331"
        };
        protected bool Identify(string id, string name)
        {
            return unique.ContainsKey(id) && unique.ContainsValue(name);
        }
        protected StringBuilder sb;
        protected string account;
        protected int screen;
        protected const string it = "Information that already Exists";
        protected const int waiting = 3500;
        protected const int delay = 205;
        protected const int end = 1;
        protected const int tm = 250000;
        protected const int basicAsset = 35000000;
        protected const int secret = 67;
        protected const double commission = 3e-5;
        protected const double margin = 7.5e-2;
    }
}