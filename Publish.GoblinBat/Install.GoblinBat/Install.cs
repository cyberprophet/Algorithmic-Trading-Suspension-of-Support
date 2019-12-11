using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using IWshRuntimeLibrary;
using ShareInvest.C2010;
using ShareInvest.C2012;
using ShareInvest.Control;
using ShareInvest.EventHandler;
using ShareInvest.Guide;
using ShareInvest.NetFramework;
using ShareInvest.OpenAPI;
using ShareInvest.TimerBoxs;

namespace ShareInvest.Install
{
    public partial class Install : Form
    {
        public Install()
        {
            InitializeComponent();
            buttonDOTNETFramework.Text = "Hardware Monitor";
            button2010.Click += ButtonClick;
            button2012.Click += ButtonClick;
            buttonDOTNETFramework.Click += ButtonClick;
            buttonGoblinBat.Click += ButtonClick;
            buttonOpenAPI.Click += ButtonClick;
            SuspendLayout();

            if (TimerBox.Show("The GoblinBat Works Correctly when\nAll Elements are Installed.\n\nClick on a Program\nthat is Not Installed.\n\nThe GoblinBat must be Installed Last.\n\nThe Default Font is\n'Brush Script Std'.\n\nClick 'Yes' to Change to\n'Consolas'.", "Notice", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2, 15732).Equals(DialogResult.Yes))
                SetControlsChangeFont(Controls, new Font("Consolas", Font.Size + 3.5F, FontStyle.Regular));

            ResumeLayout();
        }
        private void SetControlsChangeFont(System.Windows.Forms.Control.ControlCollection controls, Font font)
        {
            foreach (System.Windows.Forms.Control control in controls)
            {
                control.Font = font;

                if (control.Controls.Count > 0)
                    SetControlsChangeFont(control.Controls, font);
            }
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
            }
            Process.Start("shutdown.exe", "-r");
            Dispose();
            Application.ExitThread();
            Application.Exit();
        }
        private void GetControls(GoblinBat gb)
        {
            Controls.Clear();
            Controls.Add(gb);
            gb.Dock = DockStyle.Fill;
            WindowState = FormWindowState.Maximized;
            Opacity = 0.95;
            ResumeLayout();
            Show();
            gb.SetRoute(new WshShell());

            if (DialogResult.OK.Equals(MessageBox.Show("The Installation is Complete.\n\nPlease Register Your OpenAPI.", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1)))
                FormBorderStyle = FormBorderStyle.Fixed3D;

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
            Process.Start("Font.exe");
            ShowDialog();
            tc.SendQuit -= OnReceiveDialogClose;
        }
        private void OnReceiveDialogClose(object sender, ForceQuit e)
        {
            try
            {
                SuspendLayout();
                GetControls(new GoblinBat());
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Concat(ex.ToString(), "\n\nQuit the Program."), "Exception", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Application.Restart();
            }
        }
    }
}