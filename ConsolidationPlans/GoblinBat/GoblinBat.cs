using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.Control;

namespace ShareInvest
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
        }
        private void Button_Click(object sender, EventArgs e)
        {
            ButtonEvent();

            if (Kospi200F.Checked)
                new Korea(0);

            else if (Kosdaq150F.Checked)
                new Korea(24);

            else if (Nasdaq100F.Checked)
                new World();

            else if (Gold.Checked)
                Environment.Exit(0);

            else if (CrudeOil.Checked)
                Environment.Exit(0);

            else if (Copper.Checked)
                Environment.Exit(0);

            SetVisibleCore(false);
        }
        private void ButtonEvent()
        {
            button.Text = "Progressing...";
            button.ForeColor = Color.DimGray;
            button.Font = new Font("Brush Script Std", 11.2F, FontStyle.Italic);
        }
    }
}