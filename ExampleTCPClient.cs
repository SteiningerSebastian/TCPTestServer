using System;
using System.IO;
using System.Net.Sockets;

namespace TCP_Client_Example
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Int32 port = 7;
                TcpClient client = new TcpClient("localhost", port);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                StreamWriter stream = new StreamWriter(client.GetStream());
                StreamReader streamReader = new StreamReader(client.GetStream());

                // Send the message to the connected TcpServer.
                stream.Write("Hello\r\n");
                stream.Flush();

                string response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);
                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);
                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);


                stream.Write("who\r\n");
                stream.Flush();

                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);
                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);
                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);

                stream.Write("{json: {[data:{index:\"hello\"}]}}\r\n");
                stream.Flush();

                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);
                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);

                stream.Write("echo\r\n");
                stream.Flush();

                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);
                response = streamReader.ReadLine();
                Console.WriteLine("Received: {0}", response);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }

            Console.WriteLine("\n Press Enter to continue...");
            Console.Read();
        }

    }
}
