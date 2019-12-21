using ShareInvest.Communication;

namespace ShareInvest.AutoSetting
{
    public class AutomaticallySetting
    {
        public int[] SetVariableAutomatic(IAsset.Variable asset, int used, int repeat)
        {
            int i, start, destination, interval;
            int[] value;

            switch (asset)
            {
                case IAsset.Variable.ShortTick:
                case IAsset.Variable.Percent:
                    interval = 5;

                    break;

                case IAsset.Variable.LongTick:
                    interval = 10;

                    break;

                case IAsset.Variable.Base:
                    interval = 250;

                    break;

                default:
                    interval = 1;

                    break;
            };
            destination = used + repeat * interval;
            start = used - repeat * interval;

            switch (asset)
            {
                case IAsset.Variable.ShortTick when start < 35:
                    start = 35;

                    break;

                case IAsset.Variable.LongTick when start < 50:
                    start = 50;

                    break;

                case IAsset.Variable.Reaction:
                    start = used - repeat * interval * 2;
                    destination = used + repeat * interval * 2;

                    if (start < 25)
                        start = 25;

                    if (destination > 105)
                        destination = 105;

                    break;
            }
            if (start < 1)
            {
                interval = destination;
                start = 0;
            }
            switch (asset)
            {
                case IAsset.Variable.ShortDay:
                case IAsset.Variable.Hedge:
                case IAsset.Variable.Base:
                case IAsset.Variable.Quantity:
                case IAsset.Variable.Time:
                    value = new int[(destination - start) / (interval > 0 ? interval : 1) + 2];
                    value[value.Length - 1] = 0;

                    for (i = 0; i < value.Length - 1; i++)
                        value[i] = start + interval * i;

                    break;

                default:
                    value = new int[(destination - start) / (interval > 0 ? interval : 1) + 1];

                    for (i = 0; i < value.Length; i++)
                        value[i] = start + interval * i;

                    break;
            }
            return value;
        }
    }
}