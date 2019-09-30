using System;
using System.Windows.Forms;
using ShareInvest.AutoMessageBox;
using ShareInvest.BackTest;
using ShareInvest.DataMining;
using ShareInvest.Strategy;

namespace Start
{
    public partial class Form1 : Form
    {
        private readonly Scalping s;
        private readonly DayAnalysis d;
        private readonly Tick t;

        public Form1()
        {
            InitializeComponent();

            int i, l = 15;

            for (i = 13; i < l; i++)
            {
                s = new Scalping();
                d = new DayAnalysis();
                t = new Tick();

                s.SendDay += d.Analysis;
                s.SendTick += t.Analysis;

                s.StartProgress(i);
            }
            new Storage();
            Box.Show("Complete...!!", "Notice", 3500);
            Environment.Exit(0);
        }
    }
}