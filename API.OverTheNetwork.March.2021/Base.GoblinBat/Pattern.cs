namespace ShareInvest
{
	public static class Pattern
	{
		public static string RegexSymbol => @"[-“”‘’…‧~!@#$%^&*()_+|<>?:;{}\]→[.,·'""+=`/\n\r\t\v\s\b]";
		public static string RegexHtml => "<[^>]+>";
		public static string RegexScript => "<(script|style)[^>]*>[\\s\\S]*?</\\1>";
		public static string RegexParticle => "(은|는|이|가|에|께|의|을|를|와|과|습니|입니|님)";
	}
}