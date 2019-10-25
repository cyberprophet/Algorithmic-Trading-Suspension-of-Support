using System.Diagnostics;

namespace ShareInvest.Tistory
{
    public class ConnectTistory
    {
        public ConnectTistory()
        {
            SetToken();
        }
        private void SetToken()
        {
            Process.Start("https://www.tistory.com/oauth/authorize?client_id=cf431d8a1d9913741dd6393ddd3c8105&redirect_uri=https://sharecompany.tistory.com&response_type=token");
        }
    }
}