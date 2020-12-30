using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ShareInvest.EventHandler;

namespace ShareInvest
{
	sealed partial class CoreAPI : Form
	{
		internal CoreAPI(dynamic param)
		{
			security = param;
			InitializeComponent();
			(security as Security).Send += OnReceiveSecuritiesAPI;
			icon = new[] { Properties.Resources.upload_server_icon_icons_com_76732, Properties.Resources.download_server_icon_icons_com_76720, Properties.Resources.data_server_icon_icons_com_76718 };
			timer.Start();
		}
		string Message
		{
			get; set;
		}
		void OnReceiveSecuritiesAPI(object sender, SendSecuritiesAPI e)
		{
			switch (e.Convey)
			{
				case string:
					Message = e.Convey as string;
					return;
			}
		}
		void TimerTick(object sender, EventArgs e)
		{
			if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) is false)
			{
				(security as Security).StartProgress();
				WindowState = FormWindowState.Minimized;
				Visible = false;
				ShowIcon = false;
				notifyIcon.Visible = true;
			}
			else if (Visible is false && ShowIcon is false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized) && string.IsNullOrEmpty(Message) is false)
			{
				if (Message.EndsWith(false.ToString()))
					notifyIcon.Icon = icon[^1];

				else
				{
					var now = DateTime.Now;
					notifyIcon.Icon = icon[now.Second % 2];
				}
				notifyIcon.Text = Message;
			}
		}
		readonly dynamic security;
		readonly Icon[] icon;
	}
}