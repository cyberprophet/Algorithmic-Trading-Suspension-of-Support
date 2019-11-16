namespace ShareInvest.FindbyOptionsCode
{
    public class FindbyOptions
    {
        public string Code(string code)
        {
            int temp = int.Parse(code.Substring(5)), point = int.Parse(code.Substring(7));

            return string.Concat(code.Substring(0, 4), code.Substring(0, 3).Equals("201") ? temp + (point == 2 || point == 7 ? 3 : 2) : temp - (point == 2 || point == 7 ? 2 : 3));
        }
    }
}