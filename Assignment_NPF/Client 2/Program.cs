using System;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Text;
using System.Security.Cryptography;

namespace Client2
{
    class Client2
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Write("Client2: ");
                string msg = Console.ReadLine() ?? "";
                msg = msg.Equals("") ? "UDP Demo" : msg;
                string ipAddress = "127.0.0.1";
                int sendPort = 2021;
                //	Encrypt message
                try
                {
                    // Create a new DESCryptoServiceProvider object
                    // to generate a key and initialization vector (IV).
                    DESCryptoServiceProvider DESalg = new DESCryptoServiceProvider();
                    // Encrypt the string to an in-memory buffer.
                    string encrypted = EncryptText(msg, DESalg.Key, DESalg.IV);
                    byte[] data = Encoding.ASCII.GetBytes(encrypted);
                    try
                    {
                        using (var client = new UdpClient())
                        {
                            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ipAddress), sendPort);
                            client.Connect(ep);
                            client.Send(data, data.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }    
            }
        }
        public static string EncryptText(string Data, byte[] Key, byte[] IV)
        {
            return Convert.ToBase64String(EncryptTextToMemory(Data, Key, IV));
        }
        public static byte[] EncryptTextToMemory(string Data, byte[] Key, byte[] IV)
        {
            try
            {
                // Create a MemoryStream.
                MemoryStream mStream = new MemoryStream();

                // Create a CryptoStream using the MemoryStream 
                // and the passed key and initialization vector (IV).
                CryptoStream cStream = new CryptoStream(mStream,
                               new DESCryptoServiceProvider().CreateEncryptor(Key, IV),
                               CryptoStreamMode.Write);

                // Convert the passed string to a byte array.
                byte[] toEncrypt = Encoding.UTF8.GetBytes(Data);

                // Write the byte array to the crypto stream and flush it.
                cStream.Write(toEncrypt, 0, toEncrypt.Length);
                cStream.FlushFinalBlock();

                // Get an array of bytes from the 
                // MemoryStream that holds the 
                // encrypted data.
                byte[] ret = mStream.ToArray();

                // Close the streams.
                cStream.Close();
                mStream.Close();

                // Return the encrypted buffer.
                return ret;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine("A Cryptographic error occurred: {0}", e.Message);
                return null;
            }
        }
    }
}