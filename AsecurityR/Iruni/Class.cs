using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iruni
{
    class FileExe
    {
        public string Exe { get; set; }
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Argment { get; set; }
        public bool Service { get; set; }
    }

    class Class
    {
        public static string strLocalPath = null;
        public static string strAsecurityDepolyFile;
        public static string strDepolyFile;
        public static bool b_Networklogon = false;
        public static List<string> strServerIP = new List<string>();
        public static string strServiceVersion;
        public static int servicestate = 2;
        public static string Hostname;
        public static string strServiceName;
        public static int intPort = 0;
        public static string strIRUNIPort;
        public static int intIRUNIagentPort;
        public static string strServicePort;
        public static List<string> strAsecurityList = new List<string>();
        public static string strAsecurityevent = "WHORUEvent";
        public static string strAsecurityfile = "WHORUFile";
        public static string strAsecuritynetwork =  "WHORUNetwork";
        public static string strAsecurityperf = "WHORUPerf";
        public static string strAsecurityproc = "WHORUProc";
        public static int storelog = 180;
        public static List<FileExe> FileExeList = new List<FileExe>();
    }
}
