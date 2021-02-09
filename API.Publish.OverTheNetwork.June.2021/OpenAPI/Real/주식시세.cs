using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
	class 주식시세 : Real
	{
		internal override AxKHOpenAPI API
		{
			get; set;
		}
		internal override StreamWriter Server
		{
			get; set;
		}
		internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			if (Connect.GetInstance().StocksHeld.TryGetValue(e.sRealKey, out Analysis analysis))
			{
				string str_offer = API.GetCommRealData(e.sRealKey, Fid[3]), str_bid = API.GetCommRealData(e.sRealKey, Fid[4]), str_current = API.GetCommRealData(e.sRealKey, Fid[0]);

				if (int.TryParse(str_current[0] is '-' ? str_current[1..] : str_current, out int current) && int.TryParse(str_offer[0] is '-' ? str_offer[1..] : str_offer, out int offer) && int.TryParse(str_bid[0] is '-' ? str_bid[1..] : str_bid, out int bid))
				{
					analysis.Bid = bid;
					analysis.Offer = offer;
					analysis.Current = current;
				}
			}
		}
		internal override bool Lite
		{
			get; set;
		}
		protected internal override int[] Fid => new int[] { 10, 11, 12, 27, 28, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 311, 567, 568 };
	}
}