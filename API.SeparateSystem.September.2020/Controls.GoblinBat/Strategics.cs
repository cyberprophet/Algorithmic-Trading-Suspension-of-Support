using System.Windows.Forms;

using ShareInvest.Catalog;

namespace ShareInvest.Controls
{
    public partial class Strategics : UserControl
    {
        Privacies Privacy
        {
            get; set;
        }
        public Strategics()
        {
            InitializeComponent();
        }
        public void SetPrivacy(Privacies privacy)
        {
            if (string.IsNullOrEmpty(privacy.Account) == false)
                switch (privacy.Account)
                {
                    case "F":
                        label.Text = "선 물 옵 션";
                        break;

                    case "S":
                        label.Text = "위 탁 종 합";
                        break;
                }
            Privacy = privacy;
        }
    }
}