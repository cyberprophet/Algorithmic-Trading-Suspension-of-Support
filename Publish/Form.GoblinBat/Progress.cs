using System.Windows.Forms;
using ShareInvest.EventHandler;

namespace ShareInvest.Control
{
    public partial class Progress : UserControl
    {
        public Progress()
        {
            InitializeComponent();
        }
        public void Rate(object sender, ProgressRate pr)
        {
            if (progressBar.Maximum != pr.Result)
                progressBar.Maximum = pr.Result;

            progressBar.Value += 1;

            Application.DoEvents();
        }
    }
}