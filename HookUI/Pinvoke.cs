using System;
using System.Runtime.InteropServices;
using System.Text;

static class PInvoke
{
    /// <summary>
    ///     Enumerates all top-level windows on the screen by passing the handle to each window, in turn, to an
    ///     application-defined callback function. <see cref="EnumWindows" /> continues until the last top-level window is
    ///     enumerated or the callback function returns FALSE.
    ///     <para>
    ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633497%28v=vs.85%29.aspx for more
    ///     information
    ///     </para>
    /// </summary>
    /// <param name="lpEnumFunc">
    ///     C++ ( lpEnumFunc [in]. Type: WNDENUMPROC )<br />A pointer to an application-defined callback
    ///     function. For more information, see
    ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms633498%28v=vs.85%29.aspx">EnumWindowsProc</see>
    ///     .
    /// </param>
    /// <param name="lParam">
    ///     C++ ( lParam [in]. Type: LPARAM )<br />An application-defined value to be passed to the callback
    ///     function.
    /// </param>
    /// <returns>
    ///     <c>true</c> if the return value is nonzero., <c>false</c> otherwise. If the function fails, the return value
    ///     is zero.<br />To get extended error information, call GetLastError.<br />If <see cref="EnumWindowsProc" /> returns
    ///     zero, the return value is also zero. In this case, the callback function should call SetLastError to obtain a
    ///     meaningful error code to be returned to the caller of <see cref="EnumWindows" />.
    /// </returns>
    /// <remarks>
    ///     The <see cref="EnumWindows" /> function does not enumerate child windows, with the exception of a few
    ///     top-level windows owned by the system that have the WS_CHILD style.
    ///     <para />
    ///     This function is more reliable than calling the
    ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms633515%28v=vs.85%29.aspx">GetWindow</see>
    ///     function in a loop. An application that calls the GetWindow function to perform this task risks being caught in an
    ///     infinite loop or referencing a handle to a window that has been destroyed.<br />Note For Windows 8 and later,
    ///     EnumWindows enumerates only top-level windows of desktop apps.
    /// </remarks>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

    public delegate bool EnumWindowsProc(IntPtr hWnd, ref IntPtr lParam);

