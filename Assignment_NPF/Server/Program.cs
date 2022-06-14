using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Server
{
    class Server
    {
        static void Main(string[] args)
        {
            //client 1
            TCPServer();

            //client 2
            UDPServer();           
        }
        public static void TCPServer()
        {
                //1. Listen
                IPAddress address = IPAddress.Parse("127.0.0.1");
                TcpListener listener = new TcpListener(address, 8888);
                Console.WriteLine("Sever is listening...");
                listener.Start();
                Socket socket = listener.AcceptSocket();

                //2. Receive
                byte[] data = new byte[1024];
                socket.Receive(data);
                string str = Encoding.ASCII.GetString(data);
                Console.WriteLine("Client1'message: \"" + str + "\"");

                //3. Send
                socket.Send(Encoding.ASCII.GetBytes("YourMessage: " + str));

            /*4. Close
            Console.WriteLine("Server is closing...");
            socket.Close();
            listener.Stop();*/
        }
        public static void UDPServer()
        {
            bool done = false;
            int listenPort = 2021;
            using (UdpClient listener = new UdpClient(listenPort))
            {
                IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, listenPort);
                while (!done)
                {
                    byte[] receivedData = listener.Receive(ref listenEndPoint);
                    //Console.WriteLine("Received broadcast message from client {0}", listenEndPoint.ToString());
                    Console.WriteLine("Client2'message: {0}", Encoding.ASCII.GetString(receivedData));
                }
            }
        }
        public static string DecryptText(string sData, byte[] Key, byte[] IV)
        {
            byte[] Data = Convert.FromBase64String(sData);
            return DecryptTextFromMemory(Data, Key, IV);
        }
        public static string DecryptTextFromMemory(byte[] Data, byte[] Key, byte[] IV)
        {
            try
            {
                // Create a new MemoryStream using the passed 
                // array of encrypted data.
                MemoryStream msDecrypt = new MemoryStream(Data);

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream csDecrypt = new CryptoStream(msDecrypt,
                                new DESCryptoServiceProvider().CreateDecryptor(Key, IV),
                                CryptoStreamMode.Read);

                // Create buffer to hold the decrypted data.
                byte[] fromEncrypt = new byte[Data.Length];

                // Read the decrypted data out of the crypto stream
                // and place it into the temporary buffer.
                csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

                //Convert the buffer into a string and return it.
                return Encoding.UTF8.GetString(fromEncrypt);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
    }
}