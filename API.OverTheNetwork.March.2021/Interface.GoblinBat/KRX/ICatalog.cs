namespace ShareInvest.Interface.KRX
{
	interface ICatalog
	{

	}
	public enum Catalog
	{
		지수구성종목 = 0x259
	}
	public enum Line
	{
		Week = 0x5,
		Month = 0x14,
		Quarter = 0x3C,
		Semiannual = 0x78,
		Annual = 0xF0
	}
	public enum News
	{
		Naver, AjuNews, MK, EconoNews, MTN, TheKPM, Asiae, WOWTV, Edaily, SEdaily, EToday, NewsPim, MT, NewsTomato
	}
}