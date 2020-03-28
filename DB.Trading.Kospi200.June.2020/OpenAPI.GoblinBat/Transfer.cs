using System.Collections;
using System.IO;
using System.Threading.Tasks;

namespace ShareInvest.OpenAPI
{
    public class Transfer : AuxiliaryFunction, IEnumerable
    {
        public Transfer(string path, string key) : base(key)
        {
            this.path = path;
        }
        public IEnumerator GetEnumerator()
        {
            foreach (string file in Directory.GetFiles(path, "*.csv", SearchOption.AllDirectories))
                using (StreamReader sr = new StreamReader(file))
                {
                    string[] temp = file.Split('\\');
                    string code = temp[temp.Length - 1].Split('.')[0], name = temp[temp.Length - 2];

                    if (!code.Equals("Day") && !code.Equals("Tick"))
                        new Task(() => SetInsertCode(code, string.Concat(code.Substring(0, 3).Equals("201") ? "C " : "P ", name, " ", code.Substring(5, 3), code.Substring(7).Equals("7") || code.Substring(7).Equals("2") ? ".5" : ".0"), GetSecondThursday(name))).Start();

                    if (sr != null)
                        while (sr.EndOfStream == false)
                            yield return sr.ReadLine();

                    yield return code;
                }
        }
        public string GetSecondThursday(string param)
        {
            string date = string.Empty;

            switch (param)
            {
                case "201911":
                    date = "20191114";
                    break;

                case "201912":
                    date = "20191212";
                    break;

                case "202001":
                    date = "20200109";
                    break;

                case "202002":
                    date = "20200213";
                    break;
            }
            return date;
        }
        private readonly string path;
    }
}