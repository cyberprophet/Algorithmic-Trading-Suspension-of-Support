namespace ShareInvest.Interface
{
    public interface ISendOrder
    {
        string AccNo
        {
            get; set;
        }
        int OrderType
        {
            get; set;
        }
        string Code
        {
            get; set;
        }
        int Qty
        {
            get; set;
        }
        int Price
        {
            get; set;
        }
        string HogaGb
        {
            get; set;
        }
        string OrgOrderNo
        {
            get; set;
        }
    }
}