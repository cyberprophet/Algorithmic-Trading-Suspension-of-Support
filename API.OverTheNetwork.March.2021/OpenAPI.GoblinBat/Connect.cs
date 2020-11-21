using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using AxKHOpenAPILib;

using ShareInvest.DelayRequest;
using ShareInvest.Interface;
using ShareInvest.OpenAPI.Catalog;

namespace ShareInvest.OpenAPI
{
    class Connect
    {
        static string LookupScreenNo
        {
            get
            {
                if (Count++ == 0x95)
                    Count = 0;

                return (0xBB8 + Count).ToString("D4");
            }
        }
        static Connect API
        {
            get; set;
        }
        static uint Count
        {
            get; set;
        }
        internal static Connect GetInstance() => API;
        internal static Connect GetInstance(AxKHOpenAPI axAPI, StreamWriter writer)
        {
            if (API == null && axAPI.CommConnect() == 0)
                API = new Connect(axAPI, writer);

            return API;
        }
        internal void InputValueRqData(TR param) => request.RequestTrData(new Task(() =>
        {
            string[] count = param.ID.Split(';'), value = param.Value.Split(';');
            int i, l = count.Length;

            for (i = 0; i < l; i++)
                axAPI.SetInputValue(count[i], value[i]);

            SendErrorMessage(param, axAPI.CommRqData(param.RQName, param.TrCode, param.PrevNext, param.ScreenNo));
        }));
        internal void InputValueRqData(int nCodeCount, TR param)
            => request.RequestTrData(new Task(()
                => SendErrorMessage(param, axAPI.CommKwRqData(param.Value, 0, nCodeCount, param.PrevNext, param.RQName, param.ScreenNo))));
        internal void SendErrorMessage(TR tr, int error)
        {
            if (error < 0 && this.error.TryGetValue(error, out string param))
            {
                switch (error)
                {
                    case -0x6A:
                        tr.SendErrorMessage((short)error);
                        return;
                }
                Base.SendMessage(tr.GetType(), param, error);
            }
        }
        internal void SendOrder(ISendOrder o)
            => request.RequestTrData(new Task(()
                => SendErrorMessage(axAPI.SendOrder(axAPI.GetMasterCodeName(o.Code), LookupScreenNo, o.AccNo, o.OrderType, o.Code, o.Qty, o.Price, o.HogaGb, o.OrgOrderNo))));
        internal string SendErrorMessage(int code) => error[code];
        internal HashSet<TR> TR
        {
            get; private set;
        }
        internal HashSet<Real> Real
        {
            get; private set;
        }
        Connect(AxKHOpenAPI axAPI, StreamWriter server)
        {
            TR = new HashSet<TR>();
            this.axAPI = axAPI;
            request = Delay.GetInstance(0xC9);
            request.Run();
            Real = new HashSet<Real>
            {
                new 주식체결
                {
                    API = axAPI,
                    Server = server
                },
                new 주식호가잔량
                {
                    API = axAPI,
                    Server = server
                },
                new 주식시세
                {
                    API = axAPI,
                    Server = server
                },
                new 주식우선호가
                {
                    API = axAPI,
                    Server = server
                },
                new 장시작시간
                {
                    API = axAPI,
                    Server = server
                },
                new 선물시세
                {
                    API = axAPI,
                    Server = server
                },
                new 선물옵션우선호가
                {
                    API = axAPI,
                    Server = server
                },
                new 선물호가잔량
                {
                    API = axAPI,
                    Server = server
                },
                new 옵션시세
                {
                    API = axAPI,
                    Server = server
                },
                new 옵션호가잔량
                {
                    API = axAPI,
                    Server = server
                }
            };
        }
        readonly Delay request;
        readonly AxKHOpenAPI axAPI;
        readonly Dictionary<int, string> error = new Dictionary<int, string>
        {
            { 0, "정상처리" },
            { -1, "미접속상태" },
            { -10, "실패" },
            { -11, "조건번호 없음" },
            { -12, "조건번호와 조건식 불일치" },
            { -13, "조건검색 조회요청 초과" },
            { -22, "전문 처리 실패" },
            { -100, "사용자정보교환 실패" },
            { -101, "서버 접속 실패" },
            { -102, "버전처리 실패" },
            { -103, "개인방화벽 실패" },
            { -104, "메모리 보호실패" },
            { -105, "함수입력값 오류" },
            { -106, "통신연결 종료" },
            { -107, "보안모듈 오류" },
            { -108, "공인인증 로그인 필요" },
            { -200, "시세조회 과부하" },
            { -201, "전문작성 초기화 실패" },
            { -202, "전문작성 입력값 오류" },
            { -203, "데이터 없음" },
            { -204, "조회가능한 종목수 초과.(한번에 조회 가능한 종목개수는 최대 100종목)" },
            { -205, "데이터 수신 실패" },
            { -206, "조회가능한 FID수 초과.(한번에 조회 가능한 FID개수는 최대 100개)" },
            { -207, "실시간 해제오류" },
            { -209, "시세조회 제한" },
            { -211, "시세조회 과부하" },
            { -300, "입력값 오류" },
            { -301, "계좌비밀번호 없음" },
            { -302, "타인계좌 사용오류" },
            { -303, "주문가격이 주문착오 금액기준 초과." },
            { -304, "주문가격이 주문착오 금액기준 초과." },
            { -305, "주문수량이 총발행주수의 1% 초과오류" },
            { -306, "주문수량은 총발행주수의 3% 초과오류" },
            { -307, "주문전송 실패" },
            { -308, "주문전송 과부하" },
            { -309, "주문수량 300계약 초과" },
            { -310, "주문수량 500계약 초과" },
            { -311, "주문전송제한 과부하" },
            { -340, "계좌정보 없음" },
            { -500, "종목코드 없음" }
        };
    }
}