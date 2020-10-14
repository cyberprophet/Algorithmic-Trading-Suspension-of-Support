using ShareInvest.Interface.XingAPI;

namespace ShareInvest.Catalog.XingAPI
{
    public struct Order : IOrders
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
    public enum ErxPrcCndiTpCode
    {
        시장가 = 1,
        지정가 = 2
    }
    enum Cancellation
    {
        FnoIsuNo = 2,
        OrgOrdNo = 3,
        OrdQty = 6
    }
    enum Correction
    {
        FnoIsuNo = 2,
        OrgOrdNo = 3,
        FnoOrdprcPtnCode = 4,
        OrdPrc = 5,
        OrdQty = 6
    }
    enum New
    {
        FnoIsuNo = 2,
        BnsTpCode = 3,
        FnoOrdprcPtnCode = 4,
        OrdPrc = 5,
        OrdQty = 6
    }
}