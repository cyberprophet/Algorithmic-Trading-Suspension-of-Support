using System.Windows.Forms;

namespace ShareInvest.Catalog.DataBase
{
    public struct Logs
    {
        public string Code
        {
            get; set;
        }
        public long Assets
        {
            get; set;
        }
        public double Commission
        {
            get; set;
        }
        public string Strategy
        {
            get; set;
        }
        public CheckState RollOver
        {
            get; set;
        }
    }
}