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
        private readonly int remaining;
    }
}