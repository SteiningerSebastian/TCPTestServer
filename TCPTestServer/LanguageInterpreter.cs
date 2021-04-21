using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TCPTestServer;
using System.Net;
using System.Net.Sockets;

namespace TCPTestServer
{
    class LanguageInterpreter
    {
        const string ReceiveVarHeader = "$RECEIVE";
        private Dictionary<string, Object> variables;
        private string[] script;
        public LanguageInterpreter(string path)
        {
            script = File.ReadAllLines(path);
            variables = new Dictionary<string, Object>();
            if (MapVariables() == false)
            {
                Exception exception = new Exception("Could not mapp the variables!");
                throw exception;
            }
        }

        /// <summary>
        /// Mapp the names to variables
        /// </summary>
        /// <returns>Returns if the mapping was succsessfull</returns>
        private bool MapVariables()
        {
            //try
            //{
            foreach (string line in script)
            {
                string lineWS = line.Replace(" ", "");
                if (line.Split(" ")[0] == "var")
                {
                    for (int i = 1; i < lineWS.Split('=')[1].Length; i++)
                    {
                        if (lineWS.Split('=')[1][i - 1] == '"')
                        {
                            variables[lineWS.Split("var")[1].Split('=')[0]] = line.Split('=')[1].Remove(0, i + 1).Remove(line.Split('=')[1].Remove(0, i + 1).Length - 1, 1);
                            break;
                        }
                        else if (!lineWS.Contains('"'))
                        {
                            variables[lineWS.Split("var")[1].Split('=')[0]] = lineWS.Split('=')[1];
                            break;
                        }
                    }
                }
            }
            return true;
            //}
            //catch
            //{
            //    return false;
            //}
        }

        private string DeleteUnneededSpaces(string script)
        {
            return "";
        }

        /// <summary>
        /// Handesl a received message
        /// </summary>
        /// <param name="message">Is the message that is received</param>
        /// <returns></returns>
        public bool HandleReceive(int sender, string message, TCPTestServer.TCPServer tcpServer)
        {
            try
            {
                for (int lineIndex = 0; lineIndex < script.Length; lineIndex++)
                {
                    string line = script[lineIndex];
                    string lineWS = line.Replace(" ", "");
                    for(int i = 0; i < lineWS.Length; i++)
                    {
                        if(lineWS[i] == '#')
                        {
                            lineIndex++;
                            line = script[lineIndex];
                            lineWS = line.Replace(" ", "");
                            break;
                        }
                        else if(lineWS[i] != ' ')
                        {
                            break;
                        }
                    }
                    if (lineWS.Contains("if"))
                    {
                        lineWS = lineWS.Replace("if", "");
                        string var = lineWS.Split("==")[0];
                        if (variables.ContainsKey(var))
                        {
                            if (variables[var].ToString() == ReceiveVarHeader)
                            {
                                string cmd = "";
                                if (variables.ContainsKey(line.Split("==")[1].ToString().Split(":")[0]))
                                {
                                    cmd = variables[line.Split("==")[1].ToString().Split(":")[0]].ToString();
                                    if (cmd == ReceiveVarHeader)
                                    {
                                        cmd = message;
                                    }
                                }
                                else
                                {
                                    cmd = line.Split("==")[1].ToString().Remove(0, 1).Remove(line.Split("==")[1].ToString().Remove(0, 1).Length - 2, 2);
                                }
                                if (cmd == message)
                                {
                                    while (true)
                                    {
                                        lineIndex++;
                                        line = script[lineIndex];
                                        if (script[lineIndex] == "end")
                                        {
                                            break;
                                        }
                                        else
                                        {
                                            if (line.Contains("sendToAll"))
                                            {
                                                string response = line.Split("(")[1].Remove(line.Split("(")[1].Length - 1, 1);
                                                for (int i = 0; i < response.Length; i++)
                                                {
                                                    if (response[i] == ' ')
                                                    {
                                                        continue;
                                                    }
                                                    response = response.Remove(0, i);

                                                    if (variables.ContainsKey(response) && variables[response].ToString() == ReceiveVarHeader)
                                                    {
                                                        tcpServer.SendToAll(message);
                                                        break;
                                                    }
                                                    else if (variables.ContainsKey(response))
                                                    {
                                                        tcpServer.SendToAll(variables[response].ToString());
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        tcpServer.SendToAll(response.Remove(i, 1).Remove(response.Remove(i, 1).Length - 1, 1));
                                                        break;
                                                    }
                                                }
                                            }
                                            else if (line.Contains("send"))
                                            {
                                                string parameters = line.Split("(")[1].Remove(line.Split("(")[1].Length - 1, 1).Replace(" ", "");
                                                string strRec = parameters.Split(",")[1];
                                                string response = parameters.Split(",")[0];
                                                if (int.TryParse(strRec, out int id))
                                                {
                                                    for (int i = 0; i < response.Length; i++)
                                                    {
                                                        if (response[i] == ' ')
                                                        {
                                                            continue;
                                                        }
                                                        response = response.Remove(0, i);

                                                        if (variables.ContainsKey(response) && variables[response].ToString() == ReceiveVarHeader)
                                                        {
                                                            tcpServer.Send(id, message);
                                                        }
                                                        else if (variables.ContainsKey(response))
                                                        {
                                                            tcpServer.Send(id, variables[response].ToString());
                                                        }
                                                        else
                                                        {
                                                            tcpServer.Send(id, response);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (variables.ContainsKey(strRec.Split(".")[0].ToString()) && variables[strRec.Split(".")[0].ToString()].ToString() == ReceiveVarHeader)
                                                    {
                                                        if (strRec.Split(".")[1] == "id")
                                                        {
                                                            for (int i = 0; i < response.Length; i++)
                                                            {
                                                                if (response[i] == ' ')
                                                                {
                                                                    continue;
                                                                }
                                                                response = response.Remove(0, i);

                                                                if (!variables.ContainsKey(response))
                                                                {
                                                                    tcpServer.Send(sender, parameters.Split(",")[0].Remove(0, i + 1).Remove(parameters.Split(",")[0].Remove(0, i + 1).Length - 1, 1));
                                                                    break;
                                                                }
                                                                else if (variables.ContainsKey(response))
                                                                {
                                                                    if (variables.ContainsKey(response) && variables[response].ToString() == ReceiveVarHeader)
                                                                    {
                                                                        tcpServer.Send(sender, message);
                                                                        break;
                                                                    }
                                                                    else
                                                                    {
                                                                        tcpServer.Send(sender, variables[response].ToString());
                                                                        break;

                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            Exception exception = new Exception("Command is not recogniced!");
                                                            throw exception;
                                                        }
                                                    }
                                                }
                                            }
                                            else if (line.Contains("log"))
                                            {
                                                string parameters = line.Split("(")[1].Remove(line.Split("(")[1].Length - 1, 1).Replace(" ", "");
                                                string str = parameters.Split(",")[0];
                                                if (variables.ContainsKey(str) && variables[str].ToString() == ReceiveVarHeader)
                                                {
                                                    Console.WriteLine(message);
                                                }else if (variables.ContainsKey(str))
                                                {
                                                    Console.WriteLine(variables[str]);
                                                }
                                                else
                                                {
                                                    Console.WriteLine(str.Split('"')[1]);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
