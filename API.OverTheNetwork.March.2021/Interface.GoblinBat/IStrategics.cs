namespace ShareInvest.Interface
{
	public interface IStrategics
	{
		string Code
		{
			get; set;
		}
		int Short
		{
			get; set;
		}
		int Long
		{
			get; set;
		}
		int Trend
		{
			get; set;
		}
		long Date
		{
			get; set;
		}
	}
	public enum LongShort
	{
		Day = 'D',
		Minute = 'M'
	}
	public enum Trend
	{
		Day = 'd',
		Minute = 'm'
	}
	public enum Setting
	{
		Short = 'S',
		Long = 'L',
		Both = 'B',
		Reservation = 'R'
	}
	public enum Strategics
	{
		Long_Position = 'L',
		TC = 'C',
		TF = 'F',
		TS = 'T',
		TV = 'V',
		ST = 'S',
		SC = 'A'
	}
}