using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using ShareInvest.EventHandler;

namespace ShareInvest.OpenAPI
{
    public class TemporaryStorage
    {
        public TemporaryStorage(SqlConnection sql)
        {
            this.sql = sql;
            ConnectAPI.Get().SendMemorize += OnReceiveMemorize;
        }
        private void OnReceiveMemorize(object sender, Memorize e)
        {
            new Task(() =>
            {
                if (e.Date.Equals(Date))
                    Count--;

                else
                {
                    Count = 99;
                    Date = e.Date;
                }
                string date = string.Concat(e.Date, Count.ToString("D2")), table = e.Code.Contains("101") ? "Futures" : "Options";
                var command = sql.CreateCommand();
                command.CommandText = string.Concat("BEGIN IF NOT EXISTS ( SELECT Date FROM dbo.", table, " WHERE Date= ", date, " ) BEGIN INSERT INTO dbo.", table, " VALUES ( '", e.Code, "', ", date, ", ", e.Price, ", ", e.Volume, ") END END");
                command.CommandTimeout = 0;
                command.CommandType = CommandType.Text;
                command.ExecuteNonQuery();
            }).Start();
        }
        private string Date
        {
            get; set;
        }
        private int Count
        {
            get; set;
        }
        private readonly SqlConnection sql;
    }
}