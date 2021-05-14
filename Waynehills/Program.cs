using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using FFmpeg.NET;

namespace Waynehills
{
	class Program
	{
		static async Task Main()
		{
			string ffmpeg, output = string.Empty;

			if (new DirectoryInfo(path).Exists)
				ffmpeg = Path.Combine(path, Program.ffmpeg);

			else
				ffmpeg = Program.ffmpeg;

			var engine = new Engine(ffmpeg);
			var data = new List<MetaData>();

			foreach (var file in Directory.GetFiles(Environment.CurrentDirectory, "*.mp4", SearchOption.AllDirectories))
				if (new FileInfo(file) is FileInfo info && info.Name is string name && name.StartsWith("._") is false)
					data.Add(await engine.GetMetaDataAsync(new MediaFile(info)));

			var options = new ConversionOptions
			{
				VideoBitRate = data.Min(o => o.VideoData.BitRateKbs),
				VideoFps = (int)data.Min(o => o.VideoData.Fps),
				AudioBitRate = data.Find(o => o.AudioData is MetaData.Audio).AudioData.BitRateKbs
			};
			foreach (var con in data)
			{
				var convert = await engine.ConvertAsync(new MediaFile(con.FileInfo), new MediaFile(con.FileInfo.FullName.Replace(".mp4", "r.mp4")), options);
				using (var sw = new StreamWriter(Path.Combine(convert.FileInfo.DirectoryName, list), true))
				{
					sw.WriteLine($"file {convert.FileInfo.Name}");

					if (string.IsNullOrEmpty(output))
						output = Path.Combine(convert.FileInfo.DirectoryName, Program.output);
				}
				Console.WriteLine(await engine.GetMetaDataAsync(convert));
			}
			if (string.IsNullOrEmpty(output) is false)
				await engine.ExecuteAsync($"-f concat -i {output.Replace(Program.output, list)} -c copy {output}");
		}
		const string path = @"C:\ffmpeg-4.4-full_build\bin";
		const string ffmpeg = "ffmpeg.exe";
		const string list = "list.txt";
		const string output = "output.mp4";
	}
}