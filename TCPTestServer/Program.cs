using System;

namespace TCPTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.Write("ending: ");
            //string ending = Console.ReadLine();
            //Console.Write("port: ");
            //int port = int.Parse(Console.ReadLine());
            //Console.Write("script: ");
            //string path = Console.ReadLine();
            //Console.WriteLine("Start TCP-Server.");
            TCPServer tcpServer = new TCPServer(7, "\r\n", "./script.tdl");
            //TCPServer tcpServer = new TCPServer(port, ending, path);
            tcpServer.ServeForEver();
        }
    }
}
