using System.Windows.Forms;
using ShareInvest.Communicate;

namespace ShareInvest.Control
{
    public partial class Kosdaq150 : UserControl, IChoice
    {
        public Kosdaq150()
        {
            InitializeComponent();
            Length = 150;
            Type = 24;
            Path = @"\Log\";
        }
        private void Repeat()
        {
            for (Index = 1; Index < Length; Index++)
            {

            }
        }
        public int Type
        {
            get; private set;
        }
        public int Index
        {
            get; private set;
        }
        public int Length
        {
            get; private set;
        }
        public string Path
        {
            get; private set;
        }
    }
}