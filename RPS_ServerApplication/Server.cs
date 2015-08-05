using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;


namespace RPS_ServerApplication
{
    public partial class Server : Form
    {
        //Code from TCP server sample code
        private const int m_iMaxConnections = 2;
        System.Net.IPEndPoint m_LocalIPEndPoint;
        //A structure holding details about a single connection
        struct Connection_Struct    
        {
            public Socket ClientSpecific_Socket;
            public string clientNum;
            public string clientName;
            public bool bInUse;
        };
        Socket m_ListenSocket;
        static int m_iNumberOfConnectedClients;
        Connection_Struct[] m_Connection_Array = new Connection_Struct[m_iMaxConnections]; // Define an array to hold a number of connections
        private static System.Windows.Forms.Timer m_CommunicationActivity_Timer;
        
        //Declare variables
        string cName;
        string plyrNumber;
        string sendCName;
        string moveP1;
        string moveP2;

        public Server()
        {
            //Code from sample code
            InitializeComponent();
            m_CommunicationActivity_Timer = new System.Windows.Forms.Timer(); // Check for communication activity on Non-Blocking sockets every 200ms
            m_CommunicationActivity_Timer.Tick += new EventHandler(OnTimedEvent_PeriodicCommunicationActivityCheck); // Set event handler method for timer
            m_CommunicationActivity_Timer.Interval = 100;  // Timer interval is 1/10 second
            m_CommunicationActivity_Timer.Enabled = false;
            string szLocalIPAddress = GetLocalIPAddress_AsString(); // Get local IP address as a default value
            ipTB.Text = szLocalIPAddress; //Set the ipTB (ip address textbox) to the local ip
            portTB.Text = "8004";  // Default port number
            m_iNumberOfConnectedClients = 0;
            
            try
            {   // Create the Listen socket, for TCP use
                m_ListenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_ListenSocket.Blocking = false;
            }
            catch (SocketException se)
            {   // If an exception occurs, display an error message
                MessageBox.Show(se.Message);
            }
            
        }

        //From TCP server sample code
        private void Initialise_ConnectionArray()
        {
            int iIndex;
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                m_Connection_Array[iIndex].bInUse = false;
            }
        }
        
