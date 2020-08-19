using System.Windows.Forms;

using ShareInvest.Catalog;

namespace ShareInvest.Controls
{
    partial class ScenarioAccordingToTrend : UserControl
    {
        internal ScenarioAccordingToTrend(Codes codes)
        {
            InitializeComponent();
            labelName.Text = codes.Name;
        }
    }
}