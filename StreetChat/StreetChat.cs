using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Management;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Media;

namespace StreetChat
{
    public partial class StreetChat : Form
    {
        private IPEndPoint FromIPEndPoint = new IPEndPoint(new IPAddress(0), (0));
        private UdpClient udpclient;
        private TcpListener tcpListener;
        private Thread threadUdp;
        private Thread threadTcp;
        private List<User> users = new List<User>();
        private bool newMsgRecieved = false;
        private int AdminPassHashed = 1034887541;
        private User localUser = new User(Dns.GetHostName());
        private string rightClickedUserName = "";
        private int portNumber = 5000;
        private string LastPrivate = "";
        private List<IPAddress> localIPs = new List<IPAddress>();
        private List<IPAddress> broadcastIPs = new List<IPAddress>();
        private bool ChatFocused = true;
        private List<string> LastMessages = new List<string>();
        private int currentLastIndex = -1;
        private ResourceManager lang;

        public StreetChat()
        {
            InitializeComponent();
            CheckForIllegalCrossThreadCalls = false;

            CultureInfo ci = CultureInfo.CurrentUICulture;

            if (ci.TwoLetterISOLanguageName == "da")
            {
                cmbbxLanguage.SelectedIndex = 1;
                lang = new ResourceManager("StreetChat.lang.da-DK", Assembly.GetExecutingAssembly());
            }
            else
            {
                cmbbxLanguage.SelectedIndex = 0;
                lang = new ResourceManager("StreetChat.lang.en-GB", Assembly.GetExecutingAssembly());
            }

            try
            {
                udpclient = new UdpClient(portNumber);
            }
            catch (Exception)
            {
                MessageBox.Show(String.Format(lang.GetString("PortInUse"), portNumber.ToString()));
                Process.GetCurrentProcess().Kill();
                throw;
            }

            SetControlText();
            cntxtMnuItemKick.Enabled = false;
            WriteSystemText(lang.GetString("WelcomeString"));

            IPHostEntry iphostentry = Dns.GetHostEntry(localUser.Username);

            threadUdp = new Thread(new ThreadStart(listenForPeers));
            threadUdp.Start();

            tcpListener = new TcpListener(IPAddress.Any, portNumber);
            threadTcp = new Thread(new ThreadStart(ListenForTcpConnection));
            threadTcp.Start();

            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                if (ipaddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    broadcastIPs.Add(Utilities.GetBroadcastAddress(ipaddress));
                    localIPs.Add(ipaddress);
                    localUser.IPAddress = ipaddress;
                }
            }

            localUser.ChatVersion = new Version(this.ProductVersion);
            localUser.IPEndPoint = new IPEndPoint(localUser.IPAddress, portNumber);
            localUser.IsLocalUser = true;
            localUser.UniqueID = Guid.NewGuid();

            users.Add(localUser);
            UpdateUserPanel();
            tmrSendEcho.Enabled = true;
        }
        private void SetControlText()
        {
            cntxtMnuItemCopyAll.Text = lang.GetString("CopyAll");
            cntxtMnuItemCopyTxt.Text = lang.GetString("CopyText");
            cntxtMnuItemDelAll.Text = lang.GetString("DelAll");
            cntxtMnuItemKick.Text = lang.GetString("Kick");
            cntxtMnuItemKick.ToolTipText = lang.GetString("KickTT");
            cntxtMnuItemSeeIP.Text = lang.GetString("SeeIP");
            cntxtMnuItemSeeIP.ToolTipText = lang.GetString("SeeIPTT");
            cntxtMnuItemSeeVersion.Text = lang.GetString("SeeVersion");
            cntxtMnuItemSeeVersion.ToolTipText = lang.GetString("SeeVersionTT");
            cntxtMnuItemWriteTo.Text = lang.GetString("WriteTo");
            cntxtMnuItemWriteTo.ToolTipText = lang.GetString("WriteToTT");
            chkbxNotifyNewMsg.Text = lang.GetString("NotifyNewMessage");
            lblBottomText.Text = String.Format(lang.GetString("BottomText"), ProductVersion);
        }
        private void tmrSendEcho_Tick(object sender, EventArgs e)
        {
            SendEcho();
        }
        /// <summary>
        /// Sends out an UDP echo to look for other peers
        /// </summary>
        private void SendEcho()
        {
            try
            {
                byte[] send = System.Text.Encoding.Unicode.GetBytes("ECHO");

                foreach (IPAddress address in broadcastIPs)
                {
                    udpclient.Send(send, send.Length, new IPEndPoint(address, portNumber));
                }
            }
            catch (Exception)
            {
                MessageBox.Show("You are not part of a NETWORK");
            }
        }
        /// <summary>
        /// Listens for other peers via UDP.
        /// </summary>
        private void listenForPeers()
        {
            while (true)
            {
                try
                {
                    byte[] data = udpclient.Receive(ref FromIPEndPoint);

                    string message = System.Text.Encoding.Unicode.GetString(data);

                    if (message == "ECHO")
                    {
                        if (!UserExists(FromIPEndPoint.Address) && !localIPs.Contains(FromIPEndPoint.Address))
                        {
                            connectToPeer(FromIPEndPoint);
                        }
                    }
                }
                catch (ThreadAbortException)
                {
                    try
                    {
                        udpclient.Close();
                    }
                    catch (Exception)
                    {
                        MessageBox.Show("Your connection cant close");
                    }
                }
            }
        }

