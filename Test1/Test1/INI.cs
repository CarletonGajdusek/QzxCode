using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Test1
{
    public class INI
    {
        [DllImport("kernel32", CharSet = CharSet.Auto)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern long GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        string iniFileF = AppDomain.CurrentDomain.BaseDirectory + "SmsIni.ini";

        public INI(string path = "")
        {
            if (!string.IsNullOrEmpty(path))
                iniFileF = path;
            CheckDirectory(iniFileF);
        }
        public string IniReadValue(string section, string key, string defaultValue = "0")
        {
            StringBuilder temp = new StringBuilder(2048);
            GetPrivateProfileString(section, key, "", temp, 2048, iniFileF);
            string strVal = temp.ToString();
            if (string.IsNullOrWhiteSpace(strVal) && !string.IsNullOrWhiteSpace(defaultValue))
            {
                IniWriteValue(section, key, defaultValue);
                strVal = defaultValue;
            }
            return strVal;
        }

        public void IniWriteValue(string section, string key, string value)
        {
            WritePrivateProfileString(section, key, value, iniFileF);
        }
        void CheckDirectory(string filePath)
        {
            try
            {
                if (File.Exists(filePath)) return;

                if (string.IsNullOrWhiteSpace(filePath)) return;
                int lastPath = filePath.LastIndexOf('\\');
                if (lastPath < 0) return;
                string tmpStr = filePath.Substring(0, lastPath);

                string tmpPath = string.Empty;
                string[] dirs = tmpStr.Split('\\');
                foreach (string dir in dirs)
                {
                    if (string.IsNullOrWhiteSpace(tmpPath))
                        tmpPath = dir;
                    else
                        tmpPath += "\\" + dir;
                    if (!Directory.Exists(tmpPath))
                    {
                        Directory.CreateDirectory(tmpPath);
                    }
                }
                File.Create(filePath);
            }
            catch
            {
                return;
            }
        }
    }
}
