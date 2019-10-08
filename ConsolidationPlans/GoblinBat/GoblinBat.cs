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
            if (Kospi200F.Checked)
            {
                button.Text = "Progressing...";
                button.ForeColor = Color.DimGray;
                button.Font = new Font("Brush Script Std", 11.2F, FontStyle.Italic);

                new Kospi200();
            }
            SetVisibleCore(false);
        }
    }
}