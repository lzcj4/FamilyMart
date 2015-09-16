using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace DAL.Common
{
    public static class Logger
    {
        public static void Info(string str)
        {
            if (str.IsNullOrEmpty()) { return; }
            string s = string.Format("/--- Info:{0} ---/", str);
            WriteLine(s);
        }

        public static void InfoFormat(params string[] strs)
        {
            WriteFormat("/--- Info: ", " ---/", strs);
        }

        public static void Warn(string str)
        {
            if (str.IsNullOrEmpty()) { return; }
            string s = string.Format("/+++ Warn:{0} +++/", str);
            WriteLine(s);
        }

        public static void WarnFormat(params string[] strs)
        {
            WriteFormat("/+++ Warn: ", " +++/", strs);
        }

        public static void Error(string str)
        {
            if (str.IsNullOrEmpty()) { return; }
            string s = string.Format("/*** Error:{0} ***/", str);
            WriteLine(s);
        }

        public static void ErrorFormat(params string[] strs)
        {
            WriteFormat("/*** Error: ", " ***/", strs);
        }

        public static void Write(string str)
        {
            Debug.Write(str);
        }

        public static void WriteLine(string str)
        {
            Debug.WriteLine(str);
        }

        public static void WriteFormat(string prefix, string suffix, params string[] str)
        {
            if (str.IsNullOrEmpty()) { return; }
            StringBuilder sb = new StringBuilder();
            sb.Append(prefix);
            foreach (string item in str)
            {
                sb.Append(string.Format("{0},", item));
            }
            sb.Append(suffix);
            WriteLine(sb.ToString());
        }
    }
}
