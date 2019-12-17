using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using ShareInvest.FindByName;
using ShareInvest.Interface;
using ShareInvest.RecallStatistics;

namespace ShareInvest.Conservation
{
    public partial class Yield : UserControl
    {
        public Yield()
        {
            InitializeComponent();
            SuspendLayout();

            foreach (var temp in new Recall())
            {
                if (temp.GetType().Equals(typeof(string)))
                {
                    break;
                }
                IMakeUp make = (IMakeUp)temp;
                var name = Array.Find(Enum.GetNames(typeof(IRecall.ComboBoxYield)), o => o.Contains(make.FindByName.Substring(1))).FindByName<ComboBox>(this);
                int i = 0;

                foreach (KeyValuePair<string, long> kv in make.DescendingSort.OrderByDescending(o => o.Value))
                {
                    if (i++ > 999)
                        break;

                    name.Items.Add(kv.Key.Replace('^', '.'));
                }
                name.ForeColor = Color.FromArgb(96, 48, 25);
                name.Font = new Font("Komoda", 14.35F, FontStyle.Bold);
                name.DropDownHeight = 126;
                name.SelectedIndex = 0;
            }
            ResumeLayout();
        }
    }
}