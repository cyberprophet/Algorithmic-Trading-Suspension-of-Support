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
				var data = e.sRealData.Split('\t');

				if (int.TryParse(data[0][0] is '-' ? data[0][1..] : data[0], out int current) && int.TryParse(data[3][0] is '-' ? data[3][1..] : data[3], out int offer) && int.TryParse(data[4][0] is '-' ? data[4][1..] : data[4], out int bid))
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