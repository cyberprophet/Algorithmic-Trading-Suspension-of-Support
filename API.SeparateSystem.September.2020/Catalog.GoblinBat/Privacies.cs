using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct Privacies : IParameters, IAccountInformation
    {
        public string Security
        {
            get; set;
        }
        public string SecuritiesAPI
        {
            get; set;
        }
        public string SecurityAPI
        {
            get; set;
        }
        public string Account
        {
            get; set;
        }
        public string CodeStrategics
        {
            get; set;
        }
        public double Commission
        {
            get; set;
        }
        public double Coin
        {
            get; set;
        }
        public string Identity
        {
            get; set;
        }
        public string Password
        {
            get; set;
        }
        public string Certificate
        {
            get; set;
        }
        public string AccountNumber
        {
            get; set;
        }
        public string AccountPassword
        {
            get; set;
        }
        public string Name
        {
            get; set;
        }
        public string Nick
        {
            get; set;
        }
        public bool Server
        {
            get; set;
        }
    }
}