using System;

namespace ShareInvest.SecondaryIndicators
{
    public abstract class SplineInterpolator
    {
        public (double[], double[]) Interpolate()
        {
            int resolution = InterpolatedXs.Length / n;

            for (int i = 0; i < H.Length; i++)
                for (int k = 0; k < resolution; k++)
                {
                    double deltaX = (double)k / resolution * H[i];
                    int interpolatedIndex = i * resolution + k;
                    InterpolatedXs[interpolatedIndex] = deltaX + GivenXs[i];
                    InterpolatedYs[interpolatedIndex] = A[i] + (B[i] * deltaX) + (C[i] * deltaX * deltaX) + (D[i] * deltaX * deltaX * deltaX);
                }
            int pointsToKeep = resolution * (n - 1) + 1;
            double[] interpolatedXsCopy = new double[pointsToKeep];
            double[] interpolatedYsCopy = new double[pointsToKeep];
            Array.Copy(InterpolatedXs, 0, interpolatedXsCopy, 0, pointsToKeep - 1);
            Array.Copy(InterpolatedYs, 0, interpolatedYsCopy, 0, pointsToKeep - 1);
            InterpolatedXs = interpolatedXsCopy;
            InterpolatedYs = interpolatedYsCopy;
            InterpolatedXs[pointsToKeep - 1] = GivenXs[n - 1];
            InterpolatedYs[pointsToKeep - 1] = GivenYs[n - 1];

            return (InterpolatedXs, InterpolatedYs);
        }
        public SplineInterpolator Integrate()
        {
            double integral = 0;

            for (int i = 0; i < H.Length; i++)
                integral += (A[i] * H[i]) + (B[i] * Math.Pow(H[i], 2) / 2D) + (C[i] * Math.Pow(H[i], 3) / 3D) + (D[i] * Math.Pow(H[i], 4) / 4D);

            return this;
        }
        internal Matrix M
        {
            get; set;
        }
        internal MatrixSolver Gauss
        {
            get; set;
        }
        protected internal SplineInterpolator(double[] xs, double[] ys, int resolution = 10)
        {
            if (xs is null || ys is null)
                throw new ArgumentException("xs and ys cannot be null");

            if (xs.Length != ys.Length)
                throw new ArgumentException("xs and ys must have the same length");

            if (xs.Length < 4)
                throw new ArgumentException("xs and ys must have a length of 4 or greater");

            if (resolution < 1)
                throw new ArgumentException("resolution must be 1 or greater");

            GivenXs = xs;
            GivenYs = ys;
            n = xs.Length;
            InterpolatedXs = new double[n * resolution];
            InterpolatedYs = new double[n * resolution];
        }
        protected internal double[] GivenXs
        {
            get; private set;
        }
        protected internal double[] GivenYs
        {
            get; private set;
        }
        protected internal double[] A
        {
            get; set;
        }
        protected internal double[] B
        {
            get; set;
        }
        protected internal double[] C
        {
            get; set;
        }
        protected internal double[] D
        {
            get; set;
        }
        protected internal double[] H
        {
            get; set;
        }
        protected internal readonly int n;
        double[] InterpolatedXs
        {
            get; set;
        }
        double[] InterpolatedYs
        {
            get; set;
        }
    }
}