    /// <summary>
    ///     Copies the text of the specified window's title bar (if it has one) into a buffer. If the specified window is a
    ///     control, the text of the control is copied. However, GetWindowText cannot retrieve the text of a control in another
    ///     application.
    ///     <para>
    ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633520%28v=vs.85%29.aspx  for more
    ///     information
    ///     </para>
    /// </summary>
    /// <param name="hWnd">
    ///     C++ ( hWnd [in]. Type: HWND )<br />A <see cref="IntPtr" /> handle to the window or control containing the text.
    /// </param>
    /// <param name="lpString">
    ///     C++ ( lpString [out]. Type: LPTSTR )<br />The <see cref="StringBuilder" /> buffer that will receive the text. If
    ///     the string is as long or longer than the buffer, the string is truncated and terminated with a null character.
    /// </param>
    /// <param name="nMaxCount">
    ///     C++ ( nMaxCount [in]. Type: int )<br /> Should be equivalent to
    ///     <see cref="StringBuilder.Length" /> after call returns. The <see cref="int" /> maximum number of characters to copy
    ///     to the buffer, including the null character. If the text exceeds this limit, it is truncated.
    /// </param>
    /// <returns>
    ///     If the function succeeds, the return value is the length, in characters, of the copied string, not including
    ///     the terminating null character. If the window has no title bar or text, if the title bar is empty, or if the window
    ///     or control handle is invalid, the return value is zero. To get extended error information, call GetLastError.<br />
    ///     This function cannot retrieve the text of an edit control in another application.
    /// </returns>
    /// <remarks>
    ///     If the target window is owned by the current process, GetWindowText causes a WM_GETTEXT message to be sent to the
    ///     specified window or control. If the target window is owned by another process and has a caption, GetWindowText
    ///     retrieves the window caption text. If the window does not have a caption, the return value is a null string. This
    ///     behavior is by design. It allows applications to call GetWindowText without becoming unresponsive if the process
    ///     that owns the target window is not responding. However, if the target window is not responding and it belongs to
    ///     the calling application, GetWindowText will cause the calling application to become unresponsive. To retrieve the
    ///     text of a control in another process, send a WM_GETTEXT message directly instead of calling GetWindowText.<br />For
    ///     an example go to
    ///     <see cref="!:https://msdn.microsoft.com/en-us/library/windows/desktop/ms644928%28v=vs.85%29.aspx#sending">
    ///     Sending a
    ///     Message.
    ///     </see>
    /// </remarks>
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    /// <summary>
    ///     Retrieves the length, in characters, of the specified window's title bar text (if the window has a title bar). If
    ///     the specified window is a control, the function retrieves the length of the text within the control. However,
    ///     GetWindowTextLength cannot retrieve the length of the text of an edit control in another application.
    ///     <para>
    ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633521%28v=vs.85%29.aspx for more
    ///     information
    ///     </para>
    /// </summary>
    /// <param name="hWnd">C++ ( hWnd [in]. Type: HWND )<br />A <see cref="IntPtr" /> handle to the window or control.</param>
    /// <returns>
    ///     If the function succeeds, the return value is the length, in characters, of the text. Under certain
    ///     conditions, this value may actually be greater than the length of the text.<br />For more information, see the
    ///     following Remarks section. If the window has no text, the return value is zero.To get extended error information,
    ///     call GetLastError.
    /// </returns>
    /// <remarks>
    ///     If the target window is owned by the current process, <see cref="GetWindowTextLength" /> causes a
    ///     WM_GETTEXTLENGTH message to be sent to the specified window or control.<br />Under certain conditions, the
    ///     <see cref="GetWindowTextLength" /> function may return a value that is larger than the actual length of the
    ///     text.This occurs with certain mixtures of ANSI and Unicode, and is due to the system allowing for the possible
    ///     existence of double-byte character set (DBCS) characters within the text. The return value, however, will always be
    ///     at least as large as the actual length of the text; you can thus always use it to guide buffer allocation. This
    ///     behavior can occur when an application uses both ANSI functions and common dialogs, which use Unicode.It can also
    ///     occur when an application uses the ANSI version of <see cref="GetWindowTextLength" /> with a window whose window
    ///     procedure is Unicode, or the Unicode version of <see cref="GetWindowTextLength" /> with a window whose window
    ///     procedure is ANSI.<br />For more information on ANSI and ANSI functions, see Conventions for Function Prototypes.
    ///     <br />To obtain the exact length of the text, use the WM_GETTEXT, LB_GETTEXT, or CB_GETLBTEXT messages, or the
    ///     GetWindowText function.
    /// </remarks>
    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", SetLastError = true)]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

