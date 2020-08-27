using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Plotter
{
    /// <summary>
    /// TCP plot server, which listens to a specified TCP port and accepts connections.
    /// Creates a separate thread to listen to the network. Receives plotting commands
    /// as a simple ascii protocol from the network.
    /// </summary>
    public class TCPServer : MonoBehaviour
    {
        // The port number the server is listening to
        public int port = 8080;

        public bool ServerRunning = false;

        public delegate void CmdHandler(Command cmd);

        // Length in charcters of data types in communications
        // protocol.
        const int intLen = 10;
        const int floatLen = 11;

        // Socket attempt receive timeout in milliseconds
        const int timeOutms = 10000;

        private TcpListener tcpListener;

        private bool NewItems = false;

        // Lock object for multithreading
        private object threadLock = new object();

        // The command that has been parsed from the ascii stuff
        // received from the net.
        private Command currentCommand = new Command();

        private Dictionary<string, CmdHandler> cmdHandlers =
                                                new Dictionary<string, CmdHandler>();

        private Socket mainSocket = null;

        /// <summary>
        /// Register plothandlers, which want to receive plot commands for a plot
        /// </summary>
        /// <param name="plotTarget">plot name to target</param>
        /// <param name="handler">the handling delegate of the received command</param>
        public void RegisterCmdHandler(string plotTarget, CmdHandler handler)
        {
            Debug.Log("Registering handler for target: " + plotTarget);
            if (!cmdHandlers.ContainsKey(plotTarget))
            {
                cmdHandlers.Add(plotTarget, new CmdHandler(handler));
            }
            else
            {
                cmdHandlers[plotTarget] += handler;
            }
        }
        public bool IsNew()
        {
            Monitor.Enter(threadLock);
            try
            {
                return NewItems;
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
        }

        public Command GetCurrentCommand()
        {
            Monitor.Enter(threadLock);
            try
            {
                var copy = currentCommand.Clone();
                return copy;
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
        }
        public void SetNewItems(bool value)
        {
            Monitor.Enter(threadLock);
            try
            {
                NewItems = value;
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
        }

        private void SetCurrentCommand(Command command)
        {
            Monitor.Enter(threadLock);
            try
            {
                currentCommand = command;
            }
            finally
            {
                Monitor.Exit(threadLock);
            }
        }

        void Start()
        {
            StartServer();
            //Debug.Log("Hit S key to get the server running!");
        }

        void printDebugInfo()
        {
            var cCmd = GetCurrentCommand();
            cCmd.PrintDebugInfo();
        }
        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!ServerRunning)
                {
                    StartServer();
                }
                else
                {
                    ServerRunning = false;
                }
            }
            if (IsNew())
            {
                Debug.Log("New items!");
                //printDebugInfo();
                var cCmd = GetCurrentCommand();
                Debug.Log("Handler loop: " + cCmd.plotTarget);
                if (cmdHandlers.ContainsKey(cCmd.plotTarget))
                {
                    Debug.Log("Calling handler!");
                    cmdHandlers[cCmd.plotTarget](cCmd);
                }
                else
                {
                    Debug.Log("No such plot: " + cCmd.plotTarget);
                }

                // Flag we are done
                SetNewItems(false);
            }
        }


       

        // S
        void StartServer()
        {
            Thread serverThread = new Thread(new ThreadStart(RunServer));
            serverThread.IsBackground = true;
            serverThread.Start();

        }

        /// <summary>
        /// Keep the server running while the running flag is on. Implements the
        /// ascii protocol reader server side:
        /// 
        /// # Header structure:
        /// plot target
        /// cmd type
        /// float vector counts
        /// int vector counts
        /// string counts
        /// 
        /// </summary>
        void RunServer()
        {
            try
            {
                PrepareServer("127.0.0.1", port);

                while (ServerRunning)
                {
                    Debug.Log("Waiting for a connection.....");
                    mainSocket = tcpListener.AcceptSocket();
                    Debug.Log("Connection accepted from " + mainSocket.RemoteEndPoint);

                    var plotTarget = ReceiveSingleString(mainSocket);
                    Debug.Log("Plot target: " + plotTarget);

                    var commandType = (Command.CommandType)ReceiveSingleInt(mainSocket);
                    Debug.Log("Command: " + commandType);

                    var newCommand = new Command();
                    newCommand.plotTarget = plotTarget;
                    newCommand.commandType = commandType;

                    var floatVectorCount = ReceiveSingleInt(mainSocket);
                    Debug.Log("Float vector count " + floatVectorCount);

                    var intVectorCount = ReceiveSingleInt(mainSocket);
                    Debug.Log("Int vector count " + intVectorCount);

                    var stringCount = ReceiveSingleInt(mainSocket);
                    Debug.Log("String count " + stringCount);

                    for (var i = 0; i < floatVectorCount; i++)
                    {
                        var vectorName = ReceiveSingleString(mainSocket);
                        Debug.Log("Got float vector name: " + vectorName);
                        var floatVector = ReceiveFloatVector(mainSocket);
                        Debug.Log("Got " + floatVector.Length + " items.");
                        newCommand.setFloatVector(vectorName, floatVector);
                    }

                    for (var i = 0; i < intVectorCount; i++)
                    {
                        var vectorName = ReceiveSingleString(mainSocket);
                        var intVector = ReceiveIntVector(mainSocket);
                        newCommand.setIntVector(vectorName, intVector);
                    }

                    for (var i = 0; i < stringCount; i++)
                    {
                        var stringName = ReceiveSingleString(mainSocket);
                        Debug.Log("Got string with name: " + stringName);
                        var theString = ReceiveSingleString(mainSocket);
                        Debug.Log("string: " + theString);
                        newCommand.setString(stringName, theString);
                    }

                    SetCurrentCommand(newCommand);
                    SendCmdAck(mainSocket);
                    SetNewItems(true);
                    mainSocket.Close();
                }

                Debug.Log("Stopping server!");
                CloseServer();
            }
            catch (Exception e)
            {
                Debug.Log("Error..... " + e.StackTrace);

            }
        }

  
        void PrepareServer(string netaddress, int port)
        {
            IPAddress ip = IPAddress.Parse(netaddress);
            tcpListener = new TcpListener(ip, port);
            tcpListener.Start();

            Debug.Log("The server is running at port: " + port);
            Debug.Log("The local End point is  :" +
                              tcpListener.LocalEndpoint);

            ServerRunning = true;
        }

        void CloseServer()
        {
            if (mainSocket != null)
                mainSocket.Close();
            tcpListener.Stop();
        }

        /// <summary>
        /// Sends an acknowledgement message after the message coming
        /// in has been handled.
        /// </summary>
        /// <param name="s">The socket used to receive the message</param>
        /// <returns></returns>
        int SendCmdAck(Socket s)
        {
            ASCIIEncoding asen = new ASCIIEncoding();

            var ackMessage = "Got cmd!";
            int k = s.Send(asen.GetBytes(ackMessage + "\n\r"));
            Debug.Log("\nSent Acknowledgement: " + ackMessage);
            return k;

        }

        //
        // Receive a single item methods
        //

        string ReceiveSingleString(Socket socket)
        {
            var returnString = "";

            int length = ReceiveSingleInt(socket);

            //Debug.Log("str lenght was: " + length);
            if (length > 0)
            {
                returnString = ReceiveString(socket, length);
            }

            return returnString;
        }

        int ReceiveSingleInt(Socket socket)
        {
            int[] rInts = ReceiveInts(socket);
            if (rInts == null)
            {
                Debug.Log("No length for single int!");
                return 0;
            }

            return rInts[0];
        }

        //
        // Receive a vector of items
        //

        int[] ReceiveIntVector(Socket socket)
        {
            int[] intVector = null;
            int length = ReceiveSingleInt(socket);

            if (length > 0)
            {
                intVector = ReceiveInts(socket, length);
            }

            return intVector;
        }

        float[] ReceiveFloatVector(Socket socket)
        {
            int floatsLength = ReceiveSingleInt(socket);
            float[] floatVector = null;

            if (floatsLength > 0)
            {
                floatVector = ReceiveFloats(socket, floatsLength);
                if (floatVector == null)
                {
                    Debug.Log("rFloats null");
                }
            }
            else
            {
                Debug.Log("Float vector length was zero!");
            }

            return floatVector;
        }

        //
        // Low level methods called by the above methods
        //

        string ReceiveString(Socket socket, int length)
        {
            byte[] bytes = ReceiveBytes(socket, length);

            if (bytes == null)
                return "";

            //Debug.Log("For string got bytes: " + bytes.Length);

            ASCIIEncoding asen = new ASCIIEncoding();
            string figAsASC = asen.GetString(bytes, 0, length);
            return figAsASC;
        }

        int[] ReceiveInts(Socket socket, int count = 1)
        {
            byte[] bytes = ReceiveBytes(socket, intLen * count);

            if (bytes == null)
                return null;

            //Debug.Log("For int got bytes: " + bytes.Length);

            ASCIIEncoding asen = new ASCIIEncoding();
            int[] buffer = new int[count];
            for (int i = 0; i < count; i++)
            {
                string figAsASC = asen.GetString(bytes, i * intLen, intLen);
                //Debug.Log("as:" + figAsASC);
                buffer[i] = int.Parse(figAsASC, CultureInfo.InvariantCulture);
            }

            return buffer;
        }

        float[] ReceiveFloats(Socket socket, int count = 1)
        {
            byte[] bytes = ReceiveBytes(socket, floatLen * count);
            if (bytes == null)
                return null;

            //Debug.Log("For floats got bytes: " + bytes.Length);

            ASCIIEncoding asen = new ASCIIEncoding();
            float[] buffer = new float[count];
            for (int i = 0; i < count; i++)
            {
                string figAsASC = asen.GetString(bytes, i * floatLen, floatLen);
                //Debug.Log("as:" + figAsASC);
                buffer[i] = float.Parse(figAsASC, CultureInfo.InvariantCulture);
            }

            return buffer;
        }

        /// <summary>
        /// Low level socket byte reader. Return the read bytes as a byte array. 
        /// </summary>
        /// <param name="socket">The socket to read from</param>
        /// <param name="bytesCount">Byte count to read from a socket</param>
        /// <returns></returns>
        byte[] ReceiveBytes(Socket socket, int bytesCount)
        {
            byte[] buffer = new byte[bytesCount];
            int bytesRead = 0;
            int timeOut = timeOutms;

            while (bytesRead < bytesCount)
            {
                int readBytes = socket.Receive(buffer, bytesRead, bytesCount - bytesRead, SocketFlags.None);

                bytesRead += readBytes;
                //Debug.Log("read bytes " + readBytes + " cum " + bytesRead + " all " + bytesCount);
                if (bytesRead < bytesCount && socket.Available == 0)
                {
                    Debug.Log("Not all read, left: " + (bytesCount - bytesRead));
                    Thread.Sleep(10);
                    timeOut -= 10;
                    if (timeOut <= 0)
                    {
                        Debug.Log("Timeout, left to read: " + (bytesCount - bytesRead));
                        break;
                    }
                }
            }

            //Debug.Log("bytes read: " + bytesRead + " bcount " + bytesCount);
            if (bytesRead == bytesCount)
            {
                return buffer;
            }

            return null;
        }


        //float ReceiveSingleFloat(Socket socket)
        //{
        //    float[] rFloats = ReceiveFloats(socket);
        //    if (rFloats == null)
        //    {
        //        Debug.Log("No length for single float!");
        //        return 0;
        //    }
        //    return rFloats[0];
        //}


        //int SendAck(Socket s, string ackMessage)
        //{
        //    ASCIIEncoding asen = new ASCIIEncoding();

        //    int k = s.Send(asen.GetBytes(ackMessage + "\n\r"));
        //    Debug.Log("\nSent Acknowledgement: " + ackMessage);
        //    return k;
        //}
    }
}