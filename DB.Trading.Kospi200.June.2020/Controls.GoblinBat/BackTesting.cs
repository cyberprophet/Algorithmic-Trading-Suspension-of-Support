using System.Windows.Forms;
using ShareInvest.Analysis;

namespace ShareInvest.GoblinBatControls
{
    public partial class BackTesting : UserControl
    {
        public BackTesting()
        {
            new Analysize();
            InitializeComponent();
        }
    }
}