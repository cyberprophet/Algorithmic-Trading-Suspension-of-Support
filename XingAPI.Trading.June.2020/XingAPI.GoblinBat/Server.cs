namespace ShareInvest.XingAPI
{
    internal class Server
    {
        internal string GetSelectServer(string server)
        {
            if (server.Equals("1"))
                return mock;

            return real;
        }
        private const string real = "hts.ebestsec.co.kr";
        private const string mock = "demo.ebestsec.co.kr";
    }
}