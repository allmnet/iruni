using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Net;
using System.Drawing;

namespace Iruniview.Monitors
{
    class PortMonitor : MonitorBase
    {
        private string _service;
        private string _host;
        private string _port;
        private string _description;
        private string _type;
        private string _agent;
        private string _version;
        private string _firewall;
        private string _health;
        private string _grouptype;
        private string _asecuritystat;
        private TaskStatus _status;
        

        public override void Execute()
        {
            if (!String.IsNullOrEmpty(_port) || _port != null)
            {
                int portno = Convert.ToInt32(_port);
                IPAddress ipa = (IPAddress)Dns.GetHostAddresses(_host)[0];
                System.Net.Sockets.Socket sock = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                try
                {
                    sock.Connect(ipa, portno);
                    if (sock.Connected == true)  // Port is in use and connection is successful
                    {
                        _description = "Success to connected service port.";
                        _status = TaskStatus.OK;
                    }
                    sock.Close();

                }
                catch (System.Net.Sockets.SocketException ex)
                {
                    _status = TaskStatus.Fail;
                    _description = "Fail to connected service port";
                    Form1.PassLog(_host, string.Format("Port {0} of '{1}' failed with the '{2}' status.", _port, _host, ex.ErrorCode.ToString()));
                    if (ex.ErrorCode == 10061)  // Port is unused and could not establish connection 
                    {
                        Notify("Failed Open Port", string.Format("Port {0} of '{1}' failed with the '{2}' status.", _port, _host, ex.ErrorCode.ToString()));
                        Form1.PassLog(_host, "Failed Open Port");
                    }
                    else
                    {
                        Notify("Failed Open Port", string.Format("Port {0} of '{1}' failed with the '{2}' status.", _port, _host, ex.ErrorCode.ToString()));
                    }
                }
            }
            else
            {
                _description = "Not define Service Port.";
            }
        }

        protected override void Initialize(Dictionary<string, string> settings)
        {
            _service = settings["service"];
            _host = settings["host"];
            _port = settings["port"];
            _version = settings["version"];
            _agent = settings["agent"];
            _health = settings["health"];
            _firewall = settings["firewall"];
            _grouptype = settings["grouptype"];
            _type = settings["type"];
            _asecuritystat = settings["asecurity"];
        }

        public override string IPAddress
        {
            get { return string.Format("{0}", _host); }
        }

        public override string Health
        {
            get
            {
                switch (_health)
                {
                    case "1":
                        return "Normal";
                    case "2":
                        return "Abnormal";
                    default:
                        return "None";
                }
            }
        }

        public override string Type
        {
            get { return _type; }
        }

        public override string Port
        {
            get { return _port; }
        }

        public override string Firewall
        {
            get { return _firewall; }
        }

        public override string Asecuritystate
        {
            get { return _asecuritystat; }
        }

        public override TaskStatus TaskStatus
        {
            get { return _status; }
        }

        public override MonitorType MonitorType
        {
            get
            {
                switch(_grouptype)
                {
                    case "Asecurity_Windows":
                        return MonitorType.Asecurity_Windows;
                    case "Asecurity_Linux":
                        return MonitorType.Asecurity_Linux;
                    case "Other_Linux":
                        return MonitorType.Linux;
                    case "Other_Windows":
                        return MonitorType.Windows;
                    default:
                        return MonitorType.Other;
                }
            }
        }

        public override string Description
        {
            get { return _description; }
        }

        public override string Service
        {
            get { return _service; }
        }

        public override string Version
        {
            get { return _version; }
        }

        public override string Agent
        {
            get { return _agent; }
        }
        public override Icon Icon
        {
            get { return Properties.Resources.ServerIcon; }
        }
    }
}
