using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ShareInvest.Controls
{
    public partial class Balance : UserControl
    {
        public static Balance Get()
        {
            if (bal == null)
                bal = new Balance();

            return bal;
        }
        private Balance()
        {
            InitializeComponent();
        }
        private static Balance bal;
    }
}