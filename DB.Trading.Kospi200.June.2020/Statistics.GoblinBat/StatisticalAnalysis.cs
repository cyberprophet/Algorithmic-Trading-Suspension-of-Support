using System;
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
        private void StatisticalAnalysisResize(object sender, EventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                SuspendLayout();
                codeGrid.AutoResizeColumns();
                codeGrid.AutoResizeRows();
                ResumeLayout();
            }));
        }
        private StatisticalAnalysis()
        {
            InitializeComponent();

            foreach (Codes codes in new CallUpStatisticalAnalysis())
                if (codes.Code.Length == 6 && Array.Exists(Markets, o => o.Equals(codes.Code)) || codes.Code.Length == 8 && DateTime.Compare(DateTime.ParseExact(codes.Info, "yyyyMMdd", null), DateTime.Now) >= 0)
                    codeGrid.Rows.Add(new string[]
                    {
                        codes.Code,
                        codes.Name,
                        codes.Code.Length == 6 ? int.Parse(codes.Info).ToString("N0") : DateTime.ParseExact(codes.Info, "yyyyMMdd", null).ToString("yy-MM-dd")
                    });
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