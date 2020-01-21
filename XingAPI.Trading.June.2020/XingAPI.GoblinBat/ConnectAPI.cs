using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XA_DATASETLib;
using XA_SESSIONLib;

namespace ShareInvest.XingAPI
{
    public class ConnectAPI
    {
        public ConnectAPI()
        {
            session = new XASessionClass();
            query = new XAQueryClass();
            real = new XARealClass();
            SetLogin(new Secret(), session.ConnectServer(server[1], 20001));
        }
        private void SetLogin(Secret secret, bool login)
        {
            if (login && session.Login(secret.id, secret.password, secret.cert, 0, true))
            {
                session._IXASessionEvents_Event_Login += ConnectAPIIXASessionEventsEventLogin;

                return;
            }
        }
        private void ConnectAPIIXASessionEventsEventLogin(string szCode, string szMsg)
        {
            if (szCode.Equals("0000"))
            {
                for (int i = 0; i < session.GetAccountListCount(); i++)
                    session.GetAccountList(i);

                query.ReceiveData += QueryReceiveData;
                query.ReceiveMessage += QueryReceiveMessage;
                query.ResFileName = Array.Find(this.res, o => o.Contains("t8435"));
                var res = GetInBlock(new Res(), true);
                query.SetFieldData(res.InBlock, res.Filed, res.OccursIndex, "MF");

                if (query.Request(false) < 0)
                {

                }
            }
        }
        private Res GetInBlock(Res res, bool set)
        {
            foreach (string str in query.GetResData().Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                if (str.Contains("레코드명:") && str.Contains("InBlock"))
                {
                    res.InBlock = str.Replace("*", string.Empty).Replace("레코드명:", string.Empty).Trim();

                    continue;
                }
                else if (str.Contains("레코드명:") && str.Contains("OutBlock"))
                {
                    OutBlock = str.Replace("*", string.Empty).Replace("레코드명:", string.Empty).Trim();
                    FieldName = new List<string>();
                    set = false;

                    continue;
                }
                else if (str.Contains("No,한글명,필드명,영문명,"))
                    continue;

                var temp = str.Split(',');

                if (set)
                {
                    res.Filed = temp[2];
                    res.OccursIndex = int.Parse(temp[6]);

                    continue;
                }
                FieldName.Add(temp[2]);
            }
            return res;
        }
        private void QueryReceiveMessage(bool bIsSystemError, string nMessageCode, string szMessage)
        {

        }
        private void QueryReceiveData(string szTrCode)
        {
            foreach (string field in FieldName)
                for (int i = 0; i < query.GetBlockCount(OutBlock); i++)
                    query.GetFieldData(OutBlock, field, i);
        }
        private List<string> FieldName
        {
            get; set;
        }
        private string OutBlock
        {
            get; set;
        }
        private readonly XASessionClass session;
        private readonly XAQueryClass query;
        private readonly XARealClass real;
        private readonly string[] server = { "hts.ebestsec.co.kr", "demo.ebestsec.co.kr" };
        private readonly string[] res = Directory.GetFiles(path, "*.res");
        private const string path = @"C:\eBEST\xingAPI\Res";
    }
}