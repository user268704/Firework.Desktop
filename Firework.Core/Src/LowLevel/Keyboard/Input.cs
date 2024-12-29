using System.Runtime.InteropServices;

namespace Firework.Core.LowLevel.Keyboard;

public class Input
{
    private const int KEYEVENTF_EXTENDEDKEY = 0x1;
    private const int KEYEVENTF_KEYUP = 0x2;

    public static readonly byte VK_SHIFT = 0x10;
    public static readonly byte VK_CONTROL = 0x11;
    public static readonly byte VK_MENU = 0x12;
    public static readonly byte VK_CAPITAL = 0x14;

    [DllImport("user32.dll")]
    private static extern void keybd_event(byte bVk, int bScan, int dwFlags, int dwExtraInfo);

    public void Key(ConsoleKey key)
    {
        var vk = (byte) key;

        keybd_event(vk, 0, 0, 0);
        keybd_event(vk, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    }

    /// <summary>
    ///     Use local class variables
    /// </summary>
    /// <param name="key">class variable</param>
    public void Key(byte key)
    {
        var vk = key;

        keybd_event(vk, 0, 0, 0);
        keybd_event(vk, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
    }
}