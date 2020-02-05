namespace ShareInvest.GoblinBatForms
{
    internal partial class Message
    {
        internal Message()
        {

        }
        internal Message(int remaining)
        {
            this.remaining = remaining;
        }
        internal Message(string name)
        {
            this.name = name;
        }
        internal string StartProgress
        {
            get
            {
                return "프로그램이 곧 시작합니다.\n\n트레이딩을 위한 데이터를 불러오는데\n\n약 2분 정도 소요됩니다.\n\n'확인'\n\n버튼을 누르지 말고 기다리세요.";
            }
        }
        internal string RemainingTime
        {
            get
            {
                return string.Concat("안정적인 서버 운영을 위하여\n\n프로그램은 ", remaining, " 분 동안\n\n대기합니다.\n\n", remaining, "분이 지나면 자동으로 실행됩니다.");
            }
        }
        internal string Exit
        {
            get
            {
                return "데이터 손실 및 수익에 영향을 줄 수 있습니다.\n\n정말 종료하시겠습니까?";
            }
        }
        internal string Identify
        {
            get
            {
                return string.Concat(name, " 님은 등록되지 않은 사용자입니다.\n\n프로그램을 종료합니다.");
            }
        }
        private readonly string name;
        private readonly int remaining;
    }
}