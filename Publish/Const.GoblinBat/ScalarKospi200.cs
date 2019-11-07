using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class ScalarKospi200 : IScalar
    {
        public int[] Reaction
        {
            get; private set;
        }
        public int[] ShortMinutePeriod
        {
            get; private set;
        }
        public int[] ShortDayPeriod
        {
            get; private set;
        }
        public int[] LongMinutePeriod
        {
            get; private set;
        }
        public int[] LongDayPeriod
        {
            get; private set;
        }
        public int[] StopLoss
        {
            get; private set;
        }
        public int[] Revenue
        {
            get; private set;
        }
        public ScalarKospi200()
        {
            StopLoss = new int[] { 5, 10, 15 };
            Revenue = new int[] { 15, 20, 25, 30 };
            ShortMinutePeriod = new int[] { 2, 5, 7 };
            ShortDayPeriod = new int[] { 2, 5 };
            LongMinutePeriod = new int[] { 10, 20, 35 };
            LongDayPeriod = new int[] { 10, 20, 60 };
            Reaction = new int[20];

            for (int i = 0; i < Reaction.Length; i++)
                Reaction[i] = (int)(i * 3.8) + 15;
        }
    }
}