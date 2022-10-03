using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace IT_M3124_Library
{
    public class IT_M3124_Emulator
    {
        private int Port;
        private IPAddress IP;

        //Test information about the power supply
        private double Voltage = 200;
        private double Current = 2;
        private bool OutputState = false;

        public IT_M3124_Emulator(string ip, int port)
        {
            this.IP = IPAddress.Parse(ip);
            this.Port = port;
        }
        public IT_M3124_Emulator(IPAddress ip, int port)
        {
            this.IP = ip;
            this.Port = port;
        }

        #region Server Connection

        public void StartServer()
        {
            IPEndPoint localEndPoint = new IPEndPoint(IP, Port);

            try
            {
                Socket listener = new Socket(IP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                listener.Bind(localEndPoint);
                listener.Listen(10);

                Console.WriteLine("Waiting for connection");
                Socket handler = listener.Accept();

                string data = null;
                byte[] bytes = null;

                while (true)
                {
                    bytes = new byte[1024];
                    int bytesReceived = handler.Receive(bytes);
                    data = Encoding.ASCII.GetString(bytes, 0, bytesReceived);

                    //Better to split the string first then read each section but this does work
                    if(data.Length > 0)
                    {
                        Console.WriteLine("Text received from client : {0}", data);

                        if (data.Contains("CURRent?"))
                        {
                            byte[] msg = Encoding.ASCII.GetBytes(Current.ToString());
                            handler.Send(msg);
                        }
                        else if (data.Contains("VOLTage?"))
                        {
                            byte[] msg = Encoding.ASCII.GetBytes(Voltage.ToString());
                            handler.Send(msg);
                        } 
                        else if (data.Contains("OUTPut?"))
                        {
                            byte[] msg = Encoding.ASCII.GetBytes(OutputState.ToString());
                            handler.Send(msg);
                        }
                        else if (data.Contains("CURRent"))
                        {
                            string[] cmdArgs = data.Split(' ');
                            double newVal = double.Parse(cmdArgs[1]);
                            Current = newVal;
                        }
                        else if (data.Contains("VOLTage"))
                        {
                            string[] cmdArgs = data.Split(' ');
                            double newVal = double.Parse(cmdArgs[1]);
                            Voltage = newVal;
                        }
                        else if (data.Contains("OUTPut"))
                        {
                            string[] cmdArgs = data.Split(' ');
                            bool newVal = bool.Parse(cmdArgs[1]);
                            OutputState = newVal;
                        }
                        else
                        {
                            //Can change to <ACK> or something and be removed client side, as this causes whitespace when receiving
                            byte[] msg = Encoding.ASCII.GetBytes(" ");
                            handler.Send(msg);
                        }
                    }
                }

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion
    }
}
