using System;
using System.Drawing;
using System.Windows.Forms;

using ShareInvest.Catalog;
using ShareInvest.Client;

namespace ShareInvest.Strategics
{
    public sealed partial class GoblinBat : Form
    {
        public GoblinBat(dynamic cookie)
        {
            InitializeComponent();
            strip.ItemClicked += OnItemClick;
            StartProgress(new Privacies { Security = cookie });
        }
        void StartProgress(IParameters param)
        {
            switch (GoblinBatClient.PostContext<Privacies>(param))
            {
                case 0xCA:
                    if (Statistical == null)
                    {
                        Statistical = new Controls.Strategics();
                        Controls.Add(Statistical);
                        Statistical.Dock = DockStyle.Fill;
                    }
                    Result = DialogResult.OK;
                    break;

                case 0xC8:
                    Result = MessageBox.Show("", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    break;

                default:
                    Result = DialogResult.Cancel;
                    break;
            }
            if (Result.Equals(DialogResult.OK) && IsApplicationAlreadyRunning(param.Security))
            {
                Privacy = new Privacies
                {
                    Security = param.Security
                };
                Opacity = 0;
                timer.Start();
            }
            else if (Result.Equals(DialogResult.Cancel))
            {
                strip.ItemClicked -= OnItemClick;
                Dispose();
            }
        }
        void GoblinBatResize(object sender, EventArgs e)
        {
            if (WindowState.Equals(FormWindowState.Minimized))
            {
                if (string.IsNullOrEmpty(OnClickMinimized) == false && OnClickMinimized.Equals(st))
                {
                    Statistical.Hide();
                    timer.Start();
                }
                Opacity = 0.8135;
                BackColor = Color.FromArgb(0x79, 0x85, 0x82);
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
            if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false && Result.Equals(DialogResult.OK))
            {
                FormBorderStyle = FormBorderStyle.FixedSingle;
                WindowState = FormWindowState.Minimized;
            }
            else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
            {
                notifyIcon.Icon = (Icon)resources.GetObject(Change ? upload : download);
                Change = !Change;

                if (IsApplicationAlreadyRunning(Privacy.Security))
                {

                }
            }
        }
        void OnItemClick(object sender, ToolStripItemClickedEventArgs e) => BeginInvoke(new Action(() =>
        {
            if (e.ClickedItem.Name.Equals(st))
            {
                if (Statistical == null)
                {
                    Statistical = new Controls.Strategics();
                    Controls.Add(Statistical);
                    Statistical.Dock = DockStyle.Fill;
                }
                if (GoblinBatClient.GetContext<Privacies>(Privacy) is Privacies privacy)
                    Statistical.SetPrivacy(privacy);

                Size = new Size(1350, 720);
                Visible = true;
                ShowIcon = true;
                notifyIcon.Visible = false;
                Statistical.Show();
                WindowState = FormWindowState.Normal;
                timer.Stop();
            }
            else
                Close();

            OnClickMinimized = e.ClickedItem.Name;
        }));
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
        DialogResult Result
        {
            get; set;
        }
        Controls.Strategics Statistical
        {
            get; set;
        }
        Privacies Privacy
        {
            get; set;
        }
    }
}