using System.Text;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    public class Temporary : AuxiliaryFunction
    {
        public Temporary()
        {
            Temp = new StringBuilder(1024);
            ConnectAPI.GetInstance().SendMemorize += OnReceiveMemorize;
        }
        private void OnReceiveMemorize(object sender, Memorize e)
        {
            if (e.SPrevNext != null)
            {
                SetStorage(e.Code, Temp.ToString().Split(';'));
                Temp = new StringBuilder(1024);

                return;
            }
            Temp.Append(string.Concat(e.Date, ",", e.Price, ",", e.Volume)).Append(';');
        }
        private StringBuilder Temp
        {
            get; set;
        }
    }
}