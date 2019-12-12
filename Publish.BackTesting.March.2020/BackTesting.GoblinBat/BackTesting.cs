using System;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.AssetManagement;
using ShareInvest.BackTesting.SettingsScreen;
using ShareInvest.Communication;

namespace ShareInvest.BackTesting
{
    public partial class BackTesting : Form
    {
        public BackTesting(int count)
        {
            InitializeComponent();
            StartingPoint(TimerBox.Show("The Default Font is\n\n'Brush Script Std'.\n\n\nClick 'Yes' to Change to\n\n'Consolas'.", "Option", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2, 15325), new GoblinBatScreen(count, new Asset()), new Progress());
            Dispose();
            Environment.Exit(0);
        }
        private void StartingPoint(DialogResult result, GoblinBatScreen gs, Progress pro)
        {
            splitContainer.Panel1.Controls.Add(gs);
            splitContainer.Panel2.Controls.Add(pro);
            Width = gs.Width + 4;
            Height = gs.Height + pro.Height + 1;
            SuspendLayout();
            pro.Width = gs.Width;
            splitContainer.SplitterDistance = gs.Height;
            CenterToScreen();
            gs.SetProgress(pro);
            SetControlsChangeFont(result, Controls, new Font("Consolas", Font.Size + 1.75F, FontStyle.Bold));
            ResumeLayout();
            ShowDialog();
        }
        private void SetControlsChangeFont(DialogResult result, Control.ControlCollection controls, Font font)
        {
            if (result.Equals(DialogResult.OK))
                foreach (Control control in controls)
                {
                    if (control.Name.Contains("label") || control.Name.Equals("button") || control.Name.Equals("checkBox"))
                        control.Font = control.Text.Contains("by Day") ? new Font("Consolas", Font.Size - 1.25F, FontStyle.Bold) : font;

                    if (control.Controls.Count > 0)
                        SetControlsChangeFont(DialogResult.OK, control.Controls, font);
                }
        }
    }
}