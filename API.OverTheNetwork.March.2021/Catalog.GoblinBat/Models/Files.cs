using System;

namespace ShareInvest.Catalog.Models
{
	public struct Files
	{
		public string Path
		{
			get; set;
		}
		public string Name
		{
			get; set;
		}
		public string ID
		{
			get; set;
		}
		public byte[] Contents
		{
			get; set;
		}
		public DateTime LastWriteTime
		{
			get; set;
		}
	}
}