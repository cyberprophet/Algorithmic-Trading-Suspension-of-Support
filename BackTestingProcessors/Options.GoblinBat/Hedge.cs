using ShareInvest.Communication;

namespace ShareInvest.Options
{
    public class Hedge
    {
        public Hedge(IStrategy strategy)
        {
            this.strategy = strategy;
        }
        private readonly IStrategy strategy;
    }
}