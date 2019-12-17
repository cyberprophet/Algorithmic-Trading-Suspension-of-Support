using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ShareInvest.FindByName;
using ShareInvest.Interface;

namespace ShareInvest.StatisticalData
{
    public partial class SelectStatisticalData : UserControl
    {
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
        public SelectStatisticalData()
        {
            InitializeComponent();
            SuspendLayout();

            foreach (string name in Enum.GetNames(typeof(IRecall.ComboBoxRecall)))
            {
                var temp = name.FindByName<ComboBox>(this);
                temp.DrawItem += SetAlignCenter;
                temp.Sorted = false;
                temp.DrawMode = DrawMode.OwnerDrawVariable;
                temp.ForeColor = Color.Black;
                temp.DropDownHeight = 129;
            }
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
    }
}