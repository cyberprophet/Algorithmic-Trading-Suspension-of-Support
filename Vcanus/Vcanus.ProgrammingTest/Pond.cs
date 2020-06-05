namespace Vcanus.ProgrammingTest
{
    class Pond
    {
        internal int FindDepthOfThePond()
        {
            DepthOfPond = new int[10, 10];
            int temp, repeat, i, j;

            for (i = 0; i < 10; i++)
                for (j = 0; j < 10; j++)
                {
                    DepthOfPond[i, j] = pond[i, j];

                    if (pond[i, j] > 0 && pond[i - 1, j] >= pond[i, j] && pond[i + 1, j] >= pond[i, j] && pond[i, j - 1] >= pond[i, j] && pond[i, j + 1] >= pond[i, j])
                        DepthOfPond[i, j] += 1;
                }
            do
            {
                temp = 0;
                repeat = 0;

                foreach (var param in DepthOfPond)
                    temp += param;

                for (i = 0; i < 10; i++)
                    for (j = 0; j < 10; j++)
                        if (DepthOfPond[i, j] > 0 && DepthOfPond[i - 1, j] >= DepthOfPond[i, j] && DepthOfPond[i + 1, j] >= DepthOfPond[i, j] && DepthOfPond[i, j - 1] >= DepthOfPond[i, j] && DepthOfPond[i, j + 1] >= DepthOfPond[i, j])
                            DepthOfPond[i, j] += 1;

                foreach (var param in DepthOfPond)
                    repeat += param;
            }
            while (temp != repeat);
            return temp;
        }
        internal int FindDepthOfThePond(int length)
        {
            DepthOfPond = new int[length, length];
            int temp, repeat, i, j;

            for (j = length - 1; j >= 0; j--)
                for (i = length - 1; i >= 0; i--)
                {
                    DepthOfPond[i, j] = pond[i, j];

                    if (pond[i, j] > 0 && pond[i - 1, j] >= pond[i, j] && pond[i + 1, j] >= pond[i, j] && pond[i, j - 1] >= pond[i, j] && pond[i, j + 1] >= pond[i, j])
                        DepthOfPond[i, j] += 1;
                }
            do
            {
                temp = 0;
                repeat = 0;

                foreach (var param in DepthOfPond)
                    repeat += param;

                for (j = length - 1; j >= 0; j--)
                    for (i = length - 1; i >= 0; i--)
                        if (DepthOfPond[i, j] > 0 && DepthOfPond[i - 1, j] >= DepthOfPond[i, j] && DepthOfPond[i + 1, j] >= DepthOfPond[i, j] && DepthOfPond[i, j - 1] >= DepthOfPond[i, j] && DepthOfPond[i, j + 1] >= DepthOfPond[i, j])
                            DepthOfPond[i, j] += 1;

                foreach (var param in DepthOfPond)
                    temp += param;
            }
            while (repeat != temp);
            return temp;
        }
        int[,] DepthOfPond
        {
            get; set;
        }
        readonly int[,] pond = new int[,]
        {
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 0, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 0, 0, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 1, 1, 1, 1, 1, 1, 1, 1, 0 },
            { 0, 0, 1, 1, 1, 1, 1, 1, 0, 0 },
            { 0, 0, 0, 1, 1, 1, 1, 0, 0, 0 },
            { 0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
            { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }
        };
    }
}