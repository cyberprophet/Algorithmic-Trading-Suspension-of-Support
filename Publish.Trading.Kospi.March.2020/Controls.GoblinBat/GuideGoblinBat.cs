using System;
using System.Windows.Forms;
using ShareInvest.FindByName;
using ShareInvest.WebBrowsers;

namespace ShareInvest.Controls
{
    public partial class GuideGoblinBat : UserControl
    {
        public GuideGoblinBat()
        {
            InitializeComponent();

            for (int i = 0; i < 5; i++)
            {
                string.Concat("connect", i).FindByName<Button>(this).Click += ButtonClick;
                string.Concat("connect", i).FindByName<Button>(this).Text = name[i];
            }
        }
        private void ButtonClick(object sender, EventArgs e)
        {
            using Browser browser = new Browser();

            if (sender.Equals(connect0))
                browser.GetWebBrowser(@"https://www.youtube.com/channel/UC7AfV6_2hoQ27XBxQH9P3lA/videos?view_as=subscriber", 1355, 741);

            else if (sender.Equals(connect1))
                browser.GetWebBrowser(@"https://www.youtube.com/watch?v=X0m2mpZ1CSg", 685, 485);

            else if (sender.Equals(connect2))
                browser.GetWebBrowser(@"https://youtu.be/jl_OLK3Alog", 685, 485);

            else if (sender.Equals(connect3))
                browser.GetWebBrowser(@"https://youtu.be/d1MQsMr4pxQ", 685, 485);

            else if (sender.Equals(connect4))
                browser.GetWebBrowser(@"https://sharecompany.tistory.com/guestbook", 855, 983);

            browser.ShowDialog();
        }
        private readonly string[] name = { "Manual", "Intro", "Overall Description", "", "GuestBook" };
    }
}