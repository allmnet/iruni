using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Iruni
{
    class Crypto
    {
        public static void Encrypt(string msg, NetworkStream network)
        {
            byte crypto = Convert.ToByte(DateTime.UtcNow.Hour + DateTime.UtcNow.Minute);
            BinaryFormatter bf = new BinaryFormatter();
            byte[] bytes;
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, msg);
            ms.Seek(0, 0);
            bytes = ms.ToArray();
            for (int i = 0; i < bytes.Length; ++i) bytes[i] ^= crypto;
            network.Write(bytes, 0, bytes.Length);
        }

        public static string Decrypt(byte[] bytes)
        {
            byte crypto = Convert.ToByte(DateTime.UtcNow.Hour + DateTime.UtcNow.Minute);
            for (int i = 0; i < bytes.Length; ++i) bytes[i] ^= crypto;
            BinaryFormatter bfx = new BinaryFormatter();
            MemoryStream msx = new MemoryStream();
            msx.Write(bytes, 0, bytes.Length);
            msx.Seek(0, 0);
            string sx = (string)bfx.Deserialize(msx);

            return sx;
        }
    }
}