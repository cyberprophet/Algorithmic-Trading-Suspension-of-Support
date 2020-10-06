namespace ShareInvest.Catalog.OpenAPI
{
    public struct SendOrderFO
    {
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
        public string SlbyTp
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
    public enum OrderType
    {
        지정가 = 1,
        조건부지정가 = 2,
        시장가 = 3,
        최유리지정가 = 4,
        지정가_IOC = 5,
        지정가_FOK = 6,
        시장가_IOC = 7,
        시장가_FOK = 8,
        최유리지정가_IOC = 9,
        최유리지정가_FOK = 'A'
    }
}