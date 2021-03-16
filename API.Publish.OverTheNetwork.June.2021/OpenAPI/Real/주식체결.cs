using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
	class 주식체결 : Real
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
			var data = e.sRealData.Split('\t');

			if (string.IsNullOrEmpty(data[6]) is false && string.IsNullOrEmpty(data[1]) is false && string.IsNullOrEmpty(data[0]) is false)
			{
				if (Connect.GetInstance().StocksHeld.TryGetValue(e.sRealKey, out Analysis analysis) && int.TryParse(data[4][0] is '-' ? data[4][1..] : data[4], out int offer) && int.TryParse(data[5][0] is '-' ? data[5][1..] : data[5], out int bid))
				{
					analysis.Bid = bid;
					analysis.Offer = offer;
					analysis.OnReceiveEvent(data[0], data[1], data[6]);
				}
				if (Lite)
					Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', data[0], ';', data[1], ';', data[6]));
			}
		}
		internal override bool Lite
		{
			get; set;
		}
		protected internal override int[] Fid => new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 25, 26, 29, 30, 31, 32, 228, 311, 290, 691, 567, 568 };
	}
}