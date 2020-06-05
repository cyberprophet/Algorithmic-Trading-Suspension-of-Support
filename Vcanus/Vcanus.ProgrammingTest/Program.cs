using System;

namespace Vcanus.ProgrammingTest
{
    class Program
    {
        static void Main()
        {
            var flow = new StackOverflow();
            var pond = new Pond();

            Console.WriteLine(Test.No1);
            Bread.GetRecipe("Bread");

            Console.WriteLine(Test.No2);
            Console.WriteLine(new Operation().Add(4).Add(5).Subtract(3).Out());

            Console.WriteLine(Test.No3);
            Console.WriteLine(flow.Factorial(4));

            Console.WriteLine(Test.No4);
            Console.WriteLine(flow.Factorial(1000000M));

            Console.WriteLine(Test.No5);
            Console.WriteLine(pond.FindDepthOfThePond());
            Console.WriteLine(pond.FindDepthOfThePond(10));

            Console.ReadLine();
        }
    }
}