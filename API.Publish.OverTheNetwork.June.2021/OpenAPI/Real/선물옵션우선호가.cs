using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
	class 선물옵션우선호가 : Real
	{
		internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			var param = base.OnReceiveRealData(e, Fid);

			if (string.IsNullOrEmpty(param[0]) is false && string.IsNullOrEmpty(param[1]) is false && string.IsNullOrEmpty(param[2]) is false)
			{
				if (Lite)
					Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', param[0], ';', param[1], ';', param[2]));
			}
		}
		internal override AxKHOpenAPI API
		{
			get; set;
		}
		internal override StreamWriter Server
		{
			get; set;
		}
		internal override bool Lite
		{
			get; set;
		}
		protected internal override int[] Fid => new int[] { 10, 27, 28 };
	}
}