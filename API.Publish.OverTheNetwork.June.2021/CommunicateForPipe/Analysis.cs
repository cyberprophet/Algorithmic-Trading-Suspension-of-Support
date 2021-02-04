using System;
using System.Collections.Generic;

using ShareInvest.Catalog.Models;
using ShareInvest.EventHandler;

namespace ShareInvest
{
	public abstract class Analysis
	{
		public abstract event EventHandler<SendConsecutive> Consecutive;
		public abstract event EventHandler<SendSecuritiesAPI> Send;
		public abstract Balance OnReceiveBalance(string kiwoom, Dictionary<int, string> balance);
		public abstract int OnReceiveConclusion(Dictionary<int, string> conclusion);
		public abstract void OnReceiveEvent(string time, string price, string volume);
		public abstract void OnReceiveDrawChart(object sender, SendConsecutive e);
		public abstract bool GetCheckOnDate(string date);
		public abstract string Code
		{
			get; set;
		}
		public abstract string Name
		{
			get; set;
		}
		public abstract string Account
		{
			get; set;
		}
		public abstract string Memo
		{
			get; set;
		}
		public abstract int Quantity
		{
			get; set;
		}
		public abstract double Purchase
		{
			get; set;
		}
		public abstract dynamic Current
		{
			get; set;
		}
		public abstract dynamic Offer
		{
			get; set;
		}
		public abstract dynamic Bid
		{
			get; set;
		}
		public abstract double Rate
		{
			get; set;
		}
		public abstract double MarketMarginRate
		{
			get; set;
		}
		public abstract long Revenue
		{
			get; set;
		}
		public abstract bool Wait
		{
			get; set;
		}
		public abstract Interface.Strategics Strategics
		{
			get; set;
		}
		public abstract Interface.IStrategics Classification
		{
			get; set;
		}
		public abstract Dictionary<string, dynamic> OrderNumber
		{
			get; set;
		}
		public abstract Stack<double> Trend
		{
			get; set;
		}
		public abstract Stack<double> Long
		{
			get; set;
		}
		public abstract Stack<double> Short
		{
			get; set;
		}
		internal abstract dynamic SellPrice
		{
			get; set;
		}
		internal abstract dynamic BuyPrice
		{
			get; set;
		}
		protected internal abstract string DateLine
		{
			get; set;
		}
		protected internal abstract double Gap
		{
			get; set;
		}
		protected internal abstract double Peek
		{
			get; set;
		}
		protected internal abstract DateTime NextOrderTime
		{
			get; set;
		}
		protected internal const string conclusion = "체결";
		protected internal const string acceptance = "접수";
		protected internal const string confirmation = "확인";
		protected internal const string cancellantion = "취소";
		protected internal const string correction = "정정";
	}
}