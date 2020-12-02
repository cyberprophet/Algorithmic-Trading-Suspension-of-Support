using System;
using System.IO;

using AxKHOpenAPILib;

using ShareInvest.DelayRequest;

namespace ShareInvest.OpenAPI.Catalog
{
	class 장시작시간 : Real
	{
		internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			var param = base.OnReceiveRealData(e, Fid);

			if (char.TryParse(param[0], out char operation))
				switch (operation)
				{
					case '0':
						if (reservation.Equals(param[1]))
							Delay.Milliseconds = 0xE7;

						break;

					case '3':
						Delay.Milliseconds = 0xC9;
						break;

					case 'e':
						Delay.Milliseconds = 0xE17;
						break;

					case '8':
						Delay.Milliseconds = 0xE11;
						break;
				}
			if (string.IsNullOrEmpty(param[2]) == false && string.IsNullOrEmpty(param[1]) == false && string.IsNullOrEmpty(param[0]) == false)
				Server.WriteLine(string.Concat(e.sRealType, '|', e.sRealKey, '|', param[0], ';', param[1], ';', param[^1]));
		}
		internal override AxKHOpenAPI API
		{
			get; set;
		}
		internal override StreamWriter Server
		{
			get; set;
		}
		protected internal override int[] Fid => new int[] { 215, 20, 214 };
		readonly string reservation = Array.Exists(Base.SAT, o => o.Equals(DateTime.Now.ToString(Base.DateFormat))) ? "095500" : "085500";
	}
}