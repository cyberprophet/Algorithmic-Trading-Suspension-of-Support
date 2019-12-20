using System.Drawing;
using System.Windows.Forms;

namespace ShareInvest.WebBrowsers
{
    public partial class Browser : Form
    {
        public Browser()
        {
            InitializeComponent();
        }
        public void GetWebBrowser(string url, int width, int height)
        {
            Size = new Size(width, height);
            webBrowser.Navigate(url);
            webBrowser.Show();
        }
    }
}