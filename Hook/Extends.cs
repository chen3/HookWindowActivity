using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace QiDiTu
{
    public static class Extensions
    {
        public static bool isEmpty(this string s)
        {
            return s.Length == 0;
        }

        public static bool is64(this Process process)
        {
            bool isWow64 = false;
            if ((Environment.OSVersion.Version.Major == 5
                    && System.Environment.OSVersion.Version.Minor >= 1)
                || Environment.OSVersion.Version.Major > 5)
            {
                bool retVal;
                if (!PInvoke.IsWow64Process(process.Handle, out retVal))
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                isWow64 = retVal;
            }
            return !isWow64;
        }
    }
}
