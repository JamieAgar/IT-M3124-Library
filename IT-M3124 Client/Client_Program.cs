using System;
using System.Net;
using IT_M3124_Library;

namespace IT_M3124_Client
{
    //Used to see the interactions between the client and server when creating them manually.
    internal class Client_Program
    {
        static void Main(string[] args)
        {
            int port = 4000;

            IPHostEntry host = Dns.GetHostEntry("localhost");
            IPAddress ip = host.AddressList[0];
            IT_M3124 controller = new IT_M3124(ip, port);

            controller.SetRemote();

            //Proof of concept. Another way would be to parse commands and send them directly to the power supply

            Console.WriteLine("1: Get Current");
            Console.WriteLine("2: Set Current");
            Console.WriteLine("3: Get Voltage");
            Console.WriteLine("4: Set Voltage");
            Console.WriteLine("5: Get Output State");
            Console.WriteLine("6: Set Output State");
            Console.WriteLine("q: Leave");
            bool cont = true;
            while (cont)
            {
                string input = Console.ReadLine();
                switch (input)
                {
                    case "1":
                        Console.WriteLine(controller.GetCurrent());
                        break;
                    case "2":
                        Console.WriteLine("What current? <double>");
                        controller.SetCurrent(double.Parse(Console.ReadLine()));
                        break;
                    case "3":
                        Console.WriteLine(controller.GetVoltage());
                        break;
                    case "4":
                        Console.WriteLine("What voltage? <double>");
                        controller.SetVoltage(double.Parse(Console.ReadLine()));
                        break;
                    case "5":
                        Console.WriteLine(controller.GetOutputState());
                        break;
                    case "6":
                        Console.WriteLine("What state? <true/false>");
                        controller.SetOutputState(bool.Parse(Console.ReadLine()));
                        break;
                    case "q":
                        cont = false;
                        break;
                    default:
                        Console.WriteLine("Unexpected input");
                        break;
                }
            }
        }
    }
}
