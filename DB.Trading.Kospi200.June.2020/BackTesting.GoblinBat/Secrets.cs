using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;

namespace ShareInvest.Strategy
{
    class Secrets
    {
        internal string Storage
        {
            get
            {
                if (DateTime.TryParseExact(message, Secrets.date, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime date))
                    return string.Concat(date.ToLongDateString(), storage);

                return string.Concat(DateTime.Now.ToLongTimeString(), failure);
            }
        }
        internal int Today
        {
            get
            {
                switch (DateTime.Now.DayOfWeek)
                {
                    case DayOfWeek.Monday:
                        if (DateTime.Now.Hour < 16)
                            return -3;

                        break;

                    case DayOfWeek.Sunday:
                        return -2;

                    case DayOfWeek.Saturday:
                        return -1;

                    default:
                        if (DateTime.Now.Hour < 16)
                            return -1;

                        break;
                }
                return 0;
            }
        }
        internal Secrets()
        {

        }
        internal Secrets(string message) => this.message = message;
        internal string Message => retry;
        internal bool State => Connect[message].Equals((char)Port.Trading);
        internal bool GetHoliday(DateTime date) => Array.Exists(holidays, o => o.Equals(date.ToString(Secrets.date)));
        internal string Path(string game, int games) => string.Concat(Application.StartupPath, @"\Identify\", game, games, ".csv");
        Dictionary<string, char> Connect => new Dictionary<string, char>()
        {
            {
                cyberprophet,
                (char)Port.Collecting
            },
            {
                shareinvest,
                (char)Port.Collecting
            },
            {
                s0,
                (char)Port.Trading
            },
            {
                s1,
                (char)Port.Collecting
            },
            {
                s2,
                (char)Port.Collecting
            },
            {
                t0,
                (char)Port.Trading
            },
            {
                t1,
                (char)Port.Trading
            },
            {
                t2,
                (char)Port.Trading
            }
        };
        readonly string[] holidays = { "201231", "201225", "201009", "201002", "201001", "200930", "200505", "200501", "200430", "200415" };
        readonly string message;
        const string retry = "자료 분석 중 알 수 없는 문제가 발생하였습니다.\n\n다시 시도해주세요.";
        const string date = "yyMMdd";
        const string failure = "알 수 없는 문제가 발생하였습니다.\n\n로그를 분석하세요.";
        const string storage = "\n\n데이터 분석이 성공적으로 완료되었습니다.\n\n이어서 다음 데이터를 분석합니다.";
        const string shareinvest = "VK7JG-NPHTM-C97JM-9MPGT-3V66T";
        const string cyberprophet = "YTMG3-N6DKC-DKB77-7M9GH-8HVX7";
        const string s0 = "RGND7-CX7H4-X969H-Y698R-369QV";
        const string s1 = "XXNBW-8QH94-CH8TB-XT22W-GVJVH";
        const string s2 = "7NJRG-WQ6PR-398CP-D7RJG-J2QDV";
        const string t0 = "3D4QN-KP34D-G4X4J-RY8JH-MBHX7";
        const string t1 = "9CNXC-C4VVD-9TDXH-VKDJB-CWF7H";
        const string t2 = "WTH6G-RKN4R-F3Q8J-HQPJ2-CKCHH";
    }
    enum Port
    {
        Collecting = 67,
        Trading = 84
    }
}