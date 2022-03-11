using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Iruni
{
    class File
    {
        public static bool FileRecive(NetworkStream network, int BufferSize, string filesave)
        {
            byte[] Filebuffer = null;
            byte[] ReadByte = null;
            int received = 0;
            int read = 0;
            int size = 1024;
            int remaining = 0;
            bool result = true;
            try
            {
                // 파일 사이즈 확인
                ReadByte = new byte[BufferSize];                
                network.Read(ReadByte, 0, (int)ReadByte.Length);
                string FileSize = Crypto.Decrypt(ReadByte);
                int length = Convert.ToInt32(FileSize);
                Filebuffer = new byte[length];

                Crypto.Encrypt("Response to File download.", network);

                // Read bytes from the client using the length sent from the client    
                while (received < length)
                {
                    remaining = length - received;
                    if (remaining < size)
                    {
                        size = remaining;
                    }

                    read = network.Read(Filebuffer, received, size);
                    received += read;
                }

                using (System.IO.FileStream fStream = new FileStream(filesave, FileMode.Create))
                {
                    fStream.Write(Filebuffer, 0, Filebuffer.Length);
                    fStream.Flush();
                    fStream.Close();
                }
            }
            catch (Exception ex)
            {
                result = false;
                EventLogger.LogEvent("network file recive error with message: " + ex.Message,
    System.Diagnostics.EventLogEntryType.Warning);
            }

            return result;
        }
    }
}
