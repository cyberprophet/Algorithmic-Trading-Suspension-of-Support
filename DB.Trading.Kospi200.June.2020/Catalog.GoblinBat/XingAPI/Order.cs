namespace ShareInvest.Catalog.XingAPI
{
    public struct Order
    {
        public string FnoIsuNo
        {
            get; set;
        }
        public string OrgOrdNo
        {
            get; set;
        }
        public string BnsTpCode
        {
            get; set;
        }
        public string FnoOrdprcPtnCode
        {
            get; set;
        }
        public string OrdPrc
        {
            get; set;
        }
        public string OrdQty
        {
            get; set;
        }
    }
    public enum Cancellation
    {
        FnoIsuNo = 2,
        OrgOrdNo = 3
    }
    public enum Correction
    {
        FnoIsuNo = 2,
        OrgOrdNo = 3,
        FnoOrdprcPtnCode = 4,
        OrdPrc = 5,
        OrdQty = 6
    }
    public enum New
    {
        FnoIsuNo = 2,
        BnsTpCode = 3,
        FnoOrdprcPtnCode = 4,
        OrdPrc = 5,
        OrdQty = 6
    }
    public enum FnoOrdprcPtnCode
    {
        지정가 = 00,
        시장가 = 03,
        조건부지정가 = 05,
        최유리지정가 = 06,
        지정가_IOC = 10,
        지정가_FOK = 20,
        시장가_IOC = 13,
        시장가_FOK = 23,
        최유리지정가_IOC = 16,
        최유리지정가_FOK = 26
    }
}