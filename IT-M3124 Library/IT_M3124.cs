using System;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace IT_M3124_Library
{
    // State object for receiving data from remote device.
    public class StateObject
    {
        // Client socket.
        public Socket workSocket = null;
        // Size of receive buffer.
        public const int BufferSize = 256;
        // Receive buffer.
        public byte[] buffer = new byte[BufferSize];
        // Received data string.
        public StringBuilder sb = new StringBuilder();
    }
    public class IT_M3124
    {
        private static IPAddress IP;
        private static int Port;

        //Access to the open connection anywhere, for sending commands
        private static Socket _socket;

        // ManualResetEvent instances signal completion.
        private static ManualResetEvent connectDone =
        new ManualResetEvent(false);
        private static ManualResetEvent sendDone =
        new ManualResetEvent(false);
        private static ManualResetEvent receiveDone =
        new ManualResetEvent(false);

        public IT_M3124(IPAddress ip, int port)
        {
            IP = ip;
            Port = port;
            StartClient();
        }
        public IT_M3124(string ip, int port)
        {   
            IP = IPAddress.Parse(ip);
            Port = port;
            StartClient();
        }

        #region Client Connection and Callbacks

        //Stores the latest response from the server. Any function can read this.
        private static String response = String.Empty;
        private static void StartClient()
        {
            // Connect to a remote device.
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(IP, Port);

                // Create a TCP/IP socket.
                Socket client = new Socket(IP.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);
                _socket = client;

                // Connect to the remote endpoint.
                client.BeginConnect(remoteEP,
                new AsyncCallback(ConnectCallback), client);
                connectDone.WaitOne();

                //Wait for the client to connect before sending or receiving
                while(!client.Connected)
                {
                    
                }
                // Send test data to the remote device.
                Send(client, "This is a test");
                sendDone.WaitOne();

                // Receive the response from the remote device.
                Receive(client);
                receiveDone.WaitOne();

                // Write the response to the console.
                Console.WriteLine("Response received : {0}", response);

                // Release the socket.
                //client.Shutdown(SocketShutdown.Both);
                //client.Close();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete the connection.
                client.EndConnect(ar);

                Console.WriteLine("Socket connected to {0}",
                client.RemoteEndPoint.ToString());

                // Signal that the connection has been made.
                connectDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Receive(Socket client)
        {
            try
            {
                // Create the state object.
                StateObject state = new StateObject();
                state.workSocket = client;

                // Begin receiving the data from the remote device.
                client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReceiveCallback), state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the state object and the client socket
                // from the asynchronous state object.
                StateObject state = (StateObject)ar.AsyncState;
                Socket client = state.workSocket;

                // Read data from the remote device.
                int bytesRead = client.EndReceive(ar);

                state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));
                response = state.sb.ToString();
                receiveDone.Set();

                /*
                if (bytesRead > 0) {
                    // There might be more data, so store the data received so far.
                    state.sb.Append(Encoding.ASCII.GetString(state.buffer, 0, bytesRead));

                    // Get the rest of the data.
                    client.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                    new AsyncCallback(ReceiveCallback), state);
                } else
                {
                    // All the data has arrived; put it in response.
                    if (state.sb.Length > 1) {
                        response = state.sb.ToString();
                    }
                    // Signal that all bytes have been received.
                    receiveDone.Set();
                }
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static void Send(Socket client, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);

            // Begin sending the data to the remote device.
            client.BeginSend(byteData, 0, byteData.Length, 0,
            new AsyncCallback(SendCallback), client);
        }

        private static void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket client = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = client.EndSend(ar);
                Console.WriteLine("Sent {0} bytes to server.", bytesSent);

                // Signal that all bytes have been sent.
                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        #endregion

        /*
         * This function is private so that messages can only be sent via the other functions in this library, 
         * this is to prevent messages that do not conform to SCPI to be sent and create errors.
         */
        private void SendCommand(string command)
        {
            //Console.WriteLine("sending: " + command);
            Send(_socket, command + "\n");
        }

        private string ReadResponse()
        {
            Thread.Sleep(20);
            Receive(_socket);
            Console.WriteLine("Response from server: " + response);
            return response;
        }

        #region IT-M3124 Commands

        //Must be done before using any of the "Set" commands   
        public void SetRemote()
        {
            SendCommand("SYSTem:REMote");
        }

        public double GetCurrent()
        {
            SendCommand("CURRent?");
            string res = ReadResponse();
            return double.Parse(res);
        }
        
        public void SetCurrent(double current)
        {
            SendCommand("CURRent " + current);
        }

        public double GetVoltage()
        {
            SendCommand("VOLTage?");
            string res = ReadResponse();
            return double.Parse(res);
        }

        public void SetVoltage(double voltage)
        {
            SendCommand("VOLTage " + voltage);
        }

        public bool GetOutputState()
        {
            SendCommand("OUTPut?");
            string res = ReadResponse();
            return bool.Parse(res);
        }

        public void SetOutputState(bool state)
        {
            SendCommand("OUTPut " + state);
        }

        #endregion

    }
}