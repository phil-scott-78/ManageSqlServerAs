using System;
using System.Runtime.InteropServices;

namespace ManageSqlServerAs.Tools
{
    public class Launcher
    {
        public const UInt32 Infinite = 0xffffffff;
        public const Int32 StartfUseStdHandles = 0x00000100;
        public const Int32 StdOutputHandle = -11;
        public const Int32 StdErrorHandle = -12;

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct StartupInfo
        {
            public int cb;
            public String reserved;
            public String desktop;
            public String title;
            public int x;
            public int y;
            public int xSize;
            public int ySize;
            public int xCountChars;
            public int yCountChars;
            public int fillAttribute;
            public int flags;
            public UInt16 showWindow;
            public UInt16 reserved2;
            public byte reserved3;
            public IntPtr stdInput;
            public IntPtr stdOutput;
            public IntPtr stdError;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        public struct ProcessInformation
        {
            public IntPtr process;
            public IntPtr thread;
            public int processId;
            public int threadId;
        }


        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool CreateProcessWithLogonW(
            String userName,
            String domain,
            String password,
            UInt32 logonFlags,
            String applicationName,
            String commandLine,
            UInt32 creationFlags,
            UInt32 environment,
            String currentDirectory,
            ref StartupInfo startupInfo,
            out ProcessInformation processInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr process, ref UInt32 exitCode);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern UInt32 WaitForSingleObject(IntPtr handle, UInt32 milliseconds);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(IntPtr handle);

        [DllImport("Kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        public static bool CreateProcess(string userName, string domain, string password, string path)
        {
            const uint logonNetcredentialsOnly = 2;
            var lpStartupInfo = new StartupInfo();
            ProcessInformation processInformation;

            return CreateProcessWithLogonW(
                userName,
                domain,
                password,
                logonNetcredentialsOnly,
                null,
                path,
                0,
                0,
                null,
                ref lpStartupInfo,
                out processInformation);
        }

    }
}