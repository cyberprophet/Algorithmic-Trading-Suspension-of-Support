namespace ShareInvest.Catalog
{
    public class ScreenNumber
    {
        protected string GetScreenNumber()
        {
            if (number > 9500 || number < 9400)
                number = 9500;

            return number--.ToString();
        }
        private static int number;
    }
}