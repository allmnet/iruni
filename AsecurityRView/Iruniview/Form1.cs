using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Iruniview.Monitors;
using Iruniview.Notifiers;
using System.Net.Sockets;
using System.Net;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Iruniview
{
    public delegate void PassValueHandler(string strValue);


    public partial class Form1 : Form
    {

        public static string Jobid;

        public static PassValueHandler PassValue;

        public static Search Searchfrm;

        public static LogView _logForm;

        public static RuleView _ruleForm;

        public static ads _ads;

        public static About _aboutForm;

        private static int ReadResult = 0;

        private Socket socket;
        private const int RECEIVE_BUFFER_SIZE = 1024;
        private byte[] receiveBuffer = new Byte[RECEIVE_BUFFER_SIZE];
        // private EndPoint remoteEndpoint = null;

        private TcpClient tcpClient = null;
        private int serverport = 0;
        private int agentprot = 0;
        private static string strLocalPath;
        private string strCheckIP_Temp;

        Thread serverThread = null;

        private TcpListener tcpListener;
        private bool state;
        ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public class SocketConnectionInfo
        {
            public const Int32 BufferSize = 1048576;
            public Socket Socket;
            public byte[] Buffer;
            public Int32 BytesRead { get; set; }
        }

        internal static class SafeNativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
            internal static extern int GetPrivateProfileString(String section, String key, String def, StringBuilder retVal, int size, String filePath);
        }

        private static String GetIniValue(String section, String key)
        {
            StringBuilder temp = new StringBuilder(255);
            SafeNativeMethods.GetPrivateProfileString(section, key, "", temp, 255, strLocalPath);
            return temp.ToString();
        }

        void ProcessIncomingConnection(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient tcpClient = listener.EndAcceptTcpClient(ar);

            ThreadPool.QueueUserWorkItem(ThreadProc, tcpClient);
            tcpClientConnected.Set();
        }

        private void Server()
        {
            try
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                this.tcpListener = new TcpListener(IPAddress.Any, serverport);
                this.tcpListener.Start();

                while (state)
                {
                    while (!tcpListener.Pending())
                    {
                        Thread.Sleep(100);
                    }
                    tcpClientConnected.Reset();
                    tcpListener.BeginAcceptTcpClient(new AsyncCallback(ProcessIncomingConnection), tcpListener);
                    tcpClientConnected.WaitOne();
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("server error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
            }
        }

        public void ThreadProc(object obj)
        {
            TcpClient clientSocket = (TcpClient)obj;
            try
            {
                NetworkStream network = clientSocket.GetStream();
                string _data = null;
                byte[] ReadByte;
                ReadByte = new byte[clientSocket.ReceiveBufferSize];
                network.Read(ReadByte, 0, (int)ReadByte.Length);

                try
                {
                    _data = Crypto.Decrypt(ReadByte);
                    if (_data.Length == 0)
                    {
                        return;
                    }
                }
                catch (Exception)
                {
                    if (clientSocket != null) clientSocket.Client.Close();
                    return;
                }

                if (_data.StartsWith("JobID"))
                {
                    string sourceIP = null;
                    string type = null;
                    string searchresult = null;

                    string[] temp = _data.Split(',');
                    if (temp.Length == 3)
                    {
                        sourceIP = temp[0];
                        type = temp[1];
                        searchresult = temp[2];
                    }
                    ListViewItem newitem = new ListViewItem(new string[] { sourceIP, type, searchresult });

                    Searchfrm.SearchlistView.Items.Add(newitem);
                }
                else
                {
                    try
                    {
                        string sourceIP;
                        string version;
                        string agent;
                        string service;
                        string port;
                        string firewall;
                        string type;
                        string health;
                        string grouptype;
                        string asecuritystate;

                        string[] temp = _data.Split(',');
                        if (temp.Length != 9)
                        {
                            version = "null";
                            agent = "null";
                            firewall = "null";
                            service = "null";
                            port = "null";
                            type = "Other";
                            health = "null";
                            grouptype = "null";
                            asecuritystate = "null";
                        }
                        else
                        {
                            type = temp[0]; //Type
                            service = temp[1];
                            version = temp[2];
                            health = temp[3];
                            agent = temp[4];
                            firewall = temp[5];
                            port = temp[6];
                            asecuritystate = temp[7];
                            grouptype = temp[8];
                        }

                        sourceIP = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString();

                        //메세지 파싱후 해당 값을 이용해 처리

                        /* 스레드를 이용해서 전송 생성 */
                        CreateTasks(sourceIP, agent, version, service, port, type, health, firewall, asecuritystate, grouptype);
                    }
                    catch (SocketException ex)
                    {
                        EventLogger.LogEvent("Error listerner with message: " + ex.Message,
    System.Diagnostics.EventLogEntryType.Warning);
                    }
                }

            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("sorket error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
            }
            finally
            {
                if (clientSocket != null) clientSocket.Client.Close();
            }
        }

        public Form1()
        {
            InitializeComponent();

            CreateGroups();
            SetUpEventHandlers();

            try
            {
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://asecurity.so/wp-content/uploads/2017/11/iruniview.txt");
                string currentversion = Properties.Resources.Version;
                StreamReader reader = new StreamReader(stream);
                String content = reader.ReadToEnd();
                String[] updatetemp = content.Split('|');
                string url = null;
                string process = null;
                string version = null;
                foreach(string item in updatetemp)
                {
                    if (item.Contains("-url")) url = item;
                    if (item.Contains("-process")) process = item;
                    if (item.Contains("version")) version = item;
                }
                
                if (!String.IsNullOrEmpty(url) & !String.IsNullOrEmpty(process) & !String.IsNullOrEmpty(version))
                {                    
                    if (version != currentversion)
                    {
                        DialogResult dialogResult = MessageBox.Show(null, "You need to update.\nWould you like to upgrade now?", "Autoupdate", MessageBoxButtons.YesNo);

                        if (dialogResult == DialogResult.Yes)
                        {
                            string startprocess = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            process += @"\\update.exe";
                            string args = url + " " + process;
                            Process.Start(process, args);
                            return;
                        }
                    }
                }
            }
            catch(Exception)
            {
                MessageBox.Show(null, "You must connect to internet for autoupdate.", "Fail to Autoupdate", MessageBoxButtons.OK);
            }

            strLocalPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            strLocalPath += "\\iruniview.ini";
            string temp = GetIniValue("Config", "Port");
            


            if(!string.IsNullOrEmpty(temp))
            {
                serverport = Convert.ToInt32(temp);
                agentprot = serverport + 1;
            }
            if (serverport == 0)
            {
                MessageBox.Show(@"iruniview.ini cannot read!!");
                Application.Exit();
            }
            strCheckIP_Temp = GetIniValue("Check", "IP");

            notifyIcon.Icon = Icon;
            notifyIcon.Text = Application.ProductName;

            textBox_ServerState.Text = "Running";
            textBox_Port.Text = serverport.ToString();
            textBox_Port.ReadOnly = true;
            button_Serverlist.Image = Iruniview.Properties.Resources.serverstop;

            state = true;

            try
            {
                this.serverThread = new Thread(new ThreadStart(Server));
                this.serverThread.Start();
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Can't create serverThread because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);

                return;
            }
            /*
            try
            {
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Tcp);
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Could not create socket because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);

                return;
            }

            try
            {
                this.socket.Bind(new IPEndPoint(IPAddress.Any, serverport));
            }
            catch (Exception ex)
            {
                this.socket.Close();
                this.socket = null;

                EventLogger.LogEvent("Could not bind socket because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);

                return;
            }

            if (this.socket == null) { return; }

            try
            {
                // Start the listen operation on the socket
                RegisterReceiveOperation();
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Could not register socket on data received event because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                return;
            }
            */
        }

        /*
        public bool RegisterReceiveOperation()
        {
            // Ensure that the listener socket is still alive
            if (this.socket == null) { return false; }

            try
            {
                // receive from anybody
                this.remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

                EndPoint ep = (EndPoint)this.remoteEndpoint;

                // Setup the receive buffer to be used when a message is received
                this.receiveBuffer = new byte[RECEIVE_BUFFER_SIZE]; // nice and big receive buffer

                // Setup the receive callback
                this.socket.BeginReceiveFrom(receiveBuffer, 0, RECEIVE_BUFFER_SIZE, SocketFlags.None, ref this.remoteEndpoint,
                    new AsyncCallback(ReceiveCallback), this.socket);
            }
            catch (Exception ex)
            {
                this.socket.Close();
                this.socket = null;

                EventLogger.LogEvent("Could not add callback method to the socket because: " + ex.Message,
                     System.Diagnostics.EventLogEntryType.Warning);
            }
            return true;
        }
        */
        /*
        private void ReceiveCallback(IAsyncResult result)
        {
            // get a reference to the socket on which the message was received
            Socket sock = (Socket)result.AsyncState;

            EndPoint ep = null;
            IPEndPoint remoteEP = null;

            // variable to store received data length
            int inlen;

            remoteEndpoint = new IPEndPoint(IPAddress.Any, 0);

            // Gather information about the message and the sender
            try
            {
                ep = (EndPoint)remoteEndpoint;
                inlen = sock.EndReceiveFrom(result, ref ep);
                remoteEP = (IPEndPoint)ep;
            }
            catch (Exception ex)
            {
                // only post messages if class socket reference is not null
                // in all other cases, the socket has been terminated
                if (this.socket != null)
                {
                    EventLogger.LogEvent("Receive operation failed with message: " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Warning);
                }
                inlen = -1;
            }

            // if socket has been closed, ignore received data and return
            if (this.socket == null) return;

            // check that received data is long enough
            if (inlen <= 0)
            {
                // request next packet
                RegisterReceiveOperation();
                return;
            }

            string packet = null;

            // Get the human readable text of the message to process
            try
            {
                packet = System.Text.Encoding.UTF8.GetString(receiveBuffer, 0, inlen);
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Could not parse packet to string because: " + ex.Message,
                        System.Diagnostics.EventLogEntryType.Warning);
            }

            if (!string.IsNullOrEmpty(packet))
            {
                if (packet.StartsWith("JobID"))
                {
                    string sourceIP = null;
                    string type = null;
                    string searchresult = null;

                    string[] temp = packet.Split(',');
                    if (temp.Length == 3)
                    {
                        sourceIP = temp[0];
                        type = temp[1];
                        searchresult = temp[2];
                    }
                    ListViewItem newitem = new ListViewItem(new string[] { sourceIP, type, searchresult });

                    Searchfrm.SearchlistView.Items.Add(newitem);
                }
                else
                {
                    try
                    {
                        string sourceIP;
                        string version;
                        string agent;
                        string service;
                        string port;
                        string firewall;
                        string type;
                        string health;
                        string grouptype;
                        string asecuritystate;

                        string[] temp = packet.Split(',');
                        if (temp.Length != 9)
                        {
                            version = "null";
                            agent = "null";
                            firewall = "null";
                            service = "null";
                            port = "null";
                            type = "Other";
                            health = "null";
                            grouptype = "null";
                            asecuritystate = "null";
                        }
                        else
                        {
                            type = temp[0]; //Type
                            service = temp[1];
                            version = temp[2];
                            health = temp[3];
                            agent = temp[4];
                            firewall = temp[5];
                            port = temp[6];
                            asecuritystate = temp[7];
                            grouptype = temp[8];
                        }

                        sourceIP = remoteEP.Address.ToString();

                        //메세지 파싱후 해당 값을 이용해 처리

                        // 스레드를 이용해서 전송 생성 
                        CreateTasks(sourceIP, agent, version, service, port, type, health, firewall, asecuritystate, grouptype);
                    }
                    catch (SocketException ex)
                    {
                        EventLogger.LogEvent("Error listerner with message: " + ex.Message,
    System.Diagnostics.EventLogEntryType.Warning);
                    }
                }
            }
            // Return the socket to the listen state
            RegisterReceiveOperation();
        }
        */
        /*
        private void Serve()
        {

            if (strCheckIP_Temp.Length != 0)
            {
                    try
                    {
                        string[] strCheckIP_List = strCheckIP_Temp.Split(',');
                        foreach (string item in strCheckIP_List)
                        {
                            string[] temp = item.Split(':');
                            CreateTasks(temp[0], "null", "null", "null", temp[1], "null", "null", "null", "null", "null");
                        }
                    }
                    catch (Exception ex)
                    {
                        EventLogger.LogEvent("Cannot create list with message: " + ex.Message,
    System.Diagnostics.EventLogEntryType.Warning);
                        PassLog("Server", "Cannot create list with message: " + ex.ToString());
                    }
            }

        }
        */
        private void CreateGroups()
        {
            foreach (string monitorType in Enum.GetNames(typeof(MonitorType)))
            {
                tasksListView.Groups.Add(new ListViewGroup(monitorType));
            }
        }

        private void CreateTasks(string ip, string agent, string version, string service, string port, string type, string health, string firewall, string asecuritystate, string grouptype)
        {
            bool state = true;
            this.Invoke(new Action(
            delegate ()
            {
                ListView.ListViewItemCollection listViewItem = tasksListView.Items;

                foreach (ListViewItem item in listViewItem)
                {
                    if (ip == item.SubItems[1].Text)
                    {
                        state = false;
                        break;
                    }
                }
            }));
            if (state == false) return;
            TimeSpan ts = TimeSpan.Parse("00:05");
            IMonitor monitor = (IMonitor)Activator.CreateInstance(System.Type.GetType("Iruniview.Monitors.PortMonitor"));
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings.Add("service", service);
            settings.Add("host", ip);
            settings.Add("agent", agent);
            settings.Add("version", version);
            settings.Add("firewall", firewall);
            settings.Add("port", port);
            settings.Add("type", type);
            settings.Add("health", health);
            settings.Add("asecurity", asecuritystate);
            settings.Add("grouptype", grouptype);
            INotifier notifier = (INotifier)Activator.CreateInstance(System.Type.GetType("Iruniview.Notifiers.BalloonTipNotifier"));
            monitor.Notifiers.Add(notifier);
            Dictionary<string, string> notifierSettings = new Dictionary<string, string>();

            notifierSettings.Add("icon", "Info");
            notifier.Initialize(notifierSettings);
            Thread.Sleep(10);
            monitor.Initialize(ts, settings);
            //모니터링이 필요한 서비스에 한해 등록

            this.Invoke(new Action(
            delegate()
            {
                if (_logForm == null)
                {
                    int x = this.Location.X;
                    int y = this.Location.Y;
                    int currentHeight = this.Size.Height;
                    y = y + currentHeight;
                    _logForm = new LogView();
                    _logForm.Show();
                    _logForm.SetDesktopLocation(x, y);
                    PassValue += new PassValueHandler(_logForm.PassText);
                }
                ListViewItem lvi = new TaskListViewItem(new Task(monitor));
                tasksListView.SmallImageList.Images.Add(monitor.Icon);
                lvi.ImageIndex = tasksListView.SmallImageList.Images.Count - 1;
                lvi.Group = tasksListView.Groups[(int)monitor.MonitorType];
                tasksListView.Items.Add(lvi);
                PassLog(ip, "Connect to server first time, create task list");
            }));
        }
        
        public void SetUpEventHandlers()
        {
            exitMenuItem.Click += delegate
            {
                Close();
            };

            showMenuItem.Click += delegate
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
            };

            notifyIcon.DoubleClick += delegate
            {
                Show();
                this.WindowState = FormWindowState.Normal;
                notifyIcon.Visible = false;
                this.ShowInTaskbar = true;
                BringToFront();
            };
           
            tasksListView.SelectedIndexChanged += delegate
            {
                SetRunNowMenuItem();
            };            

            CheckStatusMenuItem.Click += delegate
            {
                ExecuteSelectedTask();
            };

            startServiceMenuItem.Click += delegate
            {
                ServiceSelectedTask("StartService");
            };

            stopServiceMenuItem.Click += delegate
            {
                ServiceSelectedTask("StopService");
            };

            PatchMenuItem.Click += delegate
            {
                DepolyServiceSelectedTask();
            };

            CheckServerInfoMenuItem.Click += delegate
            {
                CheckVersionSelectedTask();
            };

            agentUpdateMenuItem.Click += delegate
            {
               AgentPatchSelectedTask();
            };

            clearItemMenuItem.Click += delegate
            {
                ClearSelectedTask();
            };

            IISInstallMenuItem.Click += delegate
            {
                IISInstallSelectedTask();
            };

            RunProcessMenuItem.Click += delegate
            {
                RunProcessSelectedTask();
            };

            FirewalladdRuleMenuItem.Click += delegate
            {
                RunAddfirewallSelectedTask();
            };

            AsecurityUploadMenuItem.Click += delegate
            {
                AsecurityUploadSelectedTask();
            };
            /*
            AsecurityPatchMenuItem.Click += delegate
            {
                AsecurityPatchSelectedTask();
            };
            */
            AsecurityDownloadMenuItem.Click += delegate
            {
                AsecurityDownloadSelectedTask();
            };

            AsecurityInstallMenuItem.Click += delegate
            {
                AsecurityInstallSelectedTask();
            };

            agentConfigViewMenuItem.Click += delegate
            {
                AgentDownloadSelectedTask();
            };
            //Asecurity Start
            AsecurityStartallMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStartall");
            };
            AsecurityStarteventMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStartevent");
            };
            AsecurityStartfileMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStartfile");
            };
            AsecurityStartnetworkMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStartnetwork");
            };
            AsecurityStartperfMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStartperf");
            };
            AsecurityStartprocMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStartproc");
            };
            //Asecurity Stop
            AsecurityStopallMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStopall");
            };
            AsecurityStopeventMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStopevent");
            };
            AsecurityStopfileMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStopfile");
            };
            AsecurityStopnetworkMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStopnetwork");
            };
            AsecurityStopperfMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStopperf");
            };
            AsecurityStopprocMenuItem.Click += delegate
            {
                ServiceSelectedTask("AsecurityStopproc");
            };
            FirewallenableMenuItem.Click += delegate
            {
                ServiceSelectedTask("Firewallenable");
            };
            FirewalldisableMenuItem.Click += delegate
            {
                ServiceSelectedTask("Firewalldisalbe");
            };
            RemoteDesktopMenuItem.Click += delegate
            {
                RemoteDesktopTask();
            };
        }

        private void SetRunNowMenuItem()
        {
            if (tasksListView.SelectedItems.Count != 0)
            {
                taskMenuStrip.Enabled = true;
                AsecurityPatchMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 1;
                AsecurityStopMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 1;
                AsecurityStartMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 1;
                AsecurityUploadMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 1;
                AsecurityDownloadMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 1;

                if (tasksListView.SelectedItems.Count == 1)
                {

                    AsecurityStarteventMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("e");
                    AsecurityStopeventMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("e");
                    AsecurityStartfileMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("f");
                    AsecurityStopfileMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("f");
                    AsecurityStartperfMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("p");
                    AsecurityStopperfMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("p");
                    AsecurityStartnetworkMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("n");
                    AsecurityStopnetworkMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("n");
                    AsecurityStartprocMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("c");
                    AsecurityStopprocMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems[0].SubItems[11].Text.Contains("c");
                }
                else
                {

                    AsecurityStarteventMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStopeventMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStartfileMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStopfileMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStartperfMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStopperfMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStartnetworkMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStopnetworkMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStartprocMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;
                    AsecurityStopprocMenuItem.Enabled = tasksListView.SelectedItems[0].Group.Header == "Asecurity_Windows" & tasksListView.SelectedItems.Count >= 2;

                }


                RunProcessMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                CheckStatusMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                startServiceMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                stopServiceMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                PatchMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                CheckServerInfoMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                agentUpdateMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                IISInstallMenuItem.Enabled = tasksListView.SelectedItems.Count == 1 && tasksListView.SelectedItems[0].Group.Header.Contains("Windows");

                RemoteDesktopMenuItem.Enabled = tasksListView.SelectedItems.Count == 1 && tasksListView.SelectedItems[0].Group.Header.Contains("Windows");
                agentConfigViewMenuItem.Enabled = tasksListView.SelectedItems.Count == 1;
                clearItemMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                AsecurityInstallMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                RunProcessMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                firewallToolStripMenuItem.Enabled = tasksListView.SelectedItems.Count >= 1;
                /*
                foreach (ListViewItem item in tasksListView.SelectedItems)
                {
                    if (item.Group.Name == "Windows")
                    {
                        AsecurityConfigMenuItem.Enabled = true;
                        AsecurityPatchMenuItem.Enabled = true;
                        AsecurityStartMenuItem.Enabled = true;
                        AsecurityStopMenuItem.Enabled = true;                
                    }
                }
                */

            }
            else
            {
                taskMenuStrip.Enabled = false;
            }
        }
        private void ClearSelectedTask()
        {
            if (tasksListView.SelectedItems.Count == 0)
            {
                tasksListView.Clear();
            }
            else
            {
                TaskListViewItem listViewItem = (TaskListViewItem)tasksListView.SelectedItems[0];
                listViewItem.Remove();
            }
        }

        private void RemoteDesktopTask()
        {
            if (tasksListView.SelectedItems.Count != 0)
            {
                TaskListViewItem listViewItem = (TaskListViewItem)tasksListView.SelectedItems[0];
                string ip = listViewItem.SubItems[1].Text;
                Process rdcProcess = new Process();
                rdcProcess.StartInfo.FileName = Environment.ExpandEnvironmentVariables(@"%SystemRoot%\system32\mstsc.exe");
                rdcProcess.StartInfo.Arguments = "/v:" + ip; // ip or name of computer to connect
                rdcProcess.Start();
            }
        }

        private void ExecuteSelectedTask()
        {
            TaskListViewItem listViewItem = (TaskListViewItem)tasksListView.SelectedItems[0];
            if (!String.IsNullOrEmpty(listViewItem.Task.Monitor.Port))
            {
                listViewItem.Task.Run();
                PassLog(listViewItem.Monitor.IPAddress, "Start to check agent port " + listViewItem.Monitor.Port + ".");
            }
            else
            {
                PassLog(listViewItem.Monitor.IPAddress, "This host not define port, so you can't use to port monitoring.");
            }
        }

        public void ShowBalloonTip(string title, string text, ToolTipIcon icon)
        {
            notifyIcon.ShowBalloonTip(2000, title, text, icon);
        }

        public static void PassLog(string server, string value)
        {
            if (_logForm != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(DateTime.Now.ToString());
                sb.Append(" | ");
                sb.Append(server);
                sb.Append(" | ");
                sb.Append(value);
                sb.Append("\r\n");
                PassValue(sb.ToString());                
            }
        }

        private NetworkStream TcpConnecting(TcpClient tcpClient, string ip, int port)
        {
            var result = tcpClient.BeginConnect(ip, port, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
            NetworkStream network = null;
            if (success)
            {
                network = tcpClient.GetStream();
            }
            return network;
        }

        async Task<string> ServiceTask(string ip, string commmand)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];

                    Crypto.Encrypt(commmand, network);

                    //클라이언트 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    if (returnValue.StartsWith("Response"))
                    {
                        Crypto.Encrypt("Ok", network);
                        //클라이언트 처리 결과 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                    }
                }

                return returnValue;
            }
            catch (Exception)
            {
                returnValue += " " +commmand;
                PassLog(ip, returnValue);
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }
        
        async Task<string> AsecurityUploadTask(string ip, string filename)
        {
            string returnValue = "Fail";
            try
            {                
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];

                    Crypto.Encrypt("AsecurityUpload", network);

                    //클라이언트 전송 가능 여부 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);
                    if (returnValue.StartsWith("Start"))
                    {
                        Crypto.Encrypt(bytes.Length.ToString(), network);

                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                        Form1.PassLog(ip, returnValue);

                        tcpClient.Client.SendFile(filename);
                    
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                        Form1.PassLog(ip, returnValue);
                    }
                }                
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error asecurity upload with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error asecurity upload with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async Task<string> AsecurityPatchTask(string ip, string filename)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];

                    Crypto.Encrypt("AsecurityPatch", network);
                    
                    //클라이언트 전송 가능 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);
                    if (returnValue.StartsWith("Response"))
                    {
                        //파일 이름 전송
                        Crypto.Encrypt(Path.GetFileName(filename), network);

                        //파일 크기 전송
                        Crypto.Encrypt(bytes.Length.ToString(), network);

                        //파일 전송 대기 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                        Form1.PassLog(ip, returnValue);

                        //파일 전송
                        tcpClient.Client.SendFile(filename);

                        //전송 결과 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                        Form1.PassLog(ip, returnValue);
                        if (returnValue.StartsWith("Success"))
                        {
                            ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                            returnValue = Crypto.Decrypt(ReadByte);
                            Form1.PassLog(ip, returnValue);
                        }
                    }
                }
                return returnValue;

            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error asecurity patch with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error asecurity patch with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }
        
        async Task<string> DepolyServiceTask(string ip, string filename, string version)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];
                    Crypto.Encrypt("DepolyFile", network);

                    //클라이언트 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);
                    if (returnValue.StartsWith("Start"))
                    {
                        //클라이언트 서비스 버전 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);

                        if (returnValue != version)
                        {
                            //파일 전송 진행 요청
                            Crypto.Encrypt("FileSend", network);

                            //클라이언트 확인  
                            ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                            returnValue = Crypto.Decrypt(ReadByte);
                            Form1.PassLog(ip, returnValue);

                            //Version 정보 전송
                            Crypto.Encrypt(version, network);

                            //파일 이름 전송
                            Crypto.Encrypt(Path.GetFileName(filename), network);

                            //클라이언트 확인  
                            ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                            returnValue = Crypto.Decrypt(ReadByte);
                            Form1.PassLog(ip, returnValue);

                            //파일 크기 전송
                            Crypto.Encrypt(bytes.Length.ToString(), network);

                            //파일 전송 대기 확인
                            ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                            returnValue = Crypto.Decrypt(ReadByte);
                            Form1.PassLog(ip, returnValue);

                            //파일 전송
                            tcpClient.Client.SendFile(filename);

                            //전송 결과 확인
                            ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                            returnValue = Crypto.Decrypt(ReadByte);
                        }
                        else
                        {
                            returnValue = "Seam version";
                        }
                    }
                }
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error depoly file with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error depoly file with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async Task<string> AgentPatchTask(string ip, string filename)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];
                    Crypto.Encrypt("AgentPatch", network);

                    //클라이언트 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    //파일 크기 전송
                    Crypto.Encrypt(bytes.Length.ToString(), network);

                    //파일 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    //파일 전송
                    tcpClient.Client.SendFile(filename);

                    //전송 결과 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                }
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error agent update with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error agent update with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async Task<string> IISInstallTask(string ip, string filename)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];
                    Crypto.Encrypt("IISInstall", network);

                    //클라이언트 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    //파일 이름 전송
                    Crypto.Encrypt(Path.GetFileName(filename), network);

                    //파일 크기 전송
                    Crypto.Encrypt(bytes.Length.ToString(), network);

                    //파일 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    //파일 전송
                    tcpClient.Client.SendFile(filename);

                    //전송 결과 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                }
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error iis install with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error iis install with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async Task<string> RunAddfirewallTask(string ip, string firewallrule)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];
                    Crypto.Encrypt("AddFirewall", network);

                    //클라이언트 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    Crypto.Encrypt(firewallrule, network);

                    //클라이언트 확인 
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                }
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error Run process with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error Run process with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async Task<string> RunProcessTask(string ip, List<string> process)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];
                    Crypto.Encrypt("RunProcess", network);

                    //클라이언트 전송 대기 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    Form1.PassLog(ip, returnValue);

                    foreach (string item in process)
                    {
                        Crypto.Encrypt(item, network);
                    }
                    Crypto.Encrypt("RunProcess_End", network);

                    //클라이언트 확인 
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                }
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error Run process with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error Run process with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async Task<string> AsecurityInstallTask(string ip, string filename)
        {
            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, agentprot);

                if (network != null)
                {
                    byte[] bytes = File.ReadAllBytes(filename);
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];
                    Crypto.Encrypt("AsecurityInstall", network);

                    //클라이언트에서 인스톨 가능 여부 확인
                    ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                    if (returnValue.StartsWith("Start"))
                    {
                        //파일 크기 전송
                        Crypto.Encrypt(bytes.Length.ToString(), network);

                        //파일 전송 대기 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                        Form1.PassLog(ip, returnValue);

                        //파일 전송
                        tcpClient.Client.SendFile(filename);

                        //전송 결과 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                    }
                    else
                    {
                        //실패 관련 메세지 확인
                        ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                        returnValue = Crypto.Decrypt(ReadByte);
                    }
                }
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Error asecurity install with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                PassLog(ip, "Error asecurity install with message: " + ex.ToString());
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private async void ServiceSelectedTask(string command)
        {
            taskMenuStrip.Enabled = false;
            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;
            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {
                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                string ip = item.SubItems[1].Text;
                try
                {
                    PassLog(ip, "Try to " + command + " task.");
                    Task<string> RunStartTask = ServiceTask(ip, command);
                    string StrResult = await RunStartTask;
                    Form1.PassLog(ip, StrResult);
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent("Error start service with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                    Program.ShowBalloonTip(ip, string.Format("Fail to running task "+ command +" of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to " + command + ".";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void CheckVersionSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;
            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {
                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                string ip = item.SubItems[1].Text;
                string StrResult = null;
                try
                {
                    PassLog(ip, "Try to check server information.");
                    Task<string> RunCheckVersionTask = ServiceTask(ip, "CheckVersion");
                    StrResult = await RunCheckVersionTask;
                    string[] temp = StrResult.Split(',');
                    if (temp.Count() == 7)
                    {
                        item.SubItems[0].Text = temp[0]; // Hostname
                        item.SubItems[2].Text = temp[1]; // Service
                        item.SubItems[3].Text = temp[2]; // Version
                        if (temp[3] == "1") item.SubItems[4].Text = "Normal"; else item.SubItems[4].Text = "Abnormal"; // Health
                        item.SubItems[5].Text = temp[4]; // Agent
                        item.SubItems[6].Text = temp[5]; // Firewall
                        item.SubItems[11].Text = temp[6]; // Asecurity
                        item.SubItems[7].Text = "Success to version check";
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                    else
                    {
                        Program.ShowBalloonTip(ip, string.Format("array {0} fail of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        PassLog(ip, temp.ToString());
                    }
                     
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent("Error check version with message: " + ex.Message ,
System.Diagnostics.EventLogEntryType.Warning);
                    Program.ShowBalloonTip(ip, string.Format("Fail to check service version of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to version check";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void DepolyServiceSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string filename;
            string ip;
            string version;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ZIP Files|*.Zip";
            openFileDialog1.Title = "Select a Zip File";

            if (MessageBox.Show("When you click to 'YES',\nStart depoly file and overwrite.\nFinally, Server service is stop automatically.\nAre You Agree?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = openFileDialog1.FileName;

                FileInfo fileinfo1 = new FileInfo(openFileDialog1.FileName);
                version = fileinfo1.LastWriteTime.ToString();
            }
            else
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {
                ip = item.SubItems[1].Text;
                
                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                toolStripProgressBar.Value = i;

                try
                {
                    PassLog(ip, "Try service file depoly.");
                    Task<string> RunDepolyServiceTask = DepolyServiceTask(ip, filename, version);
                    string StrResult = await RunDepolyServiceTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent("Error asecurity install with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                    Program.ShowBalloonTip(ip, string.Format("Fail to deployed file of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to deployed file";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        /*
        private async void AsecurityPatchSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string zipfilename;
            string ip;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ZIP Files|*.Zip";
            openFileDialog1.Title = "Select a Zip File";

            if (MessageBox.Show("When you click to 'YES',\nStart depoly asecurity service file and overwrite.\nFinally, asecurity service is stop automatically.\nAre You Agree?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                zipfilename = openFileDialog1.FileName;
            }
            else
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;


            foreach (ListViewItem item in listViewItem)
            {
                ip = item.SubItems[1].Text;

                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                toolStripProgressBar.Value = i;

                try
                {
                    PassLog(ip, "Try asecurity patch.");
                    Task<string> RunAsecurityPatchTask = AsecurityPatchTask(ip, zipfilename);
                    string StrResult = await RunAsecurityPatchTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to deployed file of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to deployed file";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }
        */
        private async void AsecurityUploadSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string filename;
            string ip;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "INI Files|*.ini";
            openFileDialog1.Title = "Select a ini File";

            if (MessageBox.Show("When you click to 'YES',\nStart depoly asecurity.ini file and overwrite.\nYou Agree Are?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }
            else
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;            

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;


            foreach (ListViewItem item in listViewItem)
            {
                ip = item.SubItems[1].Text;

                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                toolStripProgressBar.Value = i;

                try
                {
                    PassLog(ip, "Try asecurity change config.");
                    Task<string> RunAsecurityUploadTask = AsecurityUploadTask(ip, filename);
                    string StrResult = await RunAsecurityUploadTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to change asecurity config file of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to change asecurity config file";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void AgentDownloadSelectedTask()
        {
            taskMenuStrip.Enabled = false;

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {

                string ip = item.SubItems[1].Text;

                toolStripProgressBar.Value = i;
                item.SubItems[7].Text = TaskStatus.Trying.ToString();

                try
                {
                    PassLog(ip, "Try to iruni.ini download.");
                    Task<string> RunAgentDownloadTask = ServiceTask(ip, "AgentDownload");
                    string StrResult = await RunAgentDownloadTask;
                    if (StrResult.StartsWith("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        Dialog.ShowDialog(ip, StrResult, "IRUNI Configuration", false);
                        item.SubItems[7].Text = "Success to iruni.ini download.";
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to iruni.ini download of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to iruni.ini download.";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void AgentPatchSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string zipfilename;
            string ip;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "Zip Files|*.Zip";
            openFileDialog1.Title = "Select a IRUNI Zip file";

            if (MessageBox.Show("When you click to 'YES',\nStart update to agent and restart agent automatically.\nAre You Agree?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                zipfilename = openFileDialog1.FileName;
            }
            else
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {

                ip = item.SubItems[1].Text;

                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                try
                {
                    PassLog(ip, "Try to agent update.");
                    Task<string> RunAgentUpdateTask = AgentPatchTask(ip, zipfilename);
                    string StrResult = await RunAgentUpdateTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Info);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to agent update of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to agent update";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void IISInstallSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string zipfilename;
            string ip;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ZIP Files|Xml.Zip";
            openFileDialog1.Title = "Select a Xml.Zip File";

            if (MessageBox.Show("This Menu only use to frist install\nyou must have prepare to same file name \n[apppools.xml, sites.xml] in Xml.zip File", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                zipfilename = openFileDialog1.FileName;
            }
            else
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {

                ip = item.SubItems[1].Text;

                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();

                try
                {
                    PassLog(ip, "Try IIS install.");
                    Task<string> RuuIISInstallTask = IISInstallTask(ip, zipfilename);
                    string StrResult = await RuuIISInstallTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to install IIS of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to install IIS";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void RunAddfirewallSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string firewallrule = null;

            using (Addfirewallrule frm = new Addfirewallrule())
            {

                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK & (frm.addfwstat == "allow" || frm.addfwstat == "block") & (String.IsNullOrEmpty(frm.addfwprotocol) || frm.addfwprotocol.ToLower() == "tcp" || frm.addfwprotocol.ToLower() == "udp" || frm.addfwprotocol.ToLower() == "all"))
                {
                    //allow,block 입력
                    firewallrule = frm.addfwstat+" ";
                    //IP Address 입력
                    if (!String.IsNullOrEmpty(frm.addfwaddress))
                    {
                        //string[] temp = frm.KillProcess;
                        firewallrule += frm.addfwaddress.Trim();
                    }
                    else
                    {
                        firewallrule += "any";
                    }
                    firewallrule += " ";
                    //프로토콜 입력
                    if (!String.IsNullOrEmpty(frm.addfwprotocol))
                    {
                        //string[] temp = frm.KillProcess;
                        firewallrule += frm.addfwprotocol.Trim();
                    }
                    else
                    {
                        firewallrule += "all";
                    }

                    firewallrule += " ";
                    //local port 입력
                    if (!String.IsNullOrEmpty(frm.addfwlocalport))
                    {
                        //string[] temp = frm.KillProcess;
                        firewallrule += frm.addfwlocalport.Trim();
                    }
                    else
                    {
                        firewallrule += "any";
                    }
                    firewallrule += " ";
                    //Remote port 입력
                    if (!String.IsNullOrEmpty(frm.addfwremoteport))
                    {
                        //string[] temp = frm.KillProcess;
                        firewallrule += frm.addfwremoteport.Trim();
                    }
                    else
                    {
                        firewallrule += "any";
                    }

                    ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

                    int i = 1;
                    toolStripProgressBar.Value = 0;
                    toolStripProgressBar.Maximum = listViewItem.Count;

                    foreach (ListViewItem item in listViewItem)
                    {

                        string ip = item.SubItems[1].Text;

                        toolStripProgressBar.Value = i;
                        item.SubItems[8].Text = TaskStatus.Trying.ToString();

                        try
                        {
                            PassLog(ip, "Try to add firewall task. " + firewallrule + "");
                            Task<string> RunFirewallTask = RunAddfirewallTask(ip, firewallrule);
                            string StrResult = await RunFirewallTask;
                            if (StrResult.Contains("Fail"))
                            {
                                Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                                item.SubItems[7].Text = StrResult;
                                item.SubItems[8].Text = TaskStatus.Fail.ToString();
                            }
                            else
                            {
                                item.SubItems[7].Text = StrResult;
                                item.SubItems[8].Text = TaskStatus.OK.ToString();
                            }
                        }
                        catch (Exception)
                        {
                            Program.ShowBalloonTip(ip, string.Format("Fail to run script of {0} server.", ip), ToolTipIcon.Error);
                            item.SubItems[7].Text = "Fail to run script.";
                            item.SubItems[8].Text = TaskStatus.Fail.ToString();
                        }
                        i++;
                    }
                    taskMenuStrip.Enabled = true;
                }
                else
                {
                    MessageBox.Show("Error input value. not start task.");
                    taskMenuStrip.Enabled = true;
                    return;
                }
            }

        }

        private async void RunProcessSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            List <string> process = new List<string>();

            using (RunProcessForm frm = new RunProcessForm())
            {
                if (frm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (!String.IsNullOrEmpty(frm.RunProcess.ToString()))
                    {
                        //string[] temp = frm.KillProcess;
                        foreach (string item in frm.RunProcess) process.Add(item);
                    }
                }
                else
                {
                    taskMenuStrip.Enabled = true;
                    return;
                }
            }
            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {

                string ip = item.SubItems[1].Text;
                
                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();

                try
                {
                    PassLog(ip, "Try to run command.");
                    Task<string> RunRunProcessTask = RunProcessTask(ip, process);
                    string StrResult = await RunRunProcessTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = "OK";
                        Dialog.ShowDialog(ip, StrResult, "Script run result", true);
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to run script of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to run script.";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

       /* 
        private async void AsecurityStartallSelectedTask()
        {
            taskMenuStrip.Enabled = false;

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {

                string ip = item.SubItems[1].Text;

                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();

                try
                {
                    PassLog(ip, "Try to asecurity start.");
                    Task<string> RunAsecurityStartTask = ServiceTask(ip, "AsecurityStartAll");
                    string StrResult = await RunAsecurityStartTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to start asecurity of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to start asecurity";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }
        */
        private async void AsecurityDownloadSelectedTask()
        {
            taskMenuStrip.Enabled = false;

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {

                string ip = item.SubItems[1].Text;

                toolStripProgressBar.Value = i;
                item.SubItems[8].Text = TaskStatus.Trying.ToString();

                try
                {
                    PassLog(ip, "Try to whoru.ini download.");
                    Task<string> RunAsecurityDownloadTask = ServiceTask(ip, "AsecurityDownload");
                    string StrResult = await RunAsecurityDownloadTask;
                    if (StrResult.StartsWith("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        PassLog(ip, "Success to whoru.ini download.");
                        Dialog.ShowDialog(ip, StrResult, "ASECURITY Configuration", false);                        
                        item.SubItems[7].Text = "Success to whoru.ini download.";
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to asecurity.ini download of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to asecurity.ini download.";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private async void AsecurityInstallSelectedTask()
        {
            taskMenuStrip.Enabled = false;
            string filename;
            string ip;

            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            openFileDialog1.Filter = "ZIP, EXE Files (*.zip, *.exe)|*.zip;*.exe|ZIP Files (*.zip)|*.zip|EXE Files (*.exe)|*.exe";
            openFileDialog1.Title = "Select a Zip or Exe File";
            openFileDialog1.Multiselect = false;

            if (MessageBox.Show("When you click to 'YES',\nStart depoly file and WHORUCollector install.\nFinally, WHORUCollector service is install automatically.\nAre You Agree?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.No)
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                filename = openFileDialog1.FileName;
            }
            else
            {
                taskMenuStrip.Enabled = true;
                return;
            }

            ListView.SelectedListViewItemCollection listViewItem = tasksListView.SelectedItems;

            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = listViewItem.Count;

            foreach (ListViewItem item in listViewItem)
            {
                ip = item.SubItems[1].Text;

                item.SubItems[8].Text = TaskStatus.Trying.ToString();
                toolStripProgressBar.Value = i;

                try
                {
                    PassLog(ip, "Try to asecurity install.");
                    Task<string> RunAsecurityPatchTask = AsecurityPatchTask(ip, filename);
                    string StrResult = await RunAsecurityPatchTask;
                    if (StrResult.Contains("Fail"))
                    {
                        Program.ShowBalloonTip(ip, string.Format("{0} of {1} server.", StrResult, ip), ToolTipIcon.Error);
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.Fail.ToString();
                    }
                    else
                    {
                        item.SubItems[7].Text = StrResult;
                        item.SubItems[8].Text = TaskStatus.OK.ToString();
                    }
                }
                catch (Exception)
                {
                    Program.ShowBalloonTip(ip, string.Format("Fail to deployed file of {0} server.", ip), ToolTipIcon.Error);
                    item.SubItems[7].Text = "Fail to deployed file";
                    item.SubItems[8].Text = TaskStatus.Fail.ToString();
                }
                i++;
            }
            taskMenuStrip.Enabled = true;
        }

        private void ImportStatusForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon.Visible = true;
                Program.ShowBalloonTip("IRUNI Server Manager", "Move to Tray", ToolTipIcon.Info);
                this.ShowInTaskbar = false;
                Hide();
            }

        }

        private void Click_Logviewbutton(object sender, EventArgs e)
        {
            if (_logForm == null)
            {
                int x = this.Location.X;
                int y = this.Location.Y;
                int currentHeight = this.Size.Height;
                y = y + currentHeight;
                _logForm = new LogView();
                _logForm.Show();
                _logForm.SetDesktopLocation(x, y);
                PassValue += new PassValueHandler(_logForm.PassText);
            }
            else
            {
                try
                {
                    if (_logForm.Visible == false)
                    {
                        int x = this.Location.X;
                        int y = this.Location.Y;
                        int currentHeight = this.Size.Height;
                        y = y + currentHeight;
                        _logForm.SetDesktopLocation(x, y);
                        _logForm.Show();
                    }
                    else _logForm.Hide();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void Click_Server_button(object sender, EventArgs e)
        {
            if (this.socket != null)
            {
                this.socket.Close();
                this.socket = null;
                textBox_ServerState.Text = "Stop";
                textBox_Port.ReadOnly = false;
                button_Serverlist.Image = Iruniview.Properties.Resources.serverstart;
                tasksListView.Items.Clear();
            }
            else
            {
                serverport = Convert.ToInt32(textBox_Port.Text);
                try
                {
                    this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent("Could not create socket because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);

                    return;
                }

                try
                {
                    this.socket.Bind(new IPEndPoint(IPAddress.Any, serverport));
                }
                catch (Exception ex)
                {
                    this.socket.Close();
                    this.socket = null;

                    EventLogger.LogEvent("Could not bind socket because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);

                    return;
                }

                if (this.socket == null) { return; }

                try
                {
                    // Start the listen operation on the socket
                    // RegisterReceiveOperation();
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent("Could not register socket on data received event because: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
                    return;
                }
                textBox_ServerState.Text = "Running";
                textBox_Port.ReadOnly = true;
                button_Serverlist.Image = Iruniview.Properties.Resources.serverstop;
            }
        }

        private void Form1Closing(object sender, FormClosingEventArgs e)
        {
            if (this.socket != null)
            {
                this.socket.Close();
                this.socket = null;
            }
        }

        private void Click_Ruleviewbutton(object sender, EventArgs e)
        {
            /*
            if (_ruleForm == null)
            {
                int x = this.Location.X;
                int y = this.Location.Y;
                int currentHeight = this.Size.Height;
                y = y + currentHeight;
                _ruleForm = new RuleView();
                _ruleForm.Show();
                _ruleForm.SetDesktopLocation(x, y);

            }
            else
            {
                try
                {
                    if (_ruleForm.Visible == false)
                    {
                        int x = this.Location.X;
                        int y = this.Location.Y;
                        int currentHeight = this.Size.Height;
                        y = y + currentHeight;
                        _ruleForm.SetDesktopLocation(x, y);
                        _ruleForm.Show();
                    }
                    else _ruleForm.Hide();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            */
        }

        private void Click_Aboutbutton(object sender, EventArgs e)
        {
            if (_aboutForm == null)
            {
                int x = this.Location.X;
                int y = this.Location.Y;

                _aboutForm = new About();
                _aboutForm.Show();
                _aboutForm.SetDesktopLocation(x, y);
            }
            else
            {
                try
                {
                    if (_aboutForm.Visible == false)
                    {
                        int x = this.Location.X;
                        int y = this.Location.Y;
                        _aboutForm.SetDesktopLocation(x, y);
                        _aboutForm.Show();
                    }
                    else _aboutForm.Hide();
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            if (tasksListView.Items.Count == 0) return;

            Searchfrm = new Search();
            
            Random random = new Random();
            Jobid = random.Next(0, 1000000).ToString();
            Searchfrm.listItem = new List<string>();
            if (tasksListView.SelectedItems.Count > 0)
            {
                foreach (ListViewItem item in tasksListView.SelectedItems)
                {
                    Searchfrm.listItem.Add(item.SubItems[1].Text);
                }
            }
            else
            {
                foreach (ListViewItem item in tasksListView.Items)
                {
                    Searchfrm.listItem.Add(item.SubItems[1].Text);
                }
            }
                
            Searchfrm.Host_Count = Searchfrm.listItem.Count;
            Searchfrm.HostCount.Text = Searchfrm.Host_Count.ToString();
            Searchfrm.Show();
            
        }
    }
}
