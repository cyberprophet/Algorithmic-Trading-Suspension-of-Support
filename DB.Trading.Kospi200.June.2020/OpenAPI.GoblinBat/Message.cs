using System.Text;

namespace ShareInvest.OpenAPI
{
    internal partial class Message
    {
        internal string SetPassword
        {
            get
            {
                return password;
            }
        }
        internal string OnReceiveData
        {
            get
            {
                return data;
            }
        }
        internal string TR
        {
            get
            {
                return tr;
            }
        }
        internal string Failure
        {
            get
            {
                return failure;
            }
        }
        internal string LookUp
        {
            get
            {
                return lookUp;
            }
        }
        internal string GoblinBat
        {
            get
            {
                return goblin;
            }
        }
        internal string Restart
        {
            get
            {
                return restart;
            }
        }
        internal StringBuilder Exists
        {
            get
            {
                return new StringBuilder(exists);
            }
        }
        internal readonly string[] basic =
        {
            "모의투자 선물옵션 신규주문 완료",
            "모의투자 정상처리 되었습니다",
            "모의투자 정정/취소할 수량이 없습니다",
            "모의투자 주문가능 금액을 확인하세요",
            "모의투자 정정가격이 원주문가격과 같습니다"
        };
        internal readonly string[] exclude =
        {
            "115960",
            "006800",
            "001880",
            "072770"
        };
        private const string restart = "모의투자 서비스 지연입니다. 잠시후 재시도 바랍니다..";
        private const string exists = "Information that already Exists";
        private const string lookUp = "모의투자 조회가 완료되었습니다";
        private const string failure = "전문 처리 실패(-22)";
        private const string tr = "서비스 TR을 확인바랍니다.(0006)";
        private const string data = "트레이딩에 필요한 자료를 수집합니다.\n\n잠시만 기다려주세요.";
        private const string password = "비밀번호 설정을 하시겠습니까?\n\n자동로그인 기능을 사용하시면 편리합니다.";
        private const string goblin = "GoblinBat";
    }
}