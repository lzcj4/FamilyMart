using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SD = System.Diagnostics;

namespace FileExplorer.Helper
{
    static class LogHelper
    {
        internal static void Debug(string s)
        {
            SD.Debug.WriteLine(s);
        }

        internal static void Debug(string s, Exception ex)
        {
            SD.Debug.WriteLine(s, ex);
        }

        internal static void Debug(Exception ex)
        {
            SD.Debug.WriteLine("FileExplorer exception:", ex);
        }

        internal static void DebugFormat(string format, params object[] values)
        {
            SD.Debug.WriteLine(format, values);
        }

        internal static void Info(string s)
        {
            SD.Debug.WriteLine(s);
        }

        internal static void Error(string s)
        {
            SD.Debug.WriteLine(s);
        }
    }
}
