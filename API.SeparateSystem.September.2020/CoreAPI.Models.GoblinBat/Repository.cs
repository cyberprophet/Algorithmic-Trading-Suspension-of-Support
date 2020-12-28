using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShareInvest.Models
{
    public class Repository
    {
        Dictionary<string, string> Storage
        {
            get;
        }
        int Length
        {
            get; set;
        }
        string Compress(string param)
        {
            byte[] sourceArray = Encoding.UTF8.GetBytes(param);
            var memoryStream = new MemoryStream();
            using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
                gZipStream.Write(sourceArray, 0, sourceArray.Length);

            byte[] temporaryArray = new byte[memoryStream.Length], targetArray = new byte[temporaryArray.Length + 4];
            memoryStream.Position = 0;
            Length = memoryStream.Read(temporaryArray, 0, temporaryArray.Length);
            Buffer.BlockCopy(temporaryArray, 0, targetArray, 4, temporaryArray.Length);
            Buffer.BlockCopy(BitConverter.GetBytes(sourceArray.Length), 0, targetArray, 0, 4);

            return Convert.ToBase64String(targetArray);
        }
        public Repository(string code)
        {
            this.code = code;
            Storage = new Dictionary<string, string>(0x9C4);
        }
        public void Insert(string code, IEnumerable<Collect> collection)
        {
            if (this.code.Equals(code) && collection != null)
                foreach (var kv in collection)
                    Storage[kv.Date] = kv.Datum;
        }
        public async Task<int> InsertAsync(string code, string date, string param)
        {
            if (this.code.Equals(code))
            {
                var path = Path.Combine(@"D:\", code, date.Substring(0, 4), date.Substring(4, 2));
                var directory = new DirectoryInfo(path);

                if (directory.Exists == false)
                    directory.Create();

                using (var sw = new StreamWriter(string.Concat(path, @"\", date.Substring(6, 2), ".res"), false))
                    await sw.WriteAsync(Compress(param));

                Storage.Clear();
            }
            return Length;
        }
        public Queue<Collect> Sort
        {
            get
            {
                var queue = new Queue<Collect>(Storage.Count);

                foreach (var collect in Storage.OrderBy(o => o.Key))
                    queue.Enqueue(new Collect
                    {
                        Date = collect.Key,
                        Datum = collect.Value
                    });
                return queue;
            }
        }
        public int Count => Storage.Count;
        readonly string code;
    }
}