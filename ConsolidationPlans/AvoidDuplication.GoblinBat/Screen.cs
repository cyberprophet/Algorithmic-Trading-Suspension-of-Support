namespace ShareInvest.AvoidDuplication
{
    public class Screen
    {
        public string GetScreen()
        {
            if (count < 9000)
                count = 10000;

            count--;

            return count.ToString();
        }
        private static int count = 10000;
    }
}