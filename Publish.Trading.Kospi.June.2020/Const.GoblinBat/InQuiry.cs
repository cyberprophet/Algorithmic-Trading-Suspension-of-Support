using ShareInvest.Interface;

namespace ShareInvest.Const
{
    public class InQuiry : IAccount
    {
        public string AccNo
        {
            get; set;
        }
        public long BasicAssets
        {
            get; set;
        }
    }
}