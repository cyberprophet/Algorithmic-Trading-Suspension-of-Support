namespace ShareInvest.Interface.Struct
{
    public struct PurchaseInformation
    {
        public enum OrderType
        {
            지정가 = 1,
            조건부지정가 = 2,
            시장가 = 3,
            최유리지정가 = 4,
            지정가IOC = 5,
            지정가FOK = 6,
            시장가IOC = 7,
            시장가FOK = 8,
            최유리지정가IOC = 9,
            최유리지정가FOK = 'A'
        }
        public string RQName
        {
            get; set;
        }
        public string ScreenNo
        {
            get; set;
        }
        public string AccNo
        {
            get; set;
        }
        public string Code
        {
            get; set;
        }
        public int OrdKind
        {
            get; set;
        }
        public string SlbyTP
        {
            get; set;
        }
        public string OrdTp
        {
            get; set;
        }
        public int Qty
        {
            get; set;
        }
        public string Price
        {
            get; set;
        }
        public string OrgOrdNo
        {
            get; set;
        }
    }
}