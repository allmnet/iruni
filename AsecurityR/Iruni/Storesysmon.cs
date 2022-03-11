using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iruni
{
    class Storesysmon
    {
        public struct Capturedata
        {
            public DateTime time;
            public string message;
        }

        public static List<Capturedata> Processlog = new List<Capturedata>();
        public static List<Capturedata> Handlelog = new List<Capturedata>();
        public static List<Capturedata> Filelog = new List<Capturedata>();
        public static List<Capturedata> Reglog = new List<Capturedata>();

        public static object Processlog_QueueLock = new object();

        public static object Handlelog_QueueLock = new object();

        public static object Filelog_QueueLock = new object();

        public static object Reglog_QueueLock = new object();
    }
}