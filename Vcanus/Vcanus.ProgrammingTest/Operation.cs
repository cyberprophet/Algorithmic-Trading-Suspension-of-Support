namespace Vcanus.ProgrammingTest
{
    class Operation
    {
        internal Operation Add(int param)
        {
            Temp += param;

            return this;
        }
        internal Operation Subtract(int param)
        {
            Temp -= param;

            return this;
        }
        internal int Out()
        {
            return Temp;
        }
        int Temp
        {
            get; set;
        }
    }
}