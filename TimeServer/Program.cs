using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace TimeServer
{
    class Program
    {
        //==================== PRIVATE MEMBERS ======================//

        private static byte[] buffer = new byte[1024]; // global buffer of 1024 bytes.
        private static List<Socket> clientSockets = new List<Socket>();
        private static Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        static void Main(string[] args)
        {
            Console.Title = "World Time Server";
            SetupServer();
            Console.ReadLine();
        }

        private static void SetupServer()
        {
            Console.WriteLine("Setting up Time Server...");
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 100));
            serverSocket.Listen(5);
            //Listening to connection...
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null);
        }

        private static void AcceptCallBack(IAsyncResult AR)
        {
            // this method will accept incoming connection attempt and create a new socket to handle remote host connection
            Socket socket = serverSocket.EndAccept(AR); 
            clientSockets.Add(socket);  // adding new socket (defined in the ReceiveCallBack method) to our client list
            Console.WriteLine("Time Client Connected");
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
            serverSocket.BeginAccept(new AsyncCallback(AcceptCallBack), null); // accept again. without this it will no longer accept any more connection.
        }

        private static void ReceiveCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            // determine how many data we are receiving
            int received = socket.EndReceive(AR);
            //temp buffer
            byte[] dataBuf = new byte[received];
            Array.Copy(buffer, dataBuf, received);
            string text = Encoding.ASCII.GetString(dataBuf);
            Console.WriteLine("Text Received: " + text);

            string response = string.Empty;

            if (text.ToLower() == "get time")
            {
                response = DateTime.Now.ToLongTimeString();

            }

            else if (text.ToLower() == "get euro time")
            {
                var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time");
                var remoteTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
                response = remoteTime.TimeOfDay.ToString();
            }

            else if (text.ToLower() == "get japan time")
            {
                var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                var remoteTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
                response = remoteTime.TimeOfDay.ToString();
            }

            else if (text.ToLower() == "get gmt time")
            {
                var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                var remoteTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
                response = remoteTime.TimeOfDay.ToString();
            }

            else if (text.ToLower() == "get nz time")
            {
                var remoteTimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                var remoteTime = TimeZoneInfo.ConvertTime(DateTime.Now, remoteTimeZone);
                response = remoteTime.TimeOfDay.ToString();
            }


            else
            {
                response = "invalid request";
                
            }

            byte[] data = Encoding.ASCII.GetBytes(response);
            socket.BeginSend(data, 0, data.Length, SocketFlags.None, new AsyncCallback(SendCallBack), socket);
            socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallBack), socket);
        }

        private static void SendCallBack(IAsyncResult AR)
        {
            Socket socket = (Socket)AR.AsyncState;
            socket.EndSend(AR);
        }
    }
}
