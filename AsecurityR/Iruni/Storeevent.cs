using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Iruni
{
    public class Storeevent
    {
        public struct Capturedata
        {
            public DateTime time;
            public string type;
            public string message;
        }

        public struct SecurityLogdata
        {
            public DateTime time;
            public string login_type;
            public int login;
            public string login_address;
            public string login_name;
            public string program;
            public string msg;
        }

        // 정보 수집 
        public static List<SecurityLogdata> Securitylog = new List<SecurityLogdata>();
        public static List<Capturedata> Eventlog = new List<Capturedata>();

        public static object Securitylog_QueueLock = new object();
        public static object Eventlog_QueueLock = new object();

    }
}