using ChitChat.Helper.Exceptions;
using ChitChat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace ChitChat.Services
{
    public class SocketService : ISocketService
    {
        private readonly Socket socket;
        private readonly IPEndPoint endPoint;
        public SocketService()
        {
            try
            {
                // Establish the remote endpoint 
                // for the socket. This example 
                // uses port 11111 on the local 
                // computer.
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress ipAddr = ipHost.AddressList[0];
                endPoint = new IPEndPoint(ipAddr, 11111);

                // Creation TCP/IP Socket
                socket = new Socket(ipAddr.AddressFamily,
                        SocketType.Stream, ProtocolType.Tcp);

            }
            catch (Exception e)
            {
                Console.WriteLine("Error connecting to server: " + e.Message);
            }
        }


        // Sends a message to the connected socket.
        // The message is serialized to JSON format before sending.
        protected async Task SendMessage(string command,string message)
        {
            if (socket == null)
                throw new Exception("Socket is null");
            if(socket.Connected == false)
            {
                ConnectSocket();
            }
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(command + "|" + message);
            await socket.SendAsync(buffer);
        }

        // Receives a message from the connected socket.
        // The message is received as a byte array and then converted to a string.
        protected async Task<string[]> ReceiveMessage()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesReceived = socket.Receive(buffer);
                string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesReceived);
                return receivedMessage.Split("|");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private void CloseSocket()
        {
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        private void ConnectSocket()
        {
            try
            {
                socket.Connect(endPoint);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }


        public async Task<UserModel> PostLoginCredentialsAsync(UserCredentials userCredentials)
        {
            await SendMessage("LOGIN", JsonConvert.SerializeObject(userCredentials));
            string[] response = await ReceiveMessage();
            if (response[0].Equals("LOGIN"))
            {
                if (response[1].Equals("SUCCESS"))
                {
                    return new UserModel {
                        DisplayName = "abc",
                        ProfilePicture = "abc",
                        ConnectionID = "abc",
                    };
                }
            }

            throw new LoginException("Login failed");
        }
    }
}
