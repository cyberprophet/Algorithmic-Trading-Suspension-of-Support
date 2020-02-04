namespace ShareInvest.OpenAPI
{
    internal partial class Message
    {
        internal string SetPassword
        {
            get
            {
                return "비밀번호 설정을 하시겠습니까?\n\n자동로그인 기능을 사용하시면 편리합니다.";
            }
        }
        internal string OnReceiveData
        {
            get
            {
                return "자료 수집 중. . .\n\n약 2분 정도 소요됩니다.\n\n잠시만 기다려주세요.";
            }
        }
        internal string TR
        {
            get
            {
                return "서비스 TR을 확인바랍니다.(0006)";
            }
        }
        internal string Failure
        {
            get
            {
                return "전문 처리 실패(-22)";
            }
        }
        internal string[] Basic
        {
            get
            {
                return new string[]
                {
                    "모의투자 선물옵션 신규주문 완료",
                    "모의투자 정상처리 되었습니다",
                    "모의투자 정정/취소할 수량이 없습니다"
                };
            }
        }
    }
}