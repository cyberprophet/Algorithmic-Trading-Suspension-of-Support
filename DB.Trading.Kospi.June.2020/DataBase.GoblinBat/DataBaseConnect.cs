namespace ShareInvest.Connect.DataBase
{
    public class DataBaseConnect : Secret
    {
        public string GetConnectString()
        {
            return ConnectString;
        }
    }
}