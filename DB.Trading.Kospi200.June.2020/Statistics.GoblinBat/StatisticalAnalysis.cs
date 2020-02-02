using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ShareInvest.GoblinBatContext;
using ShareInvest.Models;

namespace ShareInvest.GoblinBatControls
{
    public partial class StatisticalAnalysis : UserControl
    {
        public static StatisticalAnalysis GetInstance(string[] markets)
        {
            if (markets != null)
                Markets = markets;

            if (Statistical == null)
                Statistical = new StatisticalAnalysis();

            return Statistical;
        }
        public Dictionary<string, string> CodeCatalog
        {
            get; private set;
        }
        private StatisticalAnalysis()
        {
            InitializeComponent();
            CodeCatalog = new Dictionary<string, string>(16);

            foreach (Codes codes in new CallUpStatisticalAnalysis())
                if (codes.Code.Length == 6 && Array.Exists(Markets, o => o.Equals(codes.Code)) || codes.Code.Length == 8 && DateTime.Compare(DateTime.ParseExact(codes.Info, "yyyyMMdd", null), DateTime.Now) >= 0)
                    CodeCatalog[codes.Name] = codes.Code;

            Markets = null;
        }
        private static StatisticalAnalysis Statistical
        {
            get; set;
        }
        private static string[] Markets
        {
            get; set;
        }
    }
}