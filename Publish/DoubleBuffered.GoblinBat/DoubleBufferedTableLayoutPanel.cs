using System.ComponentModel;
using System.Windows.Forms;

namespace ShareInvest.DoubleBuffered
{
    public class DoubleBufferdTableLayoutPanel : TableLayoutPanel
    {
        public DoubleBufferdTableLayoutPanel()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
            UpdateStyles();
        }
        public DoubleBufferdTableLayoutPanel(IContainer container)
        {
            container.Add(this);
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint, true);
        }
    }
}