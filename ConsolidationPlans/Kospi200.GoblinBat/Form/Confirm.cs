using System.Windows.Forms;

namespace ShareInvest.Identify
{
    public partial class Confirm : Form
    {
        public static Confirm Get()
        {
            if (cf == null)
                cf = new Confirm();

            return cf;
        }
        public bool CheckCurrent()
        {
            return checkBox.Checked;
        }
        private void OnReceiveIdentify(object sender, EventHandler.Identify e)
        {
            checkBox.Text = e.Confirm;
        }
        private Confirm()
        {
            InitializeComponent();

            Futures.Get().SendConfirm += OnReceiveIdentify;
        }
        private static Confirm cf;
    }
}