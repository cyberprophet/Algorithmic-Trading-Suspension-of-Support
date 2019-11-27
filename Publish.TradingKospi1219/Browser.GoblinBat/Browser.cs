using System.Windows.Forms;
using System.Drawing;

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