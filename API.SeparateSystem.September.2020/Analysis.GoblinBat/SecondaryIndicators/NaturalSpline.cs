using System;

namespace ShareInvest.Analysis.SecondaryIndicators
{
    sealed class NaturalSpline : SplineInterpolator
    {
        void CalcParameters()
        {
            int i;

            for (i = 0; i < n; i++)
                A[i] = GivenYs[i];

            for (i = 0; i < n - 1; i++)
                H[i] = GivenXs[i + 1] - GivenXs[i];

            for (i = 0; i < n - 2; i++)
                for (int k = 0; k < n - 2; k++)
                {
                    M.a[i, k] = 0D;
                    M.y[i] = 0D;
                    M.x[i] = 0D;
                }
            for (i = 0; i < n - 2; i++)
            {
                if (i == 0)
                {
                    M.a[i, 0] = 2D * (H[0] + H[1]);
                    M.a[i, 1] = H[1];
                }
                else
                {
                    M.a[i, i - 1] = H[i];
                    M.a[i, i] = 2D * (H[i] + H[i + 1]);

                    if (i < n - 3)
                        M.a[i, i + 1] = H[i + 1];
                }
                if ((H[i] != 0D) && (H[i + 1] != 0D))
                    M.y[i] = ((A[i + 2] - A[i + 1]) / H[i + 1] - (A[i + 1] - A[i]) / H[i]) * 3D;

                else
                    M.y[i] = 0D;
            }
            if (Gauss.Eliminate() == false)
                throw new InvalidOperationException("error in matrix calculation");

            Gauss.Solve();
            C[0] = 0D;
            C[n - 1] = 0D;

            for (i = 1; i < n - 1; i++)
                C[i] = M.x[i - 1];

            for (i = 0; i < n - 1; i++)
                if (H[i] != 0D)
                {
                    D[i] = 1D / 3D / H[i] * (C[i + 1] - C[i]);
                    B[i] = 1D / H[i] * (A[i + 1] - A[i]) - H[i] / 3D * (C[i + 1] + 2 * C[i]);
                }
        }
        internal NaturalSpline(double[] xs, double[] ys, int resolution = 0xA) : base(xs, ys, resolution)
        {
            M = new Matrix(n - 2);
            Gauss = new MatrixSolver(n - 2, M);
            A = new double[n];
            B = new double[n];
            C = new double[n];
            D = new double[n];
            H = new double[n - 1];
            CalcParameters();
        }
    }
}