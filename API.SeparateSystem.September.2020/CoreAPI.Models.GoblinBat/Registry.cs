using System;
using System.Collections.Generic;

namespace ShareInvest.Models
{
    public static class Registry
    {
        public static Dictionary<string, string> Retentions = new Dictionary<string, string>();
        public static Dictionary<string, Tuple<string, string>> CodesDictionary = new Dictionary<string, Tuple<string, string>>();
        public static Queue<string> Codes = new Queue<string>();
    }
}