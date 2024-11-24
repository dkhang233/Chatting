using System.Net.Sockets;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Client
{
    class Client
    {
        // Main Method
        static async Task Main(string[] args)
        {
            await ExecuteClient();
        }

        // ExecuteClient() Method
        static async Task ExecuteClient()
        {
            try
            {
                // Establish the remote endpoint 
                // for the socket. This example 
                // uses port 11111 on the local 
                // computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

                // Creation TCP/IP Socket using 
                // Socket Class Constructor
                Socket sender = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    // Connect Socket to the remote endpoint
                    sender.Connect(localEndPoint);

                    // Print EndPoint information
                    Console.WriteLine("Socket connected to -> {0} ", sender.RemoteEndPoint.ToString());

                    // Send and receive message
                    await HandleMessage(sender);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    // Close Socket
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static async Task HandleMessage(Socket sender)
        {
            while(true)
            {
                Console.Write("Enter command: ");
                string name = Console.ReadLine();
                if(name == "exit")
                {
                    break;
                }
                // Creation message data
                var data = new
                {
                    username = "your_username",
                    password = "your_password"
                };

                // Convert to JSON
                string jsonString = JsonConvert.SerializeObject(data);

                // Message command 
                string command = "LOGIN";

                // Convert to byte array
                byte[] message = Encoding.ASCII.GetBytes(command + "|" + jsonString);

                // Send message to server
                sender.Send(message);

                // Buffer
                byte[] buffer = new byte[1024];

                // Receive message from server
                int byteRecv = sender.Receive(buffer);
                string[] receivedMessage = Encoding.UTF8.GetString(buffer, 0, byteRecv).Split("|");
                Console.WriteLine($"Received: command -> {receivedMessage[0]}, data -> {receivedMessage[1]}");
            }
        }
    }
}
