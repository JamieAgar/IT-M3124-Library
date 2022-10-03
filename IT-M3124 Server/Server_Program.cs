using System;
using System.Net;
using IT_M3124_Library;

namespace IT_M3124_Server
{
    //Used to see the interactions between the client and server when creating them manually.
    internal class Server_Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello world!");

            int port = 4000;

            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ip = host.AddressList[0];

            IT_M3124_Emulator server = new IT_M3124_Emulator(ip, port);
            server.StartServer();
        }
    }
}
