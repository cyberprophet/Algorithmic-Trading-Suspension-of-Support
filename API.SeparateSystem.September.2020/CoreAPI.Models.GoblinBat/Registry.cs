using System.Collections.Generic;

namespace ShareInvest.Models
{
    public static class Registry
    {
        public static Dictionary<string, string> Retentions = new Dictionary<string, string>();
        public static Dictionary<string, Stack<Charts>> Catalog = new Dictionary<string, Stack<Charts>>();
    }
}