using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Threading;

namespace ShareInvest
{
    static class Server
    {
        [SupportedOSPlatform("windows")]
        internal static void StartProgress(string path, int param)
        {
            if (new Process()
            {
                StartInfo = new ProcessStartInfo()
                {
                    FileName = Security.CoreAPI,
                    Arguments = KeyDecoder.ProductKeyFromRegistry,
                    WorkingDirectory = path
                }
            }.Start())
            {
                GC.Collect();
                Process.GetCurrentProcess().Kill();
            }
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Bind(new IPEndPoint(IPAddress.Any, param));
            socket.Listen();
            new Thread(new ThreadStart(() =>
            {
                while (true)
                {
                    Thread.Sleep(0x64);
                    new Thread(new ParameterizedThreadStart((object socket) =>
                    {
                        if (socket is Socket s)
                        {
                            var receive = OnReceiveUpdateFile(s);

                            if (receive is not null && InstallTheReceiveFile(receive))
                            {
                                foreach (var process in Process.GetProcessesByName(core_api.Split('.')[0], Dns.GetHostName()))
                                    process.Kill();

                                Process.Start(shut, "-r");
                                GC.Collect();
                                Process.GetCurrentProcess().Kill();
                            }
                        }
                    })).Start(socket.Accept());
                }
            })).Start();
        }
        static bool InstallTheReceiveFile(byte[] file)
        {
            File.WriteAllBytes(Path.GetFullPath(string.Concat(Security.ShareInvest, core_api)), file);
            var info = new FileInfo(string.Concat(Security.ShareInvest, core_api));



            using (var archive = ZipFile.OpenRead(info.FullName))
                foreach (var entry in archive.Entries)
                    entry.ExtractToFile(Path.GetFullPath(Path.Combine(Security.Install, entry.FullName)), true);

            info.Delete();

            return false;
        }
        static byte[] OnReceiveUpdateFile(Socket socket)
        {
            try
            {
                byte[] data_size = new byte[4], data;
                int recv_data = socket.Receive(data_size, 0, 4, SocketFlags.None), size = BitConverter.ToInt32(data_size, 0), left_data = size, total = 0;
                data = new byte[size];

                while (total < size)
                {
                    recv_data = socket.Receive(data, total, left_data, 0);

                    if (recv_data == 0)
                        break;

                    total += recv_data;
                    left_data -= recv_data;
                }
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            return null;
        }
        const string shut = "shutdown.exe";
        const string core_api = "CoreAPI.zip";
    }
}