using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Iruniview;

namespace Iruniview
{
    class Dialog
    {

        public static void ShowDialog(string caption, string text, string select, bool read)
        {
            Label textLabel = new Label() { Left = 16, Top = 20, Width = 240 };
            RichTextBox textBox = new RichTextBox() { Left = 16, Top = 50, Width = 430, Height = 320, TabIndex = 0, TabStop = true, Multiline = true, ScrollBars = RichTextBoxScrollBars.Vertical, ShortcutsEnabled = true };
            Button confirmation = new Button() { Text = "Okay!", Left = 16, Width = 80, Top = 380, TabIndex = 1, TabStop = true };
            Button update = new Button() { Text = "Upload now", Left = 100, Width = 100, Top = 380, TabIndex = 2, TabStop = true };
            string ip = null;

            Form prompt = new Form();
            ip = caption;
            prompt.Icon = Iruniview.Properties.Resources.ServerIcon;
            prompt.Width = 480;
            prompt.Height = 460;
            prompt.Text = caption;
            textLabel.Text = select ;
            textBox.Text = text;
            confirmation.Click += (sender, e) => { prompt.Close(); };
            update.Click += myButton_Click;
            prompt.Controls.Add(textLabel);
            prompt.Controls.Add(textBox);
            if (read == true)
            {
                textBox.ReadOnly = true;
            }
            else
            {
                prompt.Controls.Add(update);
            }
            prompt.Controls.Add(confirmation);
            prompt.AcceptButton = confirmation;
            prompt.StartPosition = FormStartPosition.CenterScreen;
            prompt.ShowDialog();
            return;
        }

        public async static void myButton_Click(object sender, EventArgs e)
        {
            Label textLabel = new Label() { Left = 16, Top = 20, Width = 240 };
            RichTextBox textBox = new RichTextBox() { Left = 16, Top = 50, Width = 430, Height = 320, TabIndex = 0, TabStop = true, Multiline = true, ScrollBars = RichTextBoxScrollBars.Vertical, ShortcutsEnabled = true };
            string ip = null;
            if (textLabel.Text.Contains("ASECURITY"))
            {
                Form1.PassLog(textLabel.Text, "Try to whoru.ini update.");
                Task<string> RunAsecurityUpdateTask = AsecurityUpdateTask(ip, textBox.Text.ToString());
                string StrResult = await RunAsecurityUpdateTask;
            }
            if (textLabel.Text.Contains("IRUNI"))
            {
                Form1.PassLog(textLabel.Text, "Try to iruni.ini update.");
                Task<string> RunAsecurityUpdateTask = AgentUpdateTask(ip, textBox.Text.ToString());
                string StrResult = await RunAsecurityUpdateTask;
            }
        }

        async static Task<string> AsecurityUpdateTask(string ip, string config)
        {
            string returnValue = "Fail";
            RichTextBox textBox = new RichTextBox() { Left = 16, Top = 50, Width = 430, Height = 320, TabIndex = 0, TabStop = true, Multiline = true, ScrollBars = RichTextBoxScrollBars.Vertical, ShortcutsEnabled = true };
            TcpClient tcpClient = new TcpClient();
            try
            {
                var result = tcpClient.BeginConnect(ip, 9051, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                if (!success)
                {
                    returnValue = "Failed to connect.";
                }
                //LocalHost에 지정포트로 TCP Connection생성 후 데이터 송수신 스트림 얻음 읽기/쓰기                
                // StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
                // StreamReader reader = new StreamReader(tcpClient.GetStream());
                NetworkStream network = tcpClient.GetStream();
                Crypto.Encrypt("AsecurityUpdate", network);
                byte[] ReadByte;
                ReadByte = new byte[tcpClient.ReceiveBufferSize];
                int ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                Form1.PassLog(ip, returnValue);

                Crypto.Encrypt(textBox.Text, network);

                ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                returnValue = Crypto.Decrypt(ReadByte);
                Form1.PassLog(ip, returnValue);

                return returnValue;
            }
            catch (Exception)
            {
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }

        async static Task<string> AgentUpdateTask(string ip, string config)
        {
            string returnValue = "Fail";
            RichTextBox textBox = new RichTextBox() { Left = 16, Top = 50, Width = 430, Height = 320, TabIndex = 0, TabStop = true, Multiline = true, ScrollBars = RichTextBoxScrollBars.Vertical, ShortcutsEnabled = true };
            TcpClient tcpClient = new TcpClient();
            try
            {                
                var result = tcpClient.BeginConnect(ip, 9051, null, null);
                var success = result.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2));
                string _data = null;
                if (!success)
                {
                    returnValue = "Failed to connect."; 
                }
                //LocalHost에 지정포트로 TCP Connection생성 후 데이터 송수신 스트림 얻음 읽기/쓰기                
                // StreamWriter sWriter = new StreamWriter(tcpClient.GetStream());
                // StreamReader reader = new StreamReader(tcpClient.GetStream());
                NetworkStream network = tcpClient.GetStream();
                Crypto.Encrypt("AgentUpdate", network);
                byte[] ReadByte;
                ReadByte = new byte[tcpClient.ReceiveBufferSize];
                int ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                _data = Crypto.Decrypt(ReadByte);
                Form1.PassLog(ip, _data);

                Crypto.Encrypt(textBox.Text, network);

                ReadResult = await network.ReadAsync(ReadByte, 0, (int)ReadByte.Length);
                returnValue = Crypto.Decrypt(ReadByte);
                Form1.PassLog(ip, returnValue);
                
                return returnValue;
            }
            catch (Exception)
            {
                return returnValue;
            }
            finally
            {
                tcpClient.Close();
            }
        }
    }
}
