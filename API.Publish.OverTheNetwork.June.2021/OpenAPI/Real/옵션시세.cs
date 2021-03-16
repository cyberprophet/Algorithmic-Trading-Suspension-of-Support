using System.IO;

using AxKHOpenAPILib;

namespace ShareInvest.OpenAPI.Catalog
{
	class 옵션시세 : Real
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
				if (Lite)
					Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', data[0], ';', data[1], ';', data[6]));
			}
		}
		internal override bool Lite
		{
			get; set;
		}
		protected internal override int[] Fid => new int[] { 20, 10, 11, 12, 27, 28, 15, 13, 14, 16, 17, 18, 195, 182, 186, 190, 191, 193, 192, 194, 181, 25, 26, 137, 187, 197, 246, 247, 248, 219, 196, 188, 189, 30, 391, 392, 393, 1365, 1366, 1367, 305, 306 };
	}
}