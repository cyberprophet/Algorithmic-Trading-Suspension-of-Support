namespace ShareInvest.Catalog
{
    public abstract class TR
    {
        public abstract string ID
        {
            get; protected set;
        }
        public abstract string Value
        {
            get; set;
        }
        public abstract string RQName
        {
            get; set;
        }
        public abstract string TrCode
        {
            get; protected set;
        }
        public abstract int PrevNext
        {
            get; set;
        }
        public abstract string ScreenNo
        {
            get; protected set;
        }
    }
}