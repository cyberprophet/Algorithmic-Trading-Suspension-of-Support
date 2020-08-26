using System;

namespace ShareInvest.Analysis.SecondaryIndicators
{
    sealed class EndSlopeSpline : SplineInterpolator
    {
        void CalcParameters(double alpha, double beta)
        {
            int i;

            for (i = 0; i < n; i++)
                A[i] = GivenYs[i];

            for (i = 0; i < n - 1; i++)
                H[i] = GivenXs[i + 1] - GivenXs[i];

            M.a[0, 0] = 2D * H[0];
            M.a[0, 1] = H[0];
            M.y[0] = 3 * ((A[1] - A[0]) / H[0] - Math.Tan(alpha * Math.PI / 0xB4));

            for (i = 0; i < n - 2; i++)
            {
                M.a[i + 1, i] = H[i];
                M.a[i + 1, i + 1] = 2D * (H[i] + H[i + 1]);

                if (i < n - 2)
                    M.a[i + 1, i + 2] = H[i + 1];

                if ((H[i] != 0D) && (H[i + 1] != 0D))
                    M.y[i + 1] = ((A[i + 2] - A[i + 1]) / H[i + 1] - (A[i + 1] - A[i]) / H[i]) * 3D;

                else
                    M.y[i + 1] = 0D;
            }
            M.a[n - 1, n - 2] = H[n - 2];
            M.a[n - 1, n - 1] = 2D * H[n - 2];
            M.y[n - 1] = 3D * (Math.Tan(beta * Math.PI / 0xB4) - (A[n - 1] - A[n - 2]) / H[n - 2]);

            if (Gauss.Eliminate() == false)
                throw new InvalidOperationException();

            Gauss.Solve();

            for (i = 0; i < n; i++)
                C[i] = M.x[i];

            for (i = 0; i < n; i++)
                if (H[i] != 0D)
                {
                    D[i] = 1D / 3D / H[i] * (C[i + 1] - C[i]);
                    B[i] = 1D / H[i] * (A[i + 1] - A[i]) - H[i] / 3D * (C[i + 1] + 2 * C[i]);
                }
        }
        internal EndSlopeSpline(double[] xs, double[] ys, int resolution = 0xA, double firstSlopeDegrees = 0, double lastSlopeDegrees = 0) : base(xs, ys, resolution)
        {
            M = new Matrix(n);
            Gauss = new MatrixSolver(n, M);
            A = new double[n];
            B = new double[n];
            C = new double[n];
            D = new double[n];
            H = new double[n];
            CalcParameters(firstSlopeDegrees, lastSlopeDegrees);         
        }
    }
}