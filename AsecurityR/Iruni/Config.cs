using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Iruni
{
    class Config
    {
        internal static class SafeNativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
            internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

            [DllImport("kernel32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
            internal static extern int WritePrivateProfileString(string section, string key, string val, string filePath);
        }

        public static void SetIniValue(string section, string key, string value)
        {
            SafeNativeMethods.WritePrivateProfileString(section, key, value, Class.strLocalPath);
        }

        public static string GetIniValue(string section, string key)
        {
            StringBuilder temp = new StringBuilder(255);
            SafeNativeMethods.GetPrivateProfileString(section, key, "", temp, 255, Class.strLocalPath);
            return temp.ToString();
        }

        public static void Load()
        {
            Class.strServiceName = null;
            Class.FileExeList = new List<FileExe>();

            string temp = GetIniValue("Config", "Port");
            if (!string.IsNullOrEmpty(temp))
            {
                Class.intPort = Convert.ToInt32(temp);
                Class.strIRUNIPort = temp;
                Class.intIRUNIagentPort = Class.intPort + 1;
            }
            else 
            {
                Class.intPort = 9051;
                Class.intIRUNIagentPort = Class.intPort + 1;
                Class.strIRUNIPort = "9051";
            }

            //관리할 서비스 가져오기
            temp = GetIniValue("Service", "Exe");
            if (!string.IsNullOrEmpty(temp))
            {
                string[] result = temp.Split(',');

                foreach (string item in result)
                {
                    FileExe fileitem = new FileExe();
                    string servicename = null;
                    string pathget = null;
                    string filepath = null;
                    string arg = null;
                    if (!item.Contains(@"\"))
                    {
                        string[] args = item.Split(new char[] { ' ' }, 2);
                        if (args.Count() == 2)
                        {
                            arg = args[1];
                            if (args[0].Contains(@"."))
                            {
                                string[] servicenametemp = args[0].Split('.');
                                servicename = servicenametemp[0];
                            }
                            else
                            {
                                servicename = args[0];
                            }
                            // 서비스 이름이 있는지 확인 한다
                            ManagementClass mc = new ManagementClass("Win32_Service");
                            foreach (ManagementObject mo in mc.GetInstances())
                            {
                                if (mo.GetPropertyValue("Name").ToString() == servicename)
                                {
                                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                                    fileitem.FullPath = pathget;
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(pathget))
                            {
                                filepath = Class.strDepolyFile;
                            }
                            else
                            {
                                string[] pathtemp = pathget.Split('\\');
                                for (int i = 0; pathtemp.Length - 1 > i; i++)
                                {
                                    filepath += pathtemp[i];
                                    filepath += @"\";
                                }
                                Class.strDepolyFile = filepath;
                            }
                        }
                        else
                        {
                            if (item.Contains(@"."))
                            {
                                string[] servicenametemp = item.Split('.');
                                servicename = servicenametemp[0];
                            }
                            else
                            {
                                servicename = item;
                            }
                            // 서비스 이름이 있는지 확인 한다
                            ManagementClass mc = new ManagementClass("Win32_Service");
                            foreach (ManagementObject mo in mc.GetInstances())
                            {
                                if (mo.GetPropertyValue("Name").ToString() == servicename)
                                {
                                    pathget = mo.GetPropertyValue("PathName").ToString().Trim('"');
                                    fileitem.FullPath = pathget;
                                    break;
                                }
                            }
                            if (string.IsNullOrEmpty(pathget))
                            {
                                filepath = Class.strDepolyFile;
                            }
                            else
                            {
                                string[] pathtemp = pathget.Split('\\');
                                for (int i = 0; pathtemp.Length - 1 > i; i++)
                                {
                                    filepath += pathtemp[i];
                                    filepath += @"\";
                                }
                                Class.strDepolyFile = filepath;
                            }
                        }

                        fileitem.Path = filepath;
                        fileitem.Service = true;
                        fileitem.Exe = servicename;
                        fileitem.Argment = arg;
                        Class.strServiceName += servicename;
                        Class.strServiceName += " ";
                        Class.FileExeList.Add(fileitem);
                    }
                    else
                    {
                        string[] args = item.Split(new char[] { ' ' }, 2);
                        if (args.Count() == 2)
                        {
                            arg = args[1];
                            fileitem.FullPath = args[0];
                            string[] pathtemp = args[0].Split('\\');
                            for (int i = 0; pathtemp.Length - 1 > i; i++)
                            {

                                filepath += pathtemp[i];
                                filepath += @"\";
                            }
                            if (pathtemp[pathtemp.Length - 1].Contains(@"."))
                            {
                                string[] servicenametemp = pathtemp[pathtemp.Length - 1].Split('.');
                                servicename = servicenametemp[0];
                            }

                            fileitem.Exe = pathtemp[pathtemp.Length - 1];
                            Class.strServiceName += pathtemp[pathtemp.Length - 1];
                            Class.strServiceName += " ";
                            if (string.IsNullOrEmpty(Class.strDepolyFile))
                            {
                                Class.strDepolyFile = filepath;
                            }
                        }
                        else
                        {
                            fileitem.FullPath = item;
                            string[] pathtemp = item.Split('\\');
                            for (int i = 0; pathtemp.Length - 1 > i; i++)
                            {

                                filepath += pathtemp[i];
                                filepath += @"\";
                            }
                            if (pathtemp[pathtemp.Length - 1].Contains(@"."))
                            {
                                string[] servicenametemp = pathtemp[pathtemp.Length - 1].Split('.');
                                servicename = servicenametemp[0];
                            }

                            fileitem.Exe = pathtemp[pathtemp.Length - 1];
                            Class.strServiceName += pathtemp[pathtemp.Length - 1];
                            Class.strServiceName += " ";
                            if (string.IsNullOrEmpty(Class.strDepolyFile))
                            {
                                Class.strDepolyFile = filepath;
                            }
                        }

                        fileitem.Argment = arg;
                        fileitem.Path = filepath;
                        fileitem.Service = false;
                        Class.FileExeList.Add(fileitem);
                    }
                }
                Class.strServiceName.Trim();
                // 배포 파일경로 가져오기
                string filetemp = GetIniValue("Service", "File");
                if (!string.IsNullOrEmpty(filetemp))
                {
                    Class.strDepolyFile = filetemp;
                }
                if (string.IsNullOrEmpty(Class.strDepolyFile)) Class.strDepolyFile = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            }
            else
            {
                Class.strServiceName = "Null";
                Class.strServiceVersion = "Null";
            }
            Class.strServicePort = GetIniValue("Service", "Port");
            // Asecurity 서비스 리스트 가져오기
            Class.strServerIP = new List<string>();
            // 서버 IP 가져오기
            try
            {
                string iptemp = GetIniValue("Config", "Server");
                string[] serverlist = iptemp.Split(',');
                if (serverlist.Count() == 1)
                {
                    Class.strServerIP.Add(iptemp);
                }
                else
                {
                    foreach (string item in serverlist)
                    {
                        Class.strServerIP.Add(item);
                    }
                }
            }
            catch(Exception ex)
            {
                EventLogger.LogEvent(ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
