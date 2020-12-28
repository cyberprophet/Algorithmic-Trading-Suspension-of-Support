using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models
{
	public class QuarterlyFinancialStatements
	{
		[ForeignKey("Codes"), Column(Order = 1), MinLength(6), MaxLength(8)]
		public string Code
		{
			get; set;
		}
		[Column(Order = 2), StringLength(8)]
		public string Date
		{
			get; set;
		}
		[Column(Order = 3)]
		public string Revenues
		{
			get; set;
		}
		[Column(Order = 4)]
		public string IncomeFromOperations
		{
			get; set;
		}
		[Column(Order = 5)]
		public string IncomeFromOperation
		{
			get; set;
		}
		[Column(Order = 6)]
		public string ProfitFromContinuingOperations
		{
			get; set;
		}
		[Column(Order = 7)]
		public string NetIncome
		{
			get; set;
		}
		[Column(Order = 8)]
		public string ControllingNetIncome
		{
			get; set;
		}
		[Column(Order = 9)]
		public string NonControllingNetIncome
		{
			get; set;
		}
		[Column(Order = 0xA)]
		public string TotalAssets
		{
			get; set;
		}
		[Column(Order = 0xB)]
		public string TotalLiabilites
		{
			get; set;
		}
		[Column(Order = 0xC)]
		public string TotalEquity
		{
			get; set;
		}
		[Column(Order = 0xD)]
		public string ControllingEquity
		{
			get; set;
		}
		[Column(Order = 0xE)]
		public string NonControllingEquity
		{
			get; set;
		}
		[Column(Order = 0xF)]
		public string EquityCapital
		{
			get; set;
		}
		[Column(Order = 0x10)]
		public string OperatingActivities
		{
			get; set;
		}
		[Column(Order = 0x11)]
		public string InvestingActivities
		{
			get; set;
		}
		[Column(Order = 0x12)]
		public string FinancingActivities
		{
			get; set;
		}
		[Column(Order = 0x13)]
		public string CAPEX
		{
			get; set;
		}
		[Column(Order = 0x14)]
		public string FCF
		{
			get; set;
		}
		[Column(Order = 0x15)]
		public string InterestAccruingLiabilities
		{
			get; set;
		}
		[Column(Order = 0x16)]
		public string OperatingMargin
		{
			get; set;
		}
		[Column(Order = 0x17)]
		public string NetMargin
		{
			get; set;
		}
		[Column(Order = 0x18)]
		public string ROE
		{
			get; set;
		}
		[Column(Order = 0x19)]
		public string ROA
		{
			get; set;
		}
		[Column(Order = 0x1A)]
		public string DebtRatio
		{
			get; set;
		}
		[Column(Order = 0x1B)]
		public string RetentionRatio
		{
			get; set;
		}
		[Column(Order = 0x1C)]
		public string EPS
		{
			get; set;
		}
		[Column(Order = 0x1D)]
		public string PER
		{
			get; set;
		}
		[Column(Order = 0x1E)]
		public string BPS
		{
			get; set;
		}
		[Column(Order = 0x1F)]
		public string PBR
		{
			get; set;
		}
		[Column(Order = 0x20)]
		public string DPS
		{
			get; set;
		}
		[Column(Order = 0x21)]
		public string DividendYield
		{
			get; set;
		}
		[Column(Order = 0x22)]
		public string PayoutRatio
		{
			get; set;
		}
		[Column(Order = 0x23)]
		public string IssuedStocks
		{
			get; set;
		}
	}
}