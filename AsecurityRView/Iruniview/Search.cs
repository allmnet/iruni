using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Iruniview
{
   

    public partial class Search : Form
    {
        private TcpClient tcpClient = null;
        public List<String> listItem { get; set; }
        public int Host_Count { get; set; }
        public int Host_done { get; set; }

        public Search()
        {
            InitializeComponent();            
        }

        public void ClickSearch()
        {
            Form1.Searchfrm.SearchlistView = new ListView();            
        }

        private async void RunTask(string jobid, string type, string command)
        {
            int i = 1;
            toolStripProgressBar.Value = 0;
            toolStripProgressBar.Maximum = Host_Count;

            foreach (string item in listItem)
            {
                toolStripProgressBar.Value = i;

                string ip = item;
                try
                {
                    Task<string> RunStartTask = ServiceTask(ip, command);
                    Form1.PassLog(ip, "Run :"+ command + " Job: "+ jobid + " Type:"+ type);
                }
                catch (Exception ex)
                {
                    EventLogger.LogEvent("Error start service with message: " + ex.Message,
System.Diagnostics.EventLogEntryType.Warning);
                    Program.ShowBalloonTip(ip, string.Format("Fail to running search " + command + " of {0} server.", ip), ToolTipIcon.Error);
                }
                i++;
            }
        }

        async Task<string> ServiceTask(string ip, string commmand)
        {           

            string returnValue = "Fail";
            try
            {
                tcpClient = new TcpClient();
                NetworkStream network = TcpConnecting(tcpClient, ip, 9051);

                if (network != null)
                {
                    byte[] ReadByte;
                    ReadByte = new byte[tcpClient.ReceiveBufferSize];

                    Crypto.Encrypt(commmand, network);

                    //클라이언트 전송 대기 확인
                    int ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                    returnValue = Crypto.Decrypt(ReadByte);
                }
                return returnValue;
            }
            catch (Exception)
            {
                returnValue += " " + commmand;
                Form1.PassLog(ip, returnValue);
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        private NetworkStream TcpConnecting(TcpClient Tcpclient, string ip, int port)
        {
            var result = Tcpclient.BeginConnect(ip, 9051, null, null);
            var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
            NetworkStream network = null;
            if (success)
            {
                network = tcpClient.GetStream();
            }
            return network;
        }
    }
}
