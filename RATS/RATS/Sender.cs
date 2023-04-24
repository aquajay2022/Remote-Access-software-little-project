using RATS_Sender;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading;

namespace RATS
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("I advise you to read the documentation before usage, to not get confused");
            Console.WriteLine();
            instru instru = new instru();

            IPEndPoint ipend = new IPEndPoint(IPAddress.Loopback, 1024);
            Console.Write("Input your instruction: ");
            instru.inst = Console.ReadLine();
            Console.WriteLine();
            Console.Write($"Supply an argument for {instru.inst}: ");
            instru.arg = Console.ReadLine();
            if (instru.inst == "popup")
            {
                Console.WriteLine();
                Console.Write("Input an extra argument: ");
                instru.arg2 = Console.ReadLine();
            }
            if (instru.inst == "sendmessage")
            {
                if (instru.arg == "specific, close")
                {
                    Console.WriteLine();
                    Console.Write($"Supply a window name: ");
                    instru.arg2 = Console.ReadLine();
                }
                /*if (instru.arg == "specific, close")
                {
                    Console.WriteLine();
                    Console.Write($"Supply a window name: ");
                    instru.arg2 = Console.ReadLine();
                }
                */
            }
            byte[] tosend = JsonSerializer.SerializeToUtf8Bytes(instru);
            
            using (Socket sc = new Socket(SocketType.Stream, ProtocolType.Tcp))
            {
                sc.Connect(ipend);
                sc.Send(tosend);
            }

            Console.ReadKey();
        }
    }
}
