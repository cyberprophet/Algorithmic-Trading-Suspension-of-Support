using System;

using AxKHOpenAPILib;

using ShareInvest.EventHandler;
using ShareInvest.Interface.OpenAPI;

namespace ShareInvest.OpenAPI
{
	abstract class Chejan : ISendSecuritiesAPI<SendSecuritiesAPI>
	{
		public abstract event EventHandler<SendSecuritiesAPI> Send;
		protected internal abstract AxKHOpenAPI API
		{
			get; set;
		}
		protected internal abstract string Identity
		{
			get; set;
		}
		internal abstract void OnReceiveChejanData(_DKHOpenAPIEvents_OnReceiveChejanDataEvent e);
	}
	enum ChejanType
	{
		주문체결 = 0,
		잔고 = 1,
		파생잔고 = 4
	}
	enum Conclusion
	{
		계좌번호 = 9201,
		주문번호 = 9203,
		관리자사번 = 9205,
		종목코드_업종코드 = 9001,
		주문업무분류 = 912,
		주문상태 = 913,
		종목명 = 302,
		주문수량 = 900,
		주문가격 = 901,
		미체결수량 = 902,
		체결누계금액 = 903,
		원주문번호 = 904,
		주문구분 = 905,
		매매구분 = 906,
		매도수구분 = 907,
		주문_체결시간 = 908,
		체결번호 = 909,
		체결가 = 910,
		체결량 = 911,
		현재가 = 10,
		매도호가 = 27,
		매수호가 = 28,
		단위체결가 = 914,
		단위체결량 = 915,
		당일매매수수료 = 938,
		당일매매세금 = 939,
		거부사유 = 919,
		화면번호 = 920,
		터미널번호 = 921,
		신용구분 = 922,
		대출일 = 923
	}
	enum Balance
	{
		계좌번호 = 9201,
		종목코드_업종코드 = 9001,
		신용구분 = 917,
		대출일 = 916,
		종목명 = 302,
		현재가 = 10,
		보유수량 = 930,
		매입단가 = 931,
		총매입가 = 932,
		주문가능수량 = 933,
		당일순매수량 = 945,
		매도_매수구분 = 946,
		당일총매도손익 = 950,
		예수금 = 951,
		매도호가 = 27,
		매수호가 = 28,
		기준가 = 307,
		손익율 = 8019,
		신용금액 = 957,
		신용이자 = 958,
		만기일 = 918,
		당일실현손익_유가 = 990,
		당일실현손익률_유가 = 991,
		당일실현손익_신용 = 992,
		당일실현손익률_신용 = 993,
		담보대출수량 = 959,
		ExtraItem = 924
	}
	enum Derivatives
	{
		계좌번호 = 9201,
		종목코드_업종코드 = 9001,
		종목명 = 302,
		현재가 = 10,
		보유수량 = 930,
		매입단가 = 931,
		총매입가 = 932,
		주문가능수량 = 933,
		당일순매수량 = 945,
		매도_매수구분 = 946,
		당일총매도손익 = 950,
		예수금 = 951,
		매도호가 = 27,
		매수호가 = 28,
		기준가 = 307,
		손익율 = 8019,
		파생상품거래단위 = 397,
		상한가 = 305,
		하한가 = 306
	}
}