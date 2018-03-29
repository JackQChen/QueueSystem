using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace CallClient
{
    public class OperateIni
    {
        private string fFileName;
        //引用Win32 函数
        //读ini文件（数字)
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileInt(
        string lpAppName,
        string lpKeyName,
        int nDefault,
        string lpFileName
        );
        //读ini文件（字符)
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(
        string lpAppName,
        string lpKeyName,
        string lpDefault,
        StringBuilder lpReturnedString,
        int nSize,
        string lpFileName
        );
        //写INI文件
        [DllImport("kernel32")]
        private static extern bool WritePrivateProfileString(
        string lpAppName,
        string lpKeyName,
        string lpString,
        string lpFileName
        );
        /// <summary>
        /// 构造函数
        /// </summary>
        public OperateIni(string iniPath)
        {
            this.fFileName = iniPath;
        }
        /// <summary>
        /// 读Ini文件（数字）
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <returns></returns>
        public int ReadInt(string section, string key, int def)
        {
            return GetPrivateProfileInt(section, key, def, fFileName);
        }
        /// <summary>
        /// 读Ini文件（字符）
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ReadString(string section, string key)
        {
            StringBuilder temp = new StringBuilder(1024);
            GetPrivateProfileString(section, key, "", temp, 1024, fFileName);
            return temp.ToString();
        }
        /// <summary>
        /// 写Ini文件（数字）
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="iVal"></param>
        public void WriteInt(string section, string key, int iVal)
        {
            WritePrivateProfileString(section, key, iVal.ToString(), fFileName);
        }
        /// <summary>
        /// 写Ini文件（字符）
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="strVal"></param>
        public void WriteString(string section, string key, string strVal)
        {
            WritePrivateProfileString(section, key, strVal, fFileName);
        }
        /// <summary>
        /// 删除键值
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        public void DelKey(string section, string key)
        {
            WritePrivateProfileString(section, key, null, fFileName);
        }
        /// <summary>
        /// 删除模块
        /// </summary>
        /// <param name="section"></param>
        public void DelSection(string section)
        {
            WritePrivateProfileString(section, null, null, fFileName);
        }
    }
}
