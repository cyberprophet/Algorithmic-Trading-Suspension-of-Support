using System;
using System.Windows.Forms;
using ShareInvest.Analysis;
using ShareInvest.AutoMessageBox;
using ShareInvest.BackTest;
using ShareInvest.EventHandler;

namespace ShareInvest.GoblinBat
{
    public partial class GoblinBat : Form
    {
        private readonly Futures api;
        private readonly DialogResult dr;

        public GoblinBat()
        {
            InitializeComponent();

            dr = Choose.Show("Please Select the Button You Want to Proceed.", "Choose", "Invest", "BackTest", "Exit");

            if (dr == DialogResult.Yes)
            {
                api = Futures.Get();

                new Statistics();
                new Temporary();

                timer.Start();
                api.SetAPI(axAPI);
                api.StartProgress();
                api.SendExit += OnReceiveExit;
            }
            else if (dr == DialogResult.No)
            {
                axAPI.Dispose();
                timer.Dispose();

                int i, l = 100;

                for (i = 11; i < l; i++)
                    new Statistics(i);

                new Storage();

                Box.Show("Complete...!!", "Notice", 3750);

                Dispose();

                Environment.Exit(0);
            }
            else
            {
                Dispose();

                Environment.Exit(0);
            }
        }
        private void GoblinBat_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();

            Environment.Exit(0);
        }
        private void OnReceiveExit(object sender, ForceQuit e)
        {
            if (e.Quit == 1)
            {
                Dispose();

                Environment.Exit(0);
            }
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            Opacity += 7e-6;

            if (Opacity > 2.5e-1)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}