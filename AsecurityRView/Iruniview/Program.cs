using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Iruniview
{
    static class Program
    {
        private static Form1 _mainForm;

        /// <summary>
        /// 해당 응용 프로그램의 주 진입점입니다.
        /// </summary>
        [STAThread]
        static void Main()
        {
            String strZippath = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            String strupdatepath = strZippath;
            strupdatepath += @"\\update.exe"; 
            strZippath += @"\\iruniview.ini";

            FileInfo fileinfo1 = new FileInfo(strZippath);
            if (fileinfo1.Exists == false)
            {                
                byte[] aryData = Iruniview.Properties.Resources.HanView;
                FileStream fileStream = new FileStream(fileinfo1.FullName, FileMode.CreateNew);
                fileStream.Write(aryData, 0, aryData.Length);
                fileStream.Close();
            }

            FileInfo fileinfo2 = new FileInfo(strupdatepath);
            if (fileinfo2.Exists == false)
            {
                byte[] aryData = Iruniview.Properties.Resources.update;
                FileStream fileStream = new FileStream(fileinfo2.FullName, FileMode.CreateNew);
                fileStream.Write(aryData, 0, aryData.Length);
                fileStream.Close();
            }

            Application.EnableVisualStyles();
            _mainForm = new Form1();
            Application.Run(_mainForm);
        }

        public static void ShowBalloonTip(string title, string text, ToolTipIcon icon)
        {
            _mainForm.ShowBalloonTip(title, text, icon);            
        }
    }
}
