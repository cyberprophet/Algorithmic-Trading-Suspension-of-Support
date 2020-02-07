namespace ShareInvest.Catalog
{
    public class ScreenNumber
    {
        internal protected string GetScreenNumber()
        {
            if (Number > 9500 || Number < 9400)
                Number = 9500;

            return Number--.ToString();
        }
        private int Number
        {
            get; set;
        }
    }
}