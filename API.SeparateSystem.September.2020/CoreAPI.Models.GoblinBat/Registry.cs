using System.Collections.Generic;

namespace ShareInvest.Models
{
    public static class Registry
    {
        public static Dictionary<string, string> Retentions = new Dictionary<string, string>();
        public static Dictionary<string, Stack<Charts>> Catalog = new Dictionary<string, Stack<Charts>>();
        public static void RemoveRetentions()
        {
            var temp = new Dictionary<string, string>();

            foreach (var kv in Retentions)
                if (string.IsNullOrEmpty(kv.Key) == false && kv.Value != null)
                    temp[kv.Key] = kv.Value;

            Retentions.Clear();
            Retentions = temp;
        }
    }
}