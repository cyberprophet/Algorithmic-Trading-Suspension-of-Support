using ShareInvest.Communicate;

namespace ShareInvest.Const
{
    public class FreeVersion : IConfirm
    {
        public string Confirm
        {
            get; private set;
        }
        public bool Identify(string id, string name)
        {
            Confirm = string.Concat(id.Substring(0, 1).ToUpper(), id.Substring(1));

            return true;
        }
    }
}