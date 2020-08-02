using ShareInvest.Interface;

namespace ShareInvest.Catalog
{
    public struct TrendFollowingBasicFutures : IStrategics
    {
        public int Short
        {
            get; set;
        }
        public int Long
        {
            get; set;
        }
        public TrendFollowingBasicFutures[] SetCatalog(TrendFollowingBasicFutures tf) => new TrendFollowingBasicFutures[2]
        {
            new TrendFollowingBasicFutures
            {
                Code = tf.Code,
                Commission = tf.Commission,
                MarginRate = tf.MarginRate,
                ReactionShort = tf.ReactionShort,
                ReactionLong = tf.ReactionLong,
                RollOver = tf.RollOver,
                Minute = 0x5A0,
                Short = tf.DayShort,
                Long = tf.DayLong,
                QuantityShort = tf.QuantityShort,
                QuantityLong = tf.QuantityLong
            },
            new TrendFollowingBasicFutures
            {
                Code = tf.Code,
                Commission = tf.Commission,
                MarginRate = tf.MarginRate,
                ReactionShort = tf.ReactionShort,
                ReactionLong = tf.ReactionLong,
                RollOver = tf.RollOver,
                Minute = tf.Minute,
                Short = tf.MinuteShort,
                Long = tf.MinuteLong,
                QuantityShort = tf.QuantityShort,
                QuantityLong = tf.QuantityLong
            }
        };
        public double MarginRate
        {
            get; set;
        }
        public double Commission
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        public bool RollOver
        {
            get; set;
        }
        public int DayShort
        {
            get; set;
        }
        public int DayLong
        {
            get; set;
        }
        public int Minute
        {
            get; set;
        }
        public int MinuteShort
        {
            get; set;
        }
        public int MinuteLong
        {
            get; set;
        }
        public int ReactionShort
        {
            get; set;
        }
        public int ReactionLong
        {
            get; set;
        }
        public int QuantityShort
        {
            get; set;
        }
        public int QuantityLong
        {
            get; set;
        }
    }
}