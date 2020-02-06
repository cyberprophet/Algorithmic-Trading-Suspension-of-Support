using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ShareInvest.GoblinBatContext;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatControls
{
    public partial class StatisticalAnalysis : UserControl
    {
        public Dictionary<string, string> CodeCatalog
        {
            get; private set;
        }
        public StatisticalAnalysis()
        {
            InitializeComponent();
            CodeCatalog = new Dictionary<string, string>(16);

            foreach (Codes codes in new CallUpStatisticalAnalysis())
                if (codes.Code.Length == 6 || codes.Code.Length == 8 && DateTime.Compare(DateTime.ParseExact(codes.Info, "yyyyMMdd", null), DateTime.Now) >= 0)
                    CodeCatalog[codes.Name] = codes.Code;
        }
    }
}