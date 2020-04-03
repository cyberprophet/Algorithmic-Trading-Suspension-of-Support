namespace ShareInvest.Catalog
{
    public class ScreenNumber
    {
        protected internal string GetScreenNumber()
        {
            if (Number < 9000)
                Number = 9030;

            return Number--.ToString();
        }
        protected internal string GetScreenNumber(uint count)
        {
            if (Count < count)
                Count = 8999;

            return Count--.ToString();
        }
        static uint Number
        {
            get; set;
        }
        static uint Count
        {
            get; set;
        }
    }
}