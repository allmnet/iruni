using Ionic.Zip;
using Microsoft.Win32;
using NetFwTypeLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Iruni
{
    [RunInstaller(true)]
    public class IruniServiceInstaller : Installer
    {
        /// <summary>
        /// Public Constructor for WindowsServiceInstaller.
        /// - Put all of your Initialization code here.
        /// </summary>
        public IruniServiceInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller =
                               new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            //# Service Account Information
            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Username = null;
            serviceProcessInstaller.Password = null;

            //# Service Information
            serviceInstaller.DisplayName = "Iruni Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;

            //# This must be identical to the WindowsService.ServiceBase name
            //# set in the constructor of WindowsService.cs
            serviceInstaller.ServiceName = "Iruni";

            this.Installers.Add(serviceProcessInstaller);
            this.Installers.Add(serviceInstaller);
        }
    }
    public class TaskInfo
    {
        // State information for the task.  These members
        // can be implemented as read-only properties, read/write
        // properties with validation, and so on, as required.

        public string Jobid;
        public string Commandtype;
        public string Searchstring;
        public string Live;
        public string Serverip;
        // Public constructor provides an easy way to supply all
        // the information needed for the task.
        public TaskInfo(string id, string type, string search, string run, string ip)
        {
            Jobid = id;
            Commandtype = type;
            Searchstring = search;
            Live = run;
            Serverip = ip;
        }
    }

    public partial class Iruniagent : ServiceBase
    {
        public static bool kor = false;
        private static EventLog eventLog1;
        private static EventLogWatcher sysmonwatcher;
        Thread securitylogworkThread = null;

        Thread serverThread = null;

        Thread searchworkThread = null;

        private TcpListener tcpListener;
        private bool state;
        private bool serviceinstall = false;
        private string asecurityinstall = null;
        private static bool BackgroundThreadStop = false;

        // 작업 실행 잡 ID
        // private static string jobid = null;
        // 작업 실행 
        // private static string commandtype = null;
        // private static string searchstring = null;

        private struct Searchdata
        {
            public string jobid;
            public string commandtype;
            public string searchstring;
            public string history;
            public string serverip;
        }

        private static List<Searchdata> Searchlist = new List<Searchdata>();
        private static object Searchlist_QueueLock = new object();
        private static object aTimer_QueueLock = new object();

        ManualResetEvent tcpClientConnected = new ManualResetEvent(false);

        public Encoding Encoding { get; set; }

        private static System.Timers.Timer aTimer1;
        private INetFwMgr icfMgr = null;
        private Type TicfMgr = Type.GetTypeFromProgID("HNetCfg.FwMgr");

        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 구성 요소 디자이너에서 생성한 코드

        /// <summary> 
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마십시오.
        /// </summary>
        private void InitializeComponent()
        {
            // 
            // EastSea
            // 
            this.ServiceName = "Iruni";

            try
            {
                EventLogQuery query1 = new EventLogQuery("Microsoft-Windows-Sysmon/Operational", PathType.LogName);
                EventLogWatcher sysmonwatcher = new EventLogWatcher(query1);
                sysmonwatcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(sysmonwatcher_EventRecordWritten);
                sysmonwatcher.Enabled = true;
                /*
                eventLog1 = new System.Diagnostics.EventLog();
                eventLog1.Log = "Security";
                int LastLogToShow = eventLog1.Entries.Count;

                ((System.ComponentModel.ISupportInitialize)(eventLog1)).BeginInit();
                eventLog1.EnableRaisingEvents = true;
                eventLog1.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(Storeevent.eventLog1_EntryWritten);
                ((System.ComponentModel.ISupportInitialize)(eventLog1)).EndInit();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't sysmon start with message: {0}", ex.Message);
            }


            try
            {
                string queryString = @"*[System[(EventID='4624')] or System[(EventID='4625')] or System[(EventID='4647')] or System[(EventID='4740')] or System[(EventID='4648')] or System[(EventID='4800')] or System[(EventID='4771')]]";
                //String queryString = @"*[EventData[Data[@Name='LogonType']='7'] and System[(EventID='4624')]]";
                EventLogQuery query2 = new EventLogQuery("Security", PathType.LogName, queryString);
                EventLogWatcher securitywatcher = new EventLogWatcher(query2);
                securitywatcher.EventRecordWritten += new EventHandler<EventRecordWrittenEventArgs>(securitywatcher_EventRecordWritten);
                securitywatcher.Enabled = true;
                /*
                eventLog1 = new System.Diagnostics.EventLog();
                eventLog1.Log = "Security";
                int LastLogToShow = eventLog1.Entries.Count;

                ((System.ComponentModel.ISupportInitialize)(eventLog1)).BeginInit();
                eventLog1.EnableRaisingEvents = true;
                eventLog1.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(Storeevent.eventLog1_EntryWritten);
                ((System.ComponentModel.ISupportInitialize)(eventLog1)).EndInit();
                */
            }
            catch (Exception ex)
            {
                Console.WriteLine("Can't security start with message: {0}", ex.Message);
            }
        }

        static void securitywatcher_EventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            //Console.WriteLine(e.EventRecord.ToXml());
            string msg = null;
            uint login = 0;
            int level = 38;
            string login_type = "Unknown";
            string login_address = null;
            string login_name = null;
            string program = null;
            switch (e.EventRecord.Id)
            {
                case 4800:
                    login_type = "Screen_Lock";
                    msg = string.Format("Screen_Lock " + (string)e.EventRecord.Properties[2].Value + "\\" + (string)e.EventRecord.Properties[1].Value +
                        " Sessionid:" + (uint)e.EventRecord.Properties[4].Value) + " [" + e.EventRecord.RecordId + ", 4800]";
                    break;
                case 4624:
                    login = (uint)e.EventRecord.Properties[8].Value;
                    if ((Class.b_Networklogon == false) & login == 3) break;
                    switch (login)
                    {
                        case 2:
                            login_type = "Interactive_logon";
                            level = 37;
                            break;
                        case 3:
                            login_type = "Network_logon";
                            break;
                        case 4:
                            login_type = "Batch_logon";
                            break;
                        case 5:
                            login_type = "Service_logon";
                            break;
                        case 7:
                            login_type = "Unlock";
                            level = 37;
                            break;
                        case 8:
                            login_type = "Network_clear_text_logon";
                            break;
                        case 9:
                            login_type = "New_credentials-based_logon";
                            break;
                        case 10:
                            login_type = "Remote_Interactive_logon";
                            level = 36;
                            break;
                        case 11:
                            login_type = "Cached_Interactive_logon";
                            level = 37;
                            break;
                        default:
                            break;
                    }
                    login_address = (string)e.EventRecord.Properties[18].Value + ":" + (string)e.EventRecord.Properties[19].Value;
                    login_name = (string)e.EventRecord.Properties[5].Value;
                    program = (string)"(" + (string)e.EventRecord.Properties[11].Value + ") " + (string)e.EventRecord.Properties[13].Value;

                    msg = string.Format("Login_Success " + (string)e.EventRecord.Properties[6].Value + "\\" + login_name +
                       " " + login_type + " " + program + " " + login_address + " [" + e.EventRecord.RecordId + ", 4624]");
                    break;
                case 4625:
                    login = (uint)e.EventRecord.Properties[10].Value;
                    if ((Class.b_Networklogon == false) & login == 3) break;
                    switch (login)
                    {
                        case 2:
                            login_type = "Interactive_logon";
                            level = 37;
                            break;
                        case 3:
                            login_type = "Network_logon";
                            break;
                        case 4:
                            login_type = "Batch_logon";
                            break;
                        case 5:
                            login_type = "Service_logon";
                            break;
                        case 7:
                            login_type = "Unlock";
                            level = 37;
                            break;
                        case 8:
                            login_type = "Network_clear_text_logon";
                            break;
                        case 9:
                            login_type = "New_credentials-based_logon";
                            break;
                        case 10:
                            login_type = "Remote_Interactive_logon";
                            level = 36;
                            break;
                        case 11:
                            login_type = "Cached_Interactive_logon";
                            level = 37;
                            break;
                        default:
                            break;
                    }
                    login_address = (string)e.EventRecord.Properties[19].Value + ":" + (string)e.EventRecord.Properties[20].Value;
                    login_name = (string)e.EventRecord.Properties[5].Value;
                    program = "(" + (string)e.EventRecord.Properties[11].Value + ") " + (string)e.EventRecord.Properties[13].Value;
                    msg = string.Format("Login_Fail " + (string)e.EventRecord.Properties[6].Value + "\\" + login_name +
                       " " + login_type + " " + program + " " + login_address + " [" + e.EventRecord.RecordId + ", 4625]");
                    break;
                case 4647:
                    login_type = "Logout";
                    login_name = (string)e.EventRecord.Properties[1].Value;
                    msg = string.Format("Logout " + (string)e.EventRecord.Properties[2].Value + "\\" + login_name + " [" + e.EventRecord.RecordId + ", 4647]");
                    break;
                case 4740:
                    login_type = "Lock";
                    login_name = (string)e.EventRecord.Properties[0].Value;
                    program = "(" + (string)e.EventRecord.Properties[5].Value + ") " + (string)e.EventRecord.Properties[4].Value;
                    msg = string.Format("Lock " + (string)e.EventRecord.Properties[6].Value + "\\" + login_name +
                      " " + program + " " + (string)e.EventRecord.Properties[11].Value + " " + login_address + " [" + e.EventRecord.RecordId + ", 4740]");
                    break;
                case 4648:
                    login_type = "Remote_Login";
                    login_address = (string)e.EventRecord.Properties[12].Value + ":" + (string)e.EventRecord.Properties[13].Value;
                    login_name = (string)e.EventRecord.Properties[5].Value;
                    program = "(" + (string)e.EventRecord.Properties[8].Value + ") " + (string)e.EventRecord.Properties[9].Value;
                    msg = string.Format("Remote_Login_Success " + (string)e.EventRecord.Properties[6].Value + "\\" + login_name +
                      " " + program + " " + (string)e.EventRecord.Properties[11].Value + " " + login_address + " [" + e.EventRecord.RecordId + ", 4648]");
                    break;
                case 4771:
                    login_type = "Lockedout";
                    login_address = (string)e.EventRecord.Properties[6].Value + ":" + (string)e.EventRecord.Properties[7].Value;
                    login_name = (string)e.EventRecord.Properties[0].Value;
                    program = "(" + (string)e.EventRecord.Properties[2].Value + ")";
                    msg = string.Format("Lockedout " + (string)e.EventRecord.Properties[0].Value +
                    " " + program + " " + login_address + " [" + e.EventRecord.RecordId + ", 4771]");
                    break;
                default:
                    break;
            }
        }

        static void sysmonwatcher_EventRecordWritten(object sender, EventRecordWrittenEventArgs e)
        {
            //Console.WriteLine(e.EventRecord.ToXml());
            string msg = null;
            switch (e.EventRecord.Id)
            {
                case 3:
                    /*
                    String[] xPathRefs = new String[8];
                    xPathRefs[0] = (string)e.EventRecord.Properties[1].Value; // UTC - 1
                    xPathRefs[1] = (string)e.EventRecord.Properties[3].Value.ToString(); // PID - 2
                    xPathRefs[2] = (string)e.EventRecord.Properties[4].Value; // Image - 3
                    xPathRefs[3] = (string)e.EventRecord.Properties[6].Value; // TCP - 4
                    xPathRefs[4] = (string)e.EventRecord.Properties[9].Value; // Src ip - 5
                    xPathRefs[5] = (string)e.EventRecord.Properties[11].Value.ToString(); // Src port -5
                    xPathRefs[6] = (string)e.EventRecord.Properties[14].Value; // Dst ip - 6
                    xPathRefs[7] = (string)e.EventRecord.Properties[16].Value; // Dst port - 6
                    */
                    msg = string.Format((string)e.EventRecord.Properties[1].Value + " " + (uint)e.EventRecord.Properties[3].Value +
                        " " + (string)e.EventRecord.Properties[4].Value + " " + (string)e.EventRecord.Properties[6].Value + " " + (string)e.EventRecord.Properties[9].Value +
                        " " + (ushort)e.EventRecord.Properties[11].Value + " -> " + (string)e.EventRecord.Properties[14].Value + " " + (ushort)e.EventRecord.Properties[16].Value);
                    Console.WriteLine(msg);
                    break;
                case 1:
                    /*
                    String[] xPathRefs = new String[8];
                    xPathRefs[0] = (string)e.EventRecord.Properties[1].Value; // UTC
                    xPathRefs[1] = (string)e.EventRecord.Properties[3].Value.ToString(); // PID
                    xPathRefs[2] = (string)e.EventRecord.Properties[4].Value; // Image
                    xPathRefs[3] = (string)e.EventRecord.Properties[6].Value; // TCP
                    xPathRefs[4] = (string)e.EventRecord.Properties[9].Value; // Src ip
                    xPathRefs[5] = (string)e.EventRecord.Properties[11].Value.ToString(); // Src port
                    xPathRefs[6] = (string)e.EventRecord.Properties[14].Value; // Dst ip
                    xPathRefs[7] = (string)e.EventRecord.Properties[16].Value; // Dst port
                    */
                    msg = string.Format((string)e.EventRecord.Properties[1].Value + " " + (uint)e.EventRecord.Properties[3].Value +
                        " " + (string)e.EventRecord.Properties[10].Value + " " + (string)e.EventRecord.Properties[12].Value + " " + (string)e.EventRecord.Properties[15].Value +
                        " " + (string)e.EventRecord.Properties[16].Value + " " + (string)e.EventRecord.Properties[18].Value + " "+ (uint)e.EventRecord.Properties[19].Value + " " + (string)e.EventRecord.Properties[20].Value);
                    Console.WriteLine(msg);
                    break;
                default:
                    break;
            }
        }
        #endregion
        public Iruniagent()
        {
            if (Class.FileExeList.Count > 0) serviceinstall = ServiceCheck(Class.FileExeList); else serviceinstall = false;
            if (Class.strAsecurityList.Count > 0) asecurityinstall = AsecurityServiceCheck(Class.strAsecurityList); else asecurityinstall = "00000";
            this.Encoding = Encoding.Default;
            InitializeComponent();
        }

        public static void SearchSubKeys(RegistryKey root, string searchKey)
        {
            foreach (string keyname in root.GetSubKeyNames())
            {
                try
                {
                    using (RegistryKey key = root.OpenSubKey(keyname))
                    {
                        if (keyname == searchKey)
                            Console.WriteLine("Registry key found : {0} contains {1} values",
                                key.Name, key.ValueCount);

                        SearchSubKeys(key, searchKey);
                    }
                }
                catch (System.Security.SecurityException)
                {

                }
            }
        }

        public NetworkStream TcpConnecting(TcpClient tcpClient, string ip, int port)
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

        void Search(object stateInfo)
        {
            TaskInfo ti = (TaskInfo)stateInfo;
            StringBuilder result = new StringBuilder();
            if (ti.Live == "yes")
            {
                result = RunnigFound(ti.Commandtype, ti.Searchstring);
            }
            else
            {

            }
            //  서버 연결 코드 삽입 
            TcpClient tcpClient = new TcpClient();
            lock (aTimer_QueueLock)
            {
                aTimer1.Stop();
                try
                {
                    NetworkStream network = TcpConnecting(tcpClient, ti.Serverip, Class.intPort);

                    if (network != null)
                    {
                        byte[] ReadByte;
                        ReadByte = new byte[tcpClient.ReceiveBufferSize];
                        Crypto.Encrypt("Jobid:" + ti.Jobid, network);
                        Crypto.Encrypt(result.ToString(), network);
                    }

                }
                catch (Exception)
                {

                }
                finally
                {
                    tcpClient.Close();
                }
                aTimer1.Start();
            }
        }

        // 실행 검색        
        public StringBuilder RunnigFound(string type, string found)
        {
            StringBuilder foundlog = new StringBuilder();
            switch (type)
            {
                case "securitylog":
                    foreach (Storeevent.SecurityLogdata item in Storeevent.Securitylog)
                    {
                        if (item.login_address.Contains(found) || item.login_name.Contains(found) || item.login_type.Contains(found) || item.msg.Contains(found) || item.program.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.login_address + " " + item.login_address + " " + item.login_type + " " + item.msg + " " + item.program);
                        }
                    }
                    break;
                case "filelog":
                    foreach (Storesysmon.Capturedata item in Storesysmon.Filelog)
                    {
                        if (item.message.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.message);
                        }
                    }
                    break;
                case "processlog":
                    foreach (Storesysmon.Capturedata item in Storesysmon.Processlog)
                    {
                        if (item.message.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.message);
                        }
                    }
                    break;
                case "handlelog":
                    foreach (Storesysmon.Capturedata item in Storesysmon.Handlelog)
                    {
                        if (item.message.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.message);
                        }
                    }
                    break;
                case "all":
                    foreach (Storesysmon.Capturedata item in Storesysmon.Handlelog)
                    {
                        if (item.message.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.message);
                        }
                    }
                    foreach (Storesysmon.Capturedata item in Storesysmon.Processlog)
                    {
                        if (item.message.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.message);
                        }
                    }
                    foreach (Storesysmon.Capturedata item in Storesysmon.Filelog)
                    {
                        if (item.message.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.message);
                        }
                    }
                    foreach (Storeevent.SecurityLogdata item in Storeevent.Securitylog)
                    {

                        if (item.login_address.Contains(found) || item.login_name.Contains(found) || item.login_type.Contains(found) || item.msg.Contains(found) || item.program.Contains(found))
                        {
                            foundlog.Append(item.time + " " + item.login_address + " " + item.login_address + " " + item.login_type + " " + item.msg + " " + item.program);
                        }
                    }
                    break;
                default:
                    break;
            }
            if (foundlog.Length == 0) foundlog.Append("N/A");
            return foundlog;
        }
        private void Search_BackgroundThread()
        {
            while (!BackgroundThreadStop)
            {
                bool shouldSleep = true;

                lock (Searchlist_QueueLock)
                {
                    if (Searchlist.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if (shouldSleep)
                {
                    System.Threading.Thread.Sleep(250);
                }
                else // should process the queue
                {
                    List<Searchdata> ourSearchQueue;
                    lock (Searchlist_QueueLock)
                    {
                        // swap queues, giving the capture callback a new one
                        ourSearchQueue = Searchlist;
                        Searchlist = new List<Searchdata>();
                    }
                    foreach (Searchdata item in ourSearchQueue)
                    {
                        TaskInfo ti = new TaskInfo(item.jobid, item.commandtype, item.searchstring, item.history, item.serverip);
                        ThreadPool.QueueUserWorkItem(new WaitCallback(Search), ti);
                    }
                }
            }
        }

        private void Securitylog_BackgroundThread()
        {
            while (!BackgroundThreadStop)
            {
                bool shouldSleep = true;

                lock (Storeevent.Securitylog_QueueLock)
                {
                    if (Storeevent.Securitylog.Count != 0)
                    {
                        shouldSleep = false;
                    }
                }

                if (shouldSleep)
                {
                    System.Threading.Thread.Sleep(250);
                }
                else // should process the queue
                {
                    DateTime now = DateTime.Now.AddHours(-Class.storelog);
                    for (int count = 0; Storeevent.Securitylog[count].time < now; count++)
                    {
                        lock (Storeevent.Securitylog_QueueLock) Storeevent.Securitylog.RemoveAt(count);
                    }
                }
            }
        }

        public void loadConfig()
        {
            Config.Load();
            if (Class.FileExeList.Count > 0) serviceinstall = ServiceCheck(Class.FileExeList); else serviceinstall = false;
            if (Class.strAsecurityList.Count > 0) asecurityinstall = AsecurityServiceCheck(Class.strAsecurityList); else asecurityinstall = "";
        }

        private static void Log(string msg)
        {
            if (Environment.UserInteractive) Console.WriteLine(DateTime.Now + ": " + msg); else Debug.WriteLine(DateTime.Now + ": " + msg);
        }

        /*
        private static INetFwPolicy2 getCurrPolicy()
        {
            INetFwPolicy2 fwPolicy2;
            Type tNetFwPolicy2 = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            if (tNetFwPolicy2 != null)
                fwPolicy2 = (INetFwPolicy2)Activator.CreateInstance(tNetFwPolicy2);
            else
                return null;
            return fwPolicy2;
        }

        public static bool GetFirewallStatus()
        {
            bool result = false;
            try
            {
                INetFwPolicy2 fwPolicy2 = getCurrPolicy();
                NET_FW_PROFILE_TYPE2_ fwCurrentProfileTypes;
                //read Current Profile Types (only to increase Performace)
                //avoids access on CurrentProfileTypes from each Property
                fwCurrentProfileTypes = (NET_FW_PROFILE_TYPE2_)fwPolicy2.CurrentProfileTypes;
                result = (fwPolicy2.get_FirewallEnabled(NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result;
        }

        public static void SetFirewallStatus(bool newStatus)
        {
            try
            {
                NET_FW_PROFILE_TYPE2_ fwCurrentProfileTypes;
                INetFwPolicy2 currPolicy = getCurrPolicy();
                //read Current Profile Types (only to increase Performace)
                //avoids access on CurrentProfileTypes from each Property
                fwCurrentProfileTypes = (NET_FW_PROFILE_TYPE2_)currPolicy.CurrentProfileTypes;
                currPolicy.set_FirewallEnabled(fwCurrentProfileTypes, newStatus);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        */
        public static string FirewallAction(string action, string address, string protocol, string localport, string remoteport)
        {
            try
            {
                int intport = Convert.ToInt32(localport);
                if (isPortFound(intport) == false)
                {

                    INetFwRule firewallRule = (INetFwRule)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FWRule"));

                    firewallRule.Name = "IRUNI Firewall Action: " + action + " Port:" + localport + " IP Addresses: " + address + "";
                    firewallRule.Description = " " + action + " Incoming Connections Port:" + localport + " from IP Address: " + address + " .";

                    if (action == "block") firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_BLOCK;
                    else firewallRule.Action = NET_FW_ACTION_.NET_FW_ACTION_ALLOW;

                    firewallRule.Enabled = true;
                    firewallRule.InterfaceTypes = "All";
                    if (address != "any") firewallRule.RemoteAddresses = address;
                    else
                        switch (protocol)
                        {
                            case "tcp":
                                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                                break;
                            case "udp":
                                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_UDP;
                                break;
                            default:
                                firewallRule.Protocol = (int)NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                                break;
                        }
                    if (localport != "any") firewallRule.LocalPorts = localport;
                    if (remoteport != "any") firewallRule.RemotePorts = remoteport;
                    INetFwPolicy2 firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2"));
                    firewallPolicy.Rules.Add(firewallRule);
                    return "Success add firewall";
                }
                else
                {
                    return "Success already open firewall";
                }
            }
            catch (Exception ex)
            {
                return "Fail add firewall" + ex.Message;
            }

        }


        public void DoJoinServer(object sender, EventArgs e)
        {
            loadConfig();

            try
            {
                bool Firewallenabled = icfMgr.LocalPolicy.CurrentProfile.FirewallEnabled;
                StringBuilder sb = new StringBuilder(string.Empty);

                string message = Class.Hostname;
                message += "," + Class.strServiceName;
                message += "," + Class.strServiceVersion;
                message += "," + Class.servicestate;
                message += "," + Resource1.Version;
                message += "," + Firewallenabled;
                message += "," + Class.strServicePort;
                message += "," + asecurityinstall;
                if (!string.IsNullOrEmpty(asecurityinstall))
                {
                    message += ",Asecurity_Windows";

                }
                else
                {
                    message += ",Other_Windows";
                }
                foreach (string ip in Class.strServerIP)
                {
                    TcpClient tcpClient = new TcpClient();
                    try
                    {
                        NetworkStream network = TcpConnecting(tcpClient, ip, Class.intPort);
                        Crypto.Encrypt(message, network);
                    }
                    catch (Exception ex)
                    {
                        EventLogger.LogEvent("Iruni DoJoinServer error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);

                    }
                    finally
                    {
                        tcpClient.Close();
                    }

                }

                //int servicestate = 0;
                //if (Class.FileExeList.Count > 0) serviceinstall = ServiceCheck(Class.FileExeList); else serviceinstall = false;
                /*
                if (serviceinstall == true)
                {
                    foreach (FileExe item in Class.FileExeList)
                    {
                        if (item.Service == false)
                        {
                            try
                            {
                                //Log("Find process " + item.Exe + "");
                                if (FindProc(item.FullPath))
                                {
                                    servicestate = 1;
                                }
                                else
                                {
                                    servicestate = 2;
                                    break;
                                }

                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                                Log("Can't find process " + item.Exe + "");
                            }
                        }
                        else
                        {
                            try
                            {
                                // Log("Find service " + item.Exe + "");
                                if (IsRunning(item.Exe))
                                {
                                    servicestate = 1;
                                }
                                else
                                {
                                    servicestate = 2;
                                    break;
                                }
                            }
                            catch (Exception ex)
                            {
                                Log(ex.Message);
                                Log("Can't Stop service " + item.Exe + "");
                            }
                        }
                    }
                }
                */
            }
            catch (SocketException ex)
            {
                EventLogger.LogEvent("Iruni join error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
            }
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
                this.tcpListener = new TcpListener(IPAddress.Any, Class.intIRUNIagentPort);
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

                if (Environment.UserInteractive)
                {
                    Console.WriteLine(DateTime.Now + ": IRUNI requset to " + _data);
                }

                switch (_data)
                {
                    case "StartService":
                        {
                            Crypto.Encrypt("Response to service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);

                            string result = null;
                            if (serviceinstall == true)
                            {
                                bool Portisopen = true;
                                string temp = Config.GetIniValue("Service", "Check");
                                if (!string.IsNullOrEmpty(temp))
                                {
                                    string[] result2 = temp.Split(',');

                                    foreach (string item in result2)
                                    {
                                        string[] check = item.Split(':');
                                        int portno = Convert.ToInt32(check[1]);
                                        IPAddress ipa = (IPAddress)Dns.GetHostAddresses(check[0])[0];
                                        System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                                        try
                                        {
                                            sock.Connect(ipa, portno);
                                        }
                                        catch (System.Net.Sockets.SocketException)
                                        {
                                            result = string.Format("Port " + temp[1] + " of " + temp[0] + " not open.", network);
                                            Portisopen = false;
                                            break;
                                        }
                                    }
                                }
                                if (Portisopen)
                                {
                                    foreach (FileExe item in Class.FileExeList)
                                    {
                                        if (item.Service == false)
                                        {
                                            try
                                            {
                                                if (!FindProc(item.FullPath))
                                                {
                                                    Log("Start process " + item.Exe + "");
                                                    Process ps = new Process();
                                                    ps.StartInfo.FileName = item.FullPath;
                                                    if (!string.IsNullOrEmpty(item.Argment)) ps.StartInfo.Arguments = item.Argment;
                                                    ps.Start();
                                                    if (!FindProc(item.FullPath))
                                                    {
                                                        result += "P[" + item.Exe + "] start error";
                                                    }
                                                    else
                                                    {
                                                        result += "P[" + item.Exe + "] ";
                                                    }
                                                }
                                                else
                                                {
                                                    result += "A[" + item.Exe + "] ";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't start process " + item.Exe + "");
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Log("Start service " + item.Exe + "");
                                                if (!IsRunning(item.Exe))
                                                {
                                                    StartService(item.Exe);
                                                    result += "S[" + item.Exe + "] ";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't Start service " + item.Exe + "");
                                            }
                                        }
                                    }
                                    result += "started.";
                                }
                            }
                            else
                            {
                                result = "Service is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "StopService":
                        {
                            Crypto.Encrypt("Response to service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            if (serviceinstall == true)
                            {
                                foreach (FileExe item in Class.FileExeList.Reverse<FileExe>())
                                {
                                    if (item.Service == false)
                                    {
                                        try
                                        {
                                            if (FindProc(item.FullPath))
                                            {
                                                Log("Stop process " + item.Exe + "");

                                                Process[] processList = Process.GetProcesses();
                                                if (processList.Length > 0)
                                                {
                                                    foreach (Process p in processList)
                                                    {
                                                        try
                                                        {
                                                            if (p.Modules[0].FileName == item.FullPath)
                                                            {
                                                                p.Kill();
                                                                result += "P[" + item.Exe + "] ";
                                                            }

                                                        }
                                                        catch (Exception) { }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                result += "A[" + item.Exe + "] ";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log(ex.Message);
                                            Log("Can't stop process " + item.Exe + "");
                                        }
                                    }
                                    else
                                    {
                                        try
                                        {
                                            Log("Stop service " + item.Exe + "");
                                            if (IsRunning(item.Exe))
                                            {
                                                StopService(item.Exe);
                                                result += "S[" + item.Exe + "] ";
                                            }
                                            else
                                            {
                                                result += "A[" + item.Exe + "] ";
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            Log(ex.Message);
                                            Log("Can't Stop service " + item.Exe + "");
                                        }
                                    }
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "Service or process is not install.";
                            }

                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStartall":
                        {
                            Crypto.Encrypt("Response to whorucollector service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            if (string.IsNullOrEmpty(asecurityinstall))
                            {
                                foreach (string item in Class.strAsecurityList)
                                {
                                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == item);
                                    if (ctl != null)
                                    {
                                        if (ctl.Status == ServiceControllerStatus.Stopped)
                                        {
                                            ctl.Start();
                                            ctl.WaitForStatus(ServiceControllerStatus.Running);
                                            result += "[" + item + "] ";
                                        }
                                        else
                                        {
                                            result += "R[" + item + "] ";
                                        }
                                    }
                                }
                                result += "started.";
                            }
                            else
                            {
                                result = "whorucollector is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStartevent":
                        {
                            Crypto.Encrypt("Response to whorucollector service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityevent);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Stopped)
                                {
                                    ctl.Start();
                                    ctl.WaitForStatus(ServiceControllerStatus.Running);
                                    result += "[" + Class.strAsecurityevent + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityevent + "] ";
                                }
                                result += "started.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityevent + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStartfile":
                        {
                            Crypto.Encrypt("Response to whorucollector service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityfile);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Stopped)
                                {
                                    ctl.Start();
                                    ctl.WaitForStatus(ServiceControllerStatus.Running);
                                    result += "[" + Class.strAsecurityfile + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityfile + "] ";
                                }
                                result += "started.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityfile + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStartnetwork":
                        {
                            Crypto.Encrypt("Response to whorucollector service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecuritynetwork);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Stopped)
                                {
                                    ctl.Start();
                                    ctl.WaitForStatus(ServiceControllerStatus.Running);
                                    result += "[" + Class.strAsecuritynetwork + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecuritynetwork + "] ";
                                }
                                result += "started.";
                            }
                            else
                            {
                                result = "" + Class.strAsecuritynetwork + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStartproc":
                        {
                            Crypto.Encrypt("Response to whorucollector service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityproc);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Stopped)
                                {
                                    ctl.Start();
                                    ctl.WaitForStatus(ServiceControllerStatus.Running);
                                    result += "[" + Class.strAsecurityproc + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityproc + "] ";
                                }
                                result += "started.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityproc + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStartperf":
                        {
                            Crypto.Encrypt("Response to whorucollector service start task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityperf);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Stopped)
                                {
                                    ctl.Start();
                                    ctl.WaitForStatus(ServiceControllerStatus.Running);
                                    result += "[" + Class.strAsecurityperf + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityperf + "] ";
                                }
                            }
                            else
                            {
                                result = "" + Class.strAsecurityperf + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStopall":
                        {
                            Crypto.Encrypt("Response to whorucollector service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            if (string.IsNullOrEmpty(asecurityinstall))
                            {
                                foreach (string item in Class.strAsecurityList.Reverse<string>())
                                {
                                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == item);
                                    if (ctl != null)
                                    {
                                        if (ctl.Status == ServiceControllerStatus.Running)
                                        {
                                            ctl.Stop();
                                            ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                            result += "[" + item + "] ";
                                        }
                                        else
                                        {
                                            result += "R[" + item + "] ";
                                        }
                                    }
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "whorucollector is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStopevent":
                        {
                            Crypto.Encrypt("Response to whorucollector service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityevent);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Running)
                                {
                                    ctl.Stop();
                                    ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                    result += "[" + Class.strAsecurityevent + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityevent + "] ";
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityevent + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStopfile":
                        {
                            Crypto.Encrypt("Response to whorucollector service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityfile);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Running)
                                {
                                    ctl.Stop();
                                    ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                    result += "[" + Class.strAsecurityfile + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityfile + "] ";
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityfile + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStopnetwork":
                        {
                            Crypto.Encrypt("Response to whorucollector service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecuritynetwork);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Running)
                                {
                                    ctl.Stop();
                                    ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                    result += "[" + Class.strAsecuritynetwork + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecuritynetwork + "] ";
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "" + Class.strAsecuritynetwork + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStopperf":
                        {
                            Crypto.Encrypt("Response to whorucollector service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityperf);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Running)
                                {
                                    ctl.Stop();
                                    ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                    result += "[" + Class.strAsecurityperf + "] ";
                                }
                                else
                                {
                                    result += "R[" + Class.strAsecurityperf + "] ";
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityperf + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityStopproc":
                        {
                            Crypto.Encrypt("Response to whorucollector service stop task.", network);
                            // OK 확인용
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string result = null;
                            ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == Class.strAsecurityproc);
                            if (ctl != null)
                            {
                                if (ctl.Status == ServiceControllerStatus.Running)
                                {
                                    ctl.Stop();
                                    ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                    result += "[" + Class.strAsecurityproc + "] ";
                                }
                                else
                                {
                                    result += "A[" + Class.strAsecurityproc + "} ";
                                }
                                result += "stoped.";
                            }
                            else
                            {
                                result = "" + Class.strAsecurityproc + " is not install.";
                            }
                            result.Trim();
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityPatch":
                        {
                            string result = null;
                            if (string.IsNullOrEmpty(asecurityinstall))
                            {
                                Crypto.Encrypt("Response to whorucollector patch.", network);
                                network.Read(ReadByte, 0, (int)ReadByte.Length);
                                _data = Crypto.Decrypt(ReadByte);

                                string filesave = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                                filesave += "\\";
                                filesave += _data;
                                //string service = _data.Substring(0, _data.Length - 4);

                                bool file = File.FileRecive(network, clientSocket.ReceiveBufferSize, filesave);
                                if (file == true)
                                {
                                    Crypto.Encrypt("Success to download whorucollector file.", network);
                                    // 서비스가 실행되어 있는지 확인한다. 실행중인 경우 서비스 종료
                                    foreach (string item in Class.strAsecurityList)
                                    {
                                        ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == item);
                                        if (ctl != null)
                                        {
                                            if (ctl.Status == ServiceControllerStatus.Running)
                                            {
                                                ctl.Stop();
                                                ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                            }
                                        }
                                    }

                                    if (filesave.EndsWith(".zip"))
                                    {
                                        //Zip인 경우 압축 해제
                                        string zipToUnpack = filesave;
                                        string unpackDirectory = Class.strAsecurityDepolyFile;

                                        using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                                        {
                                            foreach (ZipEntry e in zip1)
                                            {
                                                e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        string whorudir = string.Format(Class.strAsecurityDepolyFile + @"\" + _data);
                                        FileInfo fileinfo1 = new FileInfo(filesave);
                                        FileInfo fileinfo2 = new FileInfo(whorudir);

                                        if (fileinfo1.LastWriteTime > fileinfo2.LastWriteTime)
                                        {
                                            try
                                            {
                                                try
                                                {
                                                    Directory.CreateDirectory(Class.strAsecurityDepolyFile);
                                                }
                                                catch (Exception) { }
                                                System.IO.File.Copy(filesave, whorudir, true);

                                                System.Diagnostics.Process.Start(@"" + Class.strAsecurityDepolyFile + "\\" + _data, @"-i").WaitForExit();

                                                result += _data + " ";

                                                foreach (string item in Class.strAsecurityList)
                                                {
                                                    ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == item);
                                                    if (ctl != null)
                                                    {
                                                        if (ctl.Status == ServiceControllerStatus.Stopped)
                                                        {
                                                            ctl.Start();
                                                            ctl.WaitForStatus(ServiceControllerStatus.StartPending);
                                                        }
                                                    }
                                                }
                                                result += "success to install/patch.";
                                            }
                                            catch (Exception)
                                            {
                                                result = "Fail to copy file or install.";
                                            }
                                        }
                                        else
                                        {
                                            result = "already to install/patch.";
                                        }
                                    }
                                }
                                else
                                {
                                    result = "Fail to File download.";
                                }
                            }
                            else
                            {
                                result = "whorucollector is not install.";

                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityDownload":
                        {
                            string result = null;
                            try
                            {
                                if (!string.IsNullOrEmpty(asecurityinstall))
                                {
                                    string fileread = Class.strAsecurityDepolyFile;
                                    fileread += "\\";
                                    fileread += "whoru.ini";
                                    using (System.IO.TextReader tmpReader = new System.IO.StreamReader(fileread))
                                    {
                                        result = tmpReader.ReadToEnd();
                                    }
                                }
                                else
                                {
                                    result = "whoru.ini path is wrong. you can make now.";
                                }
                            }
                            catch (Exception)
                            {
                                result = "whoru.ini path is wrong. you can make now.";
                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AgentDownload":
                        {
                            string result = null;
                            Crypto.Encrypt("Response to iruni.ini download.", network);
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string fileread = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            fileread += "\\";
                            fileread += "iruni.ini";

                            using (System.IO.TextReader tmpReader = new System.IO.StreamReader(fileread))
                            {
                                result = "Success to iruni.ini download.";
                                Crypto.Encrypt(tmpReader.ReadToEnd(), network);
                            }
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityUpdate":
                        {
                            string result = null;
                            try
                            {
                                if (!string.IsNullOrEmpty(asecurityinstall))
                                {
                                    Crypto.Encrypt("Response to whoru.ini update.", network);
                                    string fileread = Class.strAsecurityDepolyFile;
                                    fileread += "\\";
                                    fileread += "whoru.ini";
                                    network.Read(ReadByte, 0, (int)ReadByte.Length);
                                    _data = Crypto.Decrypt(ReadByte);

                                    using (System.IO.TextWriter tmpWriter = new StreamWriter(new FileStream(fileread, FileMode.Create)))
                                    {
                                        tmpWriter.Write(_data);
                                        tmpWriter.Close();
                                    }
                                    result = "Success to whoru.ini update.";
                                }
                                else
                                {
                                    result = "whoru.ini path is wrong. you can make now.";
                                }
                            }
                            catch (Exception)
                            {
                                result = "whoru.ini path is wrong. you can make now.";
                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AgentUpdate":
                        {
                            string result = null;
                            Crypto.Encrypt("Response to iruni.ini update.", network);
                            string filesave = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            filesave += "\\";
                            filesave += "iruni.ini";
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            _data = Crypto.Decrypt(ReadByte);

                            using (System.IO.TextWriter tmpWriter = new StreamWriter(new FileStream(filesave, FileMode.Create)))
                            {
                                tmpWriter.Write(_data);
                                tmpWriter.Close();
                            }
                            result = "Success to iruni.ini update.";
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AsecurityUpload":
                        {
                            string result = null;
                            try
                            {
                                if (string.IsNullOrEmpty(asecurityinstall))
                                {
                                    Crypto.Encrypt("Response to whoru.ini upload.", network);
                                    // Save the file using the filename sent by the client  

                                    string filesave = Class.strAsecurityDepolyFile;
                                    filesave += "\\";
                                    filesave += "whoru.ini";

                                    bool file = File.FileRecive(network, clientSocket.ReceiveBufferSize, filesave);

                                    result = "Success to download whoru.ini";
                                }
                                else
                                {
                                    result = "whoru.ini path is wrong.";
                                }
                            }
                            catch (Exception)
                            {
                                result = "whoru.ini path is wrong.";
                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    /*
                case "AsecurityInstall":
                    {
                        string result = null;

                        Crypto.Encrypt("Response to whorucollector install.", network);

                        // 파일 저장 진행 
                        string filesave = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        filesave += "\\";
                        filesave += "whoru.zip";

                        bool file = File.FileRecive(network, clientSocket.ReceiveBufferSize, filesave);
                        if (file == true)
                        {
                            // 서비스가 실행되어 있는지 확인한다. 실행중인 경우 서비스 종료
                            foreach (string item in Class.strAsecurityList)
                            {
                                ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == item);
                                if (ctl != null)
                                {
                                    if (ctl.Status == ServiceControllerStatus.Running)
                                    {
                                        ctl.Stop();
                                        ctl.WaitForStatus(ServiceControllerStatus.Stopped);
                                    }
                                }
                            }

                            //압축 해제
                            string zipToUnpack = filesave;
                            string unpackDirectory = Class.strAsecurityDepolyFile;
                            using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                            {
                                foreach (ZipEntry e in zip1)
                                {
                                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                                }
                            }
                        }
                        Crypto.Encrypt(result, network);
                        break;
                    }
                    */
                    case "CheckVersion":
                        {
                            Crypto.Encrypt("Response to state update.", network);
                            network.Read(ReadByte, 0, (int)ReadByte.Length);

                            string result = null;
                            loadConfig();
                            if (Class.FileExeList.Count > 0) serviceinstall = ServiceCheck(Class.FileExeList); else serviceinstall = false;

                            bool Firewallenabled = icfMgr.LocalPolicy.CurrentProfile.FirewallEnabled;
                            result = Class.Hostname;
                            result += "," + Class.strServiceName;
                            result += "," + Class.strServiceVersion;
                            result += "," + Class.servicestate;
                            result += "," + Resource1.Version;
                            result += "," + Firewallenabled;
                            result += "," + asecurityinstall;
                            Crypto.Encrypt(result, network);
                            network.Read(ReadByte, 0, (int)ReadByte.Length);

                            Crypto.Encrypt("Success to state update.", network);
                            break;
                        }
                    case "AgentPatch":
                        {
                            Crypto.Encrypt("Response to agent update.", network);

                            // 파일 저장 진행 
                            string filesave = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            filesave += "\\";
                            filesave += "iruni.zip";
                            bool file = File.FileRecive(network, clientSocket.ReceiveBufferSize, filesave);

                            // 서비스가 실행되어 있는지 확인한다. 실행중인 경우 서비스 종료
                            Crypto.Encrypt("Success to agent update, Wait 10 sec.", network);
                            System.Diagnostics.Process ps = new System.Diagnostics.Process();
                            ps.StartInfo.FileName = "agentupdate.exe";
                            ps.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            ps.StartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                            ps.StartInfo.CreateNoWindow = true;
                            ps.Start();
                            break;
                        }
                    case "Search":
                        {
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            string temp = Crypto.Decrypt(ReadByte);
                            string[] searchtemp = temp.Split('|');
                            Searchdata item = new Searchdata();
                            item.jobid = searchtemp[0];
                            item.commandtype = searchtemp[1];
                            item.searchstring = searchtemp[2];
                            item.history = searchtemp[3];
                            item.serverip = ((IPEndPoint)clientSocket.Client.RemoteEndPoint).Address.ToString();
                            lock (Searchlist_QueueLock)
                            {
                                Searchlist.Add(item);
                            }
                            //searchworkThread = new Thread(new ParameterizedThreadStart(DoSomething));
                            //ThreadPool.QueueUserWorkItem(DoSomething, jobid, commandtype, searchstring);
                            break;
                        }
                    case "DepolyFile":
                        {
                            string result = null;

                            try
                            {
                                Crypto.Encrypt("Start to Depoly file.", network);
                                Crypto.Encrypt(Class.strServiceVersion, network);

                                network.Read(ReadByte, 0, (int)ReadByte.Length);
                                _data = Crypto.Decrypt(ReadByte);

                                if ("FileSend" == _data)
                                {
                                    Crypto.Encrypt("Response to depoly file.", network);

                                    // 버전 정보 확인
                                    network.Read(ReadByte, 0, (int)ReadByte.Length);
                                    string version = Crypto.Decrypt(ReadByte);

                                    // 파일 이름 확인
                                    network.Read(ReadByte, 0, (int)ReadByte.Length);
                                    _data = Crypto.Decrypt(ReadByte);

                                    string filesave = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                                    filesave += "\\TEMP\\";
                                    try
                                    {
                                        Directory.CreateDirectory(filesave);
                                    }
                                    catch (Exception)
                                    {

                                    }
                                    filesave += Path.GetFileName(_data);

                                    Crypto.Encrypt("Response to depoly file information.", network);

                                    // 파일 전송 진행
                                    bool file = File.FileRecive(network, clientSocket.ReceiveBufferSize, filesave);


                                    // 서비스가 실행되어 있는지 확인한다. 실행중인 경우 서비스 종료
                                    foreach (FileExe item in Class.FileExeList.Reverse<FileExe>())
                                    {
                                        if (item.Exe.Contains(@"."))
                                        {
                                            try
                                            {
                                                if (FindProc(item.FullPath))
                                                {
                                                    Process[] processList = Process.GetProcessesByName(item.Exe);
                                                    if (processList.Length > 0)
                                                    {
                                                        foreach (Process p in processList)
                                                        {
                                                            p.Kill();
                                                        }
                                                    }
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't stop process " + item.Exe + "");
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Log("Stop service " + item.Exe + "");
                                                if (IsRunning(item.Exe))
                                                {
                                                    StopService(item.Exe);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't Stop service " + item.Exe + "");
                                            }
                                        }
                                    }
                                    if (filesave.EndsWith(".zip"))
                                    {
                                        //압축 해제
                                        string zipToUnpack = filesave;
                                        string unpackDirectory = Class.strDepolyFile;
                                        using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                                        {
                                            foreach (ZipEntry e in zip1)
                                            {
                                                e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                                            }
                                        }
                                        result += "ZIP[" + unpackDirectory + "] ";
                                    }
                                    else
                                    {
                                        foreach (FileExe item in Class.FileExeList)
                                        {
                                            try
                                            {
                                                string deploydir = string.Format(item.FullPath);
                                                FileInfo fileinfo1 = new FileInfo(filesave);
                                                FileInfo fileinfo2 = new FileInfo(deploydir);

                                                if (fileinfo1.LastWriteTime > fileinfo2.LastWriteTime)
                                                {
                                                    try
                                                    {
                                                        try
                                                        {
                                                            Directory.CreateDirectory(item.Path);
                                                        }
                                                        catch (Exception) { }
                                                        System.IO.File.Copy(filesave, deploydir, true);

                                                        result += "P[" + item.Exe + "] ";

                                                        result += _data + " ";
                                                    }
                                                    catch (Exception)
                                                    {
                                                        result = "Fail to copy file or install.";
                                                    }
                                                }
                                                else
                                                {
                                                    result = "already to deploy file.";
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't deploy file " + item.Exe + "");
                                            }
                                        }
                                    }
                                    // 서비스 실행
                                    foreach (FileExe item in Class.FileExeList.Reverse<FileExe>())
                                    {
                                        if (item.Exe.Contains(@"."))
                                        {
                                            try
                                            {
                                                Process ps = new Process();
                                                ps.StartInfo.FileName = item.Path + item.Exe;
                                                ps.Start();
                                                ps.WaitForExit();
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't start process " + item.Exe + "");
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                Log("Start service " + item.Exe + "");
                                                if (!IsRunning(item.Exe))
                                                {
                                                    StartService(item.Exe);
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                Log(ex.Message);
                                                Log("Can't Start service " + item.Exe + "");
                                            }
                                        }
                                    }
                                    result += "Success to service file depoly.";
                                }
                            }
                            catch
                            {
                                result = "Service deploy is error.";
                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "RunProcess":
                        {
                            string result = null;
                            Crypto.Encrypt("Response to run script.", network);

                            while (true)
                            {
                                try
                                {
                                    network.Read(ReadByte, 0, (int)ReadByte.Length);
                                    _data = Crypto.Decrypt(ReadByte);

                                    if (_data == "RunProcess_End")
                                    {
                                        break;
                                    }
                                    result += "Run command : " + _data + "\n\n";
                                    result += ExecuteCommand(_data);
                                }
                                catch (Exception ex)
                                {
                                    EventLogger.LogEvent("process run error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                                    result = "script run error with message: " + ex.Message;
                                }

                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "AddFirewall":
                        {
                            string result = null;
                            try
                            {
                                Crypto.Encrypt("Response to addfirewall task ready.", network);

                                network.Read(ReadByte, 0, (int)ReadByte.Length);
                                _data = Crypto.Decrypt(ReadByte);
                                string[] rule = _data.Split(' ');
                                if (rule.Length == 5)
                                {
                                    result = FirewallAction(rule[0], rule[1], rule[2], rule[3], rule[4]);

                                }
                                else
                                {
                                    result = "Firewall add fail.";
                                }
                            }
                            catch (Exception ex)
                            {
                                EventLogger.LogEvent("add firewall error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                                result = "add firewall error with message: " + ex.Message;
                            }

                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "FirewallEnable":
                        {
                            string result = null;
                            try
                            {
                                Crypto.Encrypt("Response to firewall enable.", network);
                                INetFwMgr manager = WinFirewallManager();
                                bool isFirewallEnabled = manager.LocalPolicy.CurrentProfile.FirewallEnabled;
                                if (isFirewallEnabled == true)
                                {
                                    result = "Already firewall enabled.";
                                }
                                else
                                {
                                    manager.LocalPolicy.CurrentProfile.FirewallEnabled = true;
                                    result = "Success to firewall enabled.";
                                }
                            }
                            catch (Exception ex)
                            {
                                EventLogger.LogEvent("firewall error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                                result = "firewall error with message: " + ex.Message;
                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "FirewallDisable":
                        {
                            string result = null;
                            try
                            {
                                Crypto.Encrypt("Response to firewall enable.", network);
                                INetFwMgr manager = WinFirewallManager();
                                bool isFirewallEnabled = manager.LocalPolicy.CurrentProfile.FirewallEnabled;
                                if (isFirewallEnabled != true)
                                {
                                    result = "Already firewall disabled.";
                                }
                                else
                                {
                                    manager.LocalPolicy.CurrentProfile.FirewallEnabled = false;
                                    result = "Success to firewall disabled.";
                                }
                            }
                            catch (Exception ex)
                            {
                                EventLogger.LogEvent("firewall error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                                result = "firewall error with message: " + ex.Message;
                            }
                            Crypto.Encrypt(result, network);
                            break;
                        }
                    case "IISInstall":
                        {
                            Crypto.Encrypt("Response to install IIS.", network);
                            network.Read(ReadByte, 0, (int)ReadByte.Length);
                            _data = Crypto.Decrypt(ReadByte);
                            string filesave = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            filesave += "\\";
                            filesave += _data;

                            bool file = File.FileRecive(network, clientSocket.ReceiveBufferSize, filesave);

                            string zipToUnpack = filesave;
                            string unpackDirectory = @"%windir%\temp";
                            using (ZipFile zip1 = ZipFile.Read(zipToUnpack))
                            {
                                foreach (ZipEntry e in zip1)
                                {
                                    e.Extract(unpackDirectory, ExtractExistingFileAction.OverwriteSilently);
                                }
                            }
                            string installpath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                            // string msisetup = @"/passive /norestart";
                            string apppool = @"add apppool /in < %windir%\temp\apppools.xml";
                            string site = @"add site /in < %windir%\temp\sites.xml";
                            string iis1 = @"/online /enable-feature /featurename:IIS-WebServerRole /featurename:IIS-WebServer /featurename:IIS-CommonHttpFeatures /featurename:IIS-StaticContent /featurename:IIS-DefaultDocument /featurename:IIS-DirectoryBrowsing /featurename:IIS-HttpErrors /all /Source:%windir%\source\sxs";
                            string iis2 = @"/online /enable-feature /featurename:IIS-HealthAndDiagnostics /featurename:IIS-HttpLogging /all /Source:%windir%\source\sxs";
                            string iis3 = @"/online /enable-feature /featurename:IIS-Performance /featurename:IIS-HttpCompressionStatic /all /Source:%windir%\source\sxs";
                            string iis4 = @"/online /enable-feature /featurename:IIS-IIS-Security /featurename:IIS-RequestFiltering /all /Source:%windir%\source\sxs";
                            string iis5 = @"/online /enable-feature /featurename:NetFx3 /featurename:NetFx4Extended-ASPNET45 /featurename:NetFx3ServerFeatures /featurename:IIS-NetFxExtensibility /featurename:WAS-ProcessModel /featurename:WAS-WindowsActivationService /featurename:WAS-NetFxEnvironment /featurename:WAS-ConfigurationAPI /all /Source:%windir%\source\sxs";
                            string iis6 = @"/online /enable-feature /featurename:IIS-ASPNET45 /featurename:IIS-NetFxExtensibility45 /featurename:WCF-HTTP-Activation /featurename:WCF-TCP-PortSharing45 /featurename:NetFx4Extended-ASPNET45 /featurename:IIS-ApplicationDevelopment /featurename:IIS-ISAPIExtensions /featurename:IIS-ISAPIFilter /featurename:IIS-WebSockets /featurename:WCF-Services45 /all /Source:%windir%\source\sxs";
                            string iis7 = @"/online /enable-feature /featurename:IIS-WebServerManagementTools /featurename:IIS-ManagementScriptingTools /all /Source:%windir%\source\sxs";

                            System.Diagnostics.Process.Start("sxs.exe").WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis1).WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis2).WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis3).WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis4).WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis5).WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis6).WaitForExit();
                            System.Diagnostics.Process.Start(@"dism.exe", iis7).WaitForExit();
                            System.Diagnostics.Process.Start(@"%windir%\system32\inetsrv\appcmd.exe", apppool).WaitForExit();
                            System.Diagnostics.Process.Start(@"%windir%\system32\inetsrv\appcmd.exe", site).WaitForExit();
                            System.Diagnostics.Process.Start(@"IISRESET.exe").WaitForExit();
                            Crypto.Encrypt("Success to install IIS.", network);
                            break;
                        }
                    default:
                        break;
                }
                loadConfig();
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

        protected override void OnStart(string[] args)
        {
            state = true;
            try
            {
                icfMgr = (INetFwMgr)Activator.CreateInstance(TicfMgr);
                aTimer1 = new System.Timers.Timer(60000);
                aTimer1.Elapsed += new ElapsedEventHandler(DoJoinServer);
                aTimer1.Enabled = true;
                aTimer1.Start();
                this.serverThread = new Thread(new ThreadStart(Server));
                this.serverThread.Start();

                securitylogworkThread = new System.Threading.Thread(Securitylog_BackgroundThread);
                securitylogworkThread.Start();

            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("service start error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
            }
        }

        private bool ServiceCheck(List<FileExe> list)
        {
            bool comp = false;
            Class.strServiceVersion = null;
            try
            {
                foreach (FileExe item in list)
                {
                    if (item.Service == false)
                    {
                        try
                        {
                            if (FindProc(item.FullPath))
                            {
                                comp = true;
                                Class.servicestate = 1;
                                FileInfo fileinfo = new FileInfo(item.FullPath);
                                Class.strServiceVersion = fileinfo.LastWriteTime.ToString(@"MM\/dd\/yyyy HH:mm");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Class.servicestate = 0;
                            Log(ex.Message);
                            Log("Can't find process " + item.Exe + "");
                        }
                    }
                    else
                    {
                        try
                        {
                            if (IsInstalled(item.Exe))
                            {
                                comp = true;
                                Class.servicestate = 1;
                                ManagementClass mc = new ManagementClass("Win32_Service");
                                string pathget = null;
                                foreach (ManagementObject mo in mc.GetInstances())
                                {
                                    if (mo.GetPropertyValue("Name").ToString() == Class.strAsecurityevent)
                                    {
                                        pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                                        break;
                                    }
                                }
                                FileInfo fileinfo = new FileInfo(pathget);
                                Class.strServiceVersion = fileinfo.LastWriteTime.ToString(@"MM\/dd\/yyyy HH:mm");
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            Class.servicestate = 0;
                            Log(ex.Message);
                            Log("Can't find service " + item.Exe + "");
                        }
                    }
                    if (comp != true)
                    {
                        FileInfo fileinfo = new FileInfo(item.FullPath);
                        if (fileinfo.Exists == true)
                        {
                            comp = true;
                            Class.servicestate = 0;
                            Class.strServiceVersion = fileinfo.LastWriteTime.ToString(@"MM\/dd\/yyyy HH:mm");
                        }
                    }
                }
            }
            catch (Exception)
            {
                comp = false;
            }

            return comp;
        }

        private string AsecurityServiceCheck(List<string> list)
        {
            string log = null;
            string file = null;
            string perf = null;
            string network = null;
            string proc = null;
            Log("Find Asecurity service");
            try
            {
                if (list.Count != 0)
                {
                    foreach (string item in list)
                    {
                        string service = item.ToLower();
                        ServiceController ctl = ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == item);
                        if (ctl != null)
                        {
                            if (service.EndsWith("event"))
                            {
                                log = "e";

                                continue;
                            }
                            if (service.EndsWith("file"))
                            {
                                file = "f";
                                continue;
                            }
                            if (service.EndsWith("perf"))
                            {
                                perf = "p";
                                continue;
                            }
                            if (service.EndsWith("network"))
                            {
                                network = "n";
                                continue;
                            }
                            if (service.EndsWith("proc"))
                            {
                                proc = "c";
                                continue;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
            }

            string result = string.Format(log + file + perf + network + proc);
            return result;
        }

        protected override void OnStop()
        {
            try
            {
                state = false;
                tcpListener.Server.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("service stop error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
            }
            finally
            {
                tcpListener.Server.Close();
            }
            serverThread.Join();
        }

        private static bool IsInstalled(string name)
        {
            Log("Check install service " + name + "");
            using (ServiceController controller =
                new ServiceController(name))
            {
                try
                {
                    ServiceControllerStatus status = controller.Status;
                }
                catch
                {
                    return false;
                }
                return true;
            }
        }

        private static bool IsRunning(string name)
        {

            Log("Check running service " + name + "");
            using (ServiceController controller =
                new ServiceController(name))
            {
                if (!IsInstalled(name)) return false;
                return (controller.Status == ServiceControllerStatus.Running);
            }
        }

        private static bool FindProc(string name)
        {
            Log("Check find process " + name + "");

            //Process[] proc = Process.GetProcesses();
            // return proc.Any(m =>  m.Modules[0].FileName == name);            


            Process[] processList = Process.GetProcesses();
            if (processList.Length > 0)
            {
                foreach (Process p in processList)
                {
                    try
                    {
                        if (p.Modules[0].FileName == name) return true;
                    }
                    catch (Exception) { }
                }
            }
            return false;

        }

        private static AssemblyInstaller GetInstaller()
        {
            AssemblyInstaller installer = new AssemblyInstaller(
                typeof(Iruniagent).Assembly, null);
            installer.UseNewContext = true;
            return installer;
        }

        private static void InstallService()
        {
            if (IsInstalled("iruni")) return;

            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Install(state);
                        installer.Commit(state);
                    }
                    catch
                    {
                        try
                        {
                            installer.Rollback(state);
                        }
                        catch { }
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void UninstallService()
        {
            if (!IsInstalled("iruni")) return;
            try
            {
                using (AssemblyInstaller installer = GetInstaller())
                {
                    IDictionary state = new Hashtable();
                    try
                    {
                        installer.Uninstall(state);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void StartService(string name)
        {
            Log("Try to start service " + name + "");
            if (!IsInstalled(name)) return;
            if (!IsRunning(name))
                using (ServiceController controller =
                new ServiceController(name))
                {
                    try
                    {
                        if (controller.Status != ServiceControllerStatus.Running)
                        {
                            controller.Start();
                            controller.WaitForStatus(ServiceControllerStatus.Running,
                                TimeSpan.FromSeconds(10));
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
        }

        private static void StopService(string name)
        {
            Log("Try to stop service " + name + "");
            if (!IsInstalled(name)) return;
            if (IsRunning(name))
                using (ServiceController controller =
                new ServiceController(name))
                {
                    try
                    {
                        if (controller.Status != ServiceControllerStatus.Stopped)
                        {
                            controller.Stop();
                            controller.WaitForStatus(ServiceControllerStatus.Stopped,
                                 TimeSpan.FromSeconds(10));
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
        }

        static void Main(string[] args)
        {
            CultureInfo ci = CultureInfo.InstalledUICulture;

            if (ci.ToString().StartsWith("ko"))
            {
                kor = true;
            }
            else
            {
                kor = false;
            }

            try
            {
                Class.strLocalPath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\iruni.ini";
                string strZippath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                strZippath += @"\\Ionic.Zip.dll";

                FileInfo fileinfo = new FileInfo(strZippath);
                if (fileinfo.Exists == false)
                {
                    byte[] aryData = Resource1.Ionic_Zip;
                    FileStream fileStream = new FileStream(fileinfo.FullName, FileMode.CreateNew);
                    fileStream.Write(aryData, 0, aryData.Length);
                    fileStream.Close();
                }

                string strZippath1 = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                strZippath1 += @"\\agentupdate.exe";

                FileInfo fileinfo1 = new FileInfo(strZippath1);
                if (fileinfo1.Exists == false)
                {
                    byte[] aryData = Resource1.agentupdate;
                    FileStream fileStream = new FileStream(fileinfo1.FullName, FileMode.CreateNew);
                    fileStream.Write(aryData, 0, aryData.Length);
                    fileStream.Close();
                }

                string strInipath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                strInipath += @"\\iruni.ini";

                FileInfo fileinfo2 = new FileInfo(strInipath);
                if (fileinfo2.Exists == false)
                {
                    byte[] aryData = Resource1.iruni;
                    FileStream fileStream = new FileStream(fileinfo2.FullName, FileMode.CreateNew);
                    fileStream.Write(aryData, 0, aryData.Length);
                    fileStream.Close();
                }

                string strUpdatepath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                strUpdatepath += @"\\update.exe";

                FileInfo fileinfo3 = new FileInfo(strUpdatepath);
                if (fileinfo3.Exists == false)
                {
                    byte[] aryData = Resource1.update;
                    FileStream fileStream = new FileStream(fileinfo3.FullName, FileMode.CreateNew);
                    fileStream.Write(aryData, 0, aryData.Length);
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                EventLogger.LogEvent("Iruni start error with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
            }

            try
            {
                string currentversion = Resource1.Version;
                WebClient client = new WebClient();
                Stream stream = client.OpenRead("https://asecurity.so/wp-content/uploads/2017/11/iruni.txt");
                StreamReader reader = new StreamReader(stream);
                string content = reader.ReadToEnd();
                string[] updatetemp = content.Split('|');
                string url = null;
                string process = null;
                string version = null;
                foreach (string item in updatetemp)
                {
                    if (item.Contains("-url")) url = item;
                    if (item.Contains("-process")) process = item;
                    if (item.Contains("version")) version = item;
                }

                if (!string.IsNullOrEmpty(url) & !string.IsNullOrEmpty(process) & !string.IsNullOrEmpty(version))
                {
                    if (version != currentversion)
                    {
                        string startprocess = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                        process += @"\\update.exe";
                        string argments = url + " " + process;
                        Process.Start(process, argments);
                        return;
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("\nFail to Autoupdate\nYou must connect to internet for autoupdate.");
            }

            Config.Load();

            string iptemp = Config.GetIniValue("Config", "Server");

            if (Environment.UserInteractive)
            {
                Console.WriteLine("IRUNI Agent Version:" + Resource1.Version + "");
                Console.WriteLine("Starting...");
                Console.WriteLine("Server Connect");
                Console.WriteLine("     Server Address: {0}", iptemp);
                Console.WriteLine("     Server Port: {0}", Class.strIRUNIPort);
                Console.WriteLine("Service Control");
                Console.WriteLine("     Service/Exe: {0}", Class.strServiceName);
                Console.WriteLine("     Service Port: {0}", Class.strServicePort);
            }
            Type NetFwMgrType = Type.GetTypeFromProgID("HNetCfg.FwMgr", false);
            INetFwMgr mgr = (INetFwMgr)Activator.CreateInstance(NetFwMgrType);

            // 포트 체크
            bool isAvailable = false;
            isAvailable = CheckAvailableServerPort(Class.intIRUNIagentPort);
            if (isAvailable == false)
            {
                Log("Already use another program " + Class.strIRUNIPort + " tcp port. use another port.");
                return;
            }

            // 이미 포트 오픈이 되었는지 확인
            if (isPortFound(Class.intIRUNIagentPort) == false)
            {
                Type type = Type.GetTypeFromProgID("HNetCfg.FWOpenPort");
                INetFwOpenPort firewallport = Activator.CreateInstance(type)
  as INetFwOpenPort;
                firewallport.Name = "Iruni Agent Listen Port TCP " + Class.strIRUNIPort + " <- " + iptemp;
                firewallport.Port = Class.intIRUNIagentPort;
                firewallport.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
                firewallport.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
                firewallport.RemoteAddresses = iptemp;
                firewallport.Enabled = true;
                mgr = WinFirewallManager();
                try
                {
                    mgr.LocalPolicy.CurrentProfile.GloballyOpenPorts.Add(firewallport);
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent(ex.Message, EventLogEntryType.Warning);
                }
            }
            // Asecurity 파일경로 가져오기 없으면 샛팅
            string pathget = null;
            ManagementClass mc = new ManagementClass("Win32_Service");
            foreach (ManagementObject mo in mc.GetInstances())
            {
                if (mo.GetPropertyValue("Name").ToString() == Class.strAsecurityevent)
                {
                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    break;
                }
                if (mo.GetPropertyValue("Name").ToString() == Class.strAsecurityfile)
                {
                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    break;
                }
                if (mo.GetPropertyValue("Name").ToString() == Class.strAsecuritynetwork)
                {
                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    break;
                }
                if (mo.GetPropertyValue("Name").ToString() == Class.strAsecurityperf)
                {
                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    break;
                }
                if (mo.GetPropertyValue("Name").ToString() == Class.strAsecurityproc)
                {
                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                    break;
                }
            }


            if (string.IsNullOrEmpty(pathget)) Class.strAsecurityDepolyFile = @"C:\WHORU";
            else
            {
                string[] pathtemp = pathget.Split('\\');
                for (int i = 0; pathtemp.Length - 1 > i; i++)
                {
                    Class.strAsecurityDepolyFile += pathtemp[i];
                    Class.strAsecurityDepolyFile += @"\";
                }
            }

            // 호스트 이름 가져오기
            Class.Hostname = System.Net.Dns.GetHostName();
            Class.strAsecurityList.Add(Class.strAsecurityevent);
            Class.strAsecurityList.Add(Class.strAsecurityfile);
            Class.strAsecurityList.Add(Class.strAsecuritynetwork);
            Class.strAsecurityList.Add(Class.strAsecurityperf);
            Class.strAsecurityList.Add(Class.strAsecurityproc);
            Iruniagent service = new Iruniagent();
            if (!Environment.UserInteractive)
            {
                ServiceBase.Run(service);
            }
            else
            {
                try
                {
                    if (args.Length == 1)
                    {
                        Console.WriteLine("Iruni Version:" + Assembly.GetEntryAssembly().GetName().Version + "");
                        switch (args[0])
                        {
                            case "-i":
                                InstallService();
                                StartService("Iruni");
                                Console.WriteLine("Success to install services type");
                                break;
                            case "-u":
                                StopService("Iruni");
                                UninstallService();
                                Console.WriteLine("Success to uninstall services type");
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                    else
                    {
                        Console.WriteLine("Iruni Version:" + Assembly.GetEntryAssembly().GetName().Version + "");
                        Console.WriteLine("Option");
                        Console.WriteLine("     -i: install services type");
                        Console.WriteLine("     -u: uninstall services type");
                    }
                    Console.WriteLine("Starting...");
                    Console.WriteLine("Iruni running; press any key to stop");
                    service.OnStart(null);
                    Console.ReadKey(true);
                    service.OnStop();
                    Console.WriteLine("Iruni stopped");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Iruni Error: " + ex.Message + "");
                }
            }
            /*
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
            new Iruni() 
            };
            ServiceBase.Run(ServicesToRun);                */

        }

        private static bool CheckAvailableServerPort(int port)
        {
            bool isAvailable = true;

            // Evaluate current system tcp connections. This is the same information provided
            // by the netstat command line application, just in .Net strongly-typed object
            // form.  We will look through the list, and if our port we would like to use
            // in our TcpClient is occupied, we will set isAvailable to false.
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] tcpConnInfoArray = ipGlobalProperties.GetActiveTcpListeners();

            foreach (IPEndPoint endpoint in tcpConnInfoArray)
            {
                if (endpoint.Port == port)
                {
                    isAvailable = false;
                    break;
                }
            }

            return isAvailable;
        }

        private StringBuilder FindSecuritylog(string found)
        {
            StringBuilder result = null;

            foreach (Storeevent.SecurityLogdata item in Storeevent.Securitylog)
            {
                if (item.login_address.Contains(found) || item.login_name.Contains(found) || item.login_type.Contains(found) || item.msg.Contains(found) || item.program.Contains(found))
                {
                    result.Append(item.time + " " + item.login_address + " " + item.login_address + " " + item.login_type + " " + item.msg + " " + item.program);
                }
            }
            return result;
        }

        private string ExecuteCommand(string command)
        {
            string content = null;
            try
            {
                using (Process p = new Process())
                {
                    ProcessStartInfo ps = new ProcessStartInfo();
                    ps.FileName = "cmd.exe";
                    ps.Arguments = "/c " + command;
                    ps.UseShellExecute = false;
                    ps.WindowStyle = ProcessWindowStyle.Hidden;
                    ps.RedirectStandardInput = true;
                    ps.RedirectStandardOutput = true;
                    ps.RedirectStandardError = true;

                    p.StartInfo = ps;
                    p.Start();

                    StreamReader stdOutput = p.StandardOutput;

                    content = stdOutput.ReadToEnd();
                    string exitStatus = p.ExitCode.ToString();

                    if (exitStatus != "0")
                    {
                        content = "Command Errored";
                        // Command Errored. Handle Here If Need Be
                    }
                }
            }
            catch (Exception ex)
            {
                content = ex.Message;
            }
            return content;
        }

        private static INetFwMgr WinFirewallManager()
        {
            Type type = Type.GetTypeFromCLSID(
                new Guid("{304CE942-6E39-40D8-943A-B913C40C9CD4}"));
            return Activator.CreateInstance(type) as INetFwMgr;
        }

        protected internal static bool isPortFound(int portNumber)
        {
            bool boolResult = false;
            INetFwOpenPorts ports = null;
            Type progID = null;
            INetFwMgr firewall = null;
            INetFwOpenPort currentPort = null;
            try
            {
                progID = Type.GetTypeFromProgID("HNetCfg.FwMgr");
                firewall = Activator.CreateInstance(progID) as INetFwMgr;
                ports = firewall.LocalPolicy.CurrentProfile.GloballyOpenPorts;
                IEnumerator portEnumerate = ports.GetEnumerator();
                while ((portEnumerate.MoveNext()))
                {
                    currentPort = portEnumerate.Current as INetFwOpenPort;
                    if (currentPort.Port == portNumber)
                    {
                        boolResult = true;
                        break;
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                if (ports != null) ports = null;
                if (progID != null) progID = null;
                if (firewall != null) firewall = null;
                if (currentPort != null) currentPort = null;
            }
            return boolResult;
        }

    }
}
