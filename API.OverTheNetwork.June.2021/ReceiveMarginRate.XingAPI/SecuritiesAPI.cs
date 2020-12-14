using System;
using System.Windows.Forms;

namespace ShareInvest
{
	sealed partial class SecuritiesAPI : Form
	{
		internal SecuritiesAPI(dynamic args)
		{
			InitializeComponent();
			this.args = args;
			timer.Start();
		}
		void TimerTick(object sender, EventArgs e)
		{
			if (FormBorderStyle.Equals(FormBorderStyle.Sizable) && WindowState.Equals(FormWindowState.Minimized) == false)
			{
				WindowState = FormWindowState.Minimized;
				Visible = false;
				ShowIcon = false;
				notifyIcon.Visible = true;
			}
			else if (Visible == false && ShowIcon == false && notifyIcon.Visible && WindowState.Equals(FormWindowState.Minimized))
			{
				var now = DateTime.Now;
				now = now.DayOfWeek switch
				{
					DayOfWeek.Sunday => now.AddDays(1),
					DayOfWeek.Saturday => now.AddDays(2),
					DayOfWeek weeks when weeks.Equals(DayOfWeek.Friday) && now.Hour > 8 => now.AddDays(3),
					_ => now.Hour > 8 || Array.Exists(Base.Holidays, o => o.Equals(now.ToString(Base.DateFormat))) ? now.AddDays(1) : now,
				};
				var sat = Base.CheckIfMarketDelay(now);
				var remain = new DateTime(now.Year, now.Month, now.Day, sat ? 0xA : 9, 0, 0) - DateTime.Now;
				notifyIcon.Text = Base.GetRemainingTime(remain);

				if (API is null && Base.IsDebug == false && remain.TotalMinutes < 0x29 && now.Hour == (sat ? 9 : 8))
				{
					API = new ConnectAPI(Base.IsDebug ? administrator : args);
					timer.Interval = 0x3A99;
				}
				else if (API is ConnectAPI && API.TryProgress() && timer.Interval == 0x3A99)
				{
					timer.Interval = 0x3E9;
					API.StartProgress();
				}
				else if (remain.TotalMinutes < 0x23)
				{
					API.Dispose();
					API = null;
					timer.Stop();
					Dispose();
				}
			}
		}
		ConnectAPI API
		{
			get; set;
		}
		readonly dynamic args;
	}
}