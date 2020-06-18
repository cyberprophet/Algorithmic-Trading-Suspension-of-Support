using System;
using System.IO;

namespace ShareInvest.Cummunication
{
    public class ExceptionMessage
    {
        string Code
        {
            get; set;
        }
        void Record(string message)
        {
            try
            {
                var directory = new DirectoryInfo(Secrecy.MessagePath);
                var benchmark = DateTime.Now.AddDays(-30);

                if (directory.Exists)
                    foreach (var file in Directory.GetFiles(Secrecy.MessagePath))
                    {
                        var exists = new FileInfo(file);

                        if (DateTime.Compare(exists.CreationTime, benchmark) < 0)
                            exists.Delete();
                    }
                else
                    directory.Create();

                using (var sw = new StreamWriter(string.Concat(Secrecy.MessagePath, DateTime.Now.ToString(Secrecy.DateFormat), Secrecy.Text), true))
                {
                    if (Code != null)
                        sw.WriteLine(Code);

                    sw.WriteLine(DateTime.Now.ToShortTimeString());
                    sw.WriteLine(message);
                }
            }
            catch (Exception ex)
            {
                Code = ex.TargetSite.Name;
                Record(message);
            }
        }
        public ExceptionMessage(string code, string message)
        {
            Code = code;
            Record(message);
        }
        public ExceptionMessage(string message) => Record(message);
    }
}