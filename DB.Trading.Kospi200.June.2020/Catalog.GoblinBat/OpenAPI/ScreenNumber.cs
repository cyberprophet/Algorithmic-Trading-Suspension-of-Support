namespace ShareInvest.Catalog
{
    public class ScreenNumber
    {
        internal protected string GetScreenNumber()
        {
            if (Number < 9000)
                Number = 9030;

            return Number--.ToString();
        }
        internal protected string GetScreenNumber(uint count)
        {
            if (Count < count)
                Count = 8999;

            return Count--.ToString();
        }
        private static uint Number
        {
            get; set;
        }
        private static uint Count
        {
            get; set;
        }
    }
}