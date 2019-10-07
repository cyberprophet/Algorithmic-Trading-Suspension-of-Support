using System.Windows.Forms;
using ShareInvest.Control;

namespace ShareInvest
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();

            kospi200 = new Kospi200();
            kospi200.Show();
        }
        private readonly Kospi200 kospi200;
    }
}