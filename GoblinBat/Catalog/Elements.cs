namespace ShareInvest.Catalog
{
    public class Elements
    {
        public Elements(string screen, string classification, int quantity, int position, double purchase)
        {
            this.screen = screen;
            this.classification = classification;
            this.quantity = quantity;
            sRQName = string.Concat(position, ';', purchase, ';', classification, ';', quantity, ";New");
        }
        public Elements(string screen, string classification, string quantity, string position, string purchase)
        {
            this.screen = screen;
            this.classification = classification;
            this.quantity = int.Parse(quantity);
            sRQName = string.Concat(position, ";", purchase, ";", classification, ";", quantity, ";ToMakeUp");
        }
        public int quantity;
        public string sRQName;
        public string screen;
        public string classification;
    }
}