        //From TCP server sample code
        private int GetnextAvailable_ConnectionArray_Entry()
        {
            int iIndex;
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                if (false == m_Connection_Array[iIndex].bInUse)
                {
                    return iIndex;  // Return the index value of the first not-in-use entry found
                }
            }
            return -1;      // Signal that there were no available entries
        }

        //Code from the sample code
        //This is used to find the local IP address and then store it in a string 
        public string GetLocalIPAddress_AsString()
        {
            string szHost = Dns.GetHostName();
            string szLocalIPaddress = "127.0.0.1";  // Default is local loopback address
            IPHostEntry IPHost = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress IP in IPHost.AddressList)
            {
                if (IP.AddressFamily == AddressFamily.InterNetwork) // Match only the IPv4 address
                {
                    szLocalIPaddress = IP.ToString();
                    break;
                }
            }
            return szLocalIPaddress;
        }

        //EnableBtn used to establish the connection to the server
        /*This section of the code is from the sample code, I merged all the code from being from 3 different
        Buttons to one button*/
        private void enableBtn_Click(object sender, EventArgs e)
        {
            //try binding
            try
            {
                // Get the Port number from the appropriate text box
                String szPort = portTB.Text;
                int iPort = System.Convert.ToInt16(szPort, 10);
                // Create an Endpoint that will cause the listening activity to apply to all the local node's interfaces
                m_LocalIPEndPoint = new System.Net.IPEndPoint(IPAddress.Any, iPort);
                // Bind to the local IP Address and selected port
                m_ListenSocket.Bind(m_LocalIPEndPoint);
                portTB.Enabled = false;
            }

            catch // Catch any errors
            {   // If an exception occurs, display an error message
                eventsTB.AppendText("Bind failed" + Environment.NewLine);
            }

            //try listening to connections
            try
            {
                m_ListenSocket.Listen(2); // Listen for connections, with a backlog / queue maximum of 2
                enableBtn.Enabled = false;
            }
            catch (SocketException se)
            {
                // If an exception occurs, display an error message
                MessageBox.Show(se.Message);
            }
            catch // Silently handle any other exception
            {
            }
            //try accept connections
            m_CommunicationActivity_Timer.Start();  // Start the timer to perform periodic checking for connection requests
            eventsTB.AppendText("Accepting (waiting for connection attempt)" + Environment.NewLine); 
        }

        //Code from sameple TCP server code but has many additional if statments added
        //This continuously checks for messages received
        private void OnTimedEvent_PeriodicCommunicationActivityCheck(Object myObject, EventArgs myEventArgs)
        {   // Periodic check whether a connection request is pending or a message has been received on a connected socket     
            // First, check for pending connection requests
            int iIndex;
            iIndex = GetnextAvailable_ConnectionArray_Entry(); // Find an available array entry for next connection request
            if (-1 != iIndex)
            {   // Only continue with Accept if there is an array entry available to hold the details
                try
                {
                    m_Connection_Array[iIndex].ClientSpecific_Socket = m_ListenSocket.Accept();  // Accept a connection (if pending) and assign a new socket to it (AcceptSocket)
                    // Will 'catch' if NO connection was pending, so statements below only occur when a connection WAS pending
                    m_Connection_Array[iIndex].bInUse = true;
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Blocking = false;           // Make the new socket operate in non-blocking mode
                    m_iNumberOfConnectedClients++;
                    //Player number is stored in a variable
                    m_Connection_Array[iIndex].clientNum = "Player " + System.Convert.ToString(m_iNumberOfConnectedClients);
                    plyrNumber = m_Connection_Array[iIndex].clientNum;
                    eventsTB.AppendText("Number of connected players: " + System.Convert.ToString(m_iNumberOfConnectedClients) + Environment.NewLine);
                    eventsTB.AppendText("A new client connected" + Environment.NewLine);
                    eventsTB.AppendText(plyrNumber + Environment.NewLine);
                    SendUpdateMesageToAllConnectedclients();
                }
                catch (SocketException se) // Handle socket-related exception
                {   // If an exception occurs, display an error message
                    if (10053 == se.ErrorCode || 10054 == se.ErrorCode) // Remote end closed the connection
                    {
                        CloseConnection(iIndex);
                    }
                    else if (10035 != se.ErrorCode)
                    {   // Ignore error messages relating to normal behaviour of non-blocking sockets
                        MessageBox.Show(se.Message);
                    }
                }
                catch // Silently handle any other exception
                {
                }
            }

            // Second, check for received messages on each connected socket
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                if (true == m_Connection_Array[iIndex].bInUse)
                {
                    try
                    {
                        EndPoint localEndPoint = (EndPoint)m_LocalIPEndPoint;
                        byte[] ReceiveBuffer = new byte[1024];
                        int iReceiveByteCount;
                        iReceiveByteCount = m_Connection_Array[iIndex].ClientSpecific_Socket.ReceiveFrom(ReceiveBuffer, ref localEndPoint);
                        string szReceivedMessage;
                        if (0 < iReceiveByteCount)
                        {   // Copy the number of bytes received, from the message buffer to the text control
                            szReceivedMessage = Encoding.ASCII.GetString(ReceiveBuffer, 0, iReceiveByteCount);
                            if ("QuitConnection" == szReceivedMessage)
                            {
                                CloseConnection(iIndex);
                            }

                            /*If the received message contains the key word "User: ", return the message to clients
                            And also client player number is sent to all clients */
                            else if (szReceivedMessage.Contains("User: "))
                            {     
                                for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
                                {
                                    if (true == m_Connection_Array[iIndex].bInUse)
                                    {
                                        string szMessage;
                                        szMessage = szReceivedMessage;
                                        byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                        m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                                    }
                                }
                                cName = szReceivedMessage.Replace("User: ", "");
                                //Player number is sent to the clients
                                sendCName = plyrNumber + " is " + cName;
                                eventsTB.AppendText(plyrNumber + " is " + cName + Environment.NewLine);
                                m_Connection_Array[iIndex].clientName = cName;
                                //Add the players connected to the Player List box
                                playerLB.Items.Add(cName);

                                for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
                                {
                                    if (true == m_Connection_Array[iIndex].bInUse)
                                    {
                                        string szMessage;
                                        szMessage = Environment.NewLine + sendCName;
                                        byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                        m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                                    }
                                }
                            }

                            /*If the received message contains the key word "CSend", return the message to clients
                             CSend is used to send an invite to the clients*/
                            else if (szReceivedMessage.Contains("CSend"))
                            {
                                for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
                                {
                                    if (true == m_Connection_Array[iIndex].bInUse)
                                    {
                                        string szMessage;
                                        szMessage = szReceivedMessage;
                                        byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                        m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                                    }
                                }
                            }

                            /*If the received message contains the key word "chatL", return the message to clients
                             chatL is a key word used to identify client chat messages*/
                            else if (szReceivedMessage.Contains("chatL"))
                            {
                                for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
                                {
                                    if (true == m_Connection_Array[iIndex].bInUse)
                                    {
                                        string szMessage;
                                        szMessage = szReceivedMessage;
                                        byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                        m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                                    }
                                }
                            }

                            //If a player has declined the match, return this message
                            else if (szReceivedMessage.Contains("CDecline"))
                            {
                                for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
                                {
                                    if (true == m_Connection_Array[iIndex].bInUse)
                                    {
                                        string szMessage;
                                        szMessage = szReceivedMessage;
                                        byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                        m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                                    }
                                }
                            }


                            /*Server checks what move is made by the player
                            The move is then stored in variables*/

                            //If move sent by the player is Rock
                            else if (szReceivedMessage.Contains("Rock"))
                            {
                                eventsTB.AppendText(m_Connection_Array[iIndex].clientNum + " sent the move Rock" + Environment.NewLine);
                                if (m_Connection_Array[iIndex].clientNum == "Player 1")
                                {
                                    moveP1 = "Rock";
                                }
                                else
                                {
                                    moveP2 = "Rock";
                                }
                                eventsTB.AppendText(checkWin() + Environment.NewLine);
                            }
                            //If move sent by the player is Paper
                            else if (szReceivedMessage.Contains("Paper"))
                            {
                                eventsTB.AppendText(m_Connection_Array[iIndex].clientNum + " sent the move Paper" + Environment.NewLine);
                                if (m_Connection_Array[iIndex].clientNum == "Player 1")
                                {
                                    moveP1 = "Paper";
                                }
                                else
                                {
                                    moveP2 = "Paper";
                                }
                                eventsTB.AppendText(checkWin() + Environment.NewLine);
                            }
                            //If move sent by the player is Scissors
                            else if (szReceivedMessage.Contains("Scissors"))
                            {
                                eventsTB.AppendText(m_Connection_Array[iIndex].clientNum + " sent the move Scissors" + Environment.NewLine);
                                if (m_Connection_Array[iIndex].clientNum == "Player 1")
                                {
                                    moveP1 = "Scissors";
                                }
                                else
                                {
                                    moveP2 = "Scissors";
                                }
                                eventsTB.AppendText(checkWin() + Environment.NewLine);
                            }

                            /*If the checkWin method returns "Player 1 won", send player 1 this message
                            And send player 2 a message notifying them they have lost
                            Then reset player 1 and player 2's moves*/
                            if (checkWin() == "Player 1 won")
                            {
                                string szMessage = "You Won by " + moveP1;
                                byte[] SendMessage;
                                SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                m_Connection_Array[0].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);

                                szMessage = "You Lost by " + moveP1;
                                SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                m_Connection_Array[1].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);

                                moveP1 = "";
                                moveP2 = "";
                            }

                            /*If the checkWin method returns "Player 2 won", send player 2 this message
                            And send player 1 a message notifying them they have lost
                            Then reset player 1 and player 2's moves*/
                            if (checkWin() == "Player 2 won")
                            {
                                string szMessage = "You Won by " + moveP2;
                                byte[] SendMessage;
                                SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                m_Connection_Array[1].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);

                                szMessage = "You Lost by " + moveP2;
                                SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                m_Connection_Array[0].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);

                                moveP1 = "";
                                moveP2 = "";
                            }

                            /*If the checkWin method returns "Draw", send player 1 & player 2 this message
                            Then reset player 1 and player 2's moves*/
                            if (checkWin() == "Draw")
                            {
                                string szMessage = "Draw";
                                byte[] SendMessage;
                                SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                m_Connection_Array[1].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);

                                szMessage = "Draw";
                                SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                                m_Connection_Array[0].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);

                                moveP1 = "";
                                moveP2 = "";
                            }
                            else
                            {
                            }
                        }
                    }
                    //Code from sample code
                    catch (SocketException se) // Handle socket-related exception
                    {   // If an exception occurs, display an error message
                        if (10053 == se.ErrorCode || 10054 == se.ErrorCode) // Remote end closed the connection
                        {
                            CloseConnection(iIndex);
                        }
                        else if (10035 != se.ErrorCode)
                        {   // Ignore error messages relating to normal behaviour of non-blocking sockets
                            MessageBox.Show(se.Message);
                        }
                    }
                    catch // Silently handle any other exception
                    {
                    }
                }
            }
        }

        //CheckWin method used to find out which player has won
        private string checkWin() {
            
            if (moveP1 == "Rock" && moveP2 == "Scissors"){
                return "Player 1 won";
            }
            else if (moveP1 == "Scissors" && moveP2 == "Paper")
            {
                return "Player 1 won";
            }
            else if (moveP1 == "Paper" && moveP2 == "Rock")
            {
                return "Player 1 won";
            }
            else if (moveP2 == "Rock" && moveP1 == "Scissors")
            {
                return "Player 2 won";
            }
            else if (moveP2 == "Scissors" && moveP1 == "Paper")
            {
                return "Player 2 won";
            }
            else if (moveP2 == "Paper" && moveP1 == "Rock")
            {
                return "Player 2 won";
            }
            else if (moveP1 == "Rock" && moveP2 == "Rock")
            {
                return "Draw";
            }
            else if (moveP1 == "Paper" && moveP2 == "Paper")
            {
                return "Draw";
            }
            else if (moveP1 == "Scissors" && moveP2 == "Scissors")
            {
                return "Draw";
            }
            return null;
        }
        
        //Code from sample code
        //Used to Send message to each connected client informing of the total number of connected clients
        private void SendUpdateMesageToAllConnectedclients()
        {   
            int iIndex;
            for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
            {
                if (true == m_Connection_Array[iIndex].bInUse)
                {
                    string szMessage;
                    if (1 == m_iNumberOfConnectedClients)
                    {
                        szMessage = string.Format("There is now {0} client connected", m_iNumberOfConnectedClients);
                    }
                    else
                    {
                        szMessage = string.Format("There are now {0} clients connected", m_iNumberOfConnectedClients);
                    }
                    byte[] SendMessage = System.Text.Encoding.ASCII.GetBytes(szMessage);
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Send(SendMessage, SocketFlags.None);
                }
            }
        }

        //Code from sample code
        //Used to close a connection to a client
        private void CloseConnection(int iIndex)
        {
            try
            {
                m_Connection_Array[iIndex].bInUse = false;
                m_Connection_Array[iIndex].ClientSpecific_Socket.Shutdown(SocketShutdown.Both);
                m_Connection_Array[iIndex].ClientSpecific_Socket.Close();
                m_iNumberOfConnectedClients--;
                eventsTB.AppendText("Number of connected clients: " + System.Convert.ToString(m_iNumberOfConnectedClients) + Environment.NewLine);
                eventsTB.AppendText("A Connection was closed" + Environment.NewLine);
                SendUpdateMesageToAllConnectedclients();
            }
            catch // Silently handle any exceptions
            {
            }
        }

        //Code from sample code
        //ShutdownBtn calls the close and quit method and closes the connection
        private void shutdownBtn_Click(object sender, EventArgs e)
        {
            Close_And_Quit();
        }

        //Code from sample code
        //Close and quit method closes the connection to client securely
        private void Close_And_Quit()
        {   // Close the sockets and exit the application
            try
            {
                m_ListenSocket.Close();
            }
            catch
            {
            }
            try
            {
                int iIndex;
                for (iIndex = 0; iIndex < m_iMaxConnections; iIndex++)
                {
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Shutdown(SocketShutdown.Both);
                    m_Connection_Array[iIndex].ClientSpecific_Socket.Close();
                }
            }
            catch
            {
            }
            try
            {
                Close();
            }
            catch
            {
            }
        }
        
    }
}
