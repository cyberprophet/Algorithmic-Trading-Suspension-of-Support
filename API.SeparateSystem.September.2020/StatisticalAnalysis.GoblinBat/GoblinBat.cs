using System;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Strategics.Controls;

namespace ShareInvest.Strategics
{
    public sealed partial class GoblinBat : Form
    {
        public GoblinBat(dynamic cookie)
        {
            Privacy = new Privacies { Security = cookie };
            InitializeComponent();
            OnReceiveItem(st);
            strip.ItemClicked += OnItemClick;
        }
        void OnReceiveItem(string item)
        {
            if (item.Equals(st))
            {
                if (Statistical == null)
                {
                    Statistical = new StatisticalControl();
                    Controls.Add(Statistical);
                    Statistical.Dock = DockStyle.Fill;
                }
                Size = new Size(1350, 255);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                Statistical.Show();
                WindowState = FormWindowState.Normal;
                timer.Stop();
                CenterToScreen();
            }
            else
                Close();

            OnClickMinimized = item;
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                if (OnClickMinimized.Equals(st))
                {
                    Statistical.Hide();
                    timer.Start();
                }
                Opacity = 0.8135;
                BackColor = Color.FromArgb(121, 133, 130);
                Visible = false;
                ShowIcon = false;
                notifyIcon.Visible = true;
            }
        }
        void GoblinBatFormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show(rExit, notifyIcon.Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning).Equals(DialogResult.Cancel))
            {
                e.Cancel = true;
                WindowState = FormWindowState.Minimized;

                return;
            }
            ClosingForm = true;
            strip.ItemClicked -= OnItemClick;
            Dispose();
        }
        void TimerTick(object sender, EventArgs e)
        {
            notifyIcon.Icon = (Icon)resources.GetObject(Change ? upload : download);
            Change = !Change;
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e) => OnReceiveItem(e.ClickedItem.Name);
        bool Change
        {
            get; set;
        }
        bool ClosingForm
        {
            get; set;
        }
        string OnClickMinimized
        {
            get; set;
        }
        Privacies Privacy
        {
            get; set;
        }
        StatisticalControl Statistical
        {
            get; set;
        }
    }
}