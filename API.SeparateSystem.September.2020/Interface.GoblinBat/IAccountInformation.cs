namespace ShareInvest.Interface
{
    public interface IAccountInformation
    {
        string Identity
        {
            get; set;
        }
        string Account
        {
            get; set;
        }
        string Name
        {
            get; set;
        }
        string Nick
        {
            get; set;
        }
        bool Server
        {
            get; set;
        }
        string Security
        {
            get; set;
        }
        string SecuritiesAPI
        {
            get; set;
        }
        string SecurityAPI
        {
            get; set;
        }
        string CodeStrategics
        {
            get; set;
        }
        double Commission
        {
            get; set;
        }
        string Password
        {
            get; set;
        }
        string Certificate
        {
            get; set;
        }
        string AccountNumber
        {
            get; set;
        }
        string AccountPassword
        {
            get; set;
        }
    }
}