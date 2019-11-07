using ShareInvest.Interface.x64;

namespace ShareInvest.Analysize.x64
{
    public class Strategy
    {
        public Strategy(IStrategy st)
        {
            this.st = st;
        }
        private readonly IStrategy st;
    }
}