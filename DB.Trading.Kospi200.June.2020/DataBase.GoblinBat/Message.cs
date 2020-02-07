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
                return progress;
            }
        }
        internal string RemainingTime
        {
            get
            {
                return string.Concat(remaining1, remaining, remaining2, remaining, remaining3);
            }
        }
        internal string Exit
        {
            get
            {
                return exit;
            }
        }
        internal string Identify
        {
            get
            {
                return string.Concat(name, identify);
            }
        }
        internal string GoblinBat
        {
            get
            {
                return goblin;
            }
        }
        internal string Path
        {
            get
            {
                return path;
            }
        }
        private readonly string name;
        private readonly int remaining;
        private const string remaining1 = "안정적인 서버 운영을 위하여\n\n프로그램은 ";
        private const string remaining2 = " 분 동안\n\n대기합니다.\n\n";
        private const string remaining3 = "분이 지나면 자동으로 실행됩니다.";
        private const string progress = "프로그램이 곧 시작합니다.\n\n트레이딩을 위한 데이터를 불러오는데\n\n약 2분 정도 소요됩니다.\n\n'확인'\n\n버튼을 누르지 말고 기다리세요.";
        private const string exit = "데이터 손실 및 수익에 영향을 줄 수 있습니다.\n\n정말 종료하시겠습니까?";
        private const string identify = " 님은 등록되지 않은 사용자입니다.\n\n프로그램을 종료합니다.";
        private const string goblin = "GoblinBat";
        private const string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    }
}