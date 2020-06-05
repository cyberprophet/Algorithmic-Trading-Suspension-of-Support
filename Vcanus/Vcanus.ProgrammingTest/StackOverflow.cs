namespace Vcanus.ProgrammingTest
{
    class StackOverflow
    {
        internal int Factorial(int param)
        {
            if (param == 1)
                return 1;

            return param * Factorial(param - 1);
        }
        internal decimal Factorial(decimal param)
        {
            return Factorial(param, 1);
        }
        decimal Factorial(decimal param, decimal accumulator)
        {
            if (param == 1)
                return accumulator;

            if (accumulator >= decimal.MaxValue / param)
                return accumulator;

            return Factorial(param - 1, accumulator * param);
        }
    }
}