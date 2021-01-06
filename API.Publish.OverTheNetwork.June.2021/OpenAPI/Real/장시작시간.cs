using System;
using System.IO;

using AxKHOpenAPILib;

using ShareInvest.Catalog.OpenAPI;
using ShareInvest.DelayRequest;
using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI.Catalog
{
	class 장시작시간 : Real, ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		internal override void OnReceiveRealData(_DKHOpenAPIEvents_OnReceiveRealDataEvent e)
		{
			var param = base.OnReceiveRealData(e, Fid);
			var operation = Enum.ToObject(typeof(Operation), int.TryParse(param[0], out int number) ? number : char.TryParse(param[0], out char initial) ? initial : null);

			switch (operation)
			{
				case Operation.장시작전:
					if (reservation.Equals(param[1]))
						Delay.Milliseconds = 0xE7;

					break;

				case Operation.장시작:
					Delay.Milliseconds = 0xC9;
					break;

				case Operation.선옵_장마감전_동시호가_종료:
					Delay.Milliseconds = 0xE01;
					break;

				case Operation.장마감:
					Delay.Milliseconds = 0xE11;
					break;
			}
			if (operation is not null)
				Send?.Invoke(this, new SendSecuritiesAPI((Operation)operation, param[1], param[^1]));

			if (string.IsNullOrEmpty(param[2]) is false && string.IsNullOrEmpty(param[1]) is false && string.IsNullOrEmpty(param[0]) is false)
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
		readonly string reservation = Base.CheckIfMarketDelay(DateTime.Now) ? "095500" : "085500";
		protected internal override int[] Fid => new int[] { 215, 20, 214 };
		public event EventHandler<SendSecuritiesAPI> Send;
	}
}