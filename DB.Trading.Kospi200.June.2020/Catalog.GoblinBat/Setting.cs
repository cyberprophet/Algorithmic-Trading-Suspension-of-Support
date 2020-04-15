using System.Windows.Forms;

namespace ShareInvest.Catalog
{
    public struct Setting
    {
        public ulong Assets
        {
            get; set;
        }
        public string Strategy
        {
            get; set;
        }
        public double Commission
        {
            get; set;
        }
        public CheckState RollOver
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
    }
}