        private void connectToPeer(IPEndPoint fromIPEndPoint)
        {
            TcpClient peer = new TcpClient();

            try
            {
                peer.Connect(fromIPEndPoint);
                Thread peerThread = new Thread(new ParameterizedThreadStart(HandleComm));
                peerThread.Start(peer);
            }
            catch (Exception)
            {
                
            }
        }
        private void ListenForTcpConnection()
        {
            this.tcpListener.Start();

            while (true)
            {
                if (tcpListener.Pending())
                {
                    TcpClient peer = this.tcpListener.AcceptTcpClient();

                    //create a thread to handle communication
                    //with connected peer
                    Thread peerThread = new Thread(new ParameterizedThreadStart(HandleComm));
                    peerThread.Start(peer);
                }

                Thread.Sleep(500);
            }
        }
        /// <summary>
        /// Sends local info object to peer
        /// </summary>
        /// <param name="peer">The peer to send the object to</param>
        private void SendLocalInfo(object peer)
        {
            TcpClient tcpPeer = (TcpClient)peer;

            while (!tcpPeer.Connected)
            {
                Thread.Sleep(500);
            }

            SendMessageToPeer(tcpPeer, MessageType.Info, Utilities.GetLocalInfoToSend(localUser));
        }
        private void HandleComm(object peer)
        {
            TcpClient tcpClient = (TcpClient)peer;
            NetworkStream clientStream = tcpClient.GetStream();

            byte[] message = new byte[4096];
            int bytesRead;

            Thread threadSendInfo = new Thread(new ParameterizedThreadStart(SendLocalInfo));
            threadSendInfo.Start(tcpClient);

            while (true)
            {
                bytesRead = 0;

                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch
                {
                    //a socket error has occured
                    break;
                }

                if (bytesRead == 0)
                {
                    //the client has disconnected from the server
                    clientStream.Close();
                    break;
                }

                //message has successfully been received
                ReadMessage(tcpClient, (Message)Utilities.ByteArrayToObject(message));
            }
        }
        /// <summary>
        /// Reads messages.
        /// <param name="fromPeer">The TcpClient who sent the message</param>
        /// <param name="messageObject">Incomming messageobject</param>
        /// </summary>
        private void ReadMessage(TcpClient fromPeer, Message messageObject)
        {
            User FromUser = getUser(messageObject.fromID);

            switch (messageObject.type)
            {
                case (MessageType.Normal):
                    //Message recieved
                    MessageRecieved();
                    WriteText(FromUser.Username + ": " + ((string)messageObject.attachment).Replace("%user%", localUser.Username));
                    break;
                case (MessageType.Info):
                    //Logged on
                    User newUser = new User((UserInfo)messageObject.attachment);
                    newUser.tcpClient = fromPeer;

                    if (AddUserToList(newUser))
                    {
                        WriteSystemText(String.Format(lang.GetString("LoggedOn"), newUser.Username));

                        if (newUser.ChatVersion > localUser.ChatVersion)
                        {
                            WriteSystemText(String.Format(lang.GetString("NewVersion"), localUser.ChatVersion.ToString(), newUser.Username, newUser.ChatVersion.ToString()));
                        }
                    }

                    if (newUser.Username == localUser.Username)
                    {
                        if (newUser.Started <= localUser.Started)
                        {
                            string newLocalUsername = localUser.Username;

                            while (newLocalUsername == newUser.Username)
                            {
                                newLocalUsername = Utilities.RandomString(10);
                            }

                            SendMessage(MessageType.System, "U" + newLocalUsername);
                            users.Find(delegate(User userToFind) { return (userToFind == localUser); }).Username = newLocalUsername;
                            localUser.Username = newLocalUsername;
                            UpdateUserPanel();
                            WriteSystemText(String.Format(lang.GetString("LocalUsernameChange"), newLocalUsername));
                        }
                    }
                    break;
                case (MessageType.System):
                    string message = (string)messageObject.attachment;
                    switch (message[0])
                    {
                        case ('A'):
                            //Admin
                            switch (message[1])
	                        {
                                case ('I'):
                                    //Info
                                    if (message[2] == 'K')
                                    {
                                        //Another user kicked
                                        WriteSystemText(String.Format(lang.GetString("UserKicked"), message.Substring(3), FromUser.Username));
                                    }
                                    break;
                                case ('K'):
                                    //Kick
                                    CloseChat();
                                    break;
                                case ('L'):
                                    //A user becomes Admin
                                    FromUser.IsAdmin = true;
                                    UpdateUserPanel();
                                    WriteSystemText(String.Format(lang.GetString("IsNowAdmin"), FromUser.Username));
                                    break;
                                default:
                                    break;
	                        }
                            break;
                        case ('I'):
                            //IP address
                            WriteSystemText(String.Format(lang.GetString("ShowIP"), FromUser.Username, FromUser.IPAddress.ToString()));
                            break;
                        case ('S'):
                            //Status
                            string newStatus = message.Substring(1);
                            string oldStatus = FromUser.Status;

                            if (newStatus.Length == 0 && oldStatus.Length != 0)
                            {
                                FromUser.Status = "";
                                WriteSystemText(String.Format(lang.GetString("StatusRemoved"), FromUser.Username, oldStatus));
                            }
                            else
                            {
                                FromUser.Status = newStatus;
                                WriteSystemText(String.Format(lang.GetString("StatusChange"), FromUser.Username, newStatus));
                            }
                            
                            UpdateUserPanel();
                            break;
                        case ('U'):
                            string oldName = FromUser.Username;
                            string newUsername = message.Substring(1);
                            FromUser.Username = newUsername;
                            WriteSystemText(String.Format(lang.GetString("UsernameChange"), oldName, newUsername));
                            UpdateUserPanel();
                            break;
                        default:
                            break;
                    }
                    break;
                case (MessageType.Private):
                    MessageRecieved();
                    WriteText(FromUser.Username + ": " + ((string)messageObject.attachment).Replace("%user%", localUser.Username), Color.MediumPurple);
                    LastPrivate = FromUser.Username;
                    break;
                default:
                    break;
            }
        }
        private void MessageRecieved()
        {
            if (!ChatFocused && chkbxNotifyNewMsg.Checked)
            {
                playSound("newMsg");
                newMsgRecieved = true;
            }
        }
        /// <summary>
        /// Sends a message.
        /// <param name="message">Message to send</param>
        /// </summary>
        private void SendMessage(string message)
        {
            foreach (User peer in users.FindAll(delegate(User userToFind) { return !userToFind.IsLocalUser && userToFind.IsConnected; }))
            {
                SendMessageToPeer(peer.tcpClient, MessageType.Normal, message);
            }
        }
        /// <summary>
        /// Sends a message.
        /// <param name="type">The message type</param>
        /// <param name="message">Message to send</param>
        /// </summary>
        private void SendMessage(MessageType type, string message)
        {
            foreach (User peer in users.FindAll(delegate(User userToFind) { return !userToFind.IsLocalUser && userToFind.IsConnected; }))
            {
                SendMessageToPeer(peer.tcpClient, type, message);
            }
        }
        /// <summary>
        /// Sends a message to a specific TcpClient.
        /// <param name="peer">The TcpClient to send the message to</param>
        /// <param name="message">Message to send</param>
        /// </summary>
        private void SendMessageToPeer(TcpClient peer, string message)
        {
            SendMessageToPeer(peer, MessageType.Normal, message);
        }
        /// <summary>
        /// Sends a message to a specific TcpClient.
        /// <param name="peer">The TcpClient to send the message to</param>
        /// <param name="type">Type of the message. Fx 'M' for message</param>
        /// <param name="attachment">The object to attach</param>
        /// </summary>
        private void SendMessageToPeer(TcpClient peer, MessageType type, object attachment)
        {
            NetworkStream clientStream = peer.GetStream();
            //UnicodeEncoding encoder = new UnicodeEncoding();
            //byte[] buffer = encoder.GetBytes(message);
            byte[] buffer = Utilities.ObjectToByteArray(new Message(localUser.UniqueID, type, attachment));

            clientStream.Write(buffer, 0, buffer.Length);
            clientStream.Flush();
        }
        private void btnSendMsg_Click(object sender, EventArgs e)
        {
            string message = Regex.Replace(rtxtbxMsg.Text, @"\s+", " ");

            if (!(message == ""))
            {
                if (message[0] == '/')
                {
                    message += "      ";

                    switch (message.Substring(1, message.IndexOf(" ") - 1).ToLower())
                    {
                        case ("admins"):
                            if (AdminExists())
                            {
                                WriteSystemText(lang.GetString("FollowingIsAdmin"));
                                foreach (User admin in users.FindAll(delegate(User userToFind) { return (userToFind.IsAdmin == true); }))
                                {
                                    WriteSystemText(admin.Username);
                                }
                                WriteSystemText("----------------");
                            }
                            else
                            {
                                WriteSystemText(lang.GetString("NoAdmin"));
                            }
                            break;
                        case ("afk"):
                            if (localUser.Status != "AFK")
                            {
                                localUser.Status = "AFK";
                                SendMessage(MessageType.System, "SAFK");
                                UpdateUserPanel();
                                WriteSystemText(String.Format(lang.GetString("LocalStatusChange"), "AFK"));
                            }
                            else
                            {
                                localUser.Status = "";
                                SendMessage(MessageType.System, "S");
                                UpdateUserPanel();
                                WriteSystemText(String.Format(lang.GetString("LocalStatusRemoved"), "AFK"));
                            }
                            break;
                        case ("help"):
                            ShowHelp();
                            break;
                        case ("info"):
                            WriteSystemText(lang.GetString("Info"));
                            break;
                        case ("ip"):
                            SendMessage(MessageType.System, "I");
                            WriteSystemText(String.Format(lang.GetString("ShowIP"), localUser.Username, localUser.IPAddress.ToString()));
                            break;
                        case ("ips"):
                            WriteSystemText(lang.GetString("ShowIPs"));
                            foreach (User User in users)
                            {
                                WriteSystemText(User.Username + " - " + User.IPAddress.ToString());
                            }
                            WriteSystemText("---------------");
                            break;
                        case ("login"):
                            if (localUser.IsAdmin)
                            {
                                WriteSystemText(lang.GetString("AlreadyAdmin"));
                            }
                            else
                            {
                                if (message.Substring(7).Trim().GetHashCode() == AdminPassHashed)
                                {
                                    ChangeToAdmin();
                                }
                                else
                                {
                                    localUser.IsAdmin = false;
                                    cntxtMnuItemKick.Enabled = false;
                                    WriteSystemText(lang.GetString("WrongLogin"));
                                }
                            }
                            break;
                        case ("kick"):
                            if (localUser.IsAdmin)
                            {
                                if (message.Substring(6, 1) == " ")
                                {
                                    WriteSystemText(lang.GetString("MissingUsername"));
                                }
                                else if (getUser(message.Trim().Substring(6)) == null)
                                {
                                    WriteSystemText(String.Format(lang.GetString("UserNotOnline"), message.Trim().Substring(6)));
                                }
                                else
                                {
                                    KickUser(message.Substring(6).Trim());
                                }
                            }
                            else
                            {
                                WriteSystemText(lang.GetString("NotPossible"));
                            }
                            break;
                        case ("love"):
                            WriteText(lang.GetString("Love"), Color.DeepPink);
                            this.Text = "<3 Henriette <3";
                            break;
                        case ("request"):
                            if (!localUser.IsAdmin)
                            {

                                if (users.Count == 1)
                                {
                                    WriteSystemText(lang.GetString("AloneAdminRequest"));
                                }
                                else
                                {
                                    if (!AdminExists())
                                    {
                                        ChangeToAdmin();
                                    }
                                    else
                                    {
                                        WriteSystemText(lang.GetString("AdminAlreadyExist"));
                                    }
                                }
                            }
                            else
                            {
                                WriteSystemText(lang.GetString("AlreadyAdmin"));
                            }
                            
                            break;
                        case ("status"):
                            string newStatus = message.Substring(8).Trim();
                            string oldStatus = localUser.Status;

                            if (oldStatus == newStatus)
                            {
                                //Breaks case if new and old status is the same. Case-sencitive
                                break;
                            }

                            if (newStatus.Length == 0)
                            {
                                if (oldStatus.Length != 0)
                                {
                                    localUser.Status = "";
                                    SendMessage(MessageType.System, "S");
                                    WriteSystemText(String.Format(lang.GetString("LocalStatusRemoved"), oldStatus));
                                    UpdateUserPanel();
                                }
                            }
                            else if (newStatus.Length > 5)
                            {
                                WriteSystemText(lang.GetString("WrongStatusFormat"));
                            }
                            else
                            {
                                localUser.Status = newStatus;
                                SendMessage(MessageType.System, "S" + newStatus);
                                UpdateUserPanel();
                                WriteSystemText(String.Format(lang.GetString("LocalStatusChange"), newStatus));
                            }
                            break;
                        case ("username"):
                            string newUsername = message.Substring(10).Trim();
                            Regex regexItem = new Regex(@"^[a-zA-Z0-9_-]{3,20}$");

                            if (!regexItem.IsMatch(newUsername))
                            {
                                WriteSystemText(lang.GetString("WrongUsernameFormat"));
                            }
                            else if (UserExists(newUsername))
                            {
                                if (newUsername == localUser.Username)
                                {
                                    WriteSystemText(String.Format(lang.GetString("UsernameExist1"), newUsername));
                                }
                                else
                                {
                                    WriteSystemText(String.Format(lang.GetString("UsernameExist2"), newUsername));
                                }
                            }
                            else
                            {
                                SendMessage(MessageType.System, "U" + newUsername);
                                users.Find(delegate(User userToFind) { return (userToFind == localUser); }).Username = newUsername;
                                UpdateUserPanel();
                                localUser.Username = newUsername;
                                WriteSystemText(String.Format(lang.GetString("LocalUsernameChange"), newUsername));
                            }
                            break;
                        case ("versions"):
                            WriteSystemText(lang.GetString("ShowVersions"));
                            foreach (User User in users)
                            {
                                WriteSystemText(User.Username + " - " + User.ChatVersion);
                            }
                            WriteSystemText("------------------");
                            break;
                        case ("w"):
                            string username = message.Split(new char[] { ' ' } )[1];
                            User userToWhisper = getUser(username);
                            string newMessage = message.Substring(message.IndexOf(' ', 3) + 1).Trim();

                            if (username == "")
                            {
                                WriteSystemText(lang.GetString("MissingUsername"));
                            }
                            else if (newMessage == "")
                            {
                                WriteSystemText(lang.GetString("EmptyMessage"));
                            }
                            else
                            {
                                if (userToWhisper != null)
                                {
                                    if (userToWhisper.IsLocalUser)
                                    {
                                        WriteSystemText(lang.GetString("PrivateToLocalError"));
                                    }
                                    else
                                    {
                                        SendMessageToPeer(userToWhisper.tcpClient, MessageType.Private, newMessage);
                                        WriteText(">" + userToWhisper.Username + ": " + newMessage, Color.MediumPurple);
                                        LastPrivate = userToWhisper.Username;
                                    }
                                }
                                else
                                {
                                    WriteSystemText(String.Format(lang.GetString("UserNotOnline"), username));
                                }
                            }
                            break;
                        default:
                            WriteSystemText(lang.GetString("WrongCommand"));
                            break;
                    }
                }
                else
                {
                    SendMessage(message);
                    WriteText(localUser.Username + ": " + message.Replace("%user%", localUser.Username));
                }

                LastMessages.Insert(0, rtxtbxMsg.Text);
                currentLastIndex = -1;
                rtxtbxMsg.Text = "";
                FocusChatFrame();
            }
        }

