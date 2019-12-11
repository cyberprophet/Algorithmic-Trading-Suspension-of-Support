using System.Runtime.InteropServices;

namespace ShareInvest.FontResource
{
    public class AppendFontResource
    {
        [DllImport("gdi32.dll")]
        public static extern int AddFontResource(string fontFilePath);
    }
}