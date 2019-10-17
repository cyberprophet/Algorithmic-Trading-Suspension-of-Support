using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShareInvest.Communicate;
using ShareInvest.Const;

namespace ShareInvest.Control
{
    public partial class Progress : UserControl
    {
        public Progress()
        {
            InitializeComponent();
            int i, j, h, x = 100, y = 100, z = 100;

            for (i = 1; i < x; i++)
                for (j = 5; j < y; j++)
                    for (h = 5; h < z; h++)
                        new SpecifyKospi200
                        {
                            Reaction = i,
                            ShortMinPeriod = j,
                            ShortDayPeriod = h
                        };
        }
    }
}