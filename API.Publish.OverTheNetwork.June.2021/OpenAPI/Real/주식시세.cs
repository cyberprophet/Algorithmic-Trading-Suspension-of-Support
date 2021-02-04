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
				analysis.Bid = int.TryParse(API.GetCommRealData(e.sRealKey, Fid[4]), out int bid) ? bid : int.MinValue;
				analysis.Offer = int.TryParse(API.GetCommRealData(e.sRealKey, Fid[3]), out int offer) ? offer : int.MinValue;
				analysis.Current = int.TryParse(API.GetCommRealData(e.sRealKey, Fid[0]), out int current) ? current : 0;
			}
		}
		protected internal override int[] Fid => new int[] { 10, 11, 12, 27, 28, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 311, 567, 568 };
	}
}