using System.Collections.Generic;
using System.Linq;

using ShareInvest.Catalog.Models;

namespace ShareInvest
{
	class Sort
	{
		internal (IEnumerable<Collect>, uint, uint, string) TheRecordedInformation(Queue<Collect> collection)
		{
			uint count = 0;
			var queue = new Queue<Collect>(collection.Count);
			var temp = new Dictionary<string, uint>(0x400);
			var storage = new Dictionary<uint, string>(0x800);

			while (collection.TryDequeue(out Collect c))
				if (string.IsNullOrEmpty(c.Time) is false && string.IsNullOrEmpty(c.Datum) is false)
				{
					string index = 0.ToString("D3"), peek = collection.Count > 0 && collection.TryPeek(out Collect p) && string.IsNullOrEmpty(p.Time) is false ? p.Time : c.Time;

					if (c.Time.CompareTo(peek) == 0)
					{
						temp[c.Time] = count;
						index = count++.ToString("D3");
					}
					else
					{
						temp[c.Time] = count;
						index = count.ToString("D3");
						count = temp.Any(o => o.Key.Equals(peek)) ? temp.First(o => o.Key.Equals(peek)).Value + 1 : 0;
					}
					if (uint.TryParse(string.Concat(c.Time, index), out uint time))
						storage[time] = c.Datum;
				}
			foreach (var collect in storage.OrderBy(o => o.Key))
				queue.Enqueue(new Collect
				{
					Time = collect.Key.ToString("D9"),
					Datum = collect.Value
				});
			var max = storage.Max(o => o.Key);

			return (queue, storage.Min(o => o.Key), max, max > 0x91DF818 && code.Length == 6 || max > 0x934DB78 && code.Length == 8 ? (storage[max] is null ? string.Empty : storage[max].Split(';')[0][1..]) : string.Empty);
		}
		internal Sort(string code) => this.code = code;
		readonly string code;
	}
}