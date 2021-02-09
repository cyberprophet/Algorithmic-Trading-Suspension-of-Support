using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
	class 주식우선호가 : Real
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
			var param = base.OnReceiveRealData(e, Fid);

			if (Connect.GetInstance().StocksHeld.TryGetValue(e.sRealKey, out Analysis analysis) && int.TryParse(param[0][0] is '-' ? param[0][1..] : param[0], out int offer) && int.TryParse(param[1][0] is '-' ? param[1][1..] : param[1], out int bid))
			{
				analysis.Bid = bid;
				analysis.Offer = offer;
			}
		}
		internal override bool Lite
		{
			get; set;
		}
		protected internal override int[] Fid => new int[] { 0x1B, 0x1C };
	}
}