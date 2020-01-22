using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ShareInvest.GoblinBatContext;
using ShareInvest.Interface;
using ShareInvest.Models;

namespace ShareInvest.Const
{
    public class Recall : IEnumerable
    {
        public Recall(string code, long assets)
        {
            this.code = code;
            this.assets = assets;
        }
        public IEnumerator GetEnumerator()
        {
            using (var db = new GoblinBatDbContext())
            {
                var list = db.Logs.ToList().FindAll(o => o.Code.Equals(code) && o.Assets.Equals(assets));
                var date = list.ConvertAll(o => o.Date).Distinct().ToArray();
                var max = date.Max();

                yield return date.Length;

                foreach (var makeUp in mp)
                    yield return MakeUp(list, makeUp, max, date.Length - 1);

                yield return string.Concat(date.Min(), "^", max);
            }
        }
        private IMakeUp MakeUp(List<Logs> list, IMakeUp ip, int recent, int length)
        {
            if (ip.FindByName.Equals("cumulative"))
                ip.Turn = length;

            foreach (var makeUp in list.FindAll(o => o.Date.Equals(recent)))
                ip.DescendingSort[makeUp.Strategy] = makeUp.Cumulative + makeUp.Unrealized - list.FindAll(o => o.Strategy.Equals(makeUp.Strategy)).OrderByDescending(o => o.Date).ElementAt(ip.Turn).Cumulative;

            return ip;
        }
        private readonly IMakeUp[] mp =
        {
            new MakeUpCumulative(),
            new MakeUpRecentDate(),
            new MakeUpWeekly(),
            new MakeUpBiweekly(),
            new MakeUpMonthly(),
            new MakeUpFor3Months(),
            new MakeUpFor6Months()
        };
        public enum ButtonYield
        {
            buttonRecent = 0,
            buttonWeekly = 1,
            buttonBiweekly = 2,
            buttonMonthly = 3,
            buttonFor3Months = 4,
            buttonFor6Months = 5,
            buttonCumulative = 6
        }
        public enum ComboBoxYield
        {
            rateCumulative = 6,
            rateFor6Months = 5,
            rateFor3Months = 4,
            rateMonthly = 3,
            rateBiweekly = 2,
            rateWeekly = 1,
            rateRecent = 0
        }
        public enum TrustYield
        {
            trustCumulative = 6,
            trustFor6Months = 5,
            trustFor3Months = 4,
            trustMonthly = 3,
            trustBiweekly = 2,
            trustWeekly = 1,
            trustRecent = 0
        }
        public enum LabelYield
        {
            labelCumulative = 6,
            labelFor6Months = 5,
            labelFor3Months = 4,
            labelMonthly = 3,
            labelBiweekly = 2,
            labelWeekly = 1,
            labelRecent = 0
        }
        public enum NumericRecall
        {
            numericShortTick = 0,
            numericShortDay = 1,
            numericLongTick = 2,
            numericLongDay = 3,
            numericReaction = 4,
            numericHedge = 5,
            numericBase = 6,
            numericSigma = 7,
            numericPercent = 8,
            numericMax = 9,
            numericQuantity = 10,
            numericTime = 11
        }
        public enum LabelRecall
        {
            labelShortTick = 0,
            labelShortDay = 1,
            labelLongTick = 2,
            labelLongDay = 3,
            labelReaction = 4,
            labelHedge = 5,
            labelBase = 6,
            labelSigma = 7,
            labelPercent = 8,
            labelMax = 9,
            labelQuantity = 10,
            labelTime = 11
        }
        public enum ComboBoxRecall
        {
            comboBoxShortTick = 0,
            comboBoxShortDay = 1,
            comboBoxLongTick = 2,
            comboBoxLongDay = 3,
            comboBoxReaction = 4,
            comboBoxHedge = 5,
            comboBoxBase = 6,
            comboBoxSigma = 7,
            comboBoxPercent = 8,
            comboBoxMax = 9,
            comboBoxQuantity = 10,
            comboBoxTime = 11
        }
        private readonly long assets;
        private readonly string code;
    }
}