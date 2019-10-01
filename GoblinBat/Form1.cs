using System;
using System.Windows.Forms;
using ShareInvest.Analysis;
using ShareInvest.EventHandler;

namespace ShareInvest.GoblinBat
{
    public partial class GoblinBat : Form
    {
        private readonly Futures api;

        public GoblinBat()
        {
            InitializeComponent();

            api = Futures.Get();

            new Statistics();
            new Temporary();

            timer.Start();
            api.SetAPI(axAPI);
            api.StartProgress();
            api.SendExit += OnReceiveExit;
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
            Opacity += 7e-5;

            if (Opacity > 3.5e-1)
            {
                timer.Stop();
                timer.Dispose();
            }
        }
    }
}