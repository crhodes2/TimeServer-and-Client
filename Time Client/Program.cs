using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Time_Client
{
    class Program
    {
        //================= PRIVATE MEMBER =================//
        private static Socket clientSocket = new Socket
            (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        //================= MAIN PROGRAM =================//
        static void Main(string[] args)
        {
            Console.Title = "Client";
            ConnectToServer();
            SendData();
        }
        //**********************************************//


        //============== LOOP CONNECT METHOD ================//
        private static void ConnectToServer()
        {
            int attempts = 0;

            while (!clientSocket.Connected)
            {
                try
                {
                    attempts++;
                    Console.WriteLine("Connection Attempts: " + attempts.ToString());
                    clientSocket.Connect(IPAddress.Loopback, 100);
                }
                
                catch (SocketException)
                {
                    Console.Clear();
                }
            }

            Console.Clear();
            Console.WriteLine("Connected");

        }
        //*************************************************//

        //============== LOOP CONNECT METHOD ================//
        private static void SendData()
        {
            while (true)
            {
                Console.Write("Enter a request: ");
                Console.WriteLine("Time Request Input(s): ");
                Console.WriteLine("get time | get euro time | get japan time | get gmt time | get nz time");
                string request = Console.ReadLine();

                byte[] buffer = Encoding.ASCII.GetBytes(request);
                clientSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

                byte[] receivedBuffer = new byte[1024];
                int receipt = clientSocket.Receive(receivedBuffer, SocketFlags.None);
                if (receipt == 0) return;
                byte[] data = new byte[receipt];
                Array.Copy(receivedBuffer, data, receipt);
                string text = Encoding.ASCII.GetString(data);
                Console.WriteLine("Received: " + text);
            }
        }



    }
}
