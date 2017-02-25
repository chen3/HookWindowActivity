using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace QiDiTu.Hook
{
    static class HookCore
    {
        private const string dllName = "hook.dll";
        private static readonly string dllFullPath = (new FileInfo(dllName)).FullName;
        private static readonly byte[] dllFullPathBytes = Encoding.Default.GetBytes(dllFullPath);
        private static int pathLength = dllFullPathBytes.Length + sizeof(byte);

        static void Main(string[] args)
        {
            if(args.Length != 2)
            {
                Environment.Exit(1);
            }
            Process taregt = Process.GetProcessById(int.Parse(args[1]));
            init();
            //HookCore hook = new HookCore(int.Parse(args[1]));
            if(args[0] == "install")
            {
                HookCore.install(taregt);
            }
            else if(args[0] == "uninstall")
            {
                HookCore.uninstall(taregt);
            }
        }

        public static void init()
        {
            if (!File.Exists(dllFullPath))
            {
                Environment.Exit(9);
            }
            foreach (ProcessModule module in Process.GetCurrentProcess().Modules)
            {
                if (module.ModuleName.ToLower() == "kernel32.dll")
                {
                    procLoadLibraryA = PInvoke.GetProcAddress(module.BaseAddress, "LoadLibraryA");
                    procFreeLibrary = PInvoke.GetProcAddress(module.BaseAddress, "FreeLibrary");
                    break;
                }
            }
        }

        private static IntPtr procLoadLibraryA;
        private static IntPtr procFreeLibrary;

        public static bool install(Process target)
        {
            IntPtr alloc = PInvoke.VirtualAllocEx(target.Handle, IntPtr.Zero,
                                                    pathLength,
                                                    PInvoke.AllocationType.Commit,
                                                    PInvoke.MemoryProtection.ReadWrite);
            if (alloc == IntPtr.Zero)
            {
                //MessageBox.Show("申请远程进程空间失败");
                Environment.Exit(2);
            }
            IntPtr numberOfBytesWritten;
            bool success = PInvoke.WriteProcessMemory(target.Handle, alloc,
                                                    dllFullPathBytes, pathLength,
                                                    out numberOfBytesWritten);
            if (!success)
            {
                //MessageBox.Show("写入远程进程数据失败");
                Environment.Exit(3);
            }
            IntPtr remoteThreadId;
            IntPtr result;
            {
                result = PInvoke.CreateRemoteThread(target.Handle, IntPtr.Zero,
                                                            0, procLoadLibraryA, alloc,
                                                            0, out remoteThreadId);
            }
            if (result == IntPtr.Zero)
            {
                //MessageBox.Show("创建远程线程失败");
                Environment.Exit(4);
            }
            PInvoke.WaitForSingleObject(result, PInvoke.Milliseconds.INFINITE);
            PInvoke.VirtualFreeEx(target.Handle, alloc, 0, PInvoke.FreeType.Release);
            return true;
        }

        public static bool uninstall(Process process)
        {
            IntPtr result = PInvoke.SendMessage(process.MainWindowHandle, 0x801, IntPtr.Zero, IntPtr.Zero);
            switch (getResult(result))
            {
                case Result.Unhendled:
                case Result.Success:
                {
                    return freeLibrary(process, dllFullPath);
                }
                case Result.Fail:
                {
                        //MessageBox.Show("卸载失败");
                    Environment.Exit(5);
                } break;
                //case Result.Unhendled:
                //{
                //    MessageBox.Show("目标进程未处理卸载消息,请检查源码");
                //    return true;
                //}
                case Result.MsgCodeOccupiedByOther:
                {
                    //MessageBox.Show("目标进程返回异常值,请检查源码");
                    Environment.Exit(6);
                } break;
            }
            return false;
        }

        private static bool freeLibrary(Process process, string dllFileFullPath)
        {
            foreach (ProcessModule moudle in process.Modules)
            {
                if (moudle.FileName == dllFileFullPath)
                {
                    IntPtr remoteThreadId;
                    IntPtr result;
                        result = PInvoke.CreateRemoteThread(process.Handle, IntPtr.Zero,
                                                            0, procFreeLibrary, moudle.BaseAddress,
                                                            0, out remoteThreadId);
                    if (result == IntPtr.Zero)
                    {
                        //MessageBox.Show("创建远程线程失败");
                        Environment.Exit(7);
                    }
                    PInvoke.WaitForSingleObject(result, PInvoke.Milliseconds.INFINITE);
                    return true;
                }
            }
            return true;
        }

        private static Result getResult(IntPtr result)
        {
            switch (result.ToInt32())
            {
                case 0:
                case 1:
                case 2:
                {
                    return (Result)result.ToInt32();
                }
                default:
                {
                    return Result.MsgCodeOccupiedByOther;
                }
            }
        }

        private enum Result
        {
            Unhendled = 0,
            Success = 1,
            Fail = 2,
            MsgCodeOccupiedByOther
        }
    }
}
