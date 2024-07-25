using System.Runtime.InteropServices;

namespace WindowsClipboard;

public static partial class Clipboard
{
    private const int GMEM_MOVEABLE = 0x02;
    private const int CF_UNICODETEXT = 13;

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool OpenClipboard(IntPtr hWndNewOwner);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool CloseClipboard();

    [LibraryImport("user32.dll")]
    private static partial IntPtr SetClipboardData(uint uFormat, IntPtr hMem);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EmptyClipboard();

    [LibraryImport("kernel32.dll")]
    private static partial IntPtr GlobalAlloc(uint uFlags, UIntPtr dwBytes);

    [LibraryImport("Kernel32.dll")]
    private static partial IntPtr GlobalLock(IntPtr hMem);

    [LibraryImport("Kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool GlobalUnlock(IntPtr hMem);

    [LibraryImport("kernel32.dll")]
    private static partial IntPtr GlobalFree(IntPtr hMem);

    public static void SetText(string s)
    {
        if (!OpenClipboard(IntPtr.Zero))
        {
            Console.WriteLine("Clipboard open failed");
            return;
        }

        EmptyClipboard();

        IntPtr hBuffer = GlobalAlloc(GMEM_MOVEABLE, (UIntPtr)((s.Length + 1) * 2));
        bool success = false;
        try
        {
            unsafe
            {
                char* p = (char*)GlobalLock(hBuffer);
                s.CopyTo(new Span<char>(p, s.Length));
                p[s.Length] = '\0';
            }
            if (SetClipboardData(CF_UNICODETEXT, hBuffer) != IntPtr.Zero)
                success = true;
        }
        finally
        {
            GlobalUnlock(hBuffer);
            // Ownership is transferred to windows if it succeeds
            if (!success)
                GlobalFree(hBuffer);
            CloseClipboard();
        }
    }
}
