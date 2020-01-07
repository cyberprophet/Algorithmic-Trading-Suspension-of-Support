namespace ShareInvest.Interface
{
    public interface IRecall
    {
        enum ButtonYield
        {
            buttonRecent = 0,
            buttonWeekly = 1,
            buttonBiweekly = 2,
            buttonMonthly = 3,
            buttonFor3Months = 4,
            buttonFor6Months = 5,
            buttonCumulative = 6
        }
        enum ComboBoxYield
        {
            rateCumulative = 6,
            rateFor6Months = 5,
            rateFor3Months = 4,
            rateMonthly = 3,
            rateBiweekly = 2,
            rateWeekly = 1,
            rateRecent = 0
        }
        enum TrustYield
        {
            trustCumulative = 6,
            trustFor6Months = 5,
            trustFor3Months = 4,
            trustMonthly = 3,
            trustBiweekly = 2,
            trustWeekly = 1,
            trustRecent = 0
        }
        enum LabelYield
        {
            labelCumulative = 6,
            labelFor6Months = 5,
            labelFor3Months = 4,
            labelMonthly = 3,
            labelBiweekly = 2,
            labelWeekly = 1,
            labelRecent = 0
        }
        enum NumericRecall
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
        enum LabelRecall
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
        enum ComboBoxRecall
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
    }
}