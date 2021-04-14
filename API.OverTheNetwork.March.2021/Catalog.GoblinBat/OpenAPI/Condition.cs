namespace ShareInvest.Catalog.OpenAPI
{
	public class Condition
	{
		public string Code
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string[] Date
		{
			get; set;
		}
		public int[] High
		{
			get; set;
		}
		public int[] Low
		{
			get; set;
		}
	}
}