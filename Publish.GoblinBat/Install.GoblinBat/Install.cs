using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.C2010;
using ShareInvest.C2012;
using ShareInvest.Control;
using ShareInvest.EventHandler;
using ShareInvest.Guide;
using ShareInvest.NetFramework;
using ShareInvest.OpenAPI;

namespace ShareInvest.Install
{
    public partial class Install : Form
    {
        public Install()
        {
            InitializeComponent();
            button2010.Click += ButtonClick;
            button2012.Click += ButtonClick;
            buttonDOTNETFramework.Click += ButtonClick;
            buttonGoblinBat.Click += ButtonClick;
            buttonOpenAPI.Click += ButtonClick;
            MessageBox.Show("The GoblinBat Works Correctly when\nAll Elements are Installed.\n\nClick on a Program\nthat is Not Installed.\n\nThe GoblinBat must be Installed Last.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void ButtonClick(object sender, EventArgs e)
        {
            if (sender.Equals(buttonOpenAPI))
            {
                new Setup().StartProgress();
                buttonOpenAPI.ForeColor = Color.Maroon;

                return;
            }
            if (sender.Equals(button2010))
            {
                new MSVC2010().StartProgress();
                button2010.ForeColor = Color.Maroon;

                return;
            }
            if (sender.Equals(button2012))
            {
                new MSVC2012().StartProgress();
                button2012.ForeColor = Color.Maroon;

                return;
            }
            if (sender.Equals(buttonDOTNETFramework))
            {
                new Net().StartProgress();
                buttonDOTNETFramework.ForeColor = Color.Maroon;

                return;
            }
            if (sender.Equals(buttonGoblinBat))
            {
                SuspendLayout();
                tableLayoutPanel.Hide();
                GetTermsAndConditions();
                WindowState = FormWindowState.Minimized;
                Controls.Clear();

                return;
            }
        }
        private void GetControls(GoblinBat gb)
        {
            Controls.Clear();
            Controls.Add(gb);
            gb.Dock = DockStyle.Fill;
            WindowState = FormWindowState.Maximized;
            Opacity = 0.95;
            ResumeLayout();

            if (DialogResult.OK.Equals(MessageBox.Show("The Installation is Complete.\n\nPlease Register Your OpenAPI.", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)))
            {
                FormBorderStyle = FormBorderStyle.Fixed3D;
                Show();
            }
            else
            {
                Close();
                Dispose();
                Application.Exit();
            }
        }
        private void GetTermsAndConditions()
        {
            using TermsConditions tc = new TermsConditions();
            Controls.Add(tc);
            tc.Dock = DockStyle.Fill;
            Size = tc.Size;
            StartPosition = FormStartPosition.CenterScreen;
            tc.SendQuit += OnReceiveDialogClose;
            SetVisibleCore(false);
            ResumeLayout();
            ShowDialog();
            tc.SendQuit -= OnReceiveDialogClose;
        }
        private void OnReceiveDialogClose(object sender, ForceQuit e)
        {
            try
            {
                SuspendLayout();
                GetControls(new GoblinBat());
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Restart();
            }
        }
    }
}