        private void KickUser(string Username)
        {
            SendMessageToPeer(getUser(Username).tcpClient, MessageType.System, "AK");
            SendMessage(MessageType.System, "AIK" + Username);
            WriteSystemText(String.Format(lang.GetString("UserKicked"), Username, localUser.Username));
        }
        /// <summary>
        /// Show help
        /// </summary>
        private void ShowHelp()
        {
            WriteSystemText(lang.GetString("Help"));
        }
        /// <summary>
        /// Checks whether any Admin exists.
        /// </summary>
        /// <returns>Bool</returns>
        private bool AdminExists()
        {
            return users.Exists(delegate(User userToFind) { return (userToFind.IsAdmin == true); });
        }
        /// <summary>
        /// Change the current user to Admin
        /// </summary>
        private void ChangeToAdmin()
        {
            WriteSystemText(String.Format(lang.GetString("IsNowAdmin"), localUser.Username));
            users.Find(delegate(User userToFind) { return (userToFind.IsLocalUser == true); }).IsAdmin = true;
            localUser.IsAdmin = true;
            cntxtMnuItemKick.Enabled = true;
            SendMessage(MessageType.System, "AL");
        }
        /// <summary>
        /// Fires when the chat is closed
        /// </summary>
        private void Chat_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseChat();
        }
        private void CloseChat()
        {
            foreach (User _user in users.FindAll(delegate(User userToFind) { return !userToFind.IsLocalUser && userToFind.IsConnected; }))
            {
                _user.tcpClient.GetStream().Close();
                _user.tcpClient.Close();
            }
            threadUdp.Abort();
            udpclient.Close();
            Process.GetCurrentProcess().Kill();
        }
        /// <summary>
        /// Adds user to List users and returns true if user was added
        /// and false if the user already existed.
        /// <param name="NewUser">The user to add</param>
        /// </summary>
        private bool AddUserToList(User NewUser)
        {
            bool isNewUser = !UserExists(NewUser.UniqueID);
            
            if (isNewUser)
            {
                users.Add(NewUser);
                
                UpdateUserPanel();
            }

            return isNewUser;
        }
        /// <summary>
        /// Checks if there already is a user in List users with the given IPAddress.
        /// <param name="userIP">The IPAddress to check.</param>
        /// </summary>
        private bool UserExists(IPAddress userIP)
        {
            bool exists = false;

            if (users.Find(delegate(User userToFind) { return userToFind.IPAddress.ToString() == userIP.ToString(); }) != null)
	        {
                exists = true;
	        }

            return exists;
        }
        /// <summary>
        /// Checks if there already is a user in List users with the given IPAddress.
        /// <param name="uniqueID">The IPAddress to check.</param>
        /// </summary>
        private bool UserExists(Guid uniqueID)
        {
            bool exists = false;

            if (users.Find(delegate(User userToFind) { return userToFind.UniqueID == uniqueID; }) != null)
            {
                exists = true;
            }

            return exists;
        }
        /// <summary>
        /// Checks if there already is a user in List users with the given username.
        /// <param name="username">The username to check.</param>
        /// </summary>
        private bool UserExists(string username)
        {
            bool exists = false;

            if (users.Find(delegate(User userToFind) { return userToFind.Username.ToLower() == username.ToLower(); }) != null)
            {
                exists = true;
            }

            return exists;
        }
        /// <summary>
        /// Clears the UserPanel and then adds all users again from the updated users-list.
        /// </summary>
        private void UpdateUserPanel()
        {
            lstbxUsers.Items.Clear();
            lstbxUsers.Items.AddRange(users.ToArray());
        }
        /// <summary>
        /// Writes a Systemmessage in the chat. Fx.
        /// ---User logged off---
        /// <param name="text">Text to write</param>
        /// </summary>
        private void WriteSystemText(string text)
        {
            WriteText(text, Color.Red);
        }
        /// <summary>
        /// Writes a new line of text in the chat. Fx
        /// Lol omg!
        /// <param name="text">Text to write</param>
        /// </summary>
        private void WriteText(string newText)
        {
            WriteText(newText, Color.Black);
        }
        private void WriteText(string newText, Color color)
        {
            if (rtxtbxChat.Text != "")
            {
                newText = "\n" + newText;
            }

            rtxtbxChat.SelectionColor = color;
            rtxtbxChat.AppendText(newText);
        }
        /// <summary>
        /// Gets a specific user from the users-list with the Guid as criteria.
        /// Returns null if no user is found
        /// <param name="uniqueID">The guid to find</param>
        /// </summary>
        private User getUser(Guid uniqueID)
        {
            User userToGet = users.Find(delegate(User userToFind) { return userToFind.UniqueID == uniqueID; });

            return userToGet;
        }
        /// <summary>
        /// Gets a specific user from the users-list with the username as criteria.
        /// Returns null if no user is found
        /// <param name="userIP">The IPAddress to find</param>
        /// </summary>
        private User getUser(string username)
        {
            User userToGet = users.Find(delegate(User userToFind) { return userToFind.Username.ToLower() == username.ToLower(); });

            return userToGet;
        }

        private void tmrCleanUpUsers_Tick(object sender, EventArgs e)
        {
            CleanUpUsers();
        }
        /// <summary>
        /// Checks each user if he/she is connected. If not removes user.
        /// </summary>
        private void CleanUpUsers()
        {
            foreach (User _user in users.FindAll(delegate(User userToFind) { return !userToFind.IsLocalUser && !userToFind.IsConnected; }))
            {
                _user.tcpClient.Close();
                users.Remove(_user);
                WriteSystemText(String.Format(lang.GetString("LoggedOff"), _user.Username));
            }

            UpdateUserPanel();
        }
        private void StreetChat_Activated(object sender, EventArgs e)
        {
            ChatFocused = true;
            Text = lang.GetString("Title");
            newMsgRecieved = false;
        }
        private void StreetChat_Deactivate(object sender, EventArgs e)
        {
            ChatFocused = false;
        }
        private void tmrNewMsg_Tick(object sender, EventArgs e)
        {
            if (newMsgRecieved)
            {
                if (Text == lang.GetString("Title")) { Text = lang.GetString("TitleNewMessage"); }
                else { Text = lang.GetString("Title"); }
            }
        }
        private void playSound(string soundToPlay)
        {
            if (soundToPlay == "newMsg")
            {
                try
                {
                    SoundPlayer sndplayr = new SoundPlayer(Properties.Resources.SpeechOn);
                    sndplayr.Play();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + ": " + ex.StackTrace.ToString(), "Error");
                }
            }
        }

        private void cntxtMnuUserList_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            User clickedUser = getUser(rightClickedUserName);

            switch (e.ClickedItem.Tag.ToString())
            {
                case ("kick"):
                    KickUser(clickedUser.Username);
                    break;
                case ("seeip"):
                    WriteSystemText(String.Format(lang.GetString("ShowIP"), clickedUser.Username, clickedUser.IPAddress.ToString()));
                    break;
                case ("seeversion"):
                    WriteSystemText(String.Format(lang.GetString("ShowVersion"), clickedUser.Username, clickedUser.ChatVersion));
                    break;
                case ("writeto"):
                    rtxtbxMsg.Text = "/w " + clickedUser.Username + " " + rtxtbxMsg.Text;
                    break;
            }
            FocusChatFrame();
        }
        private void cntxtMnuUserList_Opening(object sender, CancelEventArgs e)
        {
            string clickedUsername = ((User)lstbxUsers.SelectedItem).Username;
            cntxtMnuItemSeeIP.ToolTipText = cntxtMnuItemSeeIP.ToolTipText.Replace("%user%", clickedUsername);
            cntxtMnuItemSeeVersion.ToolTipText = cntxtMnuItemSeeVersion.ToolTipText.Replace("%user%", clickedUsername);
            cntxtMnuItemWriteTo.ToolTipText = cntxtMnuItemWriteTo.ToolTipText.Replace("%user%", clickedUsername);
            cntxtMnuItemKick.ToolTipText = cntxtMnuItemKick.ToolTipText.Replace("%user%", clickedUsername);
        }

        private void cntxtMnuChat_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            switch (e.ClickedItem.Tag.ToString())
            {
                case ("delall"):
                    rtxtbxChat.Clear();
                    break;
                case ("copytxt"):
                    Clipboard.SetText(rtxtbxChat.SelectedText.Replace("\n", Environment.NewLine));
                    rtxtbxChat.DeselectAll();
                    break;
                case ("copyall"):
                    Clipboard.SetText(rtxtbxChat.Text.Replace("\n", Environment.NewLine));
                    break;
            }
            FocusChatFrame();
        }

        private void lstbxUsers_DoubleClick(object sender, EventArgs e)
        {
            if (lstbxUsers.SelectedIndex != -1)
            {
                string insertText = "";
                string username = ((User)lstbxUsers.SelectedItem).Username;
                if (rtxtbxMsg.Text == "")
                {
                    insertText = "/w " + username + " ";
                }
                else
                {
                    if (rtxtbxMsg.Text.EndsWith(" "))
                    {
                        insertText = username + " ";
                    }
                    else
                    {
                        insertText = " " + ((User)lstbxUsers.SelectedItem).Username + " ";
                    }
                }
                rtxtbxMsg.Text += insertText;
                FocusChatFrame();
            }
        }

        private void lstbxUsers_MouseUp(object sender, MouseEventArgs e)
        {
            int selectedIndex = lstbxUsers.IndexFromPoint(e.X, e.Y);

            if (selectedIndex != -1)
            {
                if (e.Button == MouseButtons.Right)
                {
                    lstbxUsers.SelectedIndex = selectedIndex;
                    rightClickedUserName = ((User)lstbxUsers.SelectedItem).Username;
                    cntxtMnuUserList.Show(MousePosition);
                }
            }
            else
            {
                lstbxUsers.ClearSelected();
            }
        }

        private void rtxtbxChat_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (rtxtbxChat.SelectedText != "")
                {
                    cntxtMnuItemCopyTxt.Visible = true;
                    cntxtMnuItemDelAll.Visible = false;
                }
                else
                {
                    cntxtMnuItemCopyTxt.Visible = false;
                    cntxtMnuItemDelAll.Visible = true;
                }
            }
        }

        private void StreetChat_HelpRequested(object sender, HelpEventArgs hlpevent)
        {
            ShowHelp();
        }

        private void rtxtbxMsg_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyData)
            {
                case Keys.Up:
                    //Fires when the Up key is pressed
                    if (currentLastIndex < (LastMessages.Count - 1))
                    {
                        currentLastIndex++;
                    }
                    if (LastMessages.Count != 0)
                    {
                        rtxtbxMsg.Text = LastMessages[currentLastIndex];
                    }
                    e.SuppressKeyPress = true;
                    FocusChatFrame();
                    break;
                case Keys.Down:
                    //Fires when the Down key is pressed
                    if (currentLastIndex > 0)
                    {
                        currentLastIndex--;
                    }
                    if (LastMessages.Count != 0 && currentLastIndex >= 0)
                    {
                        rtxtbxMsg.Text = LastMessages[currentLastIndex];
                    }
                    e.SuppressKeyPress = true;
                    FocusChatFrame();
                    break;
                case (Keys.Control | Keys.R):
                    //Fires when CTRL+R is pressed. Also suppresses the command
                    if (LastPrivate != "")
                    {
                        rtxtbxMsg.Text = "/w " + LastPrivate + " ";
                    }
                    e.SuppressKeyPress = true;
                    FocusChatFrame();
                    break;
                default:
                    break;
            }
        }
        private void FocusChatFrame()
        {
            rtxtbxMsg.Focus();
            rtxtbxMsg.Select(rtxtbxMsg.Text.Length, 0);
        }
        /// <summary>
        /// Enum for messagetypes
        /// </summary>
        public enum MessageType
        {
            Normal,
            System,
            Info,
            Private
        };
        private void StreetChat_Shown(object sender, EventArgs e)
        {
            FocusChatFrame();
            SendEcho();
        }

        private void cmbbxLanguage_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (cmbbxLanguage.SelectedItem.ToString())
            {
                case ("English"):
                    lang = new ResourceManager("StreetChat.lang.en-GB", Assembly.GetExecutingAssembly());
                    break;
                case ("Dansk"):
                    lang = new ResourceManager("StreetChat.lang.da-DK", Assembly.GetExecutingAssembly());
                    break;
                default:
                    lang = new ResourceManager("StreetChat.lang.en-GB", Assembly.GetExecutingAssembly());
                    break;
            }
            SetControlText();
        }
    }
}