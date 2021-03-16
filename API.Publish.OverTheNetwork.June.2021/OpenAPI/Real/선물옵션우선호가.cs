using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
	class 선물옵션우선호가 : Real
	{
		internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			var data = e.sRealData.Split('\t');

			if (string.IsNullOrEmpty(data[0]) is false && string.IsNullOrEmpty(data[1]) is false && string.IsNullOrEmpty(data[2]) is false)
			{
				if (Lite)
					Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', data[0], ';', data[1], ';', data[2]));
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