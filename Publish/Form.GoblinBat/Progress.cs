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
            if (pr.Result.IsCompleted)
            {
                if (progressBar.Value > 1000)
                    progressBar.Value = 0;

                progressBar.Value += 1;
            }
            Application.DoEvents();
        }
    }
}