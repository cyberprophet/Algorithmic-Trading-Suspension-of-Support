using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using ShareInvest.EventHandler;
using ShareInvest.FindByName;
using ShareInvest.Interface;
using ShareInvest.TimerMessageBox;

namespace ShareInvest.StatisticalData
{
    public partial class SelectStatisticalData : UserControl
    {
        private void ButtonClick(object sender, EventArgs e)
        {
            if (sender is Button start && button.Text.Equals("Start Trading") && !button.ForeColor.Equals(Color.Maroon))
            {
                var temp = Enum.GetValues(typeof(IRecall.NumericRecall));
                int[] strategy = new int[temp.Length];

                foreach (var name in temp)
                    strategy[(int)name] = (int)name.ToString().FindByName<NumericUpDown>(this).Value;

                start.ForeColor = Color.Maroon;
                start.Text = GetCurrentStatus(button.ForeColor);
                SendClose?.Invoke(this, new DialogClose(strategy));
            }
        }
        private void SetAlignCenter(object sender, DrawItemEventArgs e)
        {
            if (sender is ComboBox cbx && e.Index >= 0)
            {
                StringFormat sf = new StringFormat
                {
                    LineAlignment = StringAlignment.Center,
                    Alignment = StringAlignment.Center
                };
                Brush brush = new SolidBrush(cbx.ForeColor);

                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                    brush = SystemBrushes.HighlightText;

                e.DrawBackground();
                e.DrawFocusRectangle();
                e.Graphics.DrawString(cbx.Items[e.Index].ToString(), new Font("Consolas", Font.Size - 0.75F, FontStyle.Bold), brush, e.Bounds, sf);
            }
        }
        private void OccursChange(object sender, EventArgs e)
        {
            if (sender is NumericUpDown numeric)
            {
                numeric.Name.Replace("numeric", "label").FindByName<Label>(this).ForeColor = numeric.Value > 0 ? Color.Snow : Color.Gold;
                checkBox.CheckState = CheckState.Unchecked;
            }
        }
        private void SetSelecetedValue(object sender, EventArgs e)
        {
            if (sender is ComboBox cb)
            {
                cb.Name.Replace("comboBox", "numeric").FindByName<NumericUpDown>(this).Value = int.Parse(cb.Text);
                checkBox.CheckState = CheckState.Unchecked;
            }
        }
        private void GetCheckPosture(object sender, EventArgs e)
        {
            string[] temp = Enum.GetNames(typeof(IRecall.NumericRecall));

            if (sender is CheckBox cb && cb.CheckState.Equals(CheckState.Checked))
            {
                StringBuilder sb = new StringBuilder(8);

                for (int i = 0; i < temp.Length; i++)
                {
                    sb.Append(temp[i].FindByName<NumericUpDown>(this).Value);

                    if (i < temp.Length - 1)
                        sb.Append('^');
                }
                cb.Text = "Reset";
                cb.ForeColor = Color.Gold;
                button.Text = GetCurrentStatus(button.ForeColor);
                SendStrategy?.Invoke(temp.Length, new Hermes(sb));

                return;
            }
            foreach (string name in temp)
                name.FindByName<NumericUpDown>(this).Value = 0;

            checkBox.Text = "Load";
            button.Text = GetCurrentStatus(button.ForeColor);
            checkBox.ForeColor = Color.Snow;
        }
        private string GetCurrentStatus(Color state)
        {
            if (Color.Maroon.Equals(state))
                return "Trading.";

            return checkBox.Text.Equals("Load") ? "0.6.11.20" : "Start Trading";
        }
        private bool PerformClick(CheckState check)
        {
            if (DialogResult.Cancel.Equals(TimerBox.Show("Find Most Confident Strategy. . .\n\nClick the\n\n'Cancel'\n\nButton to Manually Create a Strategy.", "Notice", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1, 7195)))
                return false;

            checkBox.CheckState = check;
            button.PerformClick();

            return checkBox.CheckState.Equals(CheckState.Unchecked) && !button.ForeColor.Equals(Color.Maroon) ? true : false;
        }
        public SelectStatisticalData()
        {
            InitializeComponent();
            SuspendLayout();

            foreach (string name in Enum.GetNames(typeof(IRecall.ComboBoxRecall)))
            {
                var temp = name.FindByName<ComboBox>(this);
                temp.DrawItem += SetAlignCenter;
                temp.SelectedValueChanged += SetSelecetedValue;
                temp.Sorted = false;
                temp.DrawMode = DrawMode.OwnerDrawVariable;
                temp.ForeColor = Color.Black;
                temp.DropDownHeight = 134;
                temp.Margin = new Padding(3, 8, 3, 0);
                name.Replace("comboBox", "label").FindByName<Label>(this).Cursor = Cursors.Default;
                var convert = name.Replace("comboBox", "numeric").FindByName<NumericUpDown>(this);
                convert.ValueChanged += OccursChange;
                convert.Margin = new Padding(3, 7, 3, 0);
            }
            checkBox.CheckStateChanged += GetCheckPosture;
            label.Cursor = Cursors.Default;
        }
        public void StartProgress(Dictionary<string, int> param)
        {
            foreach (KeyValuePair<string, int> kv in param)
            {
                string[] temp = kv.Key.Split(';');
                ((IRecall.ComboBoxRecall)kv.Value).ToString().FindByName<ComboBox>(this).Items.Add(int.Parse(temp[1]).ToString("N0"));
            }
            ResumeLayout();
        }
        public void OnReceiveHermes(object sender, Hermes e)
        {
            checkBox.CheckState = CheckState.Unchecked;

            if (sender is int && e.Check == false)
                return;

            if (e.Param != null && e.Param.Length > 0)
                for (int i = 0; i < e.Param.Length; i++)
                    ((IRecall.NumericRecall)i).ToString().FindByName<NumericUpDown>(this).Value = int.Parse(e.Param[i]);
        }
        public void GetStrategy(DialogResult result)
        {
            switch (result)
            {
                case DialogResult.Yes:
                    while (PerformClick(CheckState.Checked))
                    {
                        button.PerformClick();
                    };
                    break;

                case DialogResult.No:

                    break;

                case DialogResult.Abort:
                    foreach (string name in Enum.GetNames(typeof(IRecall.NumericRecall)))
                        name.FindByName<NumericUpDown>(this).Value = 0;

                    break;
            }
        }
        public event EventHandler<Hermes> SendStrategy;
        public event EventHandler<DialogClose> SendClose;
    }
}