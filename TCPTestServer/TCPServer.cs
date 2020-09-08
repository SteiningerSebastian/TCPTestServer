using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace TCPTestServer
{
    class TCPServer
    {
        private Dictionary<TcpClient, int> clients = new Dictionary<TcpClient, int>();
        private TcpListener server;
        private Dictionary<int, StreamWriter> streamWriters = new Dictionary<int, StreamWriter>();
        private Dictionary<int, StreamReader> streamReaders = new Dictionary<int, StreamReader>();
        internal string endingBytes;
        private LanguageInterpreter languageInterpreter;

        /// <summary>
        /// Init the TCP-Server
        /// </summary>
        public TCPServer(int port, string endingBytes, string scriptPath)
        {
            server = new TcpListener(IPAddress.Any, port);
            this.endingBytes = endingBytes;
            languageInterpreter = new LanguageInterpreter(scriptPath);
        }

        /// <summary>
        /// Starts the server
        /// </summary>
        public void ServeForEver()
        {
            server.Start();
            Thread thAcceptTCPClients = new Thread(AcceptTCPClients);
            thAcceptTCPClients.Start();
            thAcceptTCPClients.Join();
        }

        /// <summary>
        /// Sends a message to all connected Clients
        /// </summary>
        /// <param name="message"></param>
        public void SendToAll(string message)
        {
            foreach (KeyValuePair<int, StreamWriter> streamWriter in streamWriters)
            {
                streamWriter.Value.WriteLine(message);
                streamWriter.Value.Flush();
            }
        }

        /// <summary>
        /// Sends a message to a client
        /// </summary>
        /// <param name="id">Is the client id.</param>
        /// <param name="message">Is the message.</param>
        public void Send(int id, string message)
        {
            streamWriters[id].WriteLine(message);
            streamWriters[id].Flush();
        }

        /// <summary>
        /// Handels the receiving of messages
        /// </summary>
        /// <param name="id">The id of the client that should start receive</param>
        private void StartReceive(Object id)
        {
            while (true)
            {
                try
                {
                    string message = streamReaders[(int)id].ReadLine();

                    if (message != null)
                    {
                        languageInterpreter.HandleReceive((int)id, message, this);
                    }
                }
                catch
                {
                    //throw new Exception("Connection lost");
                    return;
                }
            }
        }

        /// <summary>
        /// Acept TCP Clients
        /// </summary>
        private void AcceptTCPClients()
        {
            int id = 0;
            while (true)
            {
                var client = server.AcceptTcpClient();
                clients[client] = id;
                streamWriters[id] = new StreamWriter(client.GetStream());
                streamReaders[id] = new StreamReader(client.GetStream());
                Thread thReceive = new Thread(new ParameterizedThreadStart(this.StartReceive));
                thReceive.Start(id);
                id++;
            }
        }

    }
}
