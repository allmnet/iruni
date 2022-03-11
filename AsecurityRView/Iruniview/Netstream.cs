using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace EastSeaView
{
    class Netstream
    {
        public static byte[] Protect(byte[] bytes)
        {
            for (int i = 0; i < bytes.Length; ++i) bytes[i] ^= 168;
            return bytes;
        }

        public static byte[] GetBytes(string str)
        {
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes;
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, str);
            ms.Seek(0, 0);
            bytes = ms.ToArray();
            return bytes;
        }

        public static string GetString(byte[] bytes)
        {
            BinaryFormatter bfx = new BinaryFormatter();
            MemoryStream msx = new MemoryStream();
            msx.Write(bytes, 0, bytes.Length);
            msx.Seek(0, 0);
            string sx = (string)bfx.Deserialize(msx);
            return sx;
        }

        public static void Send(string message, NetworkStream stream)
        {
            byte[] strbuf = System.Text.Encoding.UTF8.GetBytes(message);
            byte[] intbuf = BitConverter.GetBytes(System.Net.IPAddress.HostToNetworkOrder(strbuf.Length));
            stream.Write(intbuf, 0, intbuf.Length);
            stream.Write(strbuf, 0, strbuf.Length);
        }

        public static string Receive(NetworkStream stream)
        {
            byte[] intbuf = new byte[4];
            stream.Read(intbuf, 0, intbuf.Length);
            int length = System.Net.IPAddress.NetworkToHostOrder(BitConverter.ToInt32(intbuf, 0));

            byte[] strbuf = new byte[length];
            stream.Read(strbuf, 0, strbuf.Length);
            string message = System.Text.Encoding.UTF8.GetString(strbuf);
            return message;
        }
    }
}
