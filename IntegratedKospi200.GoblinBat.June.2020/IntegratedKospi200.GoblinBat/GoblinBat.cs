using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Message;

namespace ShareInvest.Kospi200
{
    public partial class GoblinBat : Form
    {
        public GoblinBat()
        {
            InitializeComponent();
            SuspendLayout();
            ChooseStrategy(TimerBox.Show(new Message().ChooseStrategy, "Option", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 15325), new Yield(new Assets().ReadCSV().Split(',')), new SelectStatisticalData());
            Application.DoEvents();
            SetVisibleCore(false);
            ShowDialog();
            Dispose();
            Environment.Exit(0);
        }
        private void ChooseStrategy(DialogResult result, Yield yield, SelectStatisticalData data)
        {
            splitContainerStrategy.Panel1.Controls.Add(yield);
            splitContainerStrategy.Panel2.Controls.Add(data);
            splitContainerGuide.Panel1.Controls.Add(guide);
            yield.Dock = DockStyle.Fill;
            data.Dock = DockStyle.Fill;
            guide.Dock = DockStyle.Fill;
            Choice = result;
            Size = new Size(1241, 491);
            splitContainerStrategy.SplitterDistance = 127;
            splitContainerStrategy.Panel1.BackColor = Color.FromArgb(121, 133, 130);
            splitContainerStrategy.Panel2.BackColor = Color.FromArgb(121, 133, 130);
            splitContainerGuide.Panel1.BackColor = Color.FromArgb(121, 133, 130);
            yield.SendHermes += data.OnReceiveHermes;
            data.SendStrategy += yield.OnReceiveStrategy;
            data.SendClose += OnReceiveClose;
            Dictionary<string, int> param = new Dictionary<string, int>(1024);

            foreach (string[] temp in yield)
                for (int i = 0; i < temp.Length; i++)
                    param[string.Concat(i, ';', temp[i])] = i;

            data.StartProgress(param);
            SetControlsChangeFont(result, Controls, new Font("Consolas", Font.Size + 0.75F, FontStyle.Regular));
            ResumeLayout();
            Show();
            CenterToScreen();
            Application.DoEvents();
            data.GetStrategy(yield.SetStrategy(TimerBox.Show("Click 'No' to Edit the Automatically generated Strategy.\n\nIf No Selection is made for 20 Seconds,\nTrading Starts with an Automatically Generated Strategy.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, 21753)));
        }
    }
}