    [DllImport("user32.dll")]
    public static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);

    /// <summary>
    ///     Determines the visibility state of the specified window.
    ///     <para>
    ///     Go to https://msdn.microsoft.com/en-us/library/windows/desktop/ms633530%28v=vs.85%29.aspx for more
    ///     information. For WS_VISIBLE information go to
    ///     https://msdn.microsoft.com/en-us/library/windows/desktop/ms632600%28v=vs.85%29.aspx
    ///     </para>
    /// </summary>
    /// <param name="hWnd">C++ ( hWnd [in]. Type: HWND )<br />A handle to the window to be tested.</param>
    /// <returns>
    ///     <c>true</c> or the return value is nonzero if the specified window, its parent window, its parent's parent
    ///     window, and so forth, have the WS_VISIBLE style; otherwise, <c>false</c> or the return value is zero.
    /// </returns>
    /// <remarks>
    ///     The visibility state of a window is indicated by the WS_VISIBLE[0x10000000L] style bit. When
    ///     WS_VISIBLE[0x10000000L] is set, the window is displayed and subsequent drawing into it is displayed as long as the
    ///     window has the WS_VISIBLE[0x10000000L] style. Any drawing to a window with the WS_VISIBLE[0x10000000L] style will
    ///     not be displayed if the window is obscured by other windows or is clipped by its parent window.
    /// </remarks>
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowVisible(IntPtr hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWindowEnabled(IntPtr hWnd);

    public enum WindowLongFlags : int
    {
        GWL_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_EXSTYLE = -20
    }

    #region Window Styles
    // Window Styles
    public const UInt32 WS_OVERLAPPED = 0;
    public const UInt32 WS_POPUP = 0x80000000;
    public const UInt32 WS_CHILD = 0x40000000;
    public const UInt32 WS_MINIMIZE = 0x20000000;
    public const UInt32 WS_VISIBLE = 0x10000000;
    public const UInt32 WS_DISABLED = 0x8000000;
    public const UInt32 WS_CLIPSIBLINGS = 0x4000000;
    public const UInt32 WS_CLIPCHILDREN = 0x2000000;
    public const UInt32 WS_MAXIMIZE = 0x1000000;
    public const UInt32 WS_CAPTION = 0xC00000;         // WS_BORDER or WS_DLGFRAME  
    public const UInt32 WS_BORDER = 0x800000;
    public const UInt32 WS_DLGFRAME = 0x400000;
    public const UInt32 WS_VSCROLL = 0x200000;
    public const UInt32 WS_HSCROLL = 0x100000;
    public const UInt32 WS_SYSMENU = 0x80000;
    public const UInt32 WS_THICKFRAME = 0x40000;
    public const UInt32 WS_GROUP = 0x20000;
    public const UInt32 WS_TABSTOP = 0x10000;
    public const UInt32 WS_MINIMIZEBOX = 0x20000;
    public const UInt32 WS_MAXIMIZEBOX = 0x10000;
    public const UInt32 WS_TILED = WS_OVERLAPPED;
    public const UInt32 WS_ICONIC = WS_MINIMIZE;
    public const UInt32 WS_SIZEBOX = WS_THICKFRAME;

    // Extended Window Styles 
    public const UInt32 WS_EX_DLGMODALFRAME = 0x0001;
    public const UInt32 WS_EX_NOPARENTNOTIFY = 0x0004;
    public const UInt32 WS_EX_TOPMOST = 0x0008;
    public const UInt32 WS_EX_ACCEPTFILES = 0x0010;
    public const UInt32 WS_EX_TRANSPARENT = 0x0020;
    public const UInt32 WS_EX_MDICHILD = 0x0040;
    public const UInt32 WS_EX_TOOLWINDOW = 0x0080;
    public const UInt32 WS_EX_WINDOWEDGE = 0x0100;
    public const UInt32 WS_EX_CLIENTEDGE = 0x0200;
    public const UInt32 WS_EX_CONTEXTHELP = 0x0400;
    public const UInt32 WS_EX_RIGHT = 0x1000;
    public const UInt32 WS_EX_LEFT = 0x0000;
    public const UInt32 WS_EX_RTLREADING = 0x2000;
    public const UInt32 WS_EX_LTRREADING = 0x0000;
    public const UInt32 WS_EX_LEFTSCROLLBAR = 0x4000;
    public const UInt32 WS_EX_RIGHTSCROLLBAR = 0x0000;
    public const UInt32 WS_EX_CONTROLPARENT = 0x10000;
    public const UInt32 WS_EX_STATICEDGE = 0x20000;
    public const UInt32 WS_EX_APPWINDOW = 0x40000;
    public const UInt32 WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE);
    public const UInt32 WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST);
    public const UInt32 WS_EX_LAYERED = 0x00080000;
    // Disable inheritence of mirroring by children
    public const UInt32 WS_EX_NOINHERITLAYOUT = 0x00100000;
    // Right to left mirroring
    public const UInt32 WS_EX_LAYOUTRTL = 0x00400000;          
    public const UInt32 WS_EX_COMPOSITED = 0x02000000;
    public const UInt32 WS_EX_NOACTIVATE = 0x08000000;
    #endregion

    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowLong(IntPtr hWnd, WindowLongFlags nIndex);

    public static IntPtr GetWindowLongPtr(IntPtr hWnd, WindowLongFlags nIndex)
    {
        int i = IntPtr.Size * 8;
        if (i == 32)
        {
            return GetWindowLongPtr32(hWnd, (int)nIndex);
        }
        else if(i == 64)
        {
            return GetWindowLongPtr64(hWnd, (int)nIndex);
        }
        else
        {
            throw new Exception("Not Support");
        }
    }

    [DllImport("user32.dll", EntryPoint = "GetWindowLong")]
    private static extern IntPtr GetWindowLongPtr32(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
    private static extern IntPtr GetWindowLongPtr64(IntPtr hWnd, int nIndex);

    public enum DWMWINDOWATTRIBUTE : uint
    {
        NCRenderingEnabled = 1,
        NCRenderingPolicy,
        TransitionsForceDisabled,
        AllowNCPaint,
        CaptionButtonBounds,
        NonClientRtlLayout,
        ForceIconicRepresentation,
        Flip3DPolicy,
        ExtendedFrameBounds,
        HasIconicBitmap,
        DisallowPeek,
        ExcludedFromPeek,
        Cloak,
        Cloaked,
        FreezeRepresentation
    }

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(IntPtr hwnd,
                                        DWMWINDOWATTRIBUTE dwAttribute,
                                        out bool pvAttribute, int cbAttribute);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName,
                                        int nMaxCount);
    
    [Flags]
    public enum AllocationType
    {
        Commit = 0x1000,
        Reserve = 0x2000,
        Decommit = 0x4000,
        Release = 0x8000,
        Reset = 0x80000,
        Physical = 0x400000,
        TopDown = 0x100000,
        WriteWatch = 0x200000,
        LargePages = 0x20000000
    }

    [Flags]
    public enum MemoryProtection
    {
        Execute = 0x10,
        ExecuteRead = 0x20,
        ExecuteReadWrite = 0x40,
        ExecuteWriteCopy = 0x80,
        NoAccess = 0x01,
        ReadOnly = 0x02,
        ReadWrite = 0x04,
        WriteCopy = 0x08,
        GuardModifierflag = 0x100,
        NoCacheModifierflag = 0x200,
        WriteCombineModifierflag = 0x400
    }

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
                                            int dwSize, AllocationType flAllocationType,
                                            MemoryProtection flProtect);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
                                                byte[] lpBuffer, int nSize,
                                                out IntPtr lpNumberOfBytesWritten);

    [DllImport("kernel32.dll", CharSet = CharSet.Ansi,
                ExactSpelling = true, SetLastError = true)]
    public static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

    [DllImport("kernel32.dll")]
    public static extern IntPtr CreateRemoteThread(IntPtr hProcess,
                                            IntPtr lpThreadAttributes, uint dwStackSize,
                                            IntPtr lpStartAddress, IntPtr lpParameter,
                                            uint dwCreationFlags, out IntPtr lpThreadId);
    
    public enum Milliseconds : UInt32
    {
        INFINITE = 0xFFFFFFFF,
        WAIT_ABANDONED = 0x00000080,
        WAIT_OBJECT_0 = 0x00000000,
        WAIT_TIMEOUT = 0x00000102
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern UInt32 WaitForSingleObject(IntPtr hHandle,
                                                    Milliseconds dwMilliseconds);

    [DllImport("kernel32.dll", SetLastError = true)]
    public static extern UInt32 WaitForSingleObject(IntPtr hHandle,
                                                    UInt32 dwMilliseconds);

    [Flags]
    public enum FreeType
    {
        Decommit = 0x4000,
        Release = 0x8000,
    }

    [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
    public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
                                            int dwSize, FreeType dwFreeType);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam,
                                            IntPtr lParam);

    [DllImport("kernel32.dll", SetLastError = true,
                CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process(
            [In] Microsoft.Win32.SafeHandles.SafeHandleZeroOrMinusOneIsInvalid hProcess,
            [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

    [DllImport("kernel32.dll", SetLastError = true,
                CallingConvention = CallingConvention.Winapi)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool IsWow64Process([In] IntPtr processHandle,
                            [Out, MarshalAs(UnmanagedType.Bool)] out bool wow64Process);

}