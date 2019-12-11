using System;
using System.Drawing.Text;
using System.IO;
using System.Windows.Forms;
using Microsoft.Win32;
using ShareInvest.FontResource;

namespace ShareInvest.Font
{
    class RegisterFont
    {
        static void Main()
        {
            try
            {
                foreach (string font in Directory.GetFiles(string.Concat(Environment.CurrentDirectory, @"\Fonts\")))
                {
                    string targetFontFileName = Path.GetFileName(font), targetFontFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Fonts), targetFontFileName);

                    if (!File.Exists(targetFontFilePath))
                        File.Copy(font, targetFontFilePath);

                    PrivateFontCollection collection = new PrivateFontCollection();
                    collection.AddFontFile(targetFontFilePath);
                    AppendFontResource.AddFontResource(targetFontFilePath);
                    Registry.SetValue(@"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Fonts", collection.Families[0].Name, targetFontFileName, RegistryValueKind.String);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Environment.Exit(0);
            }
        }
    }
}