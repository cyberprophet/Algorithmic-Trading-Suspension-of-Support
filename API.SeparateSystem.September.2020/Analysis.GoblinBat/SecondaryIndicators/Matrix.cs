namespace ShareInvest.Analysis.SecondaryIndicators
{
    class Matrix
    {
        internal Matrix(int size)
        {
            a = new double[size, size];
            y = new double[size];
            x = new double[size];
        }
        internal readonly double[,] a;
        internal readonly double[] y;
        internal readonly double[] x;
    }
}