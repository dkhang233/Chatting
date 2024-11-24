using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

namespace Server
{

    class Server
    {
        // Main Method
        static async Task Main(string[] args)
        {
            await ExecuteServer();
        }

        // Method to execute the server
        private static async Task ExecuteServer()
        {
            // Establish the local endpoint for the socket.
            // Dns.GetHostName returns the name of the host running the application.
            IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint localEndPoint = new IPEndPoint(ipAddr, 11111);

            // Creation of TCP/IP Socket using Socket Class Constructor
            Socket server = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // Using Bind() method to associate a network address to the Server Socket
                // All clients that will connect to this Server Socket must know this network address
                server.Bind(localEndPoint);

                // Using Listen() method to create the Client list that will want to connect to Server
                server.Listen(10);

                while (true)
                {
                    Console.WriteLine("Waiting for connection...");

                    // Suspend while waiting for incoming connection
                    // Using Accept() method the server will accept connection of client
                    Socket client =  await server.AcceptAsync();
                    handleClientAsync(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            finally
            {
                // Close the Server Socket
                server.Shutdown(SocketShutdown.Both);
                server.Close();
            }
        }

        private static async Task handleClientAsync(Socket client)
        {
            try
            {
                while (true)
                {
                    // Check if the client is still connected
                    if (!client.Connected)
                    {
                        break;
                    }
                    // Buffer
                    byte[] buffer = new byte[1024];


                    // Clear the buffer
                    Array.Clear(buffer, 0, buffer.Length);

                    // Receive message from client
                    int bytesRead = await client.ReceiveAsync(new ArraySegment<byte>(buffer), SocketFlags.None);

                    // Convert the received message to a string
                    string[] receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead).Split("|");


                    Console.WriteLine($"Received: command -> {receivedMessage[0]}, data -> {receivedMessage[1]}");

                    string command = receivedMessage[0];
                    string data = receivedMessage[1];

                    if (command.Equals("LOGIN"))
                    {
                        // Handle login
                        if (handleLogin(data))
                        {
                            // Send response
                            byte[] response = Encoding.UTF8.GetBytes("LOGIN|SUCCESS");
                            await client.SendAsync(new ArraySegment<byte>(response), SocketFlags.None);
                        }
                        else
                        {
                            // Send response
                            byte[] response = Encoding.UTF8.GetBytes("LOGIN|FAILED");
                            await client.SendAsync(new ArraySegment<byte>(response), SocketFlags.None);
                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error: {e.Message}");
                client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
        }

        private static Boolean handleLogin(string data)
        {
            Console.WriteLine(data);
            return true;
        }
